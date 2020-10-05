using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
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

namespace CEP
{
    /// <summary>
    /// Логика взаимодействия для CheckCVVForBuyExchange.xaml
    /// </summary>
    public partial class CheckCVVForBuy : Window
    {
        int counter = 4;

        public CheckCVVForBuy()
        {
            InitializeComponent();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        private void Close_Click(object sender, RoutedEventArgs e) => this.Close();

        private void BackToMenu(object sender, RoutedEventArgs e)
        {
            UserMenu userMenu = new UserMenu();
            userMenu.Show();
            this.Close();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            StreamReader streamReader = new StreamReader("CardNumber.txt");
            string card = streamReader.ReadLine();
            streamReader.Close();
            SqlConnection myConnectionString = new SqlConnection(@"Data Source=(local)\SQLEXPRESS; Initial Catalog=CEP_UPDATE; Integrated Security=True");
            string mySelectQuery = "SELECT CVVCode FROM BankCard WHERE CardNumber = '" + card + "' AND CVVCode = '" + cvvCode.Text + "'";
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(mySelectQuery, myConnectionString))
            {
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    BuyWindow buyWindow = new BuyWindow();
                    buyWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Вы ввели неверный CVV-код. Осталось попыток: " + (counter - 1));
                    --counter;
                    if (counter == 0)
                    {
                        Application.Current.Shutdown();
                    }
                }
            }
        }
    }
}
