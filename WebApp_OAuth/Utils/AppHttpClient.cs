using Microsoft.AspNet.Identity.Owin;
using OwinFramework.InterfacesV1.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using WebApp_OAuth.App_Start.Identity;

namespace WebApp_OAuth.Utils
{
    public class AppHttpClient : HttpClient
    {
        public HttpRequestHeaders Headers { get; }

        public AppHttpClient(string uri)
        {
            Configure(uri);
        }

        public AppHttpClient(string uri, bool RequestAuthorization)
        {

            Configure(uri);

            if (RequestAuthorization)
            {
                if (HttpContext.Current.Session["user_token"] != null)
                {
                    DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Current.Session["user_token"].ToString());
                }
            }
        }

        public void Configure(string uri)
        {
            BaseAddress = new Uri(uri);
            DefaultRequestHeaders.Accept.Clear();
            DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

    }
}