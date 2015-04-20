using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OrderWebApplication.Startup))]
namespace OrderWebApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
