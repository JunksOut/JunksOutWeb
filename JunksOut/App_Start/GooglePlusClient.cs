using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DotNetOpenAuth.AspNet.Clients;
using DotNetOpenAuth.Messaging;
using Newtonsoft.Json;

namespace JunksOut
{
    public class GooglePlusClient : OAuth2Client
    {
        /// <summary>
        /// The authorization endpoint.
        /// 
        /// </summary>
        private const string AuthorizationEndpoint = "https://accounts.google.com/o/oauth2/auth";

        /// <summary>
        /// The token endpoint.
        /// 
        /// </summary>
        private const string TokenEndpoint = "https://accounts.google.com/o/oauth2/token";

        /// <summary>
        /// The user info endpoint.
        /// 
        /// </summary>
        private const string UserInfoEndpoint = "https://www.googleapis.com/oauth2/v1/userinfo";

        /// <summary>
        /// The client id.
        /// 
        /// </summary>
        private readonly string clientId;

        /// <summary>
        /// The client secret.
        /// 
        /// </summary>
        private readonly string clientSecret;

        public GooglePlusClient(string clientId, string clientSecret)
            : base("GooglePlus")
        {
            if (clientId == null)
                throw new ArgumentNullException("clientId");
            if (clientSecret == null)
                throw new ArgumentNullException("clientSecret");

            this.clientId = clientId;
            this.clientSecret = clientSecret;
        }

        protected override Uri GetServiceLoginUrl(Uri returnUrl)
        {
            UriBuilder uriBuilder = new UriBuilder(AuthorizationEndpoint);
            uriBuilder.AppendQueryArgument("client_id", this.clientId);
            uriBuilder.AppendQueryArgument("redirect_uri", returnUrl.GetLeftPart(UriPartial.Path));
            uriBuilder.AppendQueryArgument("response_type", "code");
            uriBuilder.AppendQueryArgument("scope", "https://www.googleapis.com/auth/plus.login https://www.googleapis.com/auth/userinfo.email");
            uriBuilder.AppendQueryArgument("state", returnUrl.Query.Substring(1));

            return uriBuilder.Uri;
        }

        protected override IDictionary<string, string> GetUserData(string accessToken)
        {
            UriBuilder uriBuilder = new UriBuilder(UserInfoEndpoint);
            uriBuilder.AppendQueryArgument("access_token", accessToken);

            WebRequest webRequest = WebRequest.Create(uriBuilder.Uri);
            using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                if (webResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream == null)
                            return null;

                        StreamReader streamReader = new StreamReader(responseStream);

                        Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(streamReader.ReadToEnd());

                        // Add a username field in the dictionary
                        if (values.ContainsKey("email") && !values.ContainsKey("username"))
                            values.Add("username", values["email"]);

                        return values;
                    }
                }
            }

            return null;
        }

        protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
        {
            // Build up the form post data
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("code", authorizationCode);
            values.Add("client_id", this.clientId);
            values.Add("client_secret", this.clientSecret);
            values.Add("redirect_uri", returnUrl.GetLeftPart(UriPartial.Path));
            values.Add("grant_type", "authorization_code");
            string postData = String.Join("&",
                values.Select(x => Uri.EscapeDataString(x.Key) + "=" + Uri.EscapeDataString(x.Value))
                      .ToArray());

            // Build up the request
            WebRequest webRequest = WebRequest.Create(TokenEndpoint);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = postData.Length;
            webRequest.Method = "POST";
            using (Stream requestStream = webRequest.GetRequestStream())
            {
                StreamWriter streamWriter = new StreamWriter(requestStream);
                streamWriter.Write(postData);
                streamWriter.Flush();
            }

            // Process the response
            using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                if (webResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream responseStream = webResponse.GetResponseStream())
                    {
                        StreamReader streamReader = new StreamReader(responseStream);

                        dynamic response = JsonConvert.DeserializeObject<dynamic>(streamReader.ReadToEnd());
                        return (string)response.access_token;
                    }
                }
            }

            return null;
        }

        public static void RewriteRequest()
        {
            var ctx = HttpContext.Current;

            var stateString = HttpUtility.UrlDecode(ctx.Request.QueryString["state"]);
            if (stateString == null || !stateString.Contains("__provider__=GooglePlus"))
                return;

            var q = HttpUtility.ParseQueryString(stateString);
            q.Add(ctx.Request.QueryString);
            q.Remove("state");

            ctx.RewritePath(ctx.Request.Path + "?" + q);
        }
    }
}