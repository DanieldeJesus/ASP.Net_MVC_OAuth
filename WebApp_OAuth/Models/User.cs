using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApp_OAuth.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        [Display(Name = "Confirmar Password")]
        public string ConfirmPassword { get; set; }
        public string OldPassword { get; set; }

        [Display(Name = "Nome")]
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "E-mail em formato inválido.")]
        public string Email { get; set; }

        public bool RememberMe { get; set; }
        public bool Ativo { get; set; }
    }
}