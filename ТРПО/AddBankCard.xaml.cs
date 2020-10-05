using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для AddBankCard.xaml
    /// </summary>
    public partial class AddBankCard : Window
    {
        string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=CEP_UPDATE;Integrated Security=True";

        public AddBankCard()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            StreamReader file = new StreamReader("UserLogin.txt");
            string login = file.ReadLine();
            file.Close();

            StreamReader usrNm = new StreamReader("UserName.txt");
            string userNm = usrNm.ReadLine();
            usrNm.Close();

            StreamReader usrSnm = new StreamReader("UserSurname.txt");
            string userSnm = usrSnm.ReadLine();
            usrSnm.Close();

            if (cardNumber.Text == string.Empty)
            {
                System.Windows.MessageBox.Show("Номер карты не может быть пустым.");
                return;
            }
            else if (cardNumber.Text.Length == 16)
            {
                char[] cardNumberArray = cardNumber.Text.ToCharArray();
                for (int i = 0; i < cardNumberArray.Length; i++)
                {
                    if (!char.IsDigit(cardNumberArray[i]))
                    {
                        System.Windows.MessageBox.Show("Вы указали неверные символы для номера карты. ");
                        return;
                    }
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Вы неверно ввели номер карты.");
                return;
            }

            if (userName.Text != CheckUser.CheckUserName(userName.Text))
            {
                System.Windows.MessageBox.Show(CheckUser.CheckUserName(userName.Text));
                return;
            }
            else
            {
                if (userName.Text != userNm)
                {
                    System.Windows.MessageBox.Show("Имя на карте должно совпадать с именем пользователя.");
                    return;
                }
            }

            if (userSurname.Text != CheckUser.CheckUserSurname(userSurname.Text))
            {
                System.Windows.MessageBox.Show(CheckUser.CheckUserSurname(userSurname.Text));
                return;
            }
            else
            {
                if (userSurname.Text != userSnm)
                {
                    System.Windows.MessageBox.Show("Фамилия на карте должна совпадать с фамилией пользователя.");
                    return;
                }
            }

            if (cardExpiration.Text == string.Empty)
            {
                System.Windows.MessageBox.Show("Дата действия карты не может быть пустым.");
                return;
            }
            else
            {
                if (!Regex.IsMatch(cardExpiration.Text, @"(01|02|03|04|05|06|07|08|09|10|11|12)/2[2-9]$"))
                {
                    System.Windows.MessageBox.Show("Вы указали неверную дату.");
                    return;
                }
            }

            if (CVVCode.Text == string.Empty)
            {
                System.Windows.MessageBox.Show("CVV код карты не может быть пустым.");
                return;
            }
            else if (CVVCode.Text.Length == 3)
            {
                char[] cvvCodeArray = CVVCode.Text.ToCharArray();
                for (int i = 0; i < cvvCodeArray.Length; i++)
                {
                    if (!char.IsDigit(cvvCodeArray[i]))
                    {
                        System.Windows.MessageBox.Show("Вы указали неверные символы для CVV кода карты. ");
                        return;
                    }
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Вы неверно ввели CVV код. CVV код - трехзначиное число.");
                return;
            }    

            Random rnd = new Random();
            double money = rnd.Next(100, 2500);
            SqlConnection myConnectionString = new SqlConnection(connectionString);
            string mySelectQuery = "SELECT * FROM BankCard WHERE Cardnumber = '" + cardNumber.Text + "'";
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(mySelectQuery, myConnectionString))
            {
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    System.Windows.MessageBox.Show("Это карта уже есть была привязана другим пользователем.");
                }
                else if (table.Rows.Count == 0)
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "INSERT BankCard (UserCode, CardNumber, UserSurname, UserName, ValidDate, CVVCode, UserMoney, CurrencyBuyCode, CurrencySellCode) VALUES ((SELECT UserCode FROM Users WHERE UserLogin = '" + login + "'),'" + cardNumber.Text + "', '" + userSurname.Text + "', '" + userName.Text + "', '" + cardExpiration.Text + "', " + Convert.ToInt32(CVVCode.Text) + ", '" + money + "', 1, 1)";
                    cmd.Connection = myConnectionString;
                    myConnectionString.Open();
                    cmd.ExecuteNonQuery();
                    myConnectionString.Close();
                    System.Windows.MessageBox.Show("Карта добавлена!");
                    UserMenu userMenu = new UserMenu();
                    userMenu.Show();
                    this.Close();
                }
            }
        }

        private void BackToMenu(object sender, RoutedEventArgs e)
        {
            UserMenu userMenu = new UserMenu();
            userMenu.Show();
            this.Close();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            HelpNavigator navigator = HelpNavigator.Topic;
            Help.ShowHelp(null, "help.chm", navigator, "privyazka_karty.htm");
        }
    }
}
