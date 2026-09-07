using DocumentFormat.OpenXml.Vml;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Zoomagazin
{
    public partial class MessegeBox : Window
    {
        private DataGrid membersDataGrid;

        public event EventHandler Delete;

        public MessegeBox(DataGrid dataGrid)
        {
            InitializeComponent();
            this.membersDataGrid = dataGrid;
        }

        private void YsseSer(object sender, RoutedEventArgs e)
        {
            var selectedTovar = (Tovar)membersDataGrid.SelectedItem;

            if (selectedTovar != null)
            {
                DeleteTovarFromDatabase(selectedTovar);
                OnDelete(EventArgs.Empty);

                Close();
            }
        }

        private void NetSer(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DeleteTovarFromDatabase(Tovar tovar)
        {
            using (var db = new ZooMEntities())
            {
                var entityToDelete = db.Tovar.Find(tovar.ID);
                if (entityToDelete != null)
                {
                    db.Tovar.Remove(entityToDelete);
                    db.SaveChanges();
                }
            }
        }
        protected virtual void OnDelete(EventArgs e)
        {
            Delete?.Invoke(this, e);
        }
    }
}