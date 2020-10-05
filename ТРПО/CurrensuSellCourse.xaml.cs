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
    /// Логика взаимодействия для CurrensuSellCourse.xaml
    /// </summary>
    public partial class CurrensuSellCourse : Window
    {
        public string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=CEP_UPDATE;Integrated Security=True";
        DataTable table;
        public CurrensuSellCourse()
        {
            InitializeComponent();
            SqlConnection connection = new SqlConnection(connectionString);
            string query = "SELECT * FROM CurrencySell WHERE [CurrencySellName] != 'Белорусский рубль (BYR)'";
            connection.Open();
            table = new DataTable();
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                using (IDataReader rdr = cmd.ExecuteReader())
                {
                    table.Load(rdr);
                }
            }
            UsersGrid.ItemsSource = table.DefaultView;
            connection.Close();

        }
        private void Close_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            ChooseCurrencyCourse chooseCurrencyCourse = new ChooseCurrencyCourse();
            chooseCurrencyCourse.Show();
            this.Close();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            HelpNavigator navigator = HelpNavigator.Topic;
            Help.ShowHelp(null, "help.chm", navigator, "informatsiya_o_kurse_valyut_dlya_prodazhi.htm");
        }
    }
}
