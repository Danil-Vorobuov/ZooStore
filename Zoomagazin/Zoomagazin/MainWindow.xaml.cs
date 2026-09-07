using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Diagnostics;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using static Zoomagazin.Vhod;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Zoomagazin
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private User _currentUser;
        private ZooMEntities db = new ZooMEntities();
        private byte[] _photo;
        private string _username;
        private string _password;
        private string _position;

        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged(nameof(CurrentUser));
                SetButtonVisibility();
            }
        }

        public byte[] Photo
        {
            get => _photo;
            set
            {
                _photo = value;
                OnPropertyChanged(nameof(Photo));
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
        public string Position
        {
            get => _position;
            set
            {
                _position = value;
                OnPropertyChanged(nameof(Position));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            SetButtonVisibility();
            DataContext = this;
            DisplayUserInfo();

            mainFrame.Navigate(new HelloUsers());
            LoadAllTovar();
        }
        private void SetButtonVisibility()
        {
            switch (CurrentUser.Position)
            {
                case "Сотрудник":
                    PageAdminButton.Visibility = Visibility.Collapsed;
                    AnalyticsButton.Visibility = Visibility.Collapsed;
                    break;
                case "Директор":
                    PageAdminButton.Visibility = Visibility.Collapsed;
                    break;
                case "Администратор":
                    break;
                default:
                    break;
            }
        }
        private void DisplayUserInfo()
        {
            UpdateUserPhoto();
            usernameTextBlock.Text = _currentUser.Username;
        }

        private void UpdateUserPhoto()
        {
            if (_currentUser.Photo != null)
            {
                using (var ms = new MemoryStream(_currentUser.Photo))
                {
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = ms;
                    bitmapImage.EndInit();
                    userPhoto.ImageSource = bitmapImage;
                }
            }
        }

        private void CurrentUser_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(User.Photo))
            {
                UpdateUserPhoto();
            }
            else if (e.PropertyName == nameof(User.Username))
            {
                usernameTextBlock.Text = _currentUser.Username;
            }
        }

        private void EdditUserProfil(object sender, RoutedEventArgs e)
        {
            OpenProfileEditor(sender, e);
        }

        private void OpenProfileEditor(object sender, RoutedEventArgs e)
        {
            DimBackground.Visibility = Visibility.Visible;
            Window3 profileEditor = new Window3(_currentUser);
            profileEditor.UserUpdated += ProfileEditor_UserUpdated; // Подписываемся на событие
            profileEditor.Closed += ProfileEditor_Closed;
            profileEditor.Show();
        }

        private void ProfileEditor_Closed(object sender, EventArgs e)
        {
            DimBackground.Visibility = Visibility.Collapsed;
        }

        private void ProfileEditor_UserUpdated(object sender, User updatedUser)
        {
            // Обновляем текущего пользователя
            _currentUser = updatedUser;

            // Обновляем данные в UI
            UpdateUserPhoto();
            usernameTextBlock.Text = _currentUser.Username;
        }

        private void LoadAllTovar()
        {
            membersDataGrid.ItemsSource = db.Tovar.ToList();
        }

        private void LoadTovarByCategory(int categoryId)
        {
            var filteredTovar = db.Tovar.Where(t => t.id_категории == categoryId).ToList();
            membersDataGrid.ItemsSource = filteredTovar;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private bool IsMaximized = false;
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (IsMaximized)
                {
                    this.WindowState = WindowState.Normal;
                    this.Width = 1450;
                    this.Height = 720;
                    IsMaximized = false;
                }
                else
                {
                    this.WindowState = WindowState.Maximized;
                    IsMaximized = true;
                }
            }
        }

        private Button _currentButton2;

        System.Windows.Style myStyle1 = new System.Windows.Style();

        private void ChangeSelectedButton2(Button button)
        {
            if (_currentButton2 != null)
            {
                _currentButton2.Style = (System.Windows.Style)FindResource("tabButton");
            }
            button.Style = (System.Windows.Style)FindResource("tabButtonTwo");
            _currentButton2 = button;
        }

        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeSelectedButton2(sender as Button);
            Button clickedButton = sender as Button;
            string categoryName = clickedButton.Content.ToString();
            int categoryId = GetCategoryIdByName(categoryName);
            if (categoryId == 0)
            {
                LoadAllTovar();
            }
            else
            {
                LoadTovarByCategory(categoryId);
            }
        }
        

        

        // Другие методы и классы

        private int GetCategoryIdByName(string categoryName)
        {
            switch (categoryName.ToLower())
            {
                case "корма":
                    return 1;
                case "аксессуары":
                    return 3;
                case "игрушки":
                    return 2;
                case "для грызунов":
                    return 4;
                default:
                    return 0;
            }
        }

        private void membersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtFilter.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                LoadAllTovar();
            }
            else
            {
                var filteredTovar = db.Tovar.Where(t => t.Название.ToLower().Contains(searchText) || t.описание.ToLower().Contains(searchText)).ToList();
                membersDataGrid.ItemsSource = filteredTovar;
            }
        }

        private void AddPerehod(object sender, RoutedEventArgs e)
        {
            ToggleDimBackground(true);
            Window1 window1 = new Window1();
            window1.Closed += (s, args) =>
            {
                ToggleDimBackground(false);
            };
            window1.DataUpdated += (s, args) =>
            {
                // Обновляем данные в таблице
                UpdateMembersDataGrid();
                ToggleDimBackground(false);
            };
            window1.Show();
        }

        private void Window1_DataUpdated(object sender, EventArgs e)
        {
            membersDataGrid.ItemsSource = db.Tovar.ToList();
        }

        private void EditPere(object sender, RoutedEventArgs e)
        {
            var selectedTovar = (Tovar)membersDataGrid.SelectedItem;
            if (selectedTovar != null)
            {
                ToggleDimBackground(true);
                Window2 Window22 = new Window2(selectedTovar);
                Window22.Closed += AnotherWindow_Closed;
                Window22.DataUpdated += (s, args) =>
                {
                    // Обновляем данные в таблице
                    UpdateMembersDataGrid();
                    ToggleDimBackground(false);
                };
                Window22.Show();
            }
        }

        private void Window2_DataUpdated(object sender, EventArgs e)
        {

            membersDataGrid.ItemsSource = db.Tovar.ToList();
        }

        private void DeleteZ(object sender, RoutedEventArgs e)
        {
            var selectedTovar = (Tovar)membersDataGrid.SelectedItem;
            if (selectedTovar != null)
            {
                ToggleDimBackground(true);
                MessegeBox deleteTovarWindow = new MessegeBox(membersDataGrid);
                deleteTovarWindow.Closed += AnotherWindow_Closed;
                deleteTovarWindow.Delete += (s, args) =>
                {
                    // Обновляем данные в таблице
                    UpdateMembersDataGrid();
                    ToggleDimBackground(false);
                };
                deleteTovarWindow.Show();
            }
        }

        private void UpdateMembersDataGrid()
        {
            membersDataGrid.ItemsSource = db.Tovar.ToList();
        }

        private void DeleteTovarWindow_DataUpdated(object sender, EventArgs e)
        {
            membersDataGrid.ItemsSource = db.Tovar.ToList();
        }

        private void Obnov(object sender, RoutedEventArgs e)
        {
            membersDataGrid.ItemsSource = db.Tovar.ToList();
        }

        private void Print(object sender, RoutedEventArgs e)
        {
            CreateWordDocument();
        }

        private void CreateWordDocument()
        {
            string fileName = "TovarData.docx";
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);

            using (WordprocessingDocument doc = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());


                Paragraph title = new Paragraph();
                Run titleRun = new Run(new Text("Данные из таблицы Tovar"));


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

                headerRow.Append(CreateTableCell("Код товара", isHeader: true));
                headerRow.Append(CreateTableCell("Название", isHeader: true));
                headerRow.Append(CreateTableCell("Цена", isHeader: true));
                headerRow.Append(CreateTableCell("Кол-во товара", isHeader: true));

                table.AppendChild(headerRow);
                foreach (Tovar tovar in membersDataGrid.Items)
                {
                    TableRow dataRow = new TableRow();

                    dataRow.Append(CreateTableCell(tovar.ID.ToString()));
                    dataRow.Append(CreateTableCell(tovar.Название));
                    dataRow.Append(CreateTableCell(tovar.цена.ToString()));
                    dataRow.Append(CreateTableCell(tovar.Кол_воТовара));

                    table.AppendChild(dataRow);
                }

                body.Append(table);
            }
            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }

        private TableCell CreateTableCell(string text, bool isHeader = false)
        {
            TableCell cell = new TableCell();
            Paragraph paragraph = new Paragraph(new Run(new Text(text)));
            if (isHeader)
            {
                cell.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                Run run = paragraph.GetFirstChild<Run>();
                if (run != null)
                {
                    run.RunProperties = new RunProperties(new Bold());
                }
            }

            cell.Append(paragraph);
            return cell;
        }

        private void PrintExcel(object sender, RoutedEventArgs e)
        {

        }

        private void Page1(object sender, RoutedEventArgs e)
        {
            ChangeSelectedButton(sender as Button);
            Kalendar page1 = new Kalendar();
            mainFrame.Source = new Uri("Kalendar.xaml", UriKind.Relative);
        }
        private void Page2(object sender, RoutedEventArgs e)
        {
            ChangeSelectedButton(sender as Button);
            HelloUsers page2 = new HelloUsers();
            mainFrame.Source = new Uri("HelloUsers.xaml", UriKind.Relative);
        }

        private void PageOf(object sender, RoutedEventArgs e)
        {
            ChangeSelectedButton(sender as Button);
            mainFrame.NavigationService.Navigate(null);
        }

        private void mainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

        }

        private void UsersOpenWindow(object sender, RoutedEventArgs e)
        {
            DimBackground.Visibility = Visibility.Visible;
            Polzovatel polzovatel = new Polzovatel();
            polzovatel.Closed += AnotherWindow_Closed;
            polzovatel.Show();
        }

        private void ClosedWindow(object sender, RoutedEventArgs e)
        {
            DimBackground.Visibility = Visibility.Visible;
            MessegeBoxClosed vixod = new MessegeBoxClosed();
            vixod.Closed += AnotherWindow_Closed;
            vixod.Show();
        }

        private Button _currentButton;

        System.Windows.Style myStyle = new System.Windows.Style();

        private void ChangeSelectedButton(Button button)
        {
            if (_currentButton != null)
            {
                _currentButton.Style = (System.Windows.Style)FindResource("menuButton");
            }
            button.Style = (System.Windows.Style)FindResource("SelectedMenuButton");
            _currentButton = button;
        }
        public void AnotherWindow_Closed(object sender, EventArgs e)
        {
            DimBackground.Visibility = Visibility.Collapsed;
        }

        // Метод с тремя параметрами
        public void AnotherWindow_Closed(object sender, EventArgs e, MainWindow mainWindow)
        {
            if (mainWindow != null)
            {
                mainWindow.ToggleDimBackground(false);
                mainWindow.AnotherWindow_Closed(sender, e);
            }
        }
        public void ToggleDimBackground(bool visible)
        {
            DimBackground.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ChangeSelectedButton(sender as Button);
            Analitics page1 = new Analitics();
            mainFrame.Source = new Uri("Analitics.xaml", UriKind.Relative);
        }

        private void ClosedEmple(object sender, RoutedEventArgs e)
        {
            DimBackground.Visibility = Visibility.Visible;
            MessegeBoxUserT messegeBoxUserT = new MessegeBoxUserT();
            messegeBoxUserT.Closed += AnotherWindow_Closed;
        }

        private void PostavkaAdd(object sender, RoutedEventArgs e)
        {
            ChangeSelectedButton(sender as Button);
            Analis page1 = new Analis();
            mainFrame.Source = new Uri("Analis.xaml", UriKind.Relative);
        }

        private void Rasrab(object sender, RoutedEventArgs e)
        {
            DimBackground.Visibility = Visibility.Visible;
            rasrab rasrab = new rasrab();
            rasrab.Closed += AnotherWindow_Closed;
            rasrab.Show();
        }

        
    }
        }