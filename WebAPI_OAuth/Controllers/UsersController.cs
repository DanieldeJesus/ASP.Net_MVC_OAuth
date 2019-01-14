using Microsoft.Extensions.Options;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI_OAuth.Models;
using WebAPI_OAuth.Models.DBContext;

namespace WebAPI_OAuth.Controllers
{
    [Authorize]
    public class UsersController : ApiController
    {
        private WebAPIDBContext db = new WebAPIDBContext();

        // GET: api/Users
        [HttpGet]
        public IEnumerable<User> GetUsers()
        {
            return db.Users;
        }

        // GET: api/Users/5
        [ResponseType(typeof(User))]
        public IHttpActionResult GetUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUser(int id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.UserId)
            {
                return BadRequest();
            }

            if(user.Password == null)
            {
                User _user = db.Users.Find(id);
                user.Password = _user.Password;
            }

            var local = db.Set<User>()
                         .Local
                         .FirstOrDefault(f => f.UserId == id);

            if (local != null)
            {
                db.Entry(local).State = EntityState.Detached;
            }
            db.Entry(user).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Users
        [AllowAnonymous]
        [ResponseType(typeof(User))]
        public IHttpActionResult PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Users.Add(user);
            db.SaveChanges();
            
            return CreatedAtRoute("DefaultApi", new { id = user.UserId }, user);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<HttpResponseMessage> LoginUser([FromBody]User user)
        {
            var request = HttpContext.Current.Request;
            var tokenServiceUrl = request.Url.GetLeftPart(UriPartial.Authority) + request.ApplicationPath + "token";
            using (var client = new HttpClient())
            {
                var requestParams = new List<KeyValuePair<string, string>>
                                    {
                                        new KeyValuePair<string, string>("grant_type", "password"),
                                        new KeyValuePair<string, string>("username", user.Login),
                                        new KeyValuePair<string, string>("password", user.Password)
                                    };

                var requestParamsFormUrlEncoded = new FormUrlEncodedContent(requestParams);
                var tokenServiceResponse = await client.PostAsync(tokenServiceUrl, requestParamsFormUrlEncoded);
                var responseString = await tokenServiceResponse.Content.ReadAsStringAsync();
                var responseCode = tokenServiceResponse.StatusCode;
                var responseMsg = new HttpResponseMessage(responseCode)
                {
                    Content = new StringContent(responseString, Encoding.UTF8, "application/json")
                };
                return responseMsg;
            }
        }
        
        [ResponseType(typeof(User))]
        public IHttpActionResult DeleteUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            db.SaveChanges();

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.Users.Count(e => e.UserId == id) > 0;
        }

        private Task<ClaimsIdentity> GetClaimsIdentity(User user)
        {
            var usuario = (from a in db.Users
                           where a.Login == user.Login
                           select new User
                           {
                               UserId = a.UserId,
                               Name = a.Name,
                               Login = a.Login,
                               Email = a.Email,
                               Password = a.Password
                           }).FirstOrDefault();
            
            if (usuario != null)
            {
                var userClaims = new ClaimsIdentity(
                        new GenericIdentity(user.Login, "Token"),
                        new[] {
                                new Claim("Auth", "WebApi"),
                                new Claim("login",  usuario.Login),
                                new Claim("username", usuario.Name)
                        }
                    );
                return Task.FromResult(userClaims);
            }
            else
            {
                throw new Exception("Sem permissão de acesso!");
            }
        }
    }
}