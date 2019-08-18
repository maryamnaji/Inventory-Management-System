using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class AddProductsViewModel:BaseViewModel
    {
        public AddProductsWindow CurrentWindows { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand DoneCommand { get; set; }
        public ICommand DeleteCommand { get; set; }


        public Order Order { get; set; }

        private List<Product> _products;
        public List<Product> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                OnPropertyChanged("Products");
            }
        }

        private ObservableCollection<OrderDetail> _details;
        public ObservableCollection<OrderDetail> Details
        {
            get { return _details; }
            set
            {
                _details = value;
                OnPropertyChanged("Details");
            }
        }

        public OrderDetail SelectedDetail { get; set; }

        public OrderDetail Detail { get; set; }

        public AddProductsViewModel(AddProductsWindow window ,Order order)
        {
            this.CurrentWindows = window;
            this.Order = order;
            this.Detail = new OrderDetail();

            if (order.ID==0)
            {
                this.Details = new ObservableCollection<OrderDetail>();
            }
            else
            {
                this.Details = new ObservableCollection<OrderDetail>(order.Products);
            }

            this.CancelCommand = new RelayCommand(param => this.Cancel(), param => true);
            this.AddCommand = new RelayCommand(param => this.Add(), param => true);
            this.DoneCommand = new RelayCommand(param => this.Done(), param => true);
            this.DeleteCommand = new RelayCommand(param => this.Delete(), param => true);

            WarehouseDbContext ctx = new WarehouseDbContext();
            Products = new List<Product>(ctx.Products.ToList());
        }

        private void Done()
        {
            WarehouseDbContext ctx = new WarehouseDbContext();

            if (Order.ID==0)
            {
                Order.Products = new List<OrderDetail>();
                foreach (var item in Details)
                {
                    OrderDetail detail = new OrderDetail
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                    };
                    Order.Products.Add(detail);
                }

                ctx.Orders.Add(Order);
                ctx.SaveChanges();
            }
            else
            {
                //foreach (var item in Details)
                //{
                //    var isExist = Order.Products.Any(x => x.ProductId == item.ProductId);
                //    if (!isExist)
                //    {
                //        OrderDetail detail = new OrderDetail
                //        {
                //            ProductId = item.ProductId,
                //            Quantity = item.Quantity,
                //        };
                //        Order.Products.Add(detail);
                //    }
                //}

                //ctx.SaveChanges();
            }

            CurrentWindows.DialogResult=true;
            
            //OrdersWindow mywindow = new OrdersWindow();
            //mywindow.Show();
        }

        private void Add()
        {
            if (Detail.Quantity <1 || Detail.Product ==null)
            {
                MessageBox.Show(CurrentWindows, "Please prdoduct and quantity !");
                return;
            }

            OrderDetail od = new OrderDetail
            {
                ProductId= Detail.Product.ID,
                Product= Detail.Product,
                Quantity= Detail.Quantity
            };
            Details.Add(od);
        }

        private void Cancel()
        {
            this.CurrentWindows.Close();
        }

        private void Delete()
        {
            if (SelectedDetail == null)
            {
                MessageBox.Show(CurrentWindows, "Please select a user before deleting.");
                return;
            }

            MessageBoxResult result = MessageBox.Show(CurrentWindows, "Are you sure you want to delete:\n" +
               SelectedDetail.ProductId, "Confirm delete", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if (result == MessageBoxResult.OK)
            {
                try
                {
                    if (Order.ID != 0)
                    {
                        WarehouseDbContext ctx = new WarehouseDbContext();
                        var detail = ctx.Details.Single(u => u.ID == SelectedDetail.ID);
                        ctx.Details.Remove(detail);
                        ctx.SaveChanges();
                    }

                    Details.Remove(SelectedDetail);
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(CurrentWindows, ex.Message, "Database error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
