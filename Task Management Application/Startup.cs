using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Task_Management_Application.Startup))]
namespace Task_Management_Application
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
