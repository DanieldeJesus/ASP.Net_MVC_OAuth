using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp_OAuth.Models
{
    public class AplicationOptions
    {
        private string _ApiUrl;

        public string ApiUrl
        {
            get
            {
                return _ApiUrl;
            }
            set
            {
                var url = System.Configuration.ConfigurationManager.AppSettings["UrlWebAPI"];

                if (url.Substring(url.Length - 1, 1) != "/")
                {
                    _ApiUrl = url + "/";
                }
                else
                {
                    _ApiUrl = url;
                }
            }
        }
    }
}