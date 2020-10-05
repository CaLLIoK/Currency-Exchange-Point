using Microsoft.Office.Interop.Word;
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
    /// Логика взаимодействия для ExchangeWindow.xaml
    /// </summary>
    public partial class ExchangeWindow : System.Windows.Window
    {
        string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=CEP_UPDATE;Integrated Security=True";

        public ExchangeWindow()
        {
            InitializeComponent();
            List<string> namesList = new List<string>();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string query = @"SELECT CurrencyBuyName FROM CurrencyBuy ORDER BY CurrencyBuyName";
            SqlCommand sqlCommand = new SqlCommand(query, connection);
            SqlDataReader dataReader = sqlCommand.ExecuteReader();
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    namesList.Add(Convert.ToString(dataReader[0].ToString()));
                    firstCurrency.ItemsSource = namesList;
                    lastCurrency.ItemsSource = namesList;
                }
            }
            dataReader.Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        private void BackToMenu(object sender, RoutedEventArgs e)
        {
            UserMenu userMenu = new UserMenu();
            userMenu.Show();
            this.Close();
        }

        private void ReplaseWord(string StubReplase, string Text, Document Doc)
        {
            var range = Doc.Content;
            range.Find.ClearFormatting();
            range.Find.Execute(FindText: StubReplase, ReplaceWith: Text);
        }

        private void ExchangeButton_Click(object sender, RoutedEventArgs e)
        {
            StreamReader file = new StreamReader("UserLogin.txt");
            string login = file.ReadLine();
            file.Close();

            string cardNumber = string.Empty;

            if (firstCurrency.Text == string.Empty && lastCurrency.Text == string.Empty)
            {
                System.Windows.MessageBox.Show("Вы не выбрали все необходимые данные для обмена.");
                return;
            }
            else if (firstCurrency.Text == lastCurrency.Text)
            {
                System.Windows.MessageBox.Show("Вы не можете обменять денежные средства в ту же валюту.");
                return;
            }
            else if (firstCurrency.Text != string.Empty && lastCurrency.Text == string.Empty)
            {
                System.Windows.MessageBox.Show("Вы не выбрали валюту, в которую будете менять денежные средства.");
                return;
            }
            else if (firstCurrency.Text == string.Empty && lastCurrency.Text != string.Empty)
            {
                System.Windows.MessageBox.Show("Вы не выбрали валюту, которую будете обменивать.");
                return;
            }

            if (sumForExchange.Text != string.Empty)
            {
                char[] sumArray = sumForExchange.Text.ToCharArray();
                for (int i = 0; i < sumArray.Length; i++)
                {
                    if ((!char.IsDigit(sumArray[i]) && sumArray[i] != ','))
                    {
                        System.Windows.MessageBox.Show("Вы указали в сумме покупаемой валюты недопустимые символы.");
                        return;
                    }
                }
                if (Convert.ToDouble(sumForExchange.Text) < 0 || Convert.ToDouble(sumForExchange.Text) > 5000)
                {
                    System.Windows.MessageBox.Show("Введена неверная сумма покупаемой валюты. Можно ввести значение от 1 до 5000.");
                    return;
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Вы не ввели сумму покупаемой валюты.");
                return;
            }

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

            double generalSumForExchange = 0;
            double courseOfGeneralSum = 1;

            if (firstCurrency.Text != "Белорусский рубль (BYR)")
            {
                string takeCurrencyName = "SELECT CurrencySum FROM CurrencyInfo WHERE CurrencyName = '" + firstCurrency.Text + "' AND CardNumber = '" + cardNumber + "'";
                SqlCommand takeCurrencyNameCommand = new SqlCommand(takeCurrencyName, connection);
                SqlDataReader sumReader = takeCurrencyNameCommand.ExecuteReader();
                if (sumReader.HasRows)
                {
                    while (sumReader.Read())
                    {
                        generalSumForExchange = sumReader.GetDouble(0);
                    }
                    string takeCurrencyCourse = "SELECT CurrencyBuyCourse FROM CurrencyBuy WHERE CurrencyBuyName = '" + firstCurrency.Text + "'";
                    SqlCommand takeCurrencyCourseCommand = new SqlCommand(takeCurrencyCourse, connection);
                    sumReader.Close();
                    SqlDataReader courseReader = takeCurrencyCourseCommand.ExecuteReader();
                    if (courseReader.HasRows)
                    {
                        while (courseReader.Read())
                        {
                            courseOfGeneralSum = courseReader.GetDouble(0);
                        }
                    }
                    courseReader.Close();
                }
                else
                {
                    sumReader.Close();
                    System.Windows.MessageBox.Show("У вас нет денежных средств в этой валюте.");
                    return;
                }
            }
            else
            {
                string takeSum = "SELECT UserMoney FROM BankCard WHERE CurrencyBuyCode = (SELECT CurrencyBuyCode FROM CurrencyBuy WHERE CurrencyBuyName = '" + firstCurrency.Text + "') AND CarNumber = '" + cardNumber + "'";
                SqlCommand takeCurrencyNameCommand = new SqlCommand(takeSum, connection);
                SqlDataReader sumReader = takeCurrencyNameCommand.ExecuteReader();
                if (sumReader.HasRows)
                {
                    while (sumReader.Read())
                    {
                        generalSumForExchange = sumReader.GetDouble(0);
                    }
                }
                sumReader.Close();
            }

            double courseOfLastCurrency = 1;

            string takeCourse = "SELECT CurrencyBuyCourse FROM CurrencyBuy WHERE CurrencyBuyName = '" + lastCurrency.Text + "'";
            SqlCommand takeLastCurrencyCourseCommand = new SqlCommand(takeCourse, connection);
            SqlDataReader lastCourseReader = takeLastCurrencyCourseCommand.ExecuteReader();
            if (lastCourseReader.HasRows)
            {
                while (lastCourseReader.Read())
                {
                    courseOfLastCurrency = lastCourseReader.GetDouble(0);
                }
            }
            lastCourseReader.Close();
            double generalSumAfter = generalSumForExchange;
            if (generalSumForExchange < Convert.ToDouble(sumForExchange.Text))
            {
                System.Windows.MessageBox.Show("У вас недостаточно срества для обмена.");
                return;
            }
            else
            {
                generalSumForExchange -= Convert.ToDouble(sumForExchange.Text);
                if (courseOfGeneralSum == 1)
                {
                    using (SqlConnection changeSum = new SqlConnection(connectionString))
                    using (SqlCommand command = changeSum.CreateCommand())
                    {
                        command.CommandText = "UPDATE BankCard SET UserMoney = @sum WHERE CardNumber = @num";

                        command.Parameters.AddWithValue("@sum", generalSumForExchange);
                        command.Parameters.AddWithValue("@num", cardNumber);

                        changeSum.Open();
                        command.ExecuteNonQuery();
                        changeSum.Close();
                    }

                    string checkRepit = "SELECT CardNumber, CurrencyName FROM CurrencyInfo WHERE CurrencyName = '" + lastCurrency.Text + "' AND CardNumber = '" + cardNumber + "'";
                    SqlConnection checkConnection = new SqlConnection(connectionString);
                    checkConnection.Open();
                    SqlCommand checkCommand = new SqlCommand(checkRepit, checkConnection);
                    SqlDataReader checkReader = checkCommand.ExecuteReader();
                    if (checkReader.HasRows)
                    {
                        using (SqlConnection changeSum = new SqlConnection(connectionString))
                        using (SqlCommand command = changeSum.CreateCommand())
                        {
                            command.CommandText = "UPDATE CurrencyInfo SET CurrencySum = CurrencySum + @sum WHERE CardNumber = @num AND CurrencyName = @cur";

                            command.Parameters.AddWithValue("@sum", Convert.ToDouble(sumForExchange.Text));
                            command.Parameters.AddWithValue("@num", cardNumber);
                            command.Parameters.AddWithValue("@cur", lastCurrency.Text);

                            changeSum.Open();
                            command.ExecuteNonQuery();
                            changeSum.Close();
                        }
                    }
                    else
                    {
                        using (SqlConnection changeSum = new SqlConnection(connectionString))
                        using (SqlCommand command = changeSum.CreateCommand())
                        {
                            command.CommandText = "INSERT INTO CurrencyInfo (CardNumber, CurrencySum, CurrencyName) VALUES (@num, @sum, @cur)";

                            command.Parameters.AddWithValue("@sum", Convert.ToDouble(sumForExchange.Text));
                            command.Parameters.AddWithValue("@num", cardNumber);
                            command.Parameters.AddWithValue("@cur", lastCurrency.Text);

                            changeSum.Open();
                            command.ExecuteNonQuery();
                            changeSum.Close();
                        }
                    }
                    checkReader.Close();
                    checkConnection.Close();

                    using (SqlConnection changeSum = new SqlConnection(connectionString))
                    using (SqlCommand command = changeSum.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO Statistic (CardNumber, OperationSum, Currency, OperationType) VALUES (@num, @sum, @cur, @type)";

                        command.Parameters.AddWithValue("@sum", Convert.ToDouble(sumForExchange.Text));
                        command.Parameters.AddWithValue("@num", cardNumber);
                        command.Parameters.AddWithValue("@cur", lastCurrency.Text);
                        command.Parameters.AddWithValue("@type", "Обмен");

                        changeSum.Open();
                        command.ExecuteNonQuery();
                        changeSum.Close();
                    }
                }
                else
                {
                    string blrName = string.Empty;
                    double exchangeSum = (Convert.ToDouble(sumForExchange.Text) * courseOfGeneralSum) / courseOfLastCurrency;
                    using (SqlConnection changeSum = new SqlConnection(connectionString))
                    using (SqlCommand command = changeSum.CreateCommand())
                    {
                        command.CommandText = "UPDATE CurrencyInfo SET CurrencySum = @sum WHERE CardNumber = @num AND CurrencyName = @cur";

                        command.Parameters.AddWithValue("@sum", generalSumForExchange);
                        command.Parameters.AddWithValue("@num", cardNumber);
                        command.Parameters.AddWithValue("@cur", firstCurrency.Text);

                        changeSum.Open();
                        command.ExecuteNonQuery();
                        changeSum.Close();
                    }
                    string checkRepit = "SELECT CardNumber, CurrencyName FROM CurrencyInfo WHERE CurrencyName = '" + lastCurrency.Text + "' AND CardNumber = '" + cardNumber + "'";
                    SqlConnection checkConnection = new SqlConnection(connectionString);
                    checkConnection.Open();
                    SqlCommand checkCommand = new SqlCommand(checkRepit, checkConnection);
                    SqlDataReader checkReader = checkCommand.ExecuteReader();
                    if (checkReader.HasRows)
                    {
                        while (checkReader.Read())
                        {
                            blrName = checkReader.GetString(1);
                        }
                        if (blrName == "Белорусский рубль (BYR)")
                        {
                            using (SqlConnection changeSum = new SqlConnection(connectionString))
                            using (SqlCommand command = changeSum.CreateCommand())
                            {
                                command.CommandText = "UPDATE BankCard SET UserMoney = UserMoney + @sum WHERE CardNumber = @num AND CurrencyBuyCode = (SELECT CurrencyBuyCode FROM CurrencyBuy WHERE CurrencyBuyName = @cur)";

                                command.Parameters.AddWithValue("@sum", exchangeSum);
                                command.Parameters.AddWithValue("@num", cardNumber);
                                command.Parameters.AddWithValue("@cur", blrName);

                                changeSum.Open();
                                command.ExecuteNonQuery();
                                changeSum.Close();
                            }
                        }
                        else
                        {
                            using (SqlConnection changeSum = new SqlConnection(connectionString))
                            using (SqlCommand command = changeSum.CreateCommand())
                            {
                                command.CommandText = "UPDATE CurrencyInfo SET CurrencySum = CurrencySum + @sum WHERE CardNumber = @num AND CurrencyName = @cur";

                                command.Parameters.AddWithValue("@sum", exchangeSum);
                                command.Parameters.AddWithValue("@num", cardNumber);
                                command.Parameters.AddWithValue("@cur", lastCurrency.Text);

                                changeSum.Open();
                                command.ExecuteNonQuery();
                                changeSum.Close();
                            }
                        }
                    }
                    else
                    {
                        if (lastCurrency.Text == "Белорусский рубль (BYR)")
                        {
                            using (SqlConnection changeSum = new SqlConnection(connectionString))
                            using (SqlCommand command = changeSum.CreateCommand())
                            {
                                command.CommandText = "UPDATE BankCard SET UserMoney = UserMoney + @sum WHERE CardNumber = @num AND CurrencyBuyCode = (SELECT CurrencyBuyCode FROM CurrencyBuy WHERE CurrencyBuyName = @cur)";

                                command.Parameters.AddWithValue("@sum", exchangeSum);
                                command.Parameters.AddWithValue("@num", cardNumber);
                                command.Parameters.AddWithValue("@cur", lastCurrency.Text);

                                changeSum.Open();
                                command.ExecuteNonQuery();
                                changeSum.Close();
                            }
                        }
                        else
                        {
                            using (SqlConnection changeSum = new SqlConnection(connectionString))
                            using (SqlCommand command = changeSum.CreateCommand())
                            {
                                command.CommandText = "INSERT INTO CurrencyInfo (CardNumber, CurrencySum, CurrencyName) VALUES (@num, @sum, @cur)";

                                command.Parameters.AddWithValue("@sum", exchangeSum);
                                command.Parameters.AddWithValue("@num", cardNumber);
                                command.Parameters.AddWithValue("@cur", lastCurrency.Text);

                                changeSum.Open();
                                command.ExecuteNonQuery();
                                changeSum.Close();
                            }
                        }
                    }
                    checkReader.Close();
                    checkConnection.Close();

                    using (SqlConnection changeSum = new SqlConnection(connectionString))
                    using (SqlCommand command = changeSum.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO Statistic (CardNumber, OperationSum, Currency, OperationType) VALUES (@num, @sum, @cur, @type)";

                        command.Parameters.AddWithValue("@sum", exchangeSum);
                        command.Parameters.AddWithValue("@num", cardNumber);
                        command.Parameters.AddWithValue("@cur", lastCurrency.Text);
                        command.Parameters.AddWithValue("@type", "Обмен");

                        changeSum.Open();
                        command.ExecuteNonQuery();
                        changeSum.Close();
                    }
                }
                MessageBoxResult mboxResult = System.Windows.MessageBox.Show("Вы успешно обменяли денежные средства. Желаете напечатать чек?", "Предупреждение", MessageBoxButton.YesNo);
                if (mboxResult == MessageBoxResult.Yes)
                {
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    Microsoft.Office.Interop.Word.Application app = new Microsoft.Office.Interop.Word.Application(); // открываем Word
                    try
                    {
                        Document doc = app.Documents.Open(System.IO.Path.GetFullPath(@"ШаблонЧека.docx")); // открыаем шаблон
                        ReplaseWord("{Date}", DateTime.Now.ToString(), doc);
                        string query = "SELECT UserName, UserSurname FROM Users WHERE UserLogin = '" + login + "'";
                        SqlCommand sqlCommand = new SqlCommand(query, connection);
                        SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                        if (sqlDataReader.HasRows)
                        {
                            while (sqlDataReader.Read())
                            {
                                ReplaseWord("{UserName}", sqlDataReader[0].ToString(), doc);
                                ReplaseWord("{UserSurname}", sqlDataReader[1].ToString(), doc);
                                ReplaseWord("{OperationType}", "Обмен", doc);
                                ReplaseWord("{Cost}", String.Format("{0:0.##}", ((generalSumAfter - generalSumForExchange) / courseOfGeneralSum)), doc);
                                ReplaseWord("{Cost}", String.Format("{0:0.##}", ((generalSumAfter - generalSumForExchange) / courseOfGeneralSum)), doc);
                                ReplaseWord("{CardNumber}", cardNumber, doc);
                                ReplaseWord("{Course}", courseOfLastCurrency.ToString(), doc);
                                ReplaseWord("{CurrentCurrency}", firstCurrency.Text, doc);
                                ReplaseWord("{Currency}", lastCurrency.Text, doc);
                                ReplaseWord("{LastCost}", sumForExchange.Text, doc);
                            }
                        }
                        sqlDataReader.Close();
                        path += @"\Чек об обмене валюты.docx";
                        doc.SaveAs2(path);
                        doc.Close();
                        connection.Close();
                    }
                    catch (Exception q)
                    {
                        app.Quit();
                        System.Windows.MessageBox.Show(q.Message);
                    }
                    finally
                    {
                        app.Quit();
                    }
                }
            }
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            HelpNavigator navigator = HelpNavigator.Topic;
            Help.ShowHelp(null, "help.chm", navigator, "obmen_valyuty.htm");
        }
    }
}