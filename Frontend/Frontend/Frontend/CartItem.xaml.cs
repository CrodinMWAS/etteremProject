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
    public partial class CartContent : UserControl
    {
        public CartContent()
        {
            InitializeComponent();
        }
        // FoodImage Property
        public static readonly DependencyProperty FoodImageSourceCartProperty =
            DependencyProperty.Register("FoodImageSourceCart", typeof(ImageSource), typeof(CartContent), new PropertyMetadata(null));

        public ImageSource FoodImageSourceCart
        {
            get { return (ImageSource)GetValue(FoodImageSourceCartProperty); }
            set { SetValue(FoodImageSourceCartProperty, value); }
        }

        // FoodName Property
        public static readonly DependencyProperty FoodNameTextCartProperty =
            DependencyProperty.Register("FoodNameTextCart", typeof(string), typeof(CartContent), new PropertyMetadata("Food Name"));

        public string FoodNameTextCart
        {
            get { return (string)GetValue(FoodNameTextCartProperty); }
            set { SetValue(FoodNameTextCartProperty, value); }
        }

        // FoodPrice Property
        public static readonly DependencyProperty FoodPriceTextCartProperty =
            DependencyProperty.Register("FoodPriceTextCart", typeof(string), typeof(CartContent), new PropertyMetadata("$0.00"));

        public string FoodPriceTextCart
        {
            get { return (string)GetValue(FoodPriceTextCartProperty); }
            set { SetValue(FoodPriceTextCartProperty, value); }
        }

        // Amount Property
        public static readonly DependencyProperty AmountProperty =
            DependencyProperty.Register("Amount", typeof(int), typeof(CartContent), new PropertyMetadata(1));  // Default value is 1

        public int Amount
        {
            get { return (int)GetValue(AmountProperty); }
            set { SetValue(AmountProperty, value); }
        }

        private void ResponsiveText(object sender, SizeChangedEventArgs e)
        {
            double newFontSize = Math.Min(e.NewSize.Width / 20, e.NewSize.Height / 10) / 40;
            tbCartItemName.FontSize = 55 * newFontSize;
            tbCartItemPrice.FontSize = 55 * newFontSize;
            tbCartItemAmount.FontSize = 55 * newFontSize;
        }

        public event EventHandler<CartElement> RemoveItemRequested;
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            // Raise the event, passing the relevant CartElement back to the parent
            RemoveItemRequested?.Invoke(this, DataContext as CartElement);
        }

        public event EventHandler<CartElement> DecreaseAmountRequested;
        private void btnDecreaseAmount_Click(object sender, RoutedEventArgs e)
        {
            DecreaseAmountRequested.Invoke(this, DataContext as CartElement);
        }
    }
}
