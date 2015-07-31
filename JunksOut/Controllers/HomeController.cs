using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Linq;
using Microsoft.AspNet.Mvc.Facebook;
using Microsoft.AspNet.Mvc.Facebook.Client;
using JunksOut.Domain;
using JunksOut.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Xml;
using System.Net;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace JunksOut.Controllers
{
    public class HomeController : Controller
    {
        private readonly TytContext _dbContext;
        private readonly JunksOutModel _junksOutModel = new JunksOutModel();

        public string GeoLat { get; set; }
        public string GeoLong { get; set; }
        public string AddrName { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        
        private CloudStorageAccount _storageAccount;

        public HomeController()
        {
            _dbContext = new TytContext();
            _storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"].ToString());

        }

        //[FacebookAuthorize("email", "user_photos")]
        //public async Task<ActionResult> Index(FacebookContext context)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await context.Client.GetCurrentUserAsync<MyAppUser>();
        //        return View(user);
        //    }

        //    return View("Error");
        //}

        private void VerifyUserImagesDir()
        {
            Directory.CreateDirectory("~/UserContent");


        }

        //Function to get the coordinates and the formatted address from inputed address from user
        public void GeoCode()
        {
            //to Read the Stream
            StreamReader sr = null;

            AddrName = Request.Form["addresstextview"]; //Gets the address inputed by user from ItemUpload view
           

            //The Google Maps API Either return JSON or XML. We are using XML Here
            //Saving the url of the Google API 
            string url = String.Format("http://maps.googleapis.com/maps/api/geocode/xml?address=" +
             AddrName + "&sensor=false");

            //to Send the request to Web Client 
            WebClient wc = new WebClient();
            try
            {
                sr = new StreamReader(wc.OpenRead(url));
            }
            catch (Exception ex)
            {
                throw new Exception("The Error Occured" + ex.Message);
            }

            try
            {
                XmlTextReader xmlReader = new XmlTextReader(sr);
                bool latread = false;
                bool longread = false;
                bool properead = false;

                while (xmlReader.Read())
                {
                    xmlReader.MoveToElement();
                    switch (xmlReader.Name)
                    {
                        case "lat":

                            if (!latread)
                            {
                                xmlReader.Read();
                                GeoLat = xmlReader.Value.ToString();
                                latread = true;

                            }
                            break;
                        case "lng":
                            if (!longread)
                            {
                                xmlReader.Read();
                                GeoLong = xmlReader.Value.ToString();
                                longread = true;
                            }
                            break;
                        case "formatted_address":
                            if (!properead)
                            {
                                xmlReader.Read();
                                AddrName = xmlReader.Value.ToString();
                                properead = true;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An Error Occured" + ex.Message);
            }
        }




        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Upload(
            [Bind(Include = "address, location, tags, description, userid, imageUrl")] item userItem)
        {
            
            //if there is no file return 
            if (Request.Files[0] == null)
                return RedirectToAction("Map");


            var file = Request.Files[0];

            //azure storage for image files (container is called usercontent)
            CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("usercontent");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(file.FileName);
            blockBlob.UploadFromStream(file.InputStream);

            GeoCode(); //Gets the address and coordnates

            string tagsview = Request.Form["tagstextview"];
            string descview = Request.Form["descriptiontextview"];

            //add information to db
            //this needs to be acquired from the UI above
            _junksOutModel.items.Add(new item
            {
                address = AddrName,
                location = GeoLat + ", " + GeoLong,
                tags = tagsview,
                description = descview,
                imageUrl = file.FileName,
            });

            _junksOutModel.SaveChanges();


            return RedirectToAction("Map");
        }


        public ActionResult Items()
        {
            CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("usercontent");

            var ImageUris = new List<string>();

            foreach (var item in _junksOutModel.items)
            {
                if (item.imageUrl != null)
                {
                    ImageUris.Add(container.GetBlockBlobReference(item.imageUrl).StorageUri.PrimaryUri.ToString());
                }
            }

            return View("Items", ImageUris);
        }

        public ActionResult Map()
        {
            
            var model = new List<UploadData>
            {
                new UploadData{Latitude = "38.220630", Longitude = "-85.697239", Description="couch", TimeSince=1m},
                new UploadData{Latitude = "38.279686", Longitude = "-85.567594", Description="chairs", TimeSince=0.8m},
                new UploadData{Latitude = "38.251112", Longitude = "-85.591626", Description="car", TimeSince=0.9m},
                new UploadData{Latitude = "38.219830", Longitude = "-85.761915", Description="benchseat", TimeSince=0.9m}, 
                new UploadData{Latitude = "38.196090", Longitude = "-85.745435", Description="books", TimeSince=1m},
                new UploadData{Latitude = "38.216054", Longitude = "-85.747495", Description="toilet", TimeSince=0.6m}
            };
             

          //  var model = new UploadData
          //  {
          //     UploadDatas = this._dbContext.UploadData.ToList()
          //  };

            return View("Map", model);
        }

        
        [HttpGet]
        public ContentResult PickupKml()
        {
            var ns = XNamespace.Get("http://www.opengis.net/kml/2.2");
            var xmlDoc = new XDocument(new XDeclaration("1.0", "utf-8", null),
                new XElement(ns + "kml",
                    new XElement("Document",
                        new XElement("name", "Untitled layer"),
                        new XElement("Placemark",
                            new XElement("styleUrl", "#poly-000000-1-76"),
                            new XElement("name", "Right Now"),
                            new XElement("description", new XCData("June 6 - June 8")),
                            new XElement("Polygon",
                                new XElement("outerBoundaryIs",
                                    new XElement("LinearRing",
                                        new XElement("tessellate", 1),
                                        new XElement("coordinates", "-85.75172424447373,38.245528009039305,0.0 -85.75184226052443,38.24491290647846,0.0 -85.75260400903062,38.240859826739666,0.0 -85.75266838204698,38.237708211493356,0.0 -85.75060844552354,38.2337979962848,0.0 -85.75095176827745,38.23062921938785,0.0 -85.75181007516221,38.2271231776859,0.0 -85.75078010690049,38.22476324676824,0.0 -85.7511234296544,38.221526645583815,0.0 -85.7522392286046,38.217210953348776,0.0 -85.75284004342393,38.21208573634768,0.0 -85.75181007516221,38.21154621882361,0.0 -85.74786186349229,38.21201829687593,0.0 -85.74623108041123,38.21316475939731,0.0 -85.74288368356065,38.21458095287149,0.0 -85.74099540841416,38.215862247029214,0.0 -85.73601722848252,38.21869450142766,0.0 -85.7331848157628,38.220784904198574,0.0 -85.73052406442002,38.22395410997175,0.0 -85.72812080514268,38.2255049475923,0.0 -85.72468757760362,38.22489810202603,0.0 -85.7196235669835,38.2268534751703,0.0 -85.71541786324815,38.229078491017624,0.0 -85.71104049813584,38.223549538198554,0.0 -85.70820808541612,38.21694121408469,0.0 -85.69490432870225,38.2107369350385,0.0 -85.69378852975206,38.20621826852288,0.0 -85.69155693185166,38.203318081031455,0.0 -85.68614959847764,38.20109227738781,0.0 -85.67954063546495,38.195223923424315,0.0 -85.6646919263585,38.20284594655093,0.0 -85.66640854012803,38.205139142488704,0.0 -85.66864013802842,38.20756715356951,0.0 -85.67413330209092,38.215053011238545,0.0 -85.68511963021592,38.223212391277734,0.0 -85.6936168683751,38.22415639295685,0.0 -85.7042598737462,38.230561795045205,0.0 -85.7089805616124,38.23170796539162,0.0 -85.72443008553819,38.2402025472158,0.0 -85.72116851937608,38.24188786213779,0.0 -85.72340011727647,38.2447191032579,0.0 -85.72726249825791,38.24357313799235,0.0 -85.75172424447373,38.245528009039305,0.0")
                                    )
                                )
                            )
                        ),
                        new XElement("Style",
                            new XAttribute("id", "poly-000000-1-76"),
                            new XElement("LineStyle",
                                new XElement("color", "ff000000"),
                                new XElement("width", 1)
                            ),
                            new XElement("PolyStyle",
                                new XElement("color", "4C000000"),
                                new XElement("fill", 1),
                                new XElement("outline", 1)
                            )
                        )
                    )
                )
            );
            var wr = new Utf8StringWriter();
            xmlDoc.Save(wr);

            return Content(wr.GetStringBuilder().ToString(), "text/xml");
        }

        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding { get { return Encoding.UTF8; } }
        }

        // This action will handle the redirects from FacebookAuthorizeFilter when
        // the app doesn't have all the required permissions specified in the FacebookAuthorizeAttribute.
        // The path to this action is defined under appSettings (in Web.config) with the key 'Facebook:AuthorizationRedirectPath'.
        
        //public ActionResult Permissions(FacebookRedirectContext context)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        return View(context);
        //    }

        //    return View("Error");
        //}

        public ActionResult CreatePickup(Pickup newPickup)
        {
            byte[] image;

            var buffer = new byte[16*1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = newPickup.FileUpload.InputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                image = ms.ToArray();
            }

            _dbContext.UploadData.Add(new UploadData
            {
                Description = newPickup.Description,
                Name = newPickup.Name,
                Latitude = newPickup.Latitude,
                Longitude = newPickup.Longitude,
                ImageData = image
            });

            return View("Index");
        }

      
    }
}
