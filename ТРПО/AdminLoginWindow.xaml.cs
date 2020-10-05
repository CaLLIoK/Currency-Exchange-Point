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
    /// Логика взаимодействия для AdminLoginWindow.xaml
    /// </summary>
    public partial class AdminLoginWindow : Window
    {
        string myConnectionString = @"Data Source=(local)\SQLEXPRESS; Initial Catalog=CEP_UPDATE; Integrated Security=True";

        public AdminLoginWindow()
        {
            InitializeComponent();
        }
        private void Close_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            string userLogin = loginField.Text;
            string userPassword = passwordField.Password.ToString();

            string mySelectQuery = "SELECT * FROM Users WHERE [UserLogin] = '" + userLogin + "'and [UserPassword]='" + userPassword + "' and [AdministratorStatus] = 'true'";
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(mySelectQuery, myConnectionString))
            {
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    FileStream fs = new FileStream("UserLogin.txt", FileMode.Create);
                    StreamWriter loginFile = new StreamWriter(fs);
                    loginFile.Close();
                    AdminMenu adminMenu = new AdminMenu();
                    adminMenu.Show();
                    this.Close();
                }
                else if (table.Rows.Count == 0)
                {
                    System.Windows.MessageBox.Show("Неверный логин или пароль");
                    return;
                }
            }
        }

        private void User_Click(object sender, RoutedEventArgs e)
        {
            UserLoginWindow userLoginWindow = new UserLoginWindow();
            userLoginWindow.Show();
            this.Close();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            HelpNavigator navigator = HelpNavigator.Topic;
            Help.ShowHelp(null, "help.chm", navigator, "avtorizatsiya_ot_imeni_administratora.htm");
        }
    }
}