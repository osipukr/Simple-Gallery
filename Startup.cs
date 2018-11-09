using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Simply_Gallery.Startup))]

namespace Simply_Gallery
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}