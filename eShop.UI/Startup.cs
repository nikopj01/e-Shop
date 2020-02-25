using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(eShop.UI.Startup))]
namespace eShop.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
