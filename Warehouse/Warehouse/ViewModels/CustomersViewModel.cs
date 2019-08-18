using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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
    class CustomersViewModel : BaseViewModel
    {
        public CustomersWindow CurrentWindows { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ExportCommand { get; set; }


        private List<Customer> _customerList;
        public List<Customer> CustomerList
        {
            get { return _customerList; }
            set
            {
                _customerList = value;
                OnPropertyChanged("CustomerList");
            }
        }

        private Customer _selectedCustomer;
        public Customer SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                _selectedCustomer = value;
                IsNew = false;
                OnPropertyChanged("SelectedCustomer");
            }
        }

        public bool IsNew { get; set; }
        public string SearchTerm { get; set; }

        //Constructor
        public CustomersViewModel(CustomersWindow window)
        {
            this.CurrentWindows = window;
            this.Add();

            this.CancelCommand = new RelayCommand(param => this.Cancel(), param => true);
            this.AddCommand = new RelayCommand(param => this.Add(), param => true);
            this.SaveCommand = new RelayCommand(param => this.Save(), param => true);
            this.DeleteCommand = new RelayCommand(param => this.Delete(), param => true);
            this.SearchCommand = new RelayCommand(param => this.Search(), param => true);
            this.ExportCommand = new RelayCommand(param => this.Export(), param => true);

            //Load data from database
            WarehouseDbContext ctx = new WarehouseDbContext();
            CustomerList = new List<Customer>(ctx.Customers.ToList());
        }

        private void Export()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = @"C:\";
            saveFileDialog1.Title = "Save csv Files";
            saveFileDialog1.CheckFileExists = false;
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.DefaultExt = "csv";
            saveFileDialog1.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == true)
            {
                try
                {
                    foreach (var item in CustomerList)
                    {
                        File.AppendAllText(saveFileDialog1.FileName, item.ToString() + Environment.NewLine);
                    }
                }
                catch (IOException ex)
                {
                    MessageBox.Show(CurrentWindows, "Failed to write data to file.\n" + ex.Message, "File error");
                }
            }
        }

        private void Search()
        {
            WarehouseDbContext ctx = new WarehouseDbContext();
            CustomerList = new List<Customer>(ctx.Customers.ToList().Where(u => u.Name.Contains(SearchTerm)));
        }

        private void Delete()
        {
            if (SelectedCustomer ==null || SelectedCustomer.ID ==0)
            {
                MessageBox.Show(CurrentWindows, "Please select a user before deleting.");
                return;
            }

            MessageBoxResult result = MessageBox.Show(CurrentWindows, "Are you sure you want to delete:\n" +
               SelectedCustomer.Name, "Confirm delete", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if (result == MessageBoxResult.OK)
            {
                try
                {
                    WarehouseDbContext ctx = new WarehouseDbContext();
                    var customer = ctx.Customers.Single(u => u.ID == SelectedCustomer.ID);
                    ctx.Customers.Remove(customer);
                    ctx.SaveChanges();

                    CustomerList = new List<Customer>(ctx.Customers.ToList());
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(CurrentWindows, ex.Message, "Database error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Save()
        {
            if (!IsNew)
            {
                if (SelectedCustomer == null)
                {
                    MessageBox.Show(CurrentWindows, "Please select a user before editting.");
                    return;
                }

                WarehouseDbContext ctx = new WarehouseDbContext();
                var customer = ctx.Customers.Single(u => u.ID == SelectedCustomer.ID);

                customer.Name = SelectedCustomer.Name;
                customer.CompanyName = SelectedCustomer.CompanyName;
                customer.Address = SelectedCustomer.Address;
                customer.City = SelectedCustomer.City;
                customer.Telephone = SelectedCustomer.Telephone;
                customer.Email = SelectedCustomer.Email;

                ctx.SaveChanges();

                MessageBox.Show(CurrentWindows, "Updated successfully !");
            }
            else
            {
                WarehouseDbContext ctx = new WarehouseDbContext();

                ctx.Customers.Add(SelectedCustomer);
                ctx.SaveChanges();

                CustomerList = new List<Customer>(ctx.Customers.ToList());

                IsNew = false;
            }

        }

        private void Add()
        {
            SelectedCustomer = new Customer();
            IsNew = true;
        }

        private void Cancel()
        {
            this.CurrentWindows.Close();
        }
    }
}
