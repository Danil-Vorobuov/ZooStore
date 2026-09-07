using System;
using System.Data.Entity;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Zoomagazin
{
    public partial class Vhod : Window
    {
        public static class CurrentUser
        {
            public static int ID { get; set; }
            public static string Username { get; set; }
            public static string Password { get; set; }
            public static string Email { get; set; }
            public static string Position { get; set; }
            public static byte[] Photo { get; set; }
        }

        private Storyboard _slideAnimation;
        private ZooMEntities db = new ZooMEntities();
        private string _randomCode;
        private bool _codeWasGenerated = false;

        public Vhod()
        {
            InitializeComponent();
            _slideAnimation = (Storyboard)FindResource("SlideAnimation");
            GenerateRandomCode();
            RandomeCode.IsReadOnly = false;

            _timer = new System.Threading.Timer(ResetCode, null, TimeSpan.FromMinutes(1), Timeout.InfiniteTimeSpan);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _timer?.Dispose();
        }

        private System.Threading.Timer _timer;
        private DateTime _codeGenerationTime;
        private void GenerateRandomCode()
        {
            Random random = new Random();
            _randomCode = random.Next(100000, 999999).ToString();
            _codeGenerationTime = DateTime.Now;

            _timer = new System.Threading.Timer(ResetCode, null, TimeSpan.FromMinutes(1), Timeout.InfiniteTimeSpan);
        }

        private void ResetCode(object state)
        {
            this.Dispatcher.Invoke(() =>
            {
                _randomCode = null;
                RandomeCode.Text = string.Empty;
                _timer.Dispose();

            });
        }

        private void Icon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void Svvapinge(object sender, RoutedEventArgs e)
        {
            string username = ((TextBox)FindName("UsernameTextBox")).Text;
            string password = ((PasswordBox)FindName("PasswordTextBox")).Password;

            User user = await GetUserByUsernameAndPasswordAsync(username, password);

            if (user != null)
            {
                if (!_codeWasGenerated)
                {
                    GenerateRandomCode();
                    await SendEmailAsync(user.emaile, _randomCode);
                    _codeWasGenerated = true;
                }
                _slideAnimation.Begin();
            }
            else
            {
                MessageBox.Show("Неправильный логин или пароль.");
            }
        }

        private async void Vhodvsistem(object sender, RoutedEventArgs e)
        {
            string enteredCode = ((TextBox)FindName("RandomeCode")).Text;

            if (enteredCode == _randomCode)
            {
                RandomeCode.IsReadOnly = true;

                string username = ((TextBox)FindName("UsernameTextBox")).Text;
                string password = ((PasswordBox)FindName("PasswordTextBox")).Password;

                User user = await GetUserByUsernameAndPasswordAsync(username, password);

                if (user != null)
                {
                    // Сохраняем данные текущего пользователя через объект User
                    MainWindow newMainWindow = new MainWindow(user);
                    newMainWindow.Show();
                    Close();
                    _codeWasGenerated = false; // Сбрасываем флаг, чтобы пользователь мог запросить новый код
                }
                else
                {
                    MessageBox.Show("Пользователь не найден.");
                }
            }
            else
            {
                MessageBox.Show("Неправильный код.");
            }
        }

        private async Task<User> GetUserByUsernameAndPasswordAsync(string username, string password)
        {
            using (var context = new ZooMEntities())
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
                if (user != null)
                {
                    return new User
                    {
                        ID = user.ID,
                        Username = user.Username,
                        Password = user.Password,
                        emaile = user.emaile,
                        Position = user.Position,
                        Photo = user.Photo
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        private async Task SendEmailAsync(string email, string code)
        {
            try
            {
                MailMessage message = new MailMessage();
                message.From = new MailAddress("zooworld.38@gmail.com");
                message.To.Add(new MailAddress(email));
                message.Subject = "Код для входа в систему";
                message.Body = $"Ваш код для входа: {code}";

                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.Credentials = new System.Net.NetworkCredential("zooworld.38@gmail.com", "wulv isyx ewhm jlaq");
                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отправки email: {ex.Message}");
            }
        }

        public class User
        {
            public int ID { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string emaile { get; set; }
            public string Position { get; set; }
            public byte[] Photo { get; set; }

        }

        private async void Povtorka(object sender, RoutedEventArgs e)
        {
            User user = await GetUserByUsernameAndPasswordAsync(((TextBox)FindName("UsernameTextBox")).Text, ((PasswordBox)FindName("PasswordTextBox")).Password);

            if (user != null)
            {
                // Генерируем новый код и отправляем его на email пользователя
                GenerateRandomCode();
                await SendEmailAsync(user.emaile, _randomCode);
            }
            else
            {
                MessageBox.Show("Неправильный логин или пароль.");
            }
        }
    }
}