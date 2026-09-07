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
    /// Логика взаимодействия для BoxGraf.xaml
    /// </summary>
    public partial class BoxGraf : Window
    {
        private DataGrid GrafDataGrid;

        public event EventHandler Delete;

        public BoxGraf(DataGrid dataGrid)
        {
            InitializeComponent();
            this.GrafDataGrid = dataGrid;
        }

        private void YsseSer(object sender, RoutedEventArgs e)
        {
            OnDelete(EventArgs.Empty);
            Close();
        }

        private void NetSer(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected virtual void OnDelete(EventArgs e)
        {
            Delete?.Invoke(this, e);
        }
    }
}
