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
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public event EventHandler DataUpdated;
        ZooMEntities db = new ZooMEntities();
        public Window1()
        {
            InitializeComponent();
        }

        private void Icon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Close();

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Tovar newTovar = new Tovar();
            newTovar.Название = Nazvanie.Text;
            newTovar.описание = Opicanie.Text;
            string selectedCategory = ((ComboBoxItem)Kategori.SelectedItem).Content.ToString();

            switch (selectedCategory)
            {
                case "Корма":
                    newTovar.id_категории = 1;
                    break;
                case "Игрушки для животных":
                    newTovar.id_категории = 2;
                    break;
                case "Аксессуары для животных":
                    newTovar.id_категории = 3;
                    break;
                case "Товары для грызунов":
                    newTovar.id_категории = 4;
                    break;
                default:
                    break;
            }

            // Validate the input for the price field
            if (!decimal.TryParse(Cena.Text, out decimal price))
            {
                MessageBox.Show("Не верный формат - Цена должна быть числом", "Ошибка");
                return; // Exit the method or handle the error accordingly
            }
            newTovar.цена = price;

            // Validate the input for the quantity field
            if (!int.TryParse(Kolvo.Text, out int quantity))
            {
                MessageBox.Show("Не верный формат - Количество товара должно быть числом", "Ошибка");
                return; // Exit the method or handle the error accordingly
            }
            newTovar.Кол_воТовара = quantity.ToString();

            db.Tovar.Add(newTovar);
            db.SaveChanges();
            DataUpdated?.Invoke(this, EventArgs.Empty);
            Close();
        }
    }
}
