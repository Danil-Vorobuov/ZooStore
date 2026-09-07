using System.Windows;
using System.Windows.Controls;

namespace Zoomagazin
{
    public partial class Window4 : Window
    {
        private Analitics analiticsPage;

        public Window4(Analitics analitics)
        {
            InitializeComponent();
            this.analiticsPage = analitics;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранный месяц и торговую точку из ComboBox'ов
            string selectedMonth = (Cotrudnik.SelectedItem as ComboBoxItem)?.Content?.ToString(); // Получаем строковое значение из ComboBoxItem
            string selectedPoint = (Point.SelectedItem as ComboBoxItem)?.Content?.ToString(); // Получаем строковое значение из ComboBoxItem

            // Проверяем, что выбраны все необходимые значения
            if (string.IsNullOrEmpty(selectedMonth) || string.IsNullOrEmpty(selectedPoint))
            {
                MessageBox.Show("Выберите месяц и торговую точку");
                return;
            }

            // Получаем введенное значение прибыли
            if (!double.TryParse(Prib.Text, out double profit))
            {
                MessageBox.Show("Некорректное значение прибыли");
                return;
            }

            // Обновляем данные на странице Analitics
            if (this.analiticsPage != null)
            {
                this.analiticsPage.AddData(selectedMonth, selectedPoint, profit);
            }

            // Закрываем текущее окно Window4
            this.Close();
        }

        private void Icon_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Close();
        }
    }
}
