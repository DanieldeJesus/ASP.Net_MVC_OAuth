namespace WebAPI_OAuth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class All : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clientes",
                c => new
                    {
                        ClienteId = c.Int(nullable: false, identity: true),
                        Nome = c.String(),
                        Sexo = c.String(),
                        Telefone = c.String(),
                        Email = c.String(),
                        DtNascimento = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ClienteId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Login = c.String(),
                        Password = c.String(),
                        Name = c.String(),
                        Email = c.String(),
                        Ativo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
            DropTable("dbo.Clientes");
        }
    }
}
