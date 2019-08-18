using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Warehouse.Models;
using Warehouse.ViewModels;

namespace Warehouse.Views
{
    /// <summary>
    /// Interaction logic for ProductsWindow.xaml
    /// </summary>
    public partial class ProductsWindow : Window
    {
        public ProductsWindow()
        {
            InitializeComponent();

            //Binding view with ViewModel
            DataContext = new ProductsViewModel(this);
        }
    }
}
