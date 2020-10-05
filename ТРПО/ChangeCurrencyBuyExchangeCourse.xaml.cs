using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CEP
{
    /// <summary>
    /// Логика взаимодействия для ChangeCurrencyBuyExchangeCourse.xaml
    /// </summary>
    public partial class ChangeCurrencyBuyExchangeCourse : Window
    {
        public string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=CEP_UPDATE;Integrated Security=True";
        DataTable table;

        public ChangeCurrencyBuyExchangeCourse()
        {
            InitializeComponent();
            SqlConnection connection = new SqlConnection(connectionString);
            string query = "SELECT * FROM CurrencyBuy WHERE CurrencyBuyName != 'Белорусский рубль (BYR)'";
            connection.Open();
            table = new DataTable();
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                using (IDataReader rdr = cmd.ExecuteReader())
                {
                    table.Load(rdr);
                }
            }
            CurrencyBuyGrid.ItemsSource = table.DefaultView;
            connection.Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateCurrencyBuyExchangeCourse updateCurrencyBuyExchangeCourse = new UpdateCurrencyBuyExchangeCourse();
            updateCurrencyBuyExchangeCourse.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AdminMenu adminMenu = new AdminMenu();
            adminMenu.Show();
            this.Close();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            HelpNavigator navigator = HelpNavigator.Topic;
            Help.ShowHelp(null, "help.chm", navigator, "izmenenie_kursa_valyuty_dlya_pokupki__obmena_.htm");
        }
    }
}