using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using JunksOut.Models;

namespace JunksOut
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            OAuthWebSecurity.RegisterClient(new GooglePlusClient("721346348343-7hgpuntjqqbe5r1c2vai1s93oi6q5575.apps.googleusercontent.com", "84Xivb3MB6_mHzKhXvwSlvgC"), "Google+", null);
        }
    }
}