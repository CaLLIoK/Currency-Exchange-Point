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
    /// Логика взаимодействия для ChooseCurrencyCourse.xaml
    /// </summary>
    public partial class ChooseCurrencyCourse : Window
    {
        public ChooseCurrencyCourse()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        private void BackToMenu(object sender, RoutedEventArgs e)
        {
            UserMenu userMenu = new UserMenu();
            userMenu.Show();
            this.Close();
        }

        private void CurrencySellCourse_Click(object sender, RoutedEventArgs e)
        {
            CurrensuSellCourse currensuSellCourse = new CurrensuSellCourse();
            currensuSellCourse.Show();
            this.Close();
        }

        private void CurrencyBuyCourse_Click(object sender, RoutedEventArgs e)
        {
            CurrencyBuyExchangeCourse currencyBuyExchangeCourse = new CurrencyBuyExchangeCourse();
            currencyBuyExchangeCourse.Show();
            this.Close();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            HelpNavigator navigator = HelpNavigator.Topic;
            Help.ShowHelp(null, "help.chm", navigator, "informatsiya_o_kursakh_valyut.htm");
        }
    }
}
