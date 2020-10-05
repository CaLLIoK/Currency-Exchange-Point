using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для UserMenu.xaml
    /// </summary>
    public partial class UserMenu : Window
    {
        string connectionString = @"Data Source=(local)\SQLEXPRESS; Initial Catalog=CEP_UPDATE; Integrated Security=True";

        public UserMenu()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            UserLoginWindow userLoginWindow = new UserLoginWindow();
            userLoginWindow.Show();
            this.Close();
        }

        private void ShowInfo_Click(object sender, RoutedEventArgs e)
        {
            ChooseCurrencyCourse chooseCurrencyCourse = new ChooseCurrencyCourse();
            chooseCurrencyCourse.Show();
            this.Close();
        }

        private void ChangeAccountData_Click(object sender, RoutedEventArgs e)
        {
            ChangeAccountData changeAccountData = new ChangeAccountData();
            changeAccountData.Show();
            this.Close();
        }

        private void Buy_Click(object sender, RoutedEventArgs e)
        {
            StreamReader file = new StreamReader("UserLogin.txt");
            string login = file.ReadLine();
            file.Close();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string checkCard = "SELECT CardNumber FROM BankCard WHERE UserCode = (SELECT UserCode FROM Users WHERE UserLogin = '" + login + "')";
            SqlCommand checkCardCommand = new SqlCommand(checkCard, connection);
            SqlDataReader checkCardReader = checkCardCommand.ExecuteReader();
            if (checkCardReader.HasRows)
            {
                while (checkCardReader.Read())
                {
                    StreamWriter streamWriter = new StreamWriter("CardNumber.txt");
                    streamWriter.Write(checkCardReader[0].ToString());
                    streamWriter.Close();
                }
                CheckCVVForBuy checkCVV = new CheckCVVForBuy();
                checkCVV.Show();
                this.Close();
            }
            else
            {
                System.Windows.MessageBox.Show("Вы еще не добавили карту.");
            }
            checkCardReader.Close();
        }

        private void Sell_Click(object sender, RoutedEventArgs e)
        {
            StreamReader file = new StreamReader("UserLogin.txt");
            string login = file.ReadLine();
            file.Close();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string checkCard = "SELECT CardNumber FROM BankCard WHERE UserCode = (SELECT UserCode FROM Users WHERE UserLogin = '" + login + "')";
            SqlCommand checkCardCommand = new SqlCommand(checkCard, connection);
            SqlDataReader checkCardReader = checkCardCommand.ExecuteReader();
            if (checkCardReader.HasRows)
            {
                while (checkCardReader.Read())
                {
                    StreamWriter streamWriter = new StreamWriter("CardNumber.txt");
                    streamWriter.Write(checkCardReader[0].ToString());
                    streamWriter.Close();
                }
                CheckCVVForSell checkCVV = new CheckCVVForSell();
                checkCVV.Show();
                this.Close();
            }
            else
            {
                System.Windows.MessageBox.Show("Вы еще не добавили карту.");
            }
            checkCardReader.Close();
        }

        private void Exchange_Click(object sender, RoutedEventArgs e)
        {
            StreamReader file = new StreamReader("UserLogin.txt");
            string login = file.ReadLine();
            file.Close();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string checkCard = "SELECT CardNumber FROM BankCard WHERE UserCode = (SELECT UserCode FROM Users WHERE UserLogin = '" + login + "')";
            SqlCommand checkCardCommand = new SqlCommand(checkCard, connection);
            SqlDataReader checkCardReader = checkCardCommand.ExecuteReader();
            if (checkCardReader.HasRows)
            {
                while (checkCardReader.Read())
                {
                    StreamWriter streamWriter = new StreamWriter("CardNumber.txt");
                    streamWriter.Write(checkCardReader[0].ToString());
                    streamWriter.Close();
                }
                CheckCVVForExchange checkCVV = new CheckCVVForExchange();
                checkCVV.Show();
                this.Close();
            }
            else
            {
                System.Windows.MessageBox.Show("Вы еще не добавили карту.");
            }
            checkCardReader.Close();
        }

        private void AddMoney_Click(object sender, RoutedEventArgs e)
        {
            StreamReader file = new StreamReader("UserLogin.txt");
            string login = file.ReadLine();
            file.Close();

            string cardNumber = string.Empty;

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string takeCardNumber = "SELECT CardNumber FROM BankCard WHERE UserCode = (SELECT UserCode FROM Users WHERE userLogin = '" + login + "')";
            SqlCommand takeCardNumberCommand = new SqlCommand(takeCardNumber, connection);
            SqlDataReader cardNameReader = takeCardNumberCommand.ExecuteReader();
            if (cardNameReader.HasRows)
            {
                while (cardNameReader.Read())
                {
                    cardNumber = cardNameReader.GetString(0);
                }
                Random rnd = new Random();

                double addSum = 0;

                using (SqlConnection changeSum = new SqlConnection(connectionString))
                using (SqlCommand command = changeSum.CreateCommand())
                {
                    command.CommandText = "UPDATE BankCard SET UserMoney = UserMoney + @sum WHERE CardNumber = @num";

                    command.Parameters.AddWithValue("@sum", addSum = rnd.Next(100, 2500));
                    command.Parameters.AddWithValue("@num", cardNumber);

                    changeSum.Open();
                    command.ExecuteNonQuery();
                    changeSum.Close();
                }
                System.Windows.MessageBox.Show("Ваш баланс пополнен на: " + addSum + " рублей.");
                connection.Close();
            }
            else
            {
                System.Windows.MessageBox.Show("Вы еще не добавили карту.");
                return;
            }
            cardNameReader.Close();
        }

        private void Statistic_Click(object sender, RoutedEventArgs e)
        {
            Statistic statistic = new Statistic();
            statistic.Show();
            this.Close();
        }

        private void AddCard_Click(object sender, RoutedEventArgs e)
        {
            StreamReader file = new StreamReader("UserLogin.txt");
            string login = file.ReadLine();
            file.Close();

            string sqlExpression = "SELECT CardNumber FROM BankCard WHERE UserCode = (SELECT UserCode FROM Users WHERE UserLogin = '" + login + "')";
            SqlConnection connection = new SqlConnection(connectionString);
            string cardNumber = string.Empty;
            connection.Open();
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    cardNumber = reader.GetString(0);
                }
            }
            reader.Close();

            if (cardNumber == string.Empty)
            {
                AddBankCard addBankCard = new AddBankCard();
                addBankCard.Show();
                this.Close();
            }
            else
            {
                System.Windows.MessageBox.Show("У вас уже есть привязанная карта.");
            }
        }

        private void UserMoney_Click(object sender, RoutedEventArgs e)
        {
            StreamReader file = new StreamReader("UserLogin.txt");
            string login = file.ReadLine();
            file.Close();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string checkCard = "SELECT CardNumber FROM BankCard WHERE UserCode = (SELECT UserCode FROM Users WHERE UserLogin = '" + login + "')";
            SqlCommand checkCardCommand = new SqlCommand(checkCard, connection);
            SqlDataReader checkCardReader = checkCardCommand.ExecuteReader();
            if (checkCardReader.HasRows)
            {
                while (checkCardReader.Read())
                {
                    StreamWriter streamWriter = new StreamWriter("CardNumber.txt");
                    streamWriter.Write(checkCardReader[0].ToString());
                    streamWriter.Close();
                }
                CheckCVVForMoney checkCVV = new CheckCVVForMoney();
                checkCVV.Show();
                this.Close();
            }
            else
            {
                System.Windows.MessageBox.Show("Вы еще не добавили карту.");
            }
            checkCardReader.Close();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            HelpNavigator navigator = HelpNavigator.Topic;
            Help.ShowHelp(null, "help.chm", navigator, "rukovodstvo_polzovatelya.htm");
        }
    }
}