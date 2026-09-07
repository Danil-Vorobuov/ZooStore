using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Zoomagazin
{
    public partial class Analitics : Page, INotifyPropertyChanged
    {
        private const string DataFilePath = "chartdata.txt"; // Путь к файлу для хранения данных

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Analitics()
        {
            InitializeComponent();
            InitializeChart();
            LoadChartData();
        }

        private void InitializeChart()
        {
            SeriesCollection = new SeriesCollection();
            Labels = new string[] { "Академическая, 27", "Гоголя, 44В" }; // Начальные метки для торговых точек
            Formatter = value => value.ToString("N");

            DataContext = this;
        }

        public void AddData(string month, string point, double profit)
        {
            // Ищем серию для текущего месяца
            var monthSeries = SeriesCollection.FirstOrDefault(s => s.Title == month) as ColumnSeries;
            if (monthSeries == null)
            {
                // Если серия для текущего месяца не найдена, создаем новую
                monthSeries = new ColumnSeries
                {
                    Title = month,
                    Values = new ChartValues<double>(new double[Labels.Length]) // Инициализируем значения для всех торговых точек
                };
                SeriesCollection.Add(monthSeries);
            }

            // Ищем индекс торговой точки
            var pointIndex = Array.IndexOf(Labels, point);
            if (pointIndex == -1)
            {
                // Если торговая точка новая, добавляем её в метки (Labels)
                var labelsList = Labels.ToList();
                labelsList.Add(point);
                Labels = labelsList.ToArray();

                // Обновляем значения всех серий для новой торговой точки
                foreach (var series in SeriesCollection.OfType<ColumnSeries>())
                {
                    series.Values.Add(0); // Добавляем 0 для новой торговой точки в каждую серию
                }
                pointIndex = Labels.Length - 1; // Индекс новой торговой точки
            }

            // Устанавливаем значение прибыли для текущей торговой точки в соответствующей серии
            monthSeries.Values[pointIndex] = profit;

            // Уведомляем об изменении свойств для обновления привязок
            OnPropertyChanged(nameof(SeriesCollection));
            OnPropertyChanged(nameof(Labels));

            // Сохраняем данные после добавления новой информации
            SaveChartData();
        }

        private void SaveChartData()
        {
            using (StreamWriter writer = new StreamWriter(DataFilePath))
            {
                // Сохраняем метки (Labels)
                writer.WriteLine(string.Join(";", Labels));

                // Сохраняем данные серий
                foreach (var series in SeriesCollection.OfType<ColumnSeries>())
                {
                    var values = string.Join(";", series.Values.Cast<double>());
                    writer.WriteLine($"{series.Title};{values}");
                }
            }
        }

        private void LoadChartData()
        {
            if (File.Exists(DataFilePath))
            {
                using (StreamReader reader = new StreamReader(DataFilePath))
                {
                    // Загружаем метки (Labels)
                    var labelsLine = reader.ReadLine();
                    if (!string.IsNullOrWhiteSpace(labelsLine))
                    {
                        Labels = labelsLine.Split(';');
                    }

                    // Загружаем данные серий
                    SeriesCollection.Clear();
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var parts = line.Split(';');
                        if (parts.Length > 1)
                        {
                            var title = parts[0];
                            var values = parts.Skip(1).Select(double.Parse).ToArray();

                            var series = new ColumnSeries
                            {
                                Title = title,
                                Values = new ChartValues<double>(values)
                            };
                            SeriesCollection.Add(series);
                        }
                    }

                    // Уведомляем об изменении свойств для обновления привязок
                    OnPropertyChanged(nameof(SeriesCollection));
                    OnPropertyChanged(nameof(Labels));
                }
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window4 window4 = new Window4(this);
            window4.ShowDialog(); // Открытие окна модально для получения данных
        }
    }
}
