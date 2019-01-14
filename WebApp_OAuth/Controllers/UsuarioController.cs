using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApp_OAuth.App_Start.Identity;
using WebApp_OAuth.Models;
using WebApp_OAuth.Services;

namespace WebApp_OAuth.Controllers
{
    public class UsuarioController : Controller
    {
        private UsersService _service;
        private static AplicationOptions _options;

        public UsuarioController()
        {
            _options = new AplicationOptions();            
            _options.ApiUrl = System.Configuration.ConfigurationManager.AppSettings["UrlWebAPI"];
            _service = new UsersService(_options);
        }

        public UsuarioController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
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

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        // GET: Usuario
        public async Task<ActionResult> Index()
        {
            ViewBag.Usuarios = await _service.GetUsers();
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetUsuario(int usuarioId)
        {
            User _data = new User();
            var result = await _service.GetUsersId(usuarioId);

            if (result.IsSuccessStatusCode)
                {
                    var responseJsonText = await result.Content.ReadAsStringAsync();
                    _data = JsonConvert.DeserializeObject<User>(responseJsonText);
                }

            return Json(_data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> EditarUsuario(User user)
        {
            try
            {                
                #region Update Entity
                // get user object from the storage
                var _user = await UserManager.FindByNameAsync(user.Login);

                if (user.Email != _user.Email)
                {
                    _user.Email = user.Email;

                    // Persiste the changes
                    await UserManager.UpdateAsync(_user);
                }

                if (user.Password != null && user.ConfirmPassword != null && user.OldPassword != null)
                {
                    if (user.Password.Equals(user.ConfirmPassword))
                    {
                        var IdUserLogado = User.Identity.GetUserId();
                        var result = await UserManager.ChangePasswordAsync(_user.Id, user.OldPassword, user.Password);

                        //Executar SignInAsync apenas se o usuario alterado for o usuario logado
                        if(IdUserLogado.Equals(_user.Id))
                        { 
                            if (result.Succeeded)
                            {
                                var userUpdate = await UserManager.FindByIdAsync(_user.Id);
                                if (userUpdate != null)
                                {
                                    await SignInAsync(userUpdate, isPersistent: false);
                                }
                            }
                        }
                    }
                }
                #endregion

               var resultAPI =  _service.EditarUser(user);
               if (resultAPI)
                   TempData["success"] = "Usuário alterado com sucesso.";
               else
                   TempData["error"] = "Usuário não foi alterado.";
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }

            return RedirectToAction("Index", "Usuario");
        }

        [HttpPost]
        public async Task<ActionResult> ExcluirUsuario(int idUsuario, string login)
        {
            try
            {
                #region Remove Entity
                var _user = await UserManager.FindByNameAsync(login);

                //List Logins associated with user
                var _logins = _user.Logins;

                //Gets list of Roles associated with current user
                var rolesForUser = await UserManager.GetRolesAsync(_user.Id);


                foreach (var _login in _logins.ToList())
                {
                    await UserManager.RemoveLoginAsync(_login.UserId, new UserLoginInfo(_login.LoginProvider, _login.ProviderKey));
                }

                if (rolesForUser.Count() > 0)
                {
                    foreach (var item in rolesForUser.ToList())
                    {
                        // item should be the name of the role
                        var result = await UserManager.RemoveFromRoleAsync(_user.Id, item);
                    }
                }

                //Delete User
                var resultRemoveEntity = await UserManager.DeleteAsync(_user); 
                #endregion

                if (resultRemoveEntity.Succeeded)
                {
                    var resultAPI = await _service.DeleteUser(idUsuario);

                    if (resultAPI)
                        TempData["success"] = "Usuário excluido com sucesso.";
                    else
                        TempData["error"] = "Usuário não foi excluido.";
                }
                else
                {
                    TempData["error"] = "Usuário não foi excluido.";
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }                         

            return RedirectToAction("Index", "Usuario");
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            var clientKey = Request.Browser.Type;
            await UserManager.SignInClientAsync(user, clientKey);
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
        }
    }
}