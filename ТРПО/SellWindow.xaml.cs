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
    /// Логика взаимодействия для SellWindow.xaml
    /// </summary>
    public partial class SellWindow : System.Windows.Window
    {
        string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=CEP_UPDATE;Integrated Security=True";

        public SellWindow()
        {
            InitializeComponent();
            List<string> namesList = new List<string>();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string query = @"SELECT CurrencySellName FROM CurrencySell ORDER BY CurrencySellName";
            SqlCommand sqlCommand = new SqlCommand(query, connection);
            SqlDataReader dataReader = sqlCommand.ExecuteReader();
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    namesList.Add(Convert.ToString(dataReader[0].ToString()));
                    inputCurrencyName.ItemsSource = namesList;
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

        private void SellButton_Click(object sender, RoutedEventArgs e)
        {
            StreamReader file = new StreamReader("UserLogin.txt");
            string login = file.ReadLine();
            file.Close();

            string cardNumber = string.Empty;

            if (inputCurrencyName.Text == string.Empty)
            {
                System.Windows.MessageBox.Show("Вы не выбрали продаваемую валюту.");
                return;
            }

            if (inputCurrencySum.Text != string.Empty)
            {
                char[] sumArray = inputCurrencySum.Text.ToCharArray();
                for (int i = 0; i < sumArray.Length; i++)
                {
                    if ((!char.IsDigit(sumArray[i]) && sumArray[i] != ','))
                    {
                        System.Windows.MessageBox.Show("Вы указали в сумме продаваемой валюты недопустимые символы.");
                        return;
                    }
                }
                if (Convert.ToDouble(inputCurrencySum.Text) < 0 || Convert.ToDouble(inputCurrencySum.Text) > 5000)
                {
                    System.Windows.MessageBox.Show("Введена неверная сумма продаваемой валюты. Можно ввести значение от 1 до 5000.");
                    return;
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Вы не ввели сумму продаваемой валюты.");
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

            double sum = 0;
            double sumAfter = 0;
            double courseSellingSum = 1;

            if (inputCurrencyName.Text == "Белорусский рубль (BYR)")
            {
                System.Windows.MessageBox.Show("Вы не можете продать белорусский рубль.");
                return;
            }
            else
            {
                string takeCurrencyName = "SELECT CurrencySum, CurrencyName FROM CurrencyInfo WHERE CurrencyName = '" + inputCurrencyName.Text + "' AND CardNumber = '" + cardNumber + "'";
                SqlCommand takeCurrencyNameCommand = new SqlCommand(takeCurrencyName, connection);
                SqlDataReader sumReader = takeCurrencyNameCommand.ExecuteReader();
                if (sumReader.HasRows)
                {
                    while (sumReader.Read())
                    {
                        sum = sumReader.GetDouble(0);
                    }
                    string takeCurrencyCourse = "SELECT CurrencySellCourse FROM CurrencySell WHERE CurrencySellName = '" + inputCurrencyName.Text + "'";
                    SqlCommand takeCurrencyCourseCommand = new SqlCommand(takeCurrencyCourse, connection);
                    sumReader.Close();
                    SqlDataReader courseReader = takeCurrencyCourseCommand.ExecuteReader();
                    if (courseReader.HasRows)
                    {
                        while (courseReader.Read())
                        {
                            courseSellingSum = courseReader.GetDouble(0);
                        }
                    }
                    courseReader.Close();
                }
                else
                {
                    sumReader.Close();
                    System.Windows.MessageBox.Show("У вас нет денежных средств в этой валюте");
                    return;
                }

                sumAfter = sum;
                double sellingSum = Convert.ToDouble(inputCurrencySum.Text);

                if (sum < sellingSum)
                {
                    System.Windows.MessageBox.Show("У вас недостаточно средств для продажи такого количества валюты.");
                    return;
                }
                else
                {
                    sum -= sellingSum;
                    using (SqlConnection changeSum = new SqlConnection(connectionString))
                    using (SqlCommand command = changeSum.CreateCommand())
                    {
                        command.CommandText = "UPDATE CurrencyInfo SET CurrencySum = @sum WHERE CardNumber = @num AND CurrencyName = @cur";

                        command.Parameters.AddWithValue("@sum", sum);
                        command.Parameters.AddWithValue("@num", cardNumber);
                        command.Parameters.AddWithValue("@cur", inputCurrencyName.Text);

                        changeSum.Open();
                        command.ExecuteNonQuery();
                        changeSum.Close();
                    }

                    using (SqlConnection changeSum = new SqlConnection(connectionString))
                    using (SqlCommand command = changeSum.CreateCommand())
                    {
                        command.CommandText = "UPDATE BankCard SET UserMoney = UserMoney + @sum WHERE CardNumber = @num";

                        command.Parameters.AddWithValue("@sum", (sellingSum * courseSellingSum));
                        command.Parameters.AddWithValue("@num", cardNumber);

                        changeSum.Open();
                        command.ExecuteNonQuery();
                        changeSum.Close();
                    }

                    using (SqlConnection changeSum = new SqlConnection(connectionString))
                    using (SqlCommand command = changeSum.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO Statistic (CardNumber, OperationSum, Currency, OperationType) VALUES (@num, @sum, @cur, @type)";

                        command.Parameters.AddWithValue("@sum", (sellingSum * courseSellingSum));
                        command.Parameters.AddWithValue("@num", cardNumber);
                        command.Parameters.AddWithValue("@cur", "Белорусский рубль (BYR)");
                        command.Parameters.AddWithValue("@type", "Продажа");

                        changeSum.Open();
                        command.ExecuteNonQuery();
                        changeSum.Close();
                    }
                }
            }
            MessageBoxResult mboxResult = System.Windows.MessageBox.Show("Вы успешно продали денежные средства. Желаете напечатать чек?", "Предупреждение", MessageBoxButton.YesNo);
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
                            ReplaseWord("{OperationType}", "Продажа", doc);
                            ReplaseWord("{Cost}", String.Format("{0:0.##}", inputCurrencySum.Text), doc);
                            ReplaseWord("{Cost}", String.Format("{0:0.##}", inputCurrencySum.Text), doc);
                            ReplaseWord("{CardNumber}", cardNumber, doc);
                            ReplaseWord("{Course}", courseSellingSum.ToString(), doc);
                            ReplaseWord("{CurrentCurrency}", inputCurrencyName.Text, doc);
                            ReplaseWord("{Currency}", "Белорусский рубль (BYR)", doc);
                            ReplaseWord("{LastCost}", (Convert.ToDouble(inputCurrencySum.Text) * courseSellingSum).ToString(), doc);
                        }
                    }
                    sqlDataReader.Close();
                    path += @"\Чек о продаже валюты.docx";
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
        
        private void Info_Click(object sender, RoutedEventArgs e)
        {
            HelpNavigator navigator = HelpNavigator.Topic;
            Help.ShowHelp(null, "help.chm", navigator, "prodazha_valyuty.htm");
        }
    }
}