using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static Zoomagazin.Vhod;

namespace Zoomagazin
{
    public partial class Window3 : Window
    {
        private User _currentUser;
        private ZooMEntities db = new ZooMEntities();
        public event EventHandler<User> UserUpdated;
        public Window3()
        {
            InitializeComponent();
        }

        public Window3(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            LoadUserData();
        }

        private void LoadUserData()
        {
            UserName.Text = _currentUser.Username;
            Password.Text = _currentUser.Password;

            if (_currentUser.Photo != null)
            {
                using (MemoryStream ms = new MemoryStream(_currentUser.Photo))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = ms;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    UserImage.ImageSource = image;
                }
            }
        }

        private void UploadPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = openFileDialog.FileName;
                var bitmap = new BitmapImage(new Uri(filePath));
                UserImage.ImageSource = bitmap;

                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        _currentUser.Photo = memoryStream.ToArray();
                    }
                }
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            // Обновляем данные текущего пользователя
            _currentUser.Username = UserName.Text;
            _currentUser.Password = Password.Text;

            // Сохраняем изменения в базе данных
            var userInDb = db.Users.SingleOrDefault(u => u.ID == _currentUser.ID);
            if (userInDb != null)
            {
                userInDb.Username = _currentUser.Username;
                userInDb.Password = _currentUser.Password;
                userInDb.Photo = _currentUser.Photo;

                db.SaveChanges();

                DimBackground.Visibility = Visibility.Visible;
                MessegeBoxUserT messegeBoxUserT = new MessegeBoxUserT();
                messegeBoxUserT.Closed += AnotherWindow_Closed;
                messegeBoxUserT.Show();

                // Поднятие события обновления пользователя
                UserUpdated?.Invoke(this, _currentUser);
            }
            else
            {
                MessageBox.Show("Ошибка при сохранении изменений. Пользователь не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AnotherWindow_Closed(object sender, EventArgs e)
        {
            DimBackground.Visibility = Visibility.Collapsed;
        }

        private void Icon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}