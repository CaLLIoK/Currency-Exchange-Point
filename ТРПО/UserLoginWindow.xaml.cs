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
    /// Логика взаимодействия для UserLoginWindow.xaml
    /// </summary>
    public partial class UserLoginWindow : Window
    {
        public string connectionString = @"Data Source=(local)\SQLEXPRESS; Initial Catalog=CEP_UPDATE; Integrated Security=True";
        public UserLoginWindow()
        {
            InitializeComponent();
        }
        private void Close_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        private void MainMenu(object sender, RoutedEventArgs e)
        {
            string userLogin = loginField.Text;
            string userPassword = passwordField.Password.ToString();
            string userName = string.Empty;
            string userSurname = string.Empty;

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string query = @"SELECT UserName, UserSurname FROM Users WHERE [UserLogin] = '" + userLogin + "'and [UserPassword]='" + userPassword + "' and [AdministratorStatus] = 'false'";
            SqlCommand sqlCommand = new SqlCommand(query, connection);
            SqlDataReader dataReader = sqlCommand.ExecuteReader();
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    userName = dataReader[0].ToString();
                    userSurname = dataReader[1].ToString();
                    StreamWriter loginFile = new StreamWriter("UserLogin.txt");
                    loginFile.Write(userLogin);
                    loginFile.Close();

                    StreamWriter nameFile = new StreamWriter("UserName.txt");
                    nameFile.Write(userName);
                    nameFile.Close();

                    StreamWriter surnameFile = new StreamWriter("UserSurname.txt");
                    surnameFile.Write(userSurname);
                    surnameFile.Close();

                    UserMenu userMenu = new UserMenu();
                    userMenu.Show();
                    this.Close();
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Неверный логин или пароль");
                return;
            }
            connection.Close();
        }

        private void Havenoacc_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }

        private void AdminLogin(object sender, RoutedEventArgs e)
        {
            AdminLoginWindow adminLoginWindow = new AdminLoginWindow();
            adminLoginWindow.Show();
            this.Close();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            HelpNavigator navigator = HelpNavigator.Topic;
            Help.ShowHelp(null, "help.chm", navigator, "obnovlenie_dannykh_polzovatelej.htm");
        }
    }
}
