using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
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
    class OrdersViewModel : BaseViewModel
    {
        public OrdersWindow CurrentWindows { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand AddProductCommand { get; set; }
        public ICommand AddOrderCommand { get; set; }
        public ICommand DeleteCommand { get; set; }



        private List<Order> _orderList;
        public List<Order> OrderList
        {
            get { return _orderList; }
            set
            {
                _orderList = value;
                OnPropertyChanged("OrderList");
            }
        }

        private Order _selectedOrder;
        public Order SelectedOrder
        {
            get { return _selectedOrder; }
            set
            {
                _selectedOrder = value;
                if (_selectedOrder != null && _selectedOrder.Customer != null)
                {
                    SelectedCustomer = _selectedOrder.Customer;
                }
                OnPropertyChanged("SelectedOrder");
                IsNew = false;
            }
        }


        public bool IsNew { get; set; }
        public string SearchTerm { get; set; }

        private List<Customer> _customers;
        public List<Customer> Customers
        {
            get { return _customers; }
            set
            {
                _customers = value;
                OnPropertyChanged("Customers");
            }
        }

        private Customer _selectedCustomer;
        public Customer SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                _selectedCustomer = value;
                OnPropertyChanged("SelectedCustomer");
            }
        }

        public OrdersViewModel(OrdersWindow window)
        {
            this.CurrentWindows = window;
            AddOrder();

            this.CancelCommand = new RelayCommand(param => this.Cancel(), param => true);
            this.AddProductCommand = new RelayCommand(param => this.AddProduct(), param => true);
            this.AddOrderCommand = new RelayCommand(param => this.AddOrder(), param => true);
            this.DeleteCommand = new RelayCommand(param => this.Delete(), param => true);

            WarehouseDbContext ctx = new WarehouseDbContext();
            OrderList = new List<Order>(ctx.Orders.ToList());
            Customers = new List<Customer>(ctx.Customers.ToList());
        }

        private void AddOrder()
        {
            SelectedOrder = new Order { Date=DateTime.Now};
            IsNew = true;
        }

        private void AddProduct()
        {
            if (SelectedCustomer == null)
            {
                MessageBox.Show(CurrentWindows, "Please select a user before adding products.");
                return;
            }

            Order order;
            if (SelectedOrder.ID ==0)
            {
                order = new Order
                {
                    OrderType = SelectedOrder.OrderType,
                    Date = SelectedOrder.Date,
                    UserId = Int32.Parse(App.Current.Properties["CurrentUserID"].ToString()),
                    CustomerId = SelectedCustomer.ID,
                };
            }
            else
            {
                order = SelectedOrder;
            }

            AddProductsWindow myWindow = new AddProductsWindow(order);
            if (myWindow.ShowDialog()==true)
            {
                WarehouseDbContext ctx = new WarehouseDbContext();
                OrderList = new List<Order>(ctx.Orders.ToList());
            }
            
        }

        private void Cancel()
        {
            this.CurrentWindows.Close();
        }

        private void Delete()
        {
            if (SelectedOrder == null)
            {
                MessageBox.Show(CurrentWindows, "Please select on order before deleting!");
                return;
            }

            MessageBoxResult result = MessageBox.Show(CurrentWindows, "Are you sure you want to delete:\n" 
               , "Confirm delete", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if (result == MessageBoxResult.OK)
            {
                try
                {
                    WarehouseDbContext ctx = new WarehouseDbContext();
                    var order = ctx.Orders.Single(u => u.ID == SelectedOrder.ID);
                    ctx.Orders.Remove(order);
                    ctx.SaveChanges();

                    OrderList = new List<Order>(ctx.Orders.ToList());
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(CurrentWindows, ex.Message, "Database error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (DbUpdateException ex)
                {
                    MessageBox.Show(CurrentWindows, "Please delete the products before deleting the order.");
                }
            }
        }
    }
}
