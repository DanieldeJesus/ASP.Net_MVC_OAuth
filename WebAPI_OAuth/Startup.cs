using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.OAuth;
using Owin;
using WebAPI_OAuth.App_Start;

[assembly: OwinStartup(typeof(WebAPI_OAuth.Startup))]

namespace WebAPI_OAuth
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);           
        }
    }
}
