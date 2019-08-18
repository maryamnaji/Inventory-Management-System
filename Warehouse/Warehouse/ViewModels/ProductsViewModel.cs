using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public class ProductsViewModel:BaseViewModel
    {
        public ProductsWindow CurrentWindows { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ExportCommand { get; set; }
        public ICommand AddImageCommand { get; set; }


        private List<Product> _productList;
        public List<Product> ProductList
        {
            get { return _productList; }
            set
            {
                _productList = value;
                OnPropertyChanged("ProductList");
            }
        }

        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                if (_selectedProduct !=null && _selectedProduct.Category != null)
                {
                    SelectedCategory = _selectedProduct.Category.Name;
                }
                OnPropertyChanged("SelectedProduct");
                IsNew = false;
            }
        }

        public bool IsNew { get; set; }
        public string SearchTerm { get; set; }

        private List<Category> _categories;
        public List<Category> Categories
        {
            get { return _categories; }
            set
            {
                _categories = value;
                OnPropertyChanged("Categories");
            }
        }

        private string _selectedCategory;
        public string SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                OnPropertyChanged("SelectedCategory");
            }
        }
        

        //Constructor
        public ProductsViewModel(ProductsWindow window)
        {
            this.CurrentWindows = window;
            this.Add();

            this.CancelCommand = new RelayCommand(param => this.Cancel(), param => true);
            this.AddCommand = new RelayCommand(param => this.Add(), param => true);
            this.SaveCommand = new RelayCommand(param => this.Save(), param => true);
            this.DeleteCommand = new RelayCommand(param => this.Delete(), param => true);
            this.SearchCommand = new RelayCommand(param => this.Search(), param => true);
            this.ExportCommand = new RelayCommand(param => this.Export(), param => true);
            this.AddImageCommand = new RelayCommand(param => this.AddImage(), param => true);

            //Load data from database
            WarehouseDbContext ctx = new WarehouseDbContext();
            ProductList = new List<Product>(ctx.Products.ToList());
            Categories = new List<Category>(ctx.Categories.ToList());

        }

        private void AddImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = @"C:\";
            openFileDialog.Title = "Open Files";
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.DefaultExt = "jpg";
            openFileDialog.Filter = "image files (*.jpg)|*.png|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == true)
            {
                SelectedProduct.Image = File.ReadAllBytes(openFileDialog.FileName);
                OnPropertyChanged("SelectedProduct");
            }
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
                    foreach (var item in ProductList)
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
            ProductList = new List<Product>(ctx.Products.ToList().Where(u => u.Name.Contains(SearchTerm) || u.Description.Contains(SearchTerm)));
        }

        private void Delete()
        {
            if (SelectedProduct == null)
            {
                MessageBox.Show(CurrentWindows, "Please select a user before deleting.");
                return;
            }

            MessageBoxResult result = MessageBox.Show(CurrentWindows, "Are you sure you want to delete:\n" +
               SelectedProduct.Name, "Confirm delete", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if (result == MessageBoxResult.OK)
            {
                try
                {
                    WarehouseDbContext ctx = new WarehouseDbContext();
                    var product = ctx.Products.Single(u => u.ID == SelectedProduct.ID);
                    ctx.Products.Remove(product);
                    ctx.SaveChanges();

                    ProductList = new List<Product>(ctx.Products.ToList());
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(CurrentWindows, ex.Message, "Database error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Save()
        {
            WarehouseDbContext ctx = new WarehouseDbContext();
            if (!IsNew)
            {
                if (SelectedProduct == null)
                {
                    MessageBox.Show(CurrentWindows, "Please select a user before editting.");
                    return;
                }

                var cat = ctx.Categories.SingleOrDefault(x => x.Name == SelectedCategory);
                if (cat == null)
                {
                    cat = ctx.Categories.Add(new Category { Name = SelectedCategory });
                    ctx.SaveChanges();
                }

                var product = ctx.Products.Single(u => u.ID == SelectedProduct.ID);

                product.Name = SelectedProduct.Name;
                product.Category = cat;
                product.Price = SelectedProduct.Price;
                product.Description = SelectedProduct.Description;
                product.Image = SelectedProduct.Image;

                ctx.SaveChanges(); 
            }
            else
            {
                var cat = ctx.Categories.SingleOrDefault(x=>x.Name == SelectedCategory);

                if (cat ==null)
                {
                    cat=ctx.Categories.Add(new Category { Name = SelectedCategory });
                    ctx.SaveChanges();
                }

                SelectedProduct.Category = cat;
                ctx.Products.Add(SelectedProduct);
                ctx.SaveChanges();
            }

            //Reload data from database
            ProductList = new List<Product>(ctx.Products.ToList());
            Categories = new List<Category>(ctx.Categories.ToList());
            IsNew = false;

            MessageBox.Show(CurrentWindows, "Updated successfully !");
        }

        private void Add()
        {
            SelectedProduct = new Product();
            IsNew = true;
        }

        private void Cancel()
        {
            this.CurrentWindows.Close();
        }
    }
}
