using Frontend.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace Frontend
{
    /// <summary>
    /// Admin.xaml の相互作用ロジック
    /// </summary>
    public partial class Admin : Window
    {
        public ObservableCollection<Stock> stocks = new ObservableCollection<Stock>();
        
        public Admin()
        {
            InitializeComponent();
            stocks.Add(new Stock("Cheese", 10));
            stocks.Add(new Stock("Flour", 20));
            dgIngridients.ItemsSource = stocks;
        }

        private void btnModify(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Stock st = btn.Tag as Stock;
            AdminModify am = new AdminModify(st);
            am.Show();
        }
    }
}
