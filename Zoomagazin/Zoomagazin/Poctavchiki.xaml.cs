using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls; // Добавлено для использования Button

namespace Zoomagazin
{
    public partial class Poctavchiki : Window
    {
        private ZooMEntities db = new ZooMEntities();

        public Poctavchiki()
        {
            InitializeComponent();
            LoadAllPost();
        }

        private void LoadAllPost()
        {
            PoctavDataGrid.ItemsSource = db.Supplies.ToList();
        }

        private void Icon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var fileData = button?.Tag as byte[];

            if (fileData != null)
            {
                string tempFilePath = Path.Combine(Path.GetTempPath(), "tempFile.docx");

                try
                {
                    File.WriteAllBytes(tempFilePath, fileData);
                    Process.Start(new ProcessStartInfo(tempFilePath) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось открыть файл: {ex.Message}");
                }
            }
        }

        private void Deletez(object sender, RoutedEventArgs e)
        {
            var selectedSupply = (Supplies)PoctavDataGrid.SelectedItem; // Изменено на Supplies
            if (selectedSupply != null)
            {
                try
                {
                    db.Supplies.Remove(selectedSupply);
                    db.SaveChanges();
                    LoadAllPost();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось удалить запись: {ex.Message}");
                }
            }
        }
    }
}