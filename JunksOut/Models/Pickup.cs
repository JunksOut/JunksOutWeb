
using System.Web;

namespace TrashYourTreasure.Models
{
    public class Pickup
    {
        public HttpPostedFileBase FileUpload { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}