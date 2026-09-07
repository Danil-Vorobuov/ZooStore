using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Zoomagazin
{
    public partial class AddUsers : Window, INotifyPropertyChanged
    {
        private readonly ZooMEntities _dbContext;
        private List<string> _positionList;

        public List<string> PositionList
        {
            get => _positionList;
            set
            {
                _positionList = value;
                OnPropertyChanged(nameof(PositionList));
            }
        }

        public AddUsers()
        {
            InitializeComponent();
            _dbContext = new ZooMEntities();
            DataContext = this;
            LoadPositionList();
        }

        private void LoadPositionList()
        {
            PositionList = new List<string> { "Директор", "Сотрудник", "Администратор" };
        }

        private void Icon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsInputValid())
            {
                AddNewUser();
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
                   !string.IsNullOrWhiteSpace(emaile.Text) &&
                   Position.SelectedItem != null;
        }

        private void AddNewUser()
        {
            var newUser = new Users
            {
                Username = username.Text,
                FullName = FIO.Text,
                Password = password.Text,
                emaile = emaile.Text,
                Position = Position.SelectedItem.ToString()
            };

            _dbContext.Users.Add(newUser);
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