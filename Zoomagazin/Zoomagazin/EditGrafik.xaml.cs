using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Zoomagazin
{
    public partial class EditGrafik : Window
    {
        private int _workScheduleId;
        private ZooMEntities _db;

        public event EventHandler DataUpdated;

        public EditGrafik(int workScheduleId)
        {
            InitializeComponent();
            _workScheduleId = workScheduleId;
            _db = new ZooMEntities();
            LoadWorkSchedule();
        }

        private void LoadWorkSchedule()
        {
            var workSchedule = _db.WorkSchedule.Find(_workScheduleId);
            if (workSchedule != null)
            {
                // Устанавливаем выбранный сотрудник в ComboBox
                if (workSchedule.EmployeeID.HasValue)
                {
                    Cotrudnik.SelectedItem = GetComboBoxItemByName(Cotrudnik, GetEmployeeNameById(workSchedule.EmployeeID.Value));
                }
            }
            else
            {
                MessageBox.Show("Запись не найдена.");
            }
        }

        private string GetEmployeeNameById(int? employeeId)
        {
            if (!employeeId.HasValue)
            {
                return null;
            }

            switch (employeeId.Value)
            {
                case 2: return "Петров Петр Петрович";
                case 3: return "Сидорова Елена Александровна";
                case 5: return "Козлова Мария Ивановна";
                case 7: return "Иванова Екатерина Сергеевна";
                default: return null;
            }
        }

        private ComboBoxItem GetComboBoxItemByName(ComboBox comboBox, string name)
        {
            foreach (ComboBoxItem item in comboBox.Items)
            {
                if (item.Content.ToString() == name)
                {
                    return item;
                }
            }
            return null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var workSchedule = _db.WorkSchedule.Find(_workScheduleId);
            if (workSchedule != null)
            {
                string selectedCotrudnik = ((ComboBoxItem)Cotrudnik.SelectedItem).Content.ToString();
                switch (selectedCotrudnik)
                {
                    case "Петров Петр Петрович":
                        workSchedule.EmployeeID = 2;
                        break;
                    case "Сидорова Елена Александровна":
                        workSchedule.EmployeeID = 3;
                        break;
                    case "Козлова Мария Ивановна":
                        workSchedule.EmployeeID = 5;
                        break;
                    case "Иванова Екатерина Сергеевна":
                        workSchedule.EmployeeID = 7;
                        break;
                    default:
                        MessageBox.Show("Пожалуйста, выберите сотрудника.");
                        return;
                }

                try
                {
                    // Сохраняем изменения в базе данных
                    _db.SaveChanges();
                    MessageBox.Show("Запись успешно обновлена.");

                    DataUpdated?.Invoke(this, EventArgs.Empty);
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при сохранении изменений: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Запись не найдена.");
            }
        }

        private void Icon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}