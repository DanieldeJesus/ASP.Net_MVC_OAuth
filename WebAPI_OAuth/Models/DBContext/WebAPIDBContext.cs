using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace WebAPI_OAuth.Models.DBContext
{
    public class WebAPIDBContext : DbContext
    {
        public WebAPIDBContext() : base("name=WebAPIDBContext")
        { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Cliente> Clientes { get; set; }
    }
}