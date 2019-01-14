using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI_OAuth.Models
{
    public class Cliente
    {
        public int ClienteId { get; set; }
        public string Nome { get; set; }
        public string Sexo { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public DateTime DtNascimento { get; set; }
    }
}