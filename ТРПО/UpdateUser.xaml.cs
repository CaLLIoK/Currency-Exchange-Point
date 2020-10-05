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
    /// Логика взаимодействия для UpdateUser.xaml
    /// </summary>
    public partial class UpdateUser : Window
    {
        string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=CEP_UPDATE;Integrated Security=True";

        public UpdateUser()
        {
            InitializeComponent();
            List<int> codeList = new List<int>();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string query = @"SELECT UserCode FROM Users ORDER BY UserCode";
            SqlCommand sqlCommand = new SqlCommand(query, connection);
            SqlDataReader dataReader = sqlCommand.ExecuteReader();
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    codeList.Add(Convert.ToInt32(dataReader[0].ToString()));
                    searchCriterion.ItemsSource = codeList;
                }
            }
            connection.Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        private void BackToWindow(object sender, RoutedEventArgs e)
        {
            ChangeUserData changeUser = new ChangeUserData();
            changeUser.Show();
            this.Close();
        }

        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            if (criterion.Text != string.Empty && searchCriterion.Text != string.Empty)
            {
                int uC = Convert.ToInt32(searchCriterion.Text);
                if (criterion.Text == "Логин")
                {
                    if (changingCriterion.Text != CheckUser.CheckUserLogin(changingCriterion.Text))
                    {
                        System.Windows.MessageBox.Show(CheckUser.CheckUserLogin(changingCriterion.Text));
                        return;
                    }
                    string mySelectQuery = "SELECT UserLogin FROM Users WHERE [UserLogin] = '" + changingCriterion.Text + "'";
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(mySelectQuery, connection))
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
                            using (SqlCommand lastCommnd = connection.CreateCommand())
                            {
                                lastCommnd.CommandText = "UPDATE Users SET UserLogin = @login WHERE UserCode = @code";
                                lastCommnd.Parameters.AddWithValue("@login", changingCriterion.Text);
                                lastCommnd.Parameters.AddWithValue("@code", uC);

                                lastCommnd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                else if (criterion.Text == "Пароль")
                {
                    if (changingCriterion.Text != CheckUser.CheckUserPassword(changingCriterion.Text))
                    {
                        System.Windows.MessageBox.Show(CheckUser.CheckUserPassword(changingCriterion.Text));
                        return;
                    }
                    using (SqlCommand lastCommnd = connection.CreateCommand())
                    {
                        lastCommnd.CommandText = "UPDATE Users SET UserPassword = @password WHERE UserCode = @code";
                        lastCommnd.Parameters.AddWithValue("@password", changingCriterion.Text);
                        lastCommnd.Parameters.AddWithValue("@code", uC);

                        lastCommnd.ExecuteNonQuery();
                    }
                }
                else if (criterion.Text == "Статус администратора")
                {
                    if (changingCriterion.Text != CheckUser.CheckUserStatus(changingCriterion.Text))
                    {
                        System.Windows.MessageBox.Show(CheckUser.CheckUserStatus(changingCriterion.Text));
                        return;
                    }
                    using (SqlCommand lastCommnd = connection.CreateCommand())
                    {
                        lastCommnd.CommandText = "UPDATE Users SET AdministratorStatus = @status WHERE UserCode = @code";
                        lastCommnd.Parameters.AddWithValue("@status", changingCriterion.Text);
                        lastCommnd.Parameters.AddWithValue("@code", uC);

                        lastCommnd.ExecuteNonQuery();
                    }
                }
                else if (criterion.Text == "Имя")
                {
                    if (changingCriterion.Text != CheckUser.CheckUserName(changingCriterion.Text))
                    {
                        System.Windows.MessageBox.Show(CheckUser.CheckUserName(changingCriterion.Text));
                        return;
                    }
                    using (SqlCommand lastCommnd = connection.CreateCommand())
                    {
                        lastCommnd.CommandText = "UPDATE Users SET UserName = @name WHERE UserCode = @code";
                        lastCommnd.Parameters.AddWithValue("@name", changingCriterion.Text);
                        lastCommnd.Parameters.AddWithValue("@code", uC);

                        lastCommnd.ExecuteNonQuery();
                    }
                }
                else if (criterion.Text == "Фамилия")
                {
                    if (changingCriterion.Text != CheckUser.CheckUserSurname(changingCriterion.Text))
                    {
                        System.Windows.MessageBox.Show(CheckUser.CheckUserSurname(changingCriterion.Text));
                        return;
                    }
                    using (SqlCommand lastCommnd = connection.CreateCommand())
                    {
                        lastCommnd.CommandText = "UPDATE Users SET UserSurname = @surname WHERE UserCode = @code";
                        lastCommnd.Parameters.AddWithValue("@surname", changingCriterion.Text);
                        lastCommnd.Parameters.AddWithValue("@code", uC);

                        lastCommnd.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Вы не выбрали данные для изменения.");
                return;
            }
            MessageBoxResult mboxResult = System.Windows.MessageBox.Show("Данные обновлены. Желаете изменить что-нибудь еще?", "Предупреждение", MessageBoxButton.YesNo);
            if (mboxResult == MessageBoxResult.No)
            {
                ChangeUserData changeUserData = new ChangeUserData();
                changeUserData.Show();
                this.Close();
            }
            connection.Close();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            HelpNavigator navigator = HelpNavigator.Topic;
            Help.ShowHelp(null, "help.chm", navigator, "obnovlenie_dannykh_polzovatelej.htm");
        }
    }
}