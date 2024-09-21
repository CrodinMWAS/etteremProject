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

        // FoodImage Property
        public static readonly DependencyProperty FoodImageSourceProperty =
            DependencyProperty.Register("FoodImageSource", typeof(ImageSource), typeof(FoodCard), new PropertyMetadata(null));

        public ImageSource FoodImageSource
        {
            get { return (ImageSource)GetValue(FoodImageSourceProperty); }
            set { SetValue(FoodImageSourceProperty, value); }
        }

        public string asd
        {
            get; set;
        }
    }
}
