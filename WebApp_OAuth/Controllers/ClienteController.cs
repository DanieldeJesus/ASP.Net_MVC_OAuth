using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApp_OAuth.App_Start.Identity;
using WebApp_OAuth.Models;
using WebApp_OAuth.Services;

namespace WebApp_OAuth.Controllers
{
    public class ClienteController : Controller
    {
        private ClientesService _service;
        private static AplicationOptions _options;

        public ClienteController()
        {
            _options = new AplicationOptions();
            _options.ApiUrl = System.Configuration.ConfigurationManager.AppSettings["UrlWebAPI"];
            _service = new ClientesService(_options);
        }

        public ClienteController(ApplicationUserManager userManager)
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
        // GET: Campo
        public async Task<ActionResult> Index()
        {
            ViewBag.Clientes = await _service.GetClientes();
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetCliente(int clienteId)
        {
            Cliente _data = new Cliente();
            var result = await _service.GetClienteId(clienteId);

            if (result.IsSuccessStatusCode)
            {
                var responseJsonText = await result.Content.ReadAsStringAsync();
                _data = JsonConvert.DeserializeObject<Cliente>(responseJsonText);
            }

            return Json(_data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> ExcluirCliente(int idCliente)
        {
            try
            {
                var resultAPI = await _service.DeleteCliente(idCliente);

                if (resultAPI)
                    TempData["success"] = "Cliente excluido com sucesso.";
                else
                    TempData["error"] = "Cliente não foi excluido.";
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }

            return RedirectToAction("Index", "Cliente");
        }

        [HttpPost]
        public async Task<JsonResult> ExcluirClientes(List<Cliente> _listClientes)
        {
            try
            {
                var resultAPI = await _service.DeleteClientes(_listClientes);

                if (resultAPI)
                    TempData["success"] = "Cliente(s) excluido com sucesso.";
                else
                    TempData["error"] = "Cliente(s) não foi excluido.";
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }

            return Json(new { redirectUrl = Url.Action("Index", "Cliente", null), isRedirect = true });
        }

        [HttpPost]
        public ActionResult EditarCliente(Cliente cliente)
        {
            try
            {
                var resultAPI = _service.EditarCliente(cliente);
                if (resultAPI)
                    TempData["success"] = "Cliente alterado com sucesso.";
                else
                    TempData["error"] = "Cliente não foi alterado.";
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }

            return RedirectToAction("Index", "Cliente");
        }

        [HttpPost]
        public ActionResult NovoCliente(Cliente cliente)
        {
            var retorno = _service.NovoCliente(cliente);
            if (retorno)
            {
                TempData["success"] = "Cliente criado com sucesso.";
            }
            else
            {
                TempData["error"] = "Não foi possível criar o cliente";
            }

            return RedirectToAction("Index", "Cliente");
        }
    }
}