using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Zoomagazin
{
    public partial class Polzovatel : Window
    {
        private readonly ZooMEntities _dbContext;

        public Polzovatel()
        {
            InitializeComponent();
            _dbContext = new ZooMEntities();
            LoadAllUsers();
        }

        private void LoadAllUsers()
        {
            UsersDataGrid.ItemsSource = _dbContext.Users.ToList();
        }

        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtFilter.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                LoadAllUsers();
            }
            else
            {
                var filteredUsers = _dbContext.Users
                    .Where(u => u.Username.ToLower().Contains(searchText) ||
                                u.Password.ToLower().Contains(searchText) ||
                                u.FullName.ToLower().Contains(searchText) ||
                                u.Position.ToLower().Contains(searchText))
                    .ToList();
                UsersDataGrid.ItemsSource = filteredUsers;
            }
        }

        private void UserAdd(object sender, RoutedEventArgs e)
        {
            var addUsersWindow = new AddUsers();
            addUsersWindow.Closed += AddUsersWindow_Closed;
            addUsersWindow.Show();
        }

        private void AddUsersWindow_Closed(object sender, EventArgs e)
        {
            LoadAllUsers();
        }

        private void Icon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            var user = (sender as Button)?.DataContext as Users;
            if (user != null)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить пользователя {user.FullName}?",
                                             "Подтверждение удаления", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _dbContext.Users.Remove(user);
                    _dbContext.SaveChanges();
                    LoadAllUsers();
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _dbContext?.Dispose();
            base.OnClosed(e);
        }

        private void EditUserButton_Click(object sender, RoutedEventArgs e)
        {
            var user = (sender as Button)?.DataContext as Users;
            if (user != null)
            {
                var editUsersWindow = new EditUsers(user);
                editUsersWindow.Closed += EditUsersWindow_Closed;
                editUsersWindow.Show();
            }
        }

        private void EditUsersWindow_Closed(object sender, EventArgs e)
        {
            LoadAllUsers();
        }
    }
}