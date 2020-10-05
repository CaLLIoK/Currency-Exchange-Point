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
using Microsoft.Office.Interop.Word;

namespace CEP
{
    /// <summary>
    /// Логика взаимодействия для CheckUserMoney.xaml
    /// </summary>
    public partial class CheckUserMoney : System.Windows.Window
    {
        string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=CEP_UPDATE;Integrated Security=True";

        public CheckUserMoney()
        {
            InitializeComponent();
            StreamReader streamReader = new StreamReader("CardNumber.txt");
            string card = streamReader.ReadLine();
            streamReader.Close();
            StreamReader sreader = new StreamReader("UserLogin.txt");
            string login = sreader.ReadLine();
            sreader.Close();
            string money = string.Empty;
            SqlConnection connection = new SqlConnection(connectionString);
            string query = "SELECT CurrencyName, CurrencySum FROM CurrencyInfo WHERE CurrencyInfo.CardNumber = '" + card + "'";
            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader read = cmd.ExecuteReader();
            if(read.HasRows)
            {
                while(read.Read())
                {
                    money = read[0].ToString() + " - " + read[1].ToString();
                    moneyList.Items.Add(money);
                }
            }
            read.Close();
            string query2 = "SELECT CurrencyBuyName, UserMoney FROM BankCard, CurrencyBuy WHERE BankCard.CardNumber = '" + card + "' AND BankCard.CurrencyBuyCode = CurrencyBuy.CurrencyBuyCode";
            SqlCommand command = new SqlCommand(query2, connection);
            SqlDataReader reader = command.ExecuteReader();
            if(reader.HasRows)
            {
                while(reader.Read())
                {
                    money = reader[0].ToString() + " - " + reader[1].ToString();
                    moneyList.Items.Add(money);
                }
            }
            reader.Close();
            MessageBoxResult mboxResult = System.Windows.MessageBox.Show("Желаете напечатать справку об остатке на карте?", "Предупреждение", MessageBoxButton.YesNo);
            if (mboxResult == MessageBoxResult.Yes)
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                Microsoft.Office.Interop.Word.Application app = new Microsoft.Office.Interop.Word.Application(); // открываем Word
                try
                {
                    Document doc = app.Documents.Open(System.IO.Path.GetFullPath(@"ШаблонОстатка.docx"));
                    ReplaseWord("{Date}", DateTime.Now.ToString(), doc);
                    ReplaseWord("{CardNumber}", card, doc);
                    string query1 = "SELECT UserName, UserSurname FROM Users WHERE UserLogin = '" + login + "'";
                    SqlCommand sqlCommand = new SqlCommand(query1, connection);
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    if (sqlDataReader.HasRows)
                    {
                        while (sqlDataReader.Read())
                        {
                            ReplaseWord("{UserName}", sqlDataReader[0].ToString(), doc);
                            ReplaseWord("{UserSurname}", sqlDataReader[1].ToString(), doc);
                        }
                    }
                    sqlDataReader.Close();
                    Range r = doc.Range();
                    int i = 1;
                    foreach (var x in moneyList.Items)
                    {
                        doc.Tables[1].Cell(i, 1).Range.Text += x.ToString();
                        ++i;
                    }
                    path += @"\Остаток на карте.docx";
                    doc.SaveAs2(path);
                    doc.Close();
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
                connection.Close();
            }
        }

        private void ReplaseWord(string StubReplase, string Text, Document Doc)
        {
            var range = Doc.Content;
            range.Find.ClearFormatting();
            range.Find.Execute(FindText: StubReplase, ReplaceWith: Text);
        }

        private void Close_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            UserMenu userMenu = new UserMenu();
            userMenu.Show();
            this.Close();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            HelpNavigator navigator = HelpNavigator.Topic;
            Help.ShowHelp(null, "help.chm", navigator, "ostatok_na_karte.htm");
        }
    }
}