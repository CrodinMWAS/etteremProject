using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        //ObservableCollection<MenuItem> MenuItems = new ObservableCollection<MenuItem>();

        public ObservableCollection<MenuItem> menuItems { get; set; } = new ObservableCollection<MenuItem>();
        public MainWindow()
        {
            getMenu();
            InitializeComponent();
        }

        public void getMenu()
        {
            string basePath = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory)).FullName).FullName).FullName).FullName;
            string filePath = basePath + "\\Restaurant\\SnackDashCompressed.txt";
            StreamReader reader = new StreamReader(filePath);
            while (!reader.EndOfStream)
            {
                string[] line = reader.ReadLine().Split(';');
                string[] allergens = line[3].Split(',');
                string pic = $"{basePath}\\{line[1]}";
                MenuItem newItem;

                switch (allergens.Length)
                {
                    case 1:
                        if (allergens[0] != "null")
                        {
                            newItem = new MenuItem(line[0], pic, line[2], line[4], line[5], allergens[0]);
                        }
                        else
                        {
                            newItem = new MenuItem(line[0], pic, line[2], line[4], line[5]);
                        }
                            break;
                    case 2:
                        newItem = new MenuItem(line[0], pic, line[2], line[4], line[5], allergens[0], allergens[1]);
                        break;
                    case 3:
                        newItem = new MenuItem(line[0], pic, line[2], line[4], line[5], allergens[0], allergens[1], allergens[2]);
                        break;
                    default:
                        newItem = new MenuItem(line[0], pic, line[2], line[4], line[5]);
                        break;
                }
                menuItems.Add(newItem);
            }
            reader.Close();
        }

        private void ResponsiveText(object sender, SizeChangedEventArgs e)
        {
            double newFontSize = Math.Min(e.NewSize.Width / 20, e.NewSize.Height / 10) / 40;
            btnLogin.FontSize = 15 * newFontSize;
            lblNavbar.FontSize = 20 * newFontSize;
            tbCategory1.FontSize = 10 * newFontSize;
            tbCategory2.FontSize = 10 * newFontSize;
            tbCategory3.FontSize = 10 * newFontSize;
            tbCategory4.FontSize = 10 * newFontSize;
            lblSelectedCategory.FontSize = 15 * newFontSize;
        }

        private void btn_register_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_login_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FilterCategory(object sender, MouseButtonEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            string textBlockText = textBlock.Text;

            List<MenuItem> filteredItems = new List<MenuItem>();
            TextBlock[] categories = new TextBlock[] { tbCategory1, tbCategory2, tbCategory3, tbCategory4 };

            if (textBlock.TextDecorations != TextDecorations.Underline)
            {
                foreach (var item in categories)
                {
                    item.TextDecorations = null;
                    item.Foreground = new SolidColorBrush(Colors.Black);
                }
                textBlock.TextDecorations = TextDecorations.Underline;
                textBlock.Foreground = new SolidColorBrush(Colors.DarkRed);

                lblSelectedCategory.Text = textBlockText;

                for (int i = 0; i != menuItems.Count; i++)
                {
                    if (textBlockText == menuItems[i].Category)
                    {
                        filteredItems.Add(menuItems[i]);
                    }
                }

                itemsControl.ItemsSource = filteredItems;
            }
            else
            {
                textBlock.Foreground = new SolidColorBrush(Colors.Black);
                textBlock.TextDecorations = null;
                lblSelectedCategory.Text = "All Categories";
                itemsControl.ItemsSource = menuItems;
            }
        }
    }
}
