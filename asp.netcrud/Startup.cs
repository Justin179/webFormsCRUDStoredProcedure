using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(asp.netcrud.Startup))]
namespace asp.netcrud
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
