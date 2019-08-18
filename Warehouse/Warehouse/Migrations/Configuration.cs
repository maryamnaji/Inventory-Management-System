namespace Warehouse.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Warehouse.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<Warehouse.Models.WarehouseDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }


        //define database with seed data
        protected override void Seed(Warehouse.Models.WarehouseDbContext context)
        {
            context.Users.AddOrUpdate(new User { Name = "System", Email = "admin@ims.com", Password = "123456" });
        }
    }
}
