using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Warehouse.Helpers;
using Warehouse.Models;
using Warehouse.Views;

namespace Warehouse.ViewModels
{
    public class LoginViewModel:BaseViewModel
    {
        public LoginWindow CurrentWindows { get; set; }
        public ICommand LoginCommand { get; set; }

        public User CurrentUser { get; set; }

        public LoginViewModel(LoginWindow windows)
        {
            CurrentWindows = windows;
            this.CurrentUser = new User();

            this.LoginCommand= new RelayCommand(param => this.Login(), param => true);
        }

        private void Login()
        {
            WarehouseDbContext ctx = new WarehouseDbContext();
            var user = ctx.Users.SingleOrDefault(u=>u.Email==CurrentUser.Email);

            if (user !=null && CurrentUser.Password==user.Password)
            {
                user.LastLoginDate = DateTime.Now;
                ctx.SaveChanges();

                HomeWindow myWindow = new HomeWindow();
                myWindow.Show();

                this.CurrentWindows.Close();

                //keep current user id in the application
                App.Current.Properties["CurrentUserID"] = user.ID;
            }
            else
            {
                MessageBox.Show(CurrentWindows, "Invalid email or password !");
                return;
            }
        }
    }
}
