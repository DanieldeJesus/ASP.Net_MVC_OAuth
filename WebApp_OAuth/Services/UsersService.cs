using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApp_OAuth.App_Start.Identity;
using WebApp_OAuth.Models;
using WebApp_OAuth.Utils;

namespace WebApp_OAuth.Services
{
    public class UsersService
    {
        private readonly HttpClient client;
        private readonly AppHttpClient _client;
        private static AplicationOptions _options;

        public UsersService(AplicationOptions options)
        {
            _options = options;
            this.client = new HttpClient();
            this._client = new AppHttpClient(options.ApiUrl, true);
        }

        public async Task<List<User>> GetUsers()
        {
            List<User> _listUsers = new List<User>();
            var response = await _client.GetAsync(_options.ApiUrl + "api/Users");

            if (response.IsSuccessStatusCode)
            {
                var jsonUsers = await response.Content.ReadAsStringAsync();
                _listUsers = JsonConvert.DeserializeObject<List<User>>(jsonUsers);

                if (_listUsers == null || _listUsers.Count == 0)
                    return new List<User>();
            }

            return _listUsers;
        }

        public async Task<HttpResponseMessage> GetUsersId(int usuarioId)
        {
            return await _client.GetAsync(_options.ApiUrl + "api/Users/" + usuarioId);
        }

        public async Task<Boolean> DeleteUser(int usuarioId)
        {
            var result = await _client.DeleteAsync(_options.ApiUrl + "api/Users/" + usuarioId);

            return result.IsSuccessStatusCode;
        }

        public Boolean EditarUser(User user)
        {
            var json = JsonConvert.SerializeObject(user);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            return _client.PutAsync(_options.ApiUrl + "api/Users/" + user.UserId, stringContent).Result.IsSuccessStatusCode;
        }

        public Boolean Create(User user)
        {
            var usuario = new IdentityUser()
            {
                UserName = user.Login
            };
            var json = JsonConvert.SerializeObject(user);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var response = client.PostAsync(_options.ApiUrl + "api/Users", stringContent).Result;

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }
    }
}