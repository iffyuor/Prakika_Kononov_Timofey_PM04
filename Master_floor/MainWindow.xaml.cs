using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Master_floor
{
    // ========== МОДЕЛИ ДАННЫХ ==========
    public class Partner
    {
        public int ID { get; set; }
        public string Тип_партнера { get; set; }
        public string Наименование_партнера { get; set; }
        public string Директор { get; set; }
        public string Телефон_партнера { get; set; }
        public int Рейтинг { get; set; }
        public string Скидка { get; set; }
    }

    public class Sale
    {
        public int ID { get; set; }
        public int ID_Partner { get; set; }
        public string Наименование_продукции { get; set; }
        public int Количество { get; set; }
        public DateTime Дата { get; set; }
    }

    // ========== БАЗА ДАННЫХ (ЗАГЛУШКА) ==========
    public class Database
    {
        public List<Partner> Partners { get; set; }
        public List<Sale> Sales { get; set; }

        public Database()
        {
            // Создаем тестовых партнеров
            Partners = new List<Partner>();
            for (int i = 1; i <= 5; i++)
            {
                Partners.Add(new Partner
                {
                    ID = i,
                    Тип_партнера = i % 2 == 0 ? "ЗАО" : "ООО",
                    Наименование_партнера = $"Партнер {i}",
                    Директор = $"Директор {i}",
                    Телефон_партнера = $"+7 (999) 123-45-{i}{i}",
                    Рейтинг = 50 + i * 5
                });
            }

            // Создаем тестовые продажи
            Sales = new List<Sale>();
            Random rnd = new Random();
            string[] products = { "Кирпич", "Цемент", "Песок", "Бетон", "Плитка" };

            for (int i = 1; i <= 5; i++)
            {
                for (int j = 0; j < rnd.Next(2, 5); j++)
                {
                    Sales.Add(new Sale
                    {
                        ID = Sales.Count + 1,
                        ID_Partner = i,
                        Наименование_продукции = products[rnd.Next(products.Length)],
                        Количество = rnd.Next(1000, 50000),
                        Дата = DateTime.Now.AddDays(-rnd.Next(0, 365))
                    });
                }
            }
        }

        // Метод для расчета скидки
        public string GetDiscount(int partnerId)
        {
            int totalAmount = Sales.Where(s => s.ID_Partner == partnerId).Sum(s => s.Количество);

            if (totalAmount >= 300000) return "15%";
            if (totalAmount >= 50000) return "10%";
            if (totalAmount >= 10000) return "5%";
            return "0%";
        }
    }

    // ========== ОКНО ДОБАВЛЕНИЯ ПАРТНЕРА ==========
    public class AddPartnerWindow : Window
    {
        private Database db;
        private TextBox txtName, txtDirector, txtPhone, txtRating;
        private ComboBox cmbType;

        public AddPartnerWindow(Database database)
        {
            db = database;
            Title = "Добавление партнера";
            Width = 400;
            Height = 450;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            var panel = new StackPanel { Margin = new Thickness(20) };

            panel.Children.Add(new TextBlock { Text = "Тип партнера:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 10, 0, 5) });
            cmbType = new ComboBox { Margin = new Thickness(0, 0, 0, 10) };
            cmbType.Items.Add("ООО");
            cmbType.Items.Add("ЗАО");
            cmbType.Items.Add("ИП");
            cmbType.SelectedIndex = 0;
            panel.Children.Add(cmbType);

            panel.Children.Add(new TextBlock { Text = "Наименование:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 10, 0, 5) });
            txtName = new TextBox { Margin = new Thickness(0, 0, 0, 10) };
            panel.Children.Add(txtName);

            panel.Children.Add(new TextBlock { Text = "Директор:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 10, 0, 5) });
            txtDirector = new TextBox { Margin = new Thickness(0, 0, 0, 10) };
            panel.Children.Add(txtDirector);

            panel.Children.Add(new TextBlock { Text = "Телефон:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 10, 0, 5) });
            txtPhone = new TextBox { Margin = new Thickness(0, 0, 0, 10) };
            panel.Children.Add(txtPhone);

            panel.Children.Add(new TextBlock { Text = "Рейтинг:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 10, 0, 5) });
            txtRating = new TextBox { Text = "50", Margin = new Thickness(0, 0, 0, 10) };
            panel.Children.Add(txtRating);

            var btnPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 20, 0, 0) };
            var btnSave = new Button { Content = "Сохранить", Width = 100, Height = 35, Margin = new Thickness(5) };
            var btnCancel = new Button { Content = "Отмена", Width = 100, Height = 35, Margin = new Thickness(5) };

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => Close();

            btnPanel.Children.Add(btnSave);
            btnPanel.Children.Add(btnCancel);
            panel.Children.Add(btnPanel);

            Content = new ScrollViewer { Content = panel };
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Проверка заполнения
            if (string.IsNullOrWhiteSpace(txtName.Text)) { MessageBox.Show("Введите наименование!"); return; }
            if (string.IsNullOrWhiteSpace(txtDirector.Text)) { MessageBox.Show("Введите директора!"); return; }
            if (string.IsNullOrWhiteSpace(txtPhone.Text)) { MessageBox.Show("Введите телефон!"); return; }

            if (!int.TryParse(txtRating.Text, out int rating) || rating < 1 || rating > 100)
            { MessageBox.Show("Рейтинг должен быть числом от 1 до 100!"); return; }

            // Добавление партнера
            db.Partners.Add(new Partner
            {
                ID = db.Partners.Count + 1,
                Тип_партнера = cmbType.SelectedItem.ToString(),
                Наименование_партнера = txtName.Text,
                Директор = txtDirector.Text,
                Телефон_партнера = txtPhone.Text,
                Рейтинг = rating
            });

            DialogResult = true;
            Close();
        }
    }
    // ========== ОКНО ИСТОРИИ ==========
    public class HistoryWindow : Window
    {
        public HistoryWindow(Database db, string title = "История продаж", int? partnerId = null)
        {
            Title = title;
            Width = 900;
            Height = 500;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Создание таблицы
            var listView = new ListView { Margin = new Thickness(10) };
            var gridView = new GridView();
            gridView.Columns.Add(new GridViewColumn { Header = "Партнер", Width = 150, DisplayMemberBinding = new System.Windows.Data.Binding("PartnerName") });
            gridView.Columns.Add(new GridViewColumn { Header = "Продукт", Width = 150, DisplayMemberBinding = new System.Windows.Data.Binding("Product") });
            gridView.Columns.Add(new GridViewColumn { Header = "Количество", Width = 100, DisplayMemberBinding = new System.Windows.Data.Binding("Amount") });
            gridView.Columns.Add(new GridViewColumn { Header = "Дата", Width = 120, DisplayMemberBinding = new System.Windows.Data.Binding("Date") });
            listView.View = gridView;

            // Загрузка данных
            var sales = from sale in db.Sales
                        join partner in db.Partners on sale.ID_Partner equals partner.ID
                        where partnerId == null || sale.ID_Partner == partnerId
                        select new
                        {
                            PartnerName = partner.Наименование_партнера,
                            Product = sale.Наименование_продукции,
                            Amount = sale.Количество,
                            Date = sale.Дата.ToString("dd.MM.yyyy")
                        };

            listView.ItemsSource = sales.ToList();
            grid.Children.Add(listView);

            var btnClose = new Button { Content = "Закрыть", Width = 100, Height = 35, Margin = new Thickness(10) };
            btnClose.HorizontalAlignment = HorizontalAlignment.Center;
            btnClose.Click += (s, e) => Close();
            grid.Children.Add(btnClose);

            // ИСПРАВЛЕНО: используем Grid.SetRow (статический метод)
            Grid.SetRow(btnClose, 1);

            Content = grid;
        }
    }

    // ========== ГЛАВНОЕ ОКНО ==========
    public partial class MainWindow : Window
    {
        private Database db = new Database();

        public MainWindow()
        {
            InitializeComponent();
            RefreshPartnerList();
        }

        // Обновление списка партнеров
        private void RefreshPartnerList()
        {
            var partners = db.Partners.Select(p => new Partner
            {
                ID = p.ID,
                Тип_партнера = p.Тип_партнера,
                Наименование_партнера = p.Наименование_партнера,
                Директор = p.Директор,
                Телефон_партнера = p.Телефон_партнера,
                Рейтинг = p.Рейтинг,
                Скидка = db.GetDiscount(p.ID)
            }).ToList();

            listPartner.ItemsSource = partners;
        }

        // Двойной клик - показать историю партнера
        private void listPartner_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listPartner.SelectedItem is Partner selected)
            {
                var historyWindow = new HistoryWindow(db, $"История: {selected.Наименование_партнера}", selected.ID);
                historyWindow.ShowDialog();
            }
        }

        // Добавление партнера
        private void btnAddPartner_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddPartnerWindow(db);
            if (addWindow.ShowDialog() == true)
            {
                RefreshPartnerList();
                MessageBox.Show("Партнер добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Показать всю историю
        private void btnProduct_Click(object sender, RoutedEventArgs e)
        {
            var historyWindow = new HistoryWindow(db);
            historyWindow.Show();
        }
    }
}