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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CEP
{
    /// <summary>
    /// Логика взаимодействия для Statistic.xaml
    /// </summary>
    public partial class Statistic : Window
    {
        public string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=CEP_UPDATE;Integrated Security=True";
        DataTable table;

        public Statistic()
        {
            InitializeComponent();
            StreamReader file = new StreamReader("UserLogin.txt");
            string login = file.ReadLine();
            file.Close();
            if (login == null)
            {
                SqlConnection connection = new SqlConnection(connectionString);
                string query = "SELECT * FROM Statistic";
                connection.Open();
                table = new DataTable();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (IDataReader rdr = cmd.ExecuteReader())
                    {
                        table.Load(rdr);
                    }
                }
                StatisticGrid.ItemsSource = table.DefaultView;
                connection.Close();
            }
            else
            {
                string cardNumber = string.Empty;
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                string takeCardNumber = "SELECT CardNumber FROM BankCard WHERE UserCode = (SELECT UserCode FROM Users WHERE UserLogin = '" + login + "')";
                SqlCommand takeCardNumberCommand = new SqlCommand(takeCardNumber, connection);
                SqlDataReader cardNameReader = takeCardNumberCommand.ExecuteReader();
                if (cardNameReader.HasRows)
                {
                    while (cardNameReader.Read())
                    {
                        cardNumber = cardNameReader.GetString(0);
                    }
                }
                cardNameReader.Close();

                string query = "SELECT * FROM Statistic WHERE CardNumber = '" + cardNumber + "'";
                table = new DataTable();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (IDataReader rdr = cmd.ExecuteReader())
                    {
                        table.Load(rdr);
                    }
                }
                StatisticGrid.ItemsSource = table.DefaultView;
                connection.Close();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        private void BackToMenu(object sender, RoutedEventArgs e)
        {
            StreamReader file = new StreamReader("UserLogin.txt");
            string login = file.ReadLine();
            file.Close();
            if (login == null)
            {
                AdminMenu adminMenu = new AdminMenu();
                adminMenu.Show();
                this.Close();
            }
            else
            {
                UserMenu userMenu = new UserMenu();
                userMenu.Show();
                this.Close();
            }
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            StreamReader file = new StreamReader("UserLogin.txt");
            string login = file.ReadLine();
            file.Close();
            if (login == null)
            {
                HelpNavigator navigator = HelpNavigator.Topic;
                Help.ShowHelp(null, "help.chm", navigator, "statistika_po_tranzaktsiyam_polzovatelej.htm");
            }
            else
            {
                HelpNavigator navigator = HelpNavigator.Topic;
                Help.ShowHelp(null, "help.chm", navigator, "statistika_po_tranzaktsiyam_polzovatelya.htm");
            }
        }
    }
}
