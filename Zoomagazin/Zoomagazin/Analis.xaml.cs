using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Net.Mail;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Zoomagazin
{
    public partial class Analis : Page
    {
        public ObservableCollection<Product> Products { get; set; }
        private ZooMEntities db = new ZooMEntities();
        private List<SalesPoints> salesPoints;
        private List<Suppliers> suppliers;

        public Analis()
        {
            InitializeComponent();
            Products = new ObservableCollection<Product>();
            Postavka.ItemsSource = Products;
            PopulateComboBoxes();
        }

        private void PopulateComboBoxes()
        {
            Point.ItemsSource = db.SalesPoints.ToList();
            Poct.ItemsSource = db.Suppliers.ToList();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(Kolvo.Text, out int quantity))
            {
                Products.Add(new Product { Name = Nazvanie.Text, Quantity = quantity });
            }
            else
            {
                MessageBox.Show("Введите корректное количество", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Nazvanie.Clear();
            Kolvo.Clear();
        }

        private void Print(object sender, RoutedEventArgs e)
        {
            CreateAndSaveWordDocument();
        }

        private async void CreateAndSaveWordDocument()
        {
            string fileName = "ProductsData.docx";
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);

            using (WordprocessingDocument doc = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                Paragraph title = new Paragraph();
                Run titleRun = new Run(new Text("Данные из таблицы продуктов"));

                titleRun.RunProperties = new RunProperties(new Bold(), new FontSize() { Val = "24" }, new RunFonts() { Ascii = "Times New Roman" });
                title.Append(titleRun);
                title.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
                body.Append(title);

                Table table = new Table();

                TableProperties tableProperties = new TableProperties(
                    new TableStyle { Val = "TableGrid" },
                    new TableWidth { Type = TableWidthUnitValues.Pct, Width = "100%" },
                    new TableBorders(
                        new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Color = "000000" },
                        new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Color = "000000" },
                        new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Color = "000000" },
                        new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Color = "000000" },
                        new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Color = "000000" },
                        new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Color = "000000" }
                    )
                );
                table.AppendChild(tableProperties);

                TableRow headerRow = new TableRow();
                headerRow.Append(CreateTableCell("Название товара", isHeader: true));
                headerRow.Append(CreateTableCell("Количество", isHeader: true));
                table.AppendChild(headerRow);

                foreach (var product in Products)
                {
                    TableRow dataRow = new TableRow();
                    dataRow.Append(CreateTableCell(product.Name));
                    dataRow.Append(CreateTableCell(product.Quantity.ToString()));
                    table.AppendChild(dataRow);
                }

                body.Append(table);
            }

            SaveDocumentToDatabase(filePath);

            // Теперь открываем документ
            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });

            // Отправка документа на почту
            await SendEmailWithAttachmentAsync(filePath);
        }

        private TableCell CreateTableCell(string text, bool isHeader = false)
        {
            TableCell cell = new TableCell();
            Paragraph paragraph = new Paragraph(new Run(new Text(text)));

            if (isHeader)
            {
                paragraph.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
                Run run = paragraph.GetFirstChild<Run>();
                run.RunProperties = new RunProperties(new Bold());
            }

            cell.Append(paragraph);
            return cell;
        }

        private void SaveDocumentToDatabase(string filePath)
        {
            int? selectedSupplierId = (int?)Poct.SelectedValue;
            int? selectedSalesPointId = (int?)Point.SelectedValue;
            DateTime? selectedDate = DatePicker.SelectedDate;

            if (selectedSupplierId == null || selectedSalesPointId == null || selectedDate == null)
            {
                MessageBox.Show("Пожалуйста, выберите все необходимые данные", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            byte[] fileData = File.ReadAllBytes(filePath);

            Supplies newSupply = new Supplies
            {
                id_poctav = selectedSupplierId,
                id_tt = selectedSalesPointId,
                date = selectedDate,
                Exel = fileData
            };

            db.Supplies.Add(newSupply);
            db.SaveChanges();

            MessageBox.Show("Запись успешно добавлена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async Task SendEmailWithAttachmentAsync(string filePath)
        {
            int? selectedSupplierId = (int?)Poct.SelectedValue;

            if (selectedSupplierId == null)
            {
                MessageBox.Show("Пожалуйста, выберите поставщика", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Suppliers selectedSupplier = db.Suppliers.Find(selectedSupplierId);

            if (selectedSupplier == null || string.IsNullOrEmpty(selectedSupplier.email))
            {
                MessageBox.Show("Не удалось найти email поставщика", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string recipientEmail = selectedSupplier.email;

            try
            {
                MailMessage message = new MailMessage();
                message.From = new MailAddress("zooworld.38@gmail.com");
                message.To.Add(new MailAddress(recipientEmail));
                message.Subject = "Заказ для Зоомира";
                message.Body = "Пожалуйста, доставьте необходимые товары.";

                Attachment attachment = new Attachment(filePath);
                message.Attachments.Add(attachment);

                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.Credentials = new System.Net.NetworkCredential("zooworld.38@gmail.com", "wulv isyx ewhm jlaq");
                await client.SendMailAsync(message);

                MessageBox.Show("Письмо успешно отправлено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось отправить письмо: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public class Product
        {
            public string Name { get; set; }
            public int Quantity { get; set; }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Poctavchiki poctavchiki = new Poctavchiki();
            poctavchiki.Show();
        }

        private void CreateExcelButton_Click(object sender, RoutedEventArgs e)
        {
            CreateAndSaveWordDocument();
        }

        private void PostButton_Click(object sender, RoutedEventArgs e)
        {
            addpost addpost = new addpost(db); // Передаем db в конструктор
            addpost.Show();
        }

    }
}