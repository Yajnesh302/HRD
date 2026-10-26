using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HRD.Startup))]
namespace HRD
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
