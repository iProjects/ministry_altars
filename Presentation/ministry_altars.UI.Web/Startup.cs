using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ministry_altars.UI.Web.Startup))]
namespace ministry_altars.UI.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
