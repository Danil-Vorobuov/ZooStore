using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Zoomagazin
{
    public partial class EditUsers : Window, INotifyPropertyChanged
    {
        private readonly ZooMEntities _dbContext;
        private Users _currentUser;

        public EditUsers(Users user)
        {
            InitializeComponent();
            _dbContext = new ZooMEntities();
            DataContext = this;
            _currentUser = user;
            LoadUserData();
        }

        private void LoadUserData()
        {
            FIO.Text = _currentUser.FullName;
            username.Text = _currentUser.Username;
            password.Text = _currentUser.Password;
            emaile.Text = _currentUser.emaile;
        }

        private void Icon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsInputValid())
            {
                UpdateUser();
                Close();
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
            }
        }

        private bool IsInputValid()
        {
            return !string.IsNullOrWhiteSpace(username.Text) &&
                   !string.IsNullOrWhiteSpace(FIO.Text) &&
                   !string.IsNullOrWhiteSpace(password.Text) &&
                   !string.IsNullOrWhiteSpace(emaile.Text);
        }

        private void UpdateUser()
        {
            _currentUser.Username = username.Text;
            _currentUser.FullName = FIO.Text;
            _currentUser.Password = password.Text;
            _currentUser.emaile = emaile.Text;

            _dbContext.SaveChanges();
        }

        protected override void OnClosed(EventArgs e)
        {
            _dbContext?.Dispose();
            base.OnClosed(e);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}