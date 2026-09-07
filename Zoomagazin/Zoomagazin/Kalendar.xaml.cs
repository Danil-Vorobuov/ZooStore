using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using LiveCharts;
using LiveCharts.Wpf;

namespace Zoomagazin
{
    public partial class Kalendar : Page, INotifyPropertyChanged
    {
        ZooMEntities db = new ZooMEntities();
        private int _selectedSalesPointId = 1;
        private DateTime _currentDate = DateTime.Today;
        private string[] _monthNames = new string[]
        {
            "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь",
            "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"
        };

        private Button _currentButton;

        public Kalendar()
        {
            InitializeComponent();
            InitializeButtonVisibility();
            _operationsColumnVisibility = Visibility.Collapsed;
            UpdateOperationsColumnVisibility();
            UpdateCurrentMonthText();
            RefreshDataGrid();
            MonthComboBox.SelectionChanged += MonthComboBox_SelectionChanged;

            SeriesCollection = new SeriesCollection();
            Labels = new string[] { };
            Formatter = value => value.ToString("N");

            DataContext = this;
            UpdateChart();
        }

        public int SelectedSalesPointId
        {
            get { return _selectedSalesPointId; }
            set
            {
                _selectedSalesPointId = value;
                RefreshDataGrid();
                UpdateCurrentMonthText();
                UpdateChart();
            }
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        private void MonthComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MonthComboBox.SelectedIndex >= 0)
            {
                _currentDate = new DateTime(_currentDate.Year, MonthComboBox.SelectedIndex + 1, 1);
                RefreshDataGrid();
                UpdateCurrentMonthText();
                UpdateChart();
            }
        }

        private void UpdateCurrentMonthText()
        {
            CurrentMonthTextBlock.Text = $"{_monthNames[_currentDate.Month - 1]} {_currentDate.Year}";
        }

        private void RefreshDataGrid()
        {
            var startDate = new DateTime(_currentDate.Year, _currentDate.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            GrafDataGrid.ItemsSource = LoadDataForCurrentMonth(startDate);
        }

        private List<WorkSchedule> LoadDataForCurrentMonth(DateTime currentDate)
        {
            var startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            return db.WorkSchedule
                .Where(x => x.SalesPointID == SelectedSalesPointId && x.Date >= startDate && x.Date <= endDate)
                .OrderBy(x => x.Date)
                .ToList();
        }
        private void LoadAllGraf()
        {
            GrafDataGrid.ItemsSource = db.WorkSchedule.ToList();
        }
        private void UpdateChart()
        {
            var startDate = new DateTime(_currentDate.Year, _currentDate.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var data = db.WorkSchedule
                .Where(ws => ws.SalesPointID == SelectedSalesPointId && ws.Date >= startDate && ws.Date <= endDate)
                .GroupBy(ws => ws.Employees.FullName)
                .Select(g => new ChartData
                {
                    EmployeeName = g.Key,
                    ShiftCount = g.Count()
                })
                .ToList();

            SeriesCollection.Clear();

            SeriesCollection.Add(new ColumnSeries
            {
                Title = "Смены",
                Values = new ChartValues<int>(data.Select(d => d.ShiftCount))
            });

            Labels = data.Select(d => d.EmployeeName).ToArray();
            OnPropertyChanged(nameof(Labels));
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            SelectedSalesPointId = 1;
            ChangeSelectedButton(sender as Button);
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            SelectedSalesPointId = 2;
            ChangeSelectedButton(sender as Button);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ChangeSelectedButton(Button button)
        {
            if (_currentButton != null)
            {
                _currentButton.Style = (Style)FindResource("tabButton");
            }

            button.Style = (Style)FindResource("tabButtonTwo");
            _currentButton = button;
        }

        private bool _isEditModeActive = false;
        private Visibility _editButtonsVisibility = Visibility.Collapsed;
        private Visibility _operationsColumnVisibility = Visibility.Collapsed;

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            _isEditModeActive = true;
            _editButtonsVisibility = Visibility.Visible;
            _operationsColumnVisibility = Visibility.Visible;
            UpdateButtonVisibility();
            UpdateOperationsColumnVisibility();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _isEditModeActive = false;
            _editButtonsVisibility = Visibility.Collapsed;
            _operationsColumnVisibility = Visibility.Collapsed;
            UpdateButtonVisibility();
            UpdateOperationsColumnVisibility();
        }

        private void UpdateOperationsColumnVisibility()
        {
            (GrafDataGrid.Columns[2] as DataGridTemplateColumn).Visibility = _operationsColumnVisibility;
        }

        private void UpdateButtonVisibility()
        {
            AddButton.Visibility = _editButtonsVisibility;
            SaveButton.Visibility = _editButtonsVisibility;
        }

        private void InitializeButtonVisibility()
        {
            _editButtonsVisibility = Visibility.Collapsed;
            UpdateButtonVisibility();
        }

        private void AddGrapf(object sender, RoutedEventArgs e)
        {
            EdidAddGraf addGrafWindow = new EdidAddGraf();
            addGrafWindow.Closed += AnotherWindow_Closed;
            addGrafWindow.DataUpdated += AnotherWindow_DataUpdated;
            addGrafWindow.Show();
        }

        private void LoadGrafData()
        {

            RefreshDataGrid();
            UpdateChart();
            LoadAllGraf();
        }

        private void Deletez(object sender, RoutedEventArgs e)
        {
            var selectedWorkSchedule = (WorkSchedule)GrafDataGrid.SelectedItem;
            if (selectedWorkSchedule != null)
            {
                ToggleDimBackground(true);
                BoxGraf deleteWorkScheduleWindow = new BoxGraf(GrafDataGrid);
                deleteWorkScheduleWindow.Closed += AnotherWindow_Closed;
                deleteWorkScheduleWindow.Delete += (s, args) =>
                {
                    try
                    {
                        db.WorkSchedule.Remove(selectedWorkSchedule);
                        db.SaveChanges();
                        LoadGrafData();
                        ToggleDimBackground(false);
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        MessageBox.Show("Произошла ошибка при удалении записи. Возможно, запись уже была удалена или изменена.");
                    }
                    finally
                    {
                        ToggleDimBackground(false);
                    }
                };
                deleteWorkScheduleWindow.Show();
            }
        }

        private void AnotherWindow_Closed(object sender, EventArgs e)
        {
            ToggleDimBackground(false);
            LoadGrafData();
        }

        private void ToggleDimBackground(bool dim)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.ToggleDimBackground(dim);
        }

        private void UpdateMembersDataGrid()
        {
            var startDate = new DateTime(_currentDate.Year, _currentDate.Month, 1);
            GrafDataGrid.ItemsSource = LoadDataForCurrentMonth(startDate);
            GrafDataGrid.ItemsSource = db.WorkSchedule.ToList();
        }

        private void Button_Click_Edit(object sender, RoutedEventArgs e)
        {
            int workScheduleId = GetSelectedWorkScheduleId();

            if (workScheduleId != -1)
            {
                ToggleDimBackground(true);
                EditGrafik editGrafik = new EditGrafik(workScheduleId);
                editGrafik.Closed += AnotherWindow_Closed;
                editGrafik.DataUpdated += (s, args) =>
                {
                    // Обновляем данные в таблице
                    RefreshDataGrid();
                    ToggleDimBackground(false);
                };
                editGrafik.Show();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите запись для редактирования.");
            }
        }

        private void EditGrafik_Closed(object sender, EventArgs e)
        {
            RefreshDataGrid();  // Обновляем данные в DataGrid
            UpdateChart();      // Обновляем данные на графике, если нужно
            ToggleDimBackground(false); // Возвращаем фон в нормальное состояние
        }

        private void AnotherWindow_DataUpdated(object sender, EventArgs e)
        {
            RefreshDataGrid();
            UpdateChart();
            ToggleDimBackground(false);
        }

        private int GetSelectedWorkScheduleId()
        {
            var selectedWorkSchedule = GrafDataGrid.SelectedItem as WorkSchedule;
            return selectedWorkSchedule?.id ?? -1;
        }

        public class ChartData
        {
            public string EmployeeName { get; set; }
            public int ShiftCount { get; set; }
        }

        private void Obnovaa(object sender, RoutedEventArgs e)
        {
            GrafDataGrid.ItemsSource = db.WorkSchedule.ToList();
        }
    }
}