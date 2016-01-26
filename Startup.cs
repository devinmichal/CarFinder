using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(carfinder.Startup))]
namespace carfinder
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
