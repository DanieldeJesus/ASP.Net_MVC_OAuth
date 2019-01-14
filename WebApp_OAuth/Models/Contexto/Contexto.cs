using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebApp_OAuth.Models.Contexto
{
    public class Contexto : IdentityDbContext<ApplicationUser>
    {
        public Contexto()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<User> Client { get; set; }
        public DbSet<Cliente> Clientes { get; set; }

        public static Contexto Create()
        {
            return new Contexto();
        }
    }
}