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

namespace Zoomagazin
{
    /// <summary>
    /// Логика взаимодействия для MessegeBoxClosed.xaml
    /// </summary>
    public partial class MessegeBoxClosed : Window
    {
        public MessegeBoxClosed()
        {
            InitializeComponent();
        }

        private void YesClos(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void NoClos(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
