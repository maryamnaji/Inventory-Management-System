using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Warehouse.Models;
using Warehouse.ViewModels;

namespace Warehouse.Views
{
    /// <summary>
    /// Interaction logic for AddProductsWindows.xaml
    /// </summary>
    public partial class AddProductsWindow : Window
    {
        public AddProductsWindow(Order order)
        {
            InitializeComponent();
            
            //Binding view with ViewModel
            DataContext = new AddProductsViewModel(this, order);
        }
    }
}
