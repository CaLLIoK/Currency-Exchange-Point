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
    /// Логика взаимодействия для UpdateCurrencyBuyExchangeCourse.xaml
    /// </summary>
    public partial class UpdateCurrencyBuyExchangeCourse : Window
    {
        string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=CEP_UPDATE;Integrated Security=True";

        public UpdateCurrencyBuyExchangeCourse()
        {
            InitializeComponent();
            List<int> codeList = new List<int>();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string query = @"SELECT CurrencyBuyCode FROM CurrencyBuy ORDER BY CurrencyBuyCode";
            SqlCommand sqlCommand = new SqlCommand(query, connection);
            SqlDataReader dataReader = sqlCommand.ExecuteReader();
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    codeList.Add(Convert.ToInt32(dataReader[0].ToString()));
                    searchCriterion.ItemsSource = codeList;
                }
            }
            connection.Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        private void BackToWindow(object sender, RoutedEventArgs e)
        {
            ChangeCurrencyBuyExchangeCourse changeCurrencyBuyExchangeCourse = new ChangeCurrencyBuyExchangeCourse();
            changeCurrencyBuyExchangeCourse.Show();
            this.Close();
        }

        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            if (criterion.Text != string.Empty && searchCriterion.Text != string.Empty)
            {
                int currencyCode = Convert.ToInt32(searchCriterion.Text);
                if (criterion.Text == "Название валюты")
                {
                    if (changingCriterion.Text != CheckCurrency.CheckCurrencyName(changingCriterion.Text))
                    {
                        System.Windows.MessageBox.Show(CheckCurrency.CheckCurrencyName(changingCriterion.Text));
                        return;
                    }
                    string mySelectQuery = "SELECT * FROM CurrencyBuy WHERE CurrencyBuyName = '" + changingCriterion.Text + "'";
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(mySelectQuery, connection))
                    {
                        DataTable table = new DataTable();
                        dataAdapter.Fill(table);
                        if (table.Rows.Count > 0)
                        {
                            System.Windows.MessageBox.Show("Вы не можете изменить данные, так как эта валюта уже есть в базе.");
                            return;
                        }
                        else if (table.Rows.Count == 0)
                        {
                            using (SqlCommand lastCommnd = connection.CreateCommand())
                            {
                                lastCommnd.CommandText = "UPDATE CurrencyBuy SET CurrencyBuyName = @name WHERE CurrencyBuyCode = @code";
                                lastCommnd.Parameters.AddWithValue("@name", changingCriterion.Text);
                                lastCommnd.Parameters.AddWithValue("@code", currencyCode);

                                lastCommnd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                else if (criterion.Text == "Курс валюты")
                {
                    if (changingCriterion.Text != CheckCurrency.CheckCurrencyCourse(changingCriterion.Text))
                    {
                        System.Windows.MessageBox.Show(CheckCurrency.CheckCurrencyCourse(changingCriterion.Text));
                        return;
                    }
                    using (SqlCommand lastCommnd = connection.CreateCommand())
                    {
                        lastCommnd.CommandText = "UPDATE CurrencyBuy SET CurrencyBuyCourse = @course WHERE CurrencyBuyCode = @code";
                        lastCommnd.Parameters.AddWithValue("@course", Convert.ToDouble(changingCriterion.Text));
                        lastCommnd.Parameters.AddWithValue("@code", currencyCode);

                        lastCommnd.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Вы не выбрали данные для изменения.");
                return;
            }
            MessageBoxResult mboxResult = System.Windows.MessageBox.Show("Данные обновлены. Желаете изменить что-нибудь еще?", "Предупреждение", MessageBoxButton.YesNo);
            if (mboxResult == MessageBoxResult.No)
            {
                ChangeCurrencyBuyExchangeCourse changeCurrencyBuyExchangeCourse = new ChangeCurrencyBuyExchangeCourse();
                changeCurrencyBuyExchangeCourse.Show();
                this.Close();
            }
            connection.Close();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            HelpNavigator navigator = HelpNavigator.Topic;
            Help.ShowHelp(null, "help.chm", navigator, "obnovlenie_dannykh_kursa_dlya_pokupki__obmena_.htm");
        }
    }
}