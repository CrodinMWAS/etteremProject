using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Frontend
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        { 
            InitializeComponent(); 
        } 

        private void btn_login_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
        }

        private void ResponsiveText(object sender, SizeChangedEventArgs e)
        {
            double newFontSize = Math.Min(e.NewSize.Width / 20, e.NewSize.Height / 10)/40; 
            txt1.FontSize = 35 * newFontSize;
            txt2.FontSize = 15 * newFontSize;
            txt3.FontSize = 15 * newFontSize;
            txt4.FontSize = 15 * newFontSize;
            btn_login.FontSize = 15 * newFontSize;
        }
    }
}
