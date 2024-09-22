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
    /// Interaction logic for CartItem.xaml
    /// </summary>
    public partial class CartItem : UserControl
    {
        public CartItem()
        {
            InitializeComponent();
        }
        // FoodImage Property
        public static readonly DependencyProperty FoodImageSourceCartProperty =
            DependencyProperty.Register("FoodImageSourceCart", typeof(ImageSource), typeof(CartItem), new PropertyMetadata(null));

        public ImageSource FoodImageSourceCart
        {
            get { return (ImageSource)GetValue(FoodImageSourceCartProperty); }
            set { SetValue(FoodImageSourceCartProperty, value); }
        }

        // FoodName Property
        public static readonly DependencyProperty FoodNameTextCartProperty =
            DependencyProperty.Register("FoodNameTextCart", typeof(string), typeof(CartItem), new PropertyMetadata("Food Name"));

        public string FoodNameTextCart
        {
            get { return (string)GetValue(FoodNameTextCartProperty); }
            set { SetValue(FoodNameTextCartProperty, value); }
        }

        // FoodPrice Property
        public static readonly DependencyProperty FoodPriceTextCartProperty =
            DependencyProperty.Register("FoodPriceTextCart", typeof(string), typeof(CartItem), new PropertyMetadata("$0.00"));

        public string FoodPriceTextCart
        {
            get { return (string)GetValue(FoodPriceTextCartProperty); }
            set { SetValue(FoodPriceTextCartProperty, value); }
        }

        private void ResponsiveText(object sender, SizeChangedEventArgs e)
        {
            double newFontSize = Math.Min(e.NewSize.Width / 20, e.NewSize.Height / 10) / 40;
            tbCartItemName.FontSize = 55 * newFontSize;
            tbCartItemPrice.FontSize = 55 * newFontSize;

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
