using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI_OAuth.App_Start;
using WebAPI_OAuth.Models;
using WebAPI_OAuth.Providers;

namespace WebAPI_OAuth
{
	public partial class Startup
	{
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        public static string PublicClientId { get; private set; }        

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // configuracao WebApi
            var config = new HttpConfiguration();

            // configurando rotas
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            // ativando cors
            app.UseCors(CorsOptions.AllowAll);

            //ativando a geração dos tokens de acesso
            AtivarGeracaoTokenAcesso(app);

            // ativando configuração WebApi
            app.UseWebApi(config);
        }

        private void AtivarGeracaoTokenAcesso(IAppBuilder app)
        {
            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AllowInsecureHttp = true
            };

            app.UseOAuthAuthorizationServer(OAuthOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    
    }
}