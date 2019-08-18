using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Warehouse.Models;
using Warehouse.ViewModels;
using Warehouse.Views;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestUserViewModel_AtLeastOneUser()
        {
            UsersViewModel model = new UsersViewModel(new UsersWindow());
            Assert.AreNotEqual(0, model.UserList.Count, "By default we should have at lease one user in database");
        }

        [TestMethod]
        public void TestUserViewModel_AddUser()
        {
            UsersViewModel model = new UsersViewModel(new UsersWindow());
            model.Add();
            model.IsNew = true;
            model.SelectedUser.Name = "Unit Test";
            model.SelectedUser.Email = "unit@test.com";
            model.SelectedUser.Password = "1";
            model.Save();

            WarehouseDbContext ctx = new WarehouseDbContext();
            var user = ctx.Users.SingleOrDefault(u=>u.Name == "Unit Test");

            Assert.AreNotEqual(null, user, "User added !");

            //remove unit test user
            ctx.Users.Remove(user);
            ctx.SaveChanges();
        }
    }
}
