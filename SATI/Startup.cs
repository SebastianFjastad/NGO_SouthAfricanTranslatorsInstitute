using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SATI.Startup))]
namespace SATI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
