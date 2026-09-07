using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    /// Логика взаимодействия для Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        private Tovar selectedTovar;

        public event EventHandler DataUpdated;
        ZooMEntities db = new ZooMEntities();

        public Window2(Tovar selectedTovar)
        {
            InitializeComponent();
            this.selectedTovar = selectedTovar;

            // Заполняем поля в Window2 данными из выбранной строки
            Nazvanie1.Text = selectedTovar.Название;
            Cena1.Text = selectedTovar.цена.ToString();
            Kolvo1.Text = selectedTovar.Кол_воТовара;
            Opicanie1.Text = selectedTovar.описание;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(Kolvo1.Text, out int kolvo))
            {
                MessageBox.Show("Не верный формат - Количество товара должно быть числом", "Ошибка");
                return;
            }

            // Validate the input in Cena1 TextBox
            if (!decimal.TryParse(Cena1.Text, out decimal cena))
            {
                MessageBox.Show("Не верный формат - Цена должна быть числом", "Ошибка");
                return; 
            }

            // Обновляем выбранный объект Tovar данными из полей в Window2
            selectedTovar.Название = Nazvanie1.Text;
            selectedTovar.цена = cena;
            selectedTovar.Кол_воТовара = Kolvo1.Text;
            selectedTovar.описание = Opicanie1.Text;

            // Сохраняем изменения
            db.SaveChanges();

            // Вызываем событие обновления данных
            DataUpdated?.Invoke(this, EventArgs.Empty);

            // Закрываем окно
            this.Close();
        }

        private void Icon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}
