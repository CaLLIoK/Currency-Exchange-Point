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
    /// Логика взаимодействия для AddBuyExchangeCurrency.xaml
    /// </summary>
    public partial class AddBuyExchangeCurrency : Window
    {
        string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=CEP_UPDATE;Integrated Security=True";

        public AddBuyExchangeCurrency()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (currencyName.Text != CheckCurrency.CheckCurrencyName(currencyName.Text))
            {
                System.Windows.MessageBox.Show(CheckCurrency.CheckCurrencyName(currencyName.Text));
                return;
            }

            if (currencyCourse.Text != CheckCurrency.CheckCurrencyCourse(currencyCourse.Text))
            {
                System.Windows.MessageBox.Show(CheckCurrency.CheckCurrencyCourse(currencyCourse.Text));
                return;
            }

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string findReplays = "SELECT * FROM CurrencyBuy WHERE CurrencyBuyName = '" + currencyName.Text + "'";
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(findReplays, connection))
            {
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    System.Windows.MessageBox.Show("Вы не можете добавить данную валюту, так как она уже есть в базе.");
                    return;
                }
                else if (table.Rows.Count == 0)
                {
                    using (SqlCommand lastCommnd = connection.CreateCommand())
                    {
                        lastCommnd.CommandText = "INSERT INTO CurrencyBuy (CurrencyBuyName, CurrencyBuyCourse) VALUES (@name, @course)";
                        lastCommnd.Parameters.AddWithValue("@name", currencyName.Text);
                        lastCommnd.Parameters.AddWithValue("@course", Convert.ToDouble(currencyCourse.Text));

                        lastCommnd.ExecuteNonQuery();
                    }
                }
            }
            connection.Close();
            System.Windows.MessageBox.Show("Валюта успешно добавлена.");
        }

        private void BackToMenu(object sender, RoutedEventArgs e)
        {
            AdminMenu adminMenu = new AdminMenu();
            adminMenu.Show();
            this.Close();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            HelpNavigator navigator = HelpNavigator.Topic;
            Help.ShowHelp(null, "help.chm", navigator, "dobavlenie_valyuty_dlya_pokupki__obmena_.htm");
        }
    }
}