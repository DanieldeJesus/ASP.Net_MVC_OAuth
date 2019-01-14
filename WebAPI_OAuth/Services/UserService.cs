using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebAPI_OAuth.Models;
using WebAPI_OAuth.Models.DBContext;

namespace WebAPI_OAuth.Services
{
    public class UserService : IDisposable
    {
       static  WebAPIDBContext _entities;

        public UserService()
        {
            _entities = new WebAPIDBContext();
        }

        public static bool Login(string login, string password)
        {
            using (WebAPIDBContext entities = new WebAPIDBContext())
            {
                var result = entities.Users.Any(user =>
                               user.Login.Equals(login, StringComparison.OrdinalIgnoreCase)
                               && user.Password == password);

                entities.Dispose();
                return result;
            }            
        }

        public static User GetUserByCredentials(string login, string password)
        {
            using (WebAPIDBContext entities = new WebAPIDBContext())
            {
                var result = entities.Users.FirstOrDefault(user =>
                               user.Login.Equals(login, StringComparison.OrdinalIgnoreCase)
                               && user.Password == password);

                entities.Dispose();
                return result;
            }
        }

        public void Dispose()
        {
            _entities.Dispose();
        }
    }
 }