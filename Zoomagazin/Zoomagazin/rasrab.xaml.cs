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
    /// Логика взаимодействия для rasrab.xaml
    /// </summary>
    public partial class rasrab : Window
    {
        public rasrab()
        {
            InitializeComponent();
        }

        private void Icon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void AnotherWindow_Closed(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
