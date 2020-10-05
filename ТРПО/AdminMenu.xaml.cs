using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для AdminMenu.xaml
    /// </summary>
    public partial class AdminMenu : Window
    {
        public AdminMenu()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            AdminLoginWindow adminLoginWindow = new AdminLoginWindow();
            adminLoginWindow.Show();
            this.Close();
        }

        private void Statistic_Click(object sender, RoutedEventArgs e)
        {
            Statistic statistic = new Statistic();
            statistic.Show();
            this.Close();
        }

        private void ChangeUserdData_Click(object sender, RoutedEventArgs e)
        {
            ChangeUserData changeUserData = new ChangeUserData();
            changeUserData.Show();
            this.Close();
        }

        private void ChangeCurrencyBuyData_Click(object sender, RoutedEventArgs e)
        {
            ChangeCurrencyBuyExchangeCourse changeCurrencyBuyExchangeCourse = new ChangeCurrencyBuyExchangeCourse();
            changeCurrencyBuyExchangeCourse.Show();
            this.Close();
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            AddUser addUser = new AddUser();
            addUser.Show();
            this.Close();
        }

        private void AddCurrencyBuyCourse_Click(object sender, RoutedEventArgs e)
        {
            AddBuyExchangeCurrency addBuyExchangeCurrency = new AddBuyExchangeCurrency();
            addBuyExchangeCurrency.Show();
            this.Close();
        }

        private void DeleteCurrencyBuyCourse_Click(object sender, RoutedEventArgs e)
        {
            DeleteBuyExchangeCurrency deleteBuyExchangeCurrency = new DeleteBuyExchangeCurrency();
            deleteBuyExchangeCurrency.Show();
            this.Close();
        }

        private void DeleteCurrencySellCourse_Click(object sender, RoutedEventArgs e)
        {
            DeleteCurrencySell deleteCurrencySell = new DeleteCurrencySell();
            deleteCurrencySell.Show();
            this.Close();
        }

        private void AddCurrencySellCourse_Click(object sender, RoutedEventArgs e)
        {
            AddCurrencySell addCurrencySell = new AddCurrencySell();
            addCurrencySell.Show();
            this.Close();
        }

        private void Deleteuser_Click(object sender, RoutedEventArgs e)
        {
            DeleteUser deleteUser = new DeleteUser();
            deleteUser.Show();
            this.Close();
        }

        private void ChangeCurrencySellData_Click(object sender, RoutedEventArgs e)
        {
            ChangeCurrencySellCourse changeCurrencySellCourse = new ChangeCurrencySellCourse();
            changeCurrencySellCourse.Show();
            this.Close();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            HelpNavigator navigator = HelpNavigator.Topic;
            Help.ShowHelp(null, "help.chm", navigator, "rukovodstvo_administratora.htm");
        }
    }
}