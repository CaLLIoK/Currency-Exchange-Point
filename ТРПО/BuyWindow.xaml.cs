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
    /// Логика взаимодействия для BuyWindow.xaml
    /// </summary>
    public partial class BuyWindow : System.Windows.Window
    {
        string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=CEP_UPDATE;Integrated Security=True";
        public BuyWindow()
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
                    payingField.ItemsSource = namesList;
                    currencyField.ItemsSource = namesList;
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

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            StreamReader file = new StreamReader("UserLogin.txt");
            string login = file.ReadLine();
            file.Close();

            string cardNumber = string.Empty;

            if (payingField.Text == string.Empty && currencyField.Text == string.Empty)
            {
                System.Windows.MessageBox.Show("Вы не выбрали все необходимые данные для покупки.");
                return;
            }
            else if (payingField.Text == currencyField.Text)
            {
                System.Windows.MessageBox.Show("Вы не можете купить денежные средства той же валюты.");
                return;
            }
            else if (payingField.Text != string.Empty && currencyField.Text == string.Empty)
            {
                System.Windows.MessageBox.Show("Вы не выбрали покупаемую валюту.");
                return;
            }
            else if (payingField.Text == string.Empty && currencyField.Text != string.Empty)
            {
                System.Windows.MessageBox.Show("Вы не выбрали валюту, которой будете расплачиваться.");
                return;
            }

            if (sumField.Text != string.Empty)
            {
                char[] sumArray = sumField.Text.ToCharArray();
                for (int i = 0; i < sumArray.Length; i++)
                {
                    if ((!char.IsDigit(sumArray[i]) && sumArray[i] != ','))
                    {
                        System.Windows.MessageBox.Show("Вы указали в сумме покупаемой валюты недопустимые символы.");
                        return;
                    }
                }
                if (Convert.ToDouble(sumField.Text) < 0 || Convert.ToDouble(sumField.Text) > 5000)
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

            double payingSum = 0;
            double coursePayingSum = 1;

            if (payingField.Text != "Белорусский рубль (BYR)")
            {
                string takeCurrencyName = "SELECT CurrencySum FROM CurrencyInfo WHERE CurrencyName = '" + payingField.Text + "' AND CardNumber = '" + cardNumber + "'";
                SqlCommand command = new SqlCommand(takeCurrencyName, connection);
                SqlDataReader sumReader = command.ExecuteReader();
                if (sumReader.HasRows)
                {
                    while (sumReader.Read())
                    {
                        payingSum = sumReader.GetDouble(0); //сумма денег валюты, которую ввели в качестве расплачиваемой!!!!
                    }
                    string takeCurrencyCourse = "SELECT CurrencyBuyCourse FROM CurrencyBuy WHERE CurrencyBuyName = '" + payingField.Text + "'";
                    SqlCommand takeCurrencyCourseCommand = new SqlCommand(takeCurrencyCourse, connection);
                    sumReader.Close();
                    SqlDataReader courseReader = takeCurrencyCourseCommand.ExecuteReader();
                    if (courseReader.HasRows)
                    {
                        while (courseReader.Read())
                        {
                            coursePayingSum = courseReader.GetDouble(0); //курс денег валюты, которую ввели в качестве расплачиваемой!!!!
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
                string takeSum = "SELECT UserMoney FROM BankCard WHERE CurrencyBuyCode = (SELECT CurrencyBuyCode FROM CurrencyBuy WHERE CurrencyBuyName = '" + payingField.Text + "') AND CardNumber = '" + cardNumber + "'";
                SqlCommand takeCurrencyNameCommand = new SqlCommand(takeSum, connection);
                SqlDataReader sumReader = takeCurrencyNameCommand.ExecuteReader();
                if (sumReader.HasRows)
                {
                    while (sumReader.Read())
                    {
                        payingSum = sumReader.GetDouble(0); //сумма денег валюты, которую ввели в качестве расплачиваемой!!!!
                    }
                }
                sumReader.Close();
            }

            string inputCurrencyName = string.Empty;
            double courseOfIputCurrency = 0;

            string takeInputCurrencyAndCourse = "SELECT CurrencyBuyName, CurrencyBuyCourse FROM CurrencyBuy WHERE CurrencyBuyName = '" + currencyField.Text + "'";
            SqlCommand takeInputCurrencyCourseCommand = new SqlCommand(takeInputCurrencyAndCourse, connection);
            SqlDataReader currencyNameAndCourseReader = takeInputCurrencyCourseCommand.ExecuteReader();
            if (currencyNameAndCourseReader.HasRows)
            {
                while (currencyNameAndCourseReader.Read())
                {
                    inputCurrencyName = currencyNameAndCourseReader.GetString(0); //название валюты, которую ввели для покупки!!!!
                    courseOfIputCurrency = currencyNameAndCourseReader.GetDouble(1); //курс валюты, которую ввели для покупки!!!!
                }
            }
            currencyNameAndCourseReader.Close();
            double generalPayingSum = payingSum * coursePayingSum;
            double generalAfterSum = generalPayingSum;
            double inputSUm = courseOfIputCurrency * Convert.ToDouble(sumField.Text);

            if (generalPayingSum < inputSUm)
            {
                System.Windows.MessageBox.Show("У вас недостаточно денег для покупки.");
            }
            else
            {
                generalPayingSum -= inputSUm;
                if (coursePayingSum == 1)
                {
                    using (SqlConnection changeSum = new SqlConnection(connectionString))
                    using (SqlCommand command = changeSum.CreateCommand())
                    {
                        command.CommandText = "UPDATE BankCard SET UserMoney = @sum WHERE CardNumber = @num";

                        command.Parameters.AddWithValue("@sum", generalPayingSum);
                        command.Parameters.AddWithValue("@num", cardNumber);

                        changeSum.Open();
                        command.ExecuteNonQuery();
                        changeSum.Close();
                    }

                    string checkRepit = "SELECT CardNumber, CurrencyName FROM CurrencyInfo WHERE CurrencyName = '" + currencyField.Text + "' AND CardNumber = '" + cardNumber + "'";
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

                            command.Parameters.AddWithValue("@sum", Convert.ToDouble(sumField.Text));
                            command.Parameters.AddWithValue("@num", cardNumber);
                            command.Parameters.AddWithValue("@cur", currencyField.Text);

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

                            command.Parameters.AddWithValue("@sum", Convert.ToDouble(sumField.Text));
                            command.Parameters.AddWithValue("@num", cardNumber);
                            command.Parameters.AddWithValue("@cur", currencyField.Text);

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

                        command.Parameters.AddWithValue("@sum", Convert.ToDouble(sumField.Text));
                        command.Parameters.AddWithValue("@num", cardNumber);
                        command.Parameters.AddWithValue("@cur", currencyField.Text);
                        command.Parameters.AddWithValue("@type", "Покупка");

                        changeSum.Open();
                        command.ExecuteNonQuery();
                        changeSum.Close();
                    }
                }
                else
                {
                    string blrName = string.Empty;
                    generalPayingSum /= coursePayingSum;
                    using (SqlConnection changeSum = new SqlConnection(connectionString))
                    using (SqlCommand command = changeSum.CreateCommand())
                    {
                        command.CommandText = "UPDATE CurrencyInfo SET CurrencySum = @sum WHERE CardNumber = @num AND CurrencyName = @cur";

                        command.Parameters.AddWithValue("@sum", generalPayingSum);
                        command.Parameters.AddWithValue("@num", cardNumber);
                        command.Parameters.AddWithValue("@cur", payingField.Text);

                        changeSum.Open();
                        command.ExecuteNonQuery();
                        changeSum.Close();
                    }
                    string checkRepit = "SELECT CardNumber, CurrencyName FROM CurrencyInfo WHERE CurrencyName = '" + currencyField.Text + "' AND CardNumber = '" + cardNumber + "'";
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

                                command.Parameters.AddWithValue("@sum", Convert.ToDouble(sumField.Text));
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

                                command.Parameters.AddWithValue("@sum", Convert.ToDouble(sumField.Text));
                                command.Parameters.AddWithValue("@num", cardNumber);
                                command.Parameters.AddWithValue("@cur", currencyField.Text);

                                changeSum.Open();
                                command.ExecuteNonQuery();
                                changeSum.Close();
                            }
                        }
                    }
                    else
                    {
                        if (currencyField.Text == "Белорусский рубль (BYR)")
                        {
                            using (SqlConnection changeSum = new SqlConnection(connectionString))
                            using (SqlCommand command = changeSum.CreateCommand())
                            {
                                command.CommandText = "UPDATE BankCard SET UserMoney = UserMoney + @sum WHERE CardNumber = @num AND CurrencyBuyCode = (SELECT CurrencyBuyCode FROM CurrencyBuy WHERE CurrencyBuyName = @cur)";

                                command.Parameters.AddWithValue("@sum", Convert.ToDouble(sumField.Text));
                                command.Parameters.AddWithValue("@num", cardNumber);
                                command.Parameters.AddWithValue("@cur", currencyField.Text);

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

                                command.Parameters.AddWithValue("@sum", Convert.ToDouble(sumField.Text));
                                command.Parameters.AddWithValue("@num", cardNumber);
                                command.Parameters.AddWithValue("@cur", currencyField.Text);

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

                        command.Parameters.AddWithValue("@sum", Convert.ToDouble(sumField.Text));
                        command.Parameters.AddWithValue("@num", cardNumber);
                        command.Parameters.AddWithValue("@cur", currencyField.Text);
                        command.Parameters.AddWithValue("@type", "Покупка");

                        changeSum.Open();
                        command.ExecuteNonQuery();
                        changeSum.Close();
                    }
                }
                MessageBoxResult mboxResult = System.Windows.MessageBox.Show("Вы успешно приобрели денежные средства. Желаете напечатать чек?", "Предупреждение", MessageBoxButton.YesNo);
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
                                ReplaseWord("{OperationType}", "Покупка", doc);
                                ReplaseWord("{Cost}", String.Format("{0:0.##}", ((generalAfterSum - generalPayingSum) / coursePayingSum)), doc);
                                ReplaseWord("{Cost}", String.Format("{0:0.##}", ((generalAfterSum - generalPayingSum) / coursePayingSum)), doc);
                                ReplaseWord("{CardNumber}", cardNumber, doc);
                                ReplaseWord("{Course}", courseOfIputCurrency.ToString(), doc);
                                ReplaseWord("{CurrentCurrency}", payingField.Text, doc);
                                ReplaseWord("{Currency}", currencyField.Text, doc);
                                ReplaseWord("{LastCost}", sumField.Text, doc);
                            }
                        }
                        sqlDataReader.Close();
                        path += @"\Чек о покупке валюты.docx";
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
                connection.Close();
            }
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            HelpNavigator navigator = HelpNavigator.Topic;
            Help.ShowHelp(null, "help.chm", navigator, "pokupka_valyuty.htm");
        }
    }
}