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
    /// Логика взаимодействия для RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        SqlConnection myConnectionString = new SqlConnection(@"Data Source=(local)\SQLEXPRESS; Initial Catalog=CEP_UPDATE; Integrated Security=True");

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        private void Registration_Click(object sender, RoutedEventArgs e)
        {
            if (nameField.Text != CheckUser.CheckUserName(nameField.Text))
            {
                System.Windows.MessageBox.Show(CheckUser.CheckUserName(nameField.Text));
                return;
            }

            if (surnamField.Text != CheckUser.CheckUserSurname(surnamField.Text))
            {
                System.Windows.MessageBox.Show(CheckUser.CheckUserSurname(surnamField.Text));
                return;
            }

            if (loginField.Text != CheckUser.CheckUserLogin(loginField.Text))
            {
                System.Windows.MessageBox.Show(CheckUser.CheckUserLogin(loginField.Text));
                return;
            }

            if (passwordField.Password.ToString() != CheckUser.CheckUserPassword(passwordField.Password.ToString()))
            {
                System.Windows.MessageBox.Show(CheckUser.CheckUserPassword(passwordField.Password.ToString()));
                return;
            }

            string userLogin = loginField.Text;

            string mySelectQuery = "SELECT * FROM Users WHERE [UserLogin] = '" + userLogin + "'";
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(mySelectQuery, myConnectionString))
            {
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    System.Windows.MessageBox.Show("Этот логин занят. Введите другой");
                    return;
                }
                else if (table.Rows.Count == 0)
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "INSERT Users (UserLogin, UserPassword, UserName, UserSurname) VALUES (@login, @password, @name, @surname)";
                    cmd.Parameters.Add("@login", SqlDbType.VarChar).Value = loginField.Text;
                    cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = passwordField.Password.ToString();
                    cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = nameField.Text;
                    cmd.Parameters.Add("@surname", SqlDbType.VarChar).Value = surnamField.Text;
                    cmd.Connection = myConnectionString;
                    myConnectionString.Open();
                    cmd.ExecuteNonQuery();
                    myConnectionString.Close();
                    System.Windows.MessageBox.Show("Регистрация прошла успешно");
                    UserLoginWindow userLoginWindow = new UserLoginWindow();
                    userLoginWindow.Show();
                    this.Close();
                }
            }
        }
        
        private void Info_Click(object sender, RoutedEventArgs e)
        {
            HelpNavigator navigator = HelpNavigator.Topic;
            Help.ShowHelp(null, "help.chm", navigator, "registratsiya_polzovatelya.htm");
        }
    }
}
