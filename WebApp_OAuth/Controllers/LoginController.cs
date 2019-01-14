using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApp_OAuth.App_Start.Identity;
using WebApp_OAuth.Models;
using WebApp_OAuth.Services;
using WebApp_OAuth.Utils;

namespace WebApp_OAuth.Controllers
{
    public class LoginController : Controller
    {
        private UsersService _service;
        private static AplicationOptions _options;

        public LoginController()
        {  
            _options = new AplicationOptions();            
            _options.ApiUrl = System.Configuration.ConfigurationManager.AppSettings["UrlWebAPI"];
            _service = new UsersService(_options);
        }

        public LoginController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, AplicationOptions options)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            _service = new UsersService(options);
        }

        // Definindo a instancia UserManager presente no request.
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // Definindo a instancia SignInManager presente no request.
        private ApplicationSignInManager _signInManager;
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        // GET: Users
        public ActionResult Index()
        {
            return View();
        }     

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(User user)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index", "Login");

            using (var client = new HttpClient())
            {
                
                var json = JsonConvert.SerializeObject(user);
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                var response = client.PostAsync(_options.ApiUrl + "login", stringContent).Result;

                FormUrlEncodedContent credentialsContent = new FormUrlEncodedContent(new Dictionary<string, string> { { "username", user.Login }, { "password", user.Password }, { "scope", "read" }, { "grant_type", "password" } });

                if (response.IsSuccessStatusCode)
                {
                    var responseJsonText = await response.Content.ReadAsStringAsync();

                    var objJson = JObject.Parse(responseJsonText);
                    var _token = objJson["access_token"].ToString();
                    var username = objJson["userName"].ToString(); ;
                    var _token_type = objJson["token_type"].ToString();

                    Session.Add("user_token", _token);
                    Session.Add("usuario_logado", username);
                    Session.Add("login", user.Login);

                    var result = await SignInManager.PasswordSignInAsync(user.Login, user.Password, user.RememberMe, shouldLockout: true);

                    switch (result)
                    {
                        case SignInStatus.Success:
                            var _user = await UserManager.FindAsync(user.Login, user.Password);
                            await SignInAsync(_user, user.RememberMe);
                            return RedirectToAction("Index", "Home");
                        case SignInStatus.LockedOut:
                            return View("Lockout");
                        case SignInStatus.Failure:
                        default:
                            ModelState.AddModelError("", "Login ou Senha incorretos.");
                            ViewBag.Error = "Seu usuário ou senha estão incorretos";
                            return View("~/Views/Login/Index.cshtml");
                    }
                }
                else
                {
                    ViewBag.Error = "Seu usuário ou senha estão incorretos";
                }

                return View("~/Views/Login/Index.cshtml");
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool rememberMe)
        {
            var clientKey = Request.Browser.Type;
            await UserManager.SignInClientAsync(user, clientKey);
            // Zerando contador de logins errados.
            await UserManager.ResetAccessFailedCountAsync(user.Id);

            // Coletando Claims externos (se houver)
            ClaimsIdentity ext = await AuthenticationManager.GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn
                (
                    new AuthenticationProperties { IsPersistent = rememberMe },
                    // Criação da instancia do Identity e atribuição dos Claims
                    await user.GenerateUserIdentityAsync(UserManager, ext)
                );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(User user)
        {   
            if (ModelState.IsValid)
            {
                if(user.Password.Equals(user.ConfirmPassword))
                { 
                    //Ativar Usuário
                    user.Ativo = true;

                    var _user = new ApplicationUser { UserName = user.Login, Email = user.Email };
                    var result = await UserManager.CreateAsync(_user, user.Password);
                    if (result.Succeeded)
                    {
                        //Salvar Novo Usuario
                        var retorno = _service.Create(user);
                        ViewBag.Success = "Usuário registrado";
                    }
                    else
                    {
                        ViewBag.Error = ((string[])result.Errors)[0];
                    }
                }
                else{
                    ViewBag.Error = "As Passwords não correspondem";
                }
            }

            return View("~/Views/Login/Index.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> SignOut()
        {
            var clientKey = Request.Browser.Type;
            var user = UserManager.FindById(User.Identity.GetUserId());
            await UserManager.SignOutClientAsync(user, clientKey);
            AuthenticationManager.SignOut();

            Session.RemoveAll();

            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(User user)
        {
            //Recuperar Senha

            return View();
        }        
    }
}