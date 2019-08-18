using System;
using System.Windows;
using System.Windows.Input;
using Warehouse.Helpers;
using Warehouse.Views;

namespace Warehouse.ViewModels
{
    public class HomeViewModel:BaseViewModel
    {
        public ICommand UsersCommand { get; set; }
        public ICommand CustomersCommand { get; set; }
        public ICommand ProductsCommand { get; set; }
        public ICommand OrdersCommand { get; set; }
        public ICommand ExitCommand { get; set; }

        public HomeViewModel()
        {
            this.UsersCommand= new RelayCommand(param => this.UsersManagement(), param => true);
            this.CustomersCommand = new RelayCommand(param => this.CustomersManagement(), param => true);
            this.ProductsCommand = new RelayCommand(param => this.ProductsManagement(), param => true);
            this.OrdersCommand = new RelayCommand(param => this.OrdersManagement(), param => true);
            this.ExitCommand = new RelayCommand(param => this.Exit(), param => true);
        }

        private void Exit()
        {
            MessageBoxResult rsltMessageBox = MessageBox.Show("Do you want to exit?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            switch (rsltMessageBox)
            {
                case MessageBoxResult.Yes:
                    Application.Current.Shutdown();
                    break;

                case MessageBoxResult.No:
                    break;
            }
        }

        private void OrdersManagement()
        {
            OrdersWindow myWindow = new OrdersWindow();
            myWindow.Show();
        }

        private void ProductsManagement()
        {
            ProductsWindow myWindow = new ProductsWindow();
            myWindow.Show();
        }

        private void CustomersManagement()
        {
            CustomersWindow myWindow = new CustomersWindow();
            myWindow.Show();
        }

        private void UsersManagement()
        {
            UsersWindow myWindow = new UsersWindow();
            myWindow.Show();
        }
    }
}
