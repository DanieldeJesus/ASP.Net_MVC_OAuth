using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebApp_OAuth.Models;
using WebApp_OAuth.Utils;

namespace WebApp_OAuth.Services
{
    public class ClientesService
    {
        private readonly HttpClient client;
        private readonly AppHttpClient _client;
        private static AplicationOptions _options;

        public ClientesService(AplicationOptions options)
        {
            _options = options;
            this.client = new HttpClient();
            this._client = new AppHttpClient(options.ApiUrl, true);
        }

        public async Task<List<Cliente>> GetClientes()
        {
            List<Cliente> _listClientes = new List<Cliente>();
            var response = await _client.GetAsync(_options.ApiUrl + "api/Clientes");

            if (response.IsSuccessStatusCode)
            {
                var jsonCliente = await response.Content.ReadAsStringAsync();
                _listClientes = JsonConvert.DeserializeObject<List<Cliente>>(jsonCliente);

                if (_listClientes == null || _listClientes.Count == 0)
                    return new List<Cliente>();
            }

            return _listClientes;
        }

        public async Task<HttpResponseMessage> GetClienteId(int clienteId)
        {
            return await _client.GetAsync(_options.ApiUrl + "api/Clientes/" + clienteId);
        }

        public async Task<Boolean> DeleteCliente(int clienteId)
        {
            var result = await _client.DeleteAsync(_options.ApiUrl + "api/Clientes/" + clienteId);

            return result.IsSuccessStatusCode;
        }

        public async Task<Boolean> DeleteClientes(List<Cliente> listClientes)
        {
            var json = JsonConvert.SerializeObject(listClientes);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var result = await _client.PostAsync(_options.ApiUrl + "api/Clientes/ListClientes", stringContent);

            return result.IsSuccessStatusCode;
        }

        public Boolean EditarCliente(Cliente cliente)
        {
            var json = JsonConvert.SerializeObject(cliente);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            return _client.PutAsync(_options.ApiUrl + "api/Clientes/" + cliente.ClienteId, stringContent).Result.IsSuccessStatusCode;
        }

        public Boolean NovoCliente(Cliente cliente)
        {
            var json = JsonConvert.SerializeObject(cliente);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var result = _client.PostAsync(_options.ApiUrl + "api/Clientes", stringContent).Result;

            return result.IsSuccessStatusCode;
        }
    }
}