using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Zoomagazin
{
    /// <summary>
    /// Логика взаимодействия для addpost.xaml
    /// </summary>
    public partial class addpost : Window
    {
        private ZooMEntities db = new ZooMEntities();
        private readonly ZooMEntities _dbContext;

        public addpost(ZooMEntities dbContext)
        {
            InitializeComponent();
            _dbContext = dbContext;
        }

        private void Opicanie_TextChanged(object sender, TextChangedEventArgs e)
        {
            var emailBox = (TextBox)sender;
            var email = emailBox.Text.Trim();

            if (!IsValidEmail(email))
            {
                emailBox.Background = Brushes.LightPink;
                emailBox.ToolTip = "Пожалуйста, введите действительный адрес электронной почты.";
            }
            else
            {
                emailBox.Background = Brushes.White;
                emailBox.ToolTip = null;
            }
        }

        private bool IsValidEmail(string email)
        {
            string pattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
            return Regex.IsMatch(email, pattern);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var suppliers = new Suppliers
            {
                Name = Nazvanie.Text,
                email = Opicanie.Text
            };

            _dbContext.Suppliers.Add(suppliers);
            _dbContext.SaveChanges();
        }

        private void Icon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}

