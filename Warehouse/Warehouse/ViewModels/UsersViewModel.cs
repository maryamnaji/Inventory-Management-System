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
    public class UsersViewModel : BaseViewModel
    {
        public UsersWindow CurrentWindows { get; set; }

        public ICommand CancelCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ExportCommand { get; set; }


        private List<User> _userList;
        public List<User> UserList
        {
            get { return _userList; }
            set
            {
                _userList = value;
                OnPropertyChanged("UserList");
            }
        }

        private User _selectedUser;
        public User SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                OnPropertyChanged("SelectedUser");
            }
        }

        public bool IsNew { get; set; }
        public string SearchTerm { get; set; }

        //Constructor
        public UsersViewModel(UsersWindow window)
        {
            this.CurrentWindows = window;
            this.Add();

            this.CancelCommand=new RelayCommand(param => this.Cancel(), param => true);
            this.AddCommand = new RelayCommand(param => this.Add(), param => true);
            this.SaveCommand = new RelayCommand(param => this.Save(), param => true);
            this.DeleteCommand = new RelayCommand(param => this.Delete(), param => true);
            this.SearchCommand = new RelayCommand(param => this.Search(), param => true);
            this.ExportCommand = new RelayCommand(param => this.Export(), param => true);

            //Load data from database
            WarehouseDbContext ctx = new WarehouseDbContext();
            UserList = new List<User>(ctx.Users.ToList());
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
                    foreach (var item in UserList)
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
            UserList = new List<User>(ctx.Users.ToList().Where(u => u.Name.Contains(SearchTerm)));
        }

        private void Delete()
        {
            if (SelectedUser == null)
            {
                MessageBox.Show(CurrentWindows, "Please select a user before deleting.");
                return;
            }

            MessageBoxResult result = MessageBox.Show(CurrentWindows, "Are you sure you want to delete:\n" +
               SelectedUser.Name, "Confirm delete", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if (result == MessageBoxResult.OK)
            {
                try
                {
                    WarehouseDbContext ctx = new WarehouseDbContext();
                    var user= ctx.Users.Single(u => u.ID == SelectedUser.ID);
                    ctx.Users.Remove(user);
                    ctx.SaveChanges();

                    //Reload data from databse
                    UserList = new List<User>(ctx.Users.ToList());
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(CurrentWindows, ex.Message, "Database error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void Save()
        {
            if (!IsNew)
            {
                //Edit existing user
                if (SelectedUser == null)
                {
                    MessageBox.Show(CurrentWindows, "Please select a user before editting.");
                    return;
                }

                WarehouseDbContext ctx = new WarehouseDbContext();
                var user = ctx.Users.Single(u => u.ID == SelectedUser.ID);

                user.Name = SelectedUser.Name;
                user.Address = SelectedUser.Address;
                user.City = SelectedUser.City;
                user.Tel = SelectedUser.Tel;
                user.Email = SelectedUser.Email;
                user.Password = SelectedUser.Password;
                user.LastLoginDate = SelectedUser.LastLoginDate;

                ctx.SaveChanges();

                MessageBox.Show(CurrentWindows, "Updated successfully !");
            }
            else
            {
                //Add new user
                WarehouseDbContext ctx = new WarehouseDbContext();
                ctx.Users.Add(SelectedUser);
                ctx.SaveChanges();

                //Reload data from databse
                UserList = new List<User>(ctx.Users.ToList());
                IsNew = false;
            }

        }

        public void Add()
        {
            SelectedUser = new User();
            IsNew = true;
        }

        private void Cancel()
        {
            this.CurrentWindows.Close();
        }
    }
}
