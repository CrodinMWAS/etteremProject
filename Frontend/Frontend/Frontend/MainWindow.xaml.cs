using System;
using System.Collections.Generic;
using System.IO;
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
        List<MenuItem> Menu = new List<MenuItem>();
        public MainWindow()
        {
            InitializeComponent();
            getMenu();
        }

        public void getMenu()
        {
            string filePath = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory)).FullName).FullName).FullName).FullName + "\\Restaurant\\SnackDashCompressed.txt";
            StreamReader reader = new StreamReader(filePath);
            while (!reader.EndOfStream)
            {
                string[] line = reader.ReadLine().Split(';');
                string[] allergens = line[2].Split(',');
                MenuItem newItem = new MenuItem(line[0], line[1], allergens, line[3]);
                Menu.Add(newItem);
            }
        }

        private void ResponsiveText(object sender, SizeChangedEventArgs e)
        {
            double newFontSize = Math.Min(e.NewSize.Width / 20, e.NewSize.Height / 10) / 40;
            btnLogin.FontSize = 15 * newFontSize;
            lblNavbar.FontSize = 20 * newFontSize;
            lblCategory1.FontSize = 10 * newFontSize;
            lblCategory2.FontSize = 10 * newFontSize;
            lblCategory3.FontSize = 10 * newFontSize;
            lblCategory4.FontSize = 10 * newFontSize;
            lblSelectedCategory.FontSize = 15 * newFontSize;
        }

        private void btn_register_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_login_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
