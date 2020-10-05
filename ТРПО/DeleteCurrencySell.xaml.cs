using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для DeleteCurrencySell.xaml
    /// </summary>
    public partial class DeleteCurrencySell : Window
    {
        string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=CEP_UPDATE;Integrated Security=True";

        public DeleteCurrencySell()
        {
            InitializeComponent();
            ObservableCollection<int> codeList = new ObservableCollection<int>();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string query = @"SELECT CurrencySellCode FROM CurrencySell ORDER BY CurrencySellCode";
            SqlCommand sqlCommand = new SqlCommand(query, connection);
            SqlDataReader dataReader = sqlCommand.ExecuteReader();
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    codeList.Add(Convert.ToInt32(dataReader[0].ToString()));
                    deleteCurrency.ItemsSource = codeList;
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (deleteCurrency.Text != string.Empty)
            {
                using (SqlConnection deleteRow = new SqlConnection(connectionString))
                using (SqlCommand lastCommnd = deleteRow.CreateCommand())
                {
                    lastCommnd.CommandText = "DELETE FROM CurrencySell WHERE CurrencySellCode = @code";

                    lastCommnd.Parameters.AddWithValue("@code", deleteCurrency.Text);

                    deleteRow.Open();
                    lastCommnd.ExecuteNonQuery();
                    deleteRow.Close();
                }
                System.Windows.MessageBox.Show("Валюта удалена.");
                deleteCurrency.SelectedIndex = -1;
                ObservableCollection<int> codeList = new ObservableCollection<int>();
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                string query = @"SELECT CurrencySellCode FROM CurrencySell ORDER BY CurrencySellCode";
                SqlCommand sqlCommand = new SqlCommand(query, connection);
                SqlDataReader dataReader = sqlCommand.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        codeList.Add(Convert.ToInt32(dataReader[0].ToString()));
                        deleteCurrency.ItemsSource = codeList;
                    }
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Вы не указали код валюты, которую собираетесь удалить.");
                return;
            }
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
            Help.ShowHelp(null, "help.chm", navigator, "udalenie_kursa_valyuty_dlya_prodazhi.htm");
        }
    }
}