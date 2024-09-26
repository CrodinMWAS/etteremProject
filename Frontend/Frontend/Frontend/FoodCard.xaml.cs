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
    /// Interaction logic for FoodCard.xaml
    /// </summary>
    public partial class FoodCard : UserControl
    {
        public FoodCard()
        {
            InitializeComponent();
        }

        //Foodname Property
        public static readonly DependencyProperty FoodNameTextProperty =
        DependencyProperty.Register("FoodNameText", typeof(string), typeof(FoodCard), new PropertyMetadata("Food Name"));

        public string FoodNameText
        {
            get { return (string)GetValue(FoodNameTextProperty); }
            set { SetValue(FoodNameTextProperty, value); }
        }

        // FoodPrice Property
        public static readonly DependencyProperty FoodPriceTextProperty =
            DependencyProperty.Register("FoodPriceText", typeof(string), typeof(FoodCard), new PropertyMetadata("$0.00"));

        public string FoodPriceText
        {
            get { return (string)GetValue(FoodPriceTextProperty); }
            set { SetValue(FoodPriceTextProperty, value); }
        }

        // Allergens Property
        public static readonly DependencyProperty Allergen1Property =
        DependencyProperty.Register("Allergen1", typeof(string), typeof(FoodCard), new PropertyMetadata(null));

        public string Allergen1
        {
            get { return (string)GetValue(Allergen1Property); }
            set { SetValue(Allergen1Property, value); }
        }

        public static readonly DependencyProperty Allergen2Property =
            DependencyProperty.Register("Allergen2", typeof(string), typeof(FoodCard), new PropertyMetadata(null));

        public string Allergen2
        {
            get { return (string)GetValue(Allergen2Property); }
            set { SetValue(Allergen2Property, value); }
        }

        public static readonly DependencyProperty Allergen3Property =
            DependencyProperty.Register("Allergen3", typeof(string), typeof(FoodCard), new PropertyMetadata(null));

        public string Allergen3
        {
            get { return (string)GetValue(Allergen3Property); }
            set { SetValue(Allergen3Property, value); }
        }

        // FoodImage Property
        public static readonly DependencyProperty FoodImageSourceProperty =
            DependencyProperty.Register("FoodImageSource", typeof(ImageSource), typeof(FoodCard), new PropertyMetadata(null));

        public ImageSource FoodImageSource
        {
            get { return (ImageSource)GetValue(FoodImageSourceProperty); }
            set { SetValue(FoodImageSourceProperty, value); }
        }

        // FoodDescription Property
        public static readonly DependencyProperty FoodDescriptionProperty =
            DependencyProperty.Register("FoodDescription", typeof(string), typeof(FoodCard), new PropertyMetadata(null));

        public string FoodDescription
        {
            get { return (string)GetValue(FoodDescriptionProperty); }
            set { SetValue(FoodDescriptionProperty, value); }
        }

        private void ResponsiveText(object sender, SizeChangedEventArgs e)
        {
            double newFontSize = Math.Min(e.NewSize.Width / 20, e.NewSize.Height / 10) / 40;
            tbFoodName.FontSize = 60 * newFontSize;
            tbFoodDescription.FontSize = 32 * newFontSize;
            lblAllergen1.FontSize = 40 * newFontSize;
            lblAllergen2.FontSize = 40 * newFontSize;
            lblAllergen3.FontSize = 40 * newFontSize;
            lblPrice.FontSize = 60 * newFontSize;
            btnOrder.FontSize = 45 * newFontSize;
        }

        private void btnOrder_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(FoodDescription);
        }
    }
}
