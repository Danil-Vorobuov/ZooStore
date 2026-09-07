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
    public partial class EdidAddGraf : Window
    {
        private DatePicker _datePicker;

        public event EventHandler DataUpdated;

        public EdidAddGraf()
        {
            InitializeComponent();
            _datePicker = (DatePicker)FindName("DatePicker");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ZooMEntities db = new ZooMEntities();

            WorkSchedule newGrafik = new WorkSchedule();

            if (_datePicker != null && _datePicker.SelectedDate.HasValue)
            {
                DateTime selectedDate = _datePicker.SelectedDate.Value;
                newGrafik.Date = selectedDate;
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите дату.");
                return;
            }

            string selectedCotrudnik = ((ComboBoxItem)((ComboBox)FindName("Cotrudnik")).SelectedItem).Content.ToString();
            switch (selectedCotrudnik)
            {
                case "Петров Петр Петрович":
                    newGrafik.EmployeeID = 2;
                    break;
                case "Сидорова Елена Александровна":
                    newGrafik.EmployeeID = 3;
                    break;
                case "Козлова Мария Ивановна":
                    newGrafik.EmployeeID = 5;
                    break;
                case "Иванова Екатерина Сергеевна":
                    newGrafik.EmployeeID = 7;
                    break;
                default:
                    break;
            }

            string selectedPoint = ((ComboBoxItem)((ComboBox)FindName("Point")).SelectedItem).Content.ToString();
            switch (selectedPoint)
            {
                case "Академическая, 27":
                    newGrafik.SalesPointID = 1;
                    break;
                case "Гоголя, 44В":
                    newGrafik.SalesPointID = 2;
                    break;
                default:
                    break;
            }

            db.WorkSchedule.Add(newGrafik);
            db.SaveChanges();
            DataUpdated?.Invoke(this, EventArgs.Empty);
            Close();
        }

        private void Icon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}