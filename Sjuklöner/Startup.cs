using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Sjuklöner.Startup))]
namespace Sjuklöner
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
