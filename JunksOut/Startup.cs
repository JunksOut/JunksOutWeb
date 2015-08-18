using Microsoft.Owin;
using Owin;


[assembly: OwinStartupAttribute(typeof(JunksOut.Startup))]
namespace JunksOut
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}