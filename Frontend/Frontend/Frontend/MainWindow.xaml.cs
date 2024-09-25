using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<MenuItem> MenuItems { get; set; } = new ObservableCollection<MenuItem>();
        public ObservableCollection<CartElement> CartItems { get; set; } = new ObservableCollection<CartElement>();
        public int cartElementCounter = 0;
        public event PropertyChangedEventHandler PropertyChanged;
        double total = 0;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _cartTotal = "Total : ";
        public string CartTotal
        {
            get { return _cartTotal; }
            set
            {
                if (_cartTotal != value)
                {
                    _cartTotal = value;
                    OnPropertyChanged(nameof(CartTotal)); // Notify the UI of the change
                }
            }
        }
        public MainWindow()
        {
            getMenu();
            InitializeComponent();
        }

        public void getMenu()
        {
            //Getting the base paths for the pics, reading through file
            string basePath = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory)).FullName).FullName).FullName).FullName;
            string filePath = basePath + "\\Restaurant\\SnackDashCompressed.txt";
            StreamReader reader = new StreamReader(filePath);
            while (!reader.EndOfStream)
            {
                //Getting the allergens into an array, making the path for the pics.
                string[] line = reader.ReadLine().Split(';');
                string[] allergens = line[3].Split(',');
                string pic = $"{basePath}\\{line[1]}";
                MenuItem newItem;

                //Determining how many allergens are there. If null, do not display allergen.
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
                MenuItems.Add(newItem);
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

            tbTotalInCart.FontSize = 15 * newFontSize;
        }

        private void btn_login_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FilterCategory(object sender, RoutedEventArgs e)
        {
            //Filtering the category on user's click. Getting all the textblocks and manipulating them.
            //If the filter has been clicked, Reset the filters / styles.
            //Category and textblock's text has to allign. !!!
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

                for (int i = 0; i != MenuItems.Count; i++)
                {
                    if (textBlockText == MenuItems[i].Category)
                    {
                        filteredItems.Add(MenuItems[i]);
                    }
                }

                itemsControl.ItemsSource = filteredItems;
            }
            else
            {
                textBlock.Foreground = new SolidColorBrush(Colors.Black);
                textBlock.TextDecorations = null;
                lblSelectedCategory.Text = "All Categories";
                itemsControl.ItemsSource = MenuItems;
            }

            //This updates the available fooditems, so their buttons can be watched
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ItemsControl_Loaded(itemsControl, new RoutedEventArgs());
            }), System.Windows.Threading.DispatcherPriority.Render);
        }

        private void ItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < itemsControl.Items.Count; i++)
            {
                // Get the ContentPresenter for the item
                ContentPresenter container = itemsControl.ItemContainerGenerator.ContainerFromIndex(i) as ContentPresenter;

                if (container != null)
                {
                    // If you need to access the FoodCard, you might need to traverse the visual tree
                    FoodCard foodCardVisual = FindVisualChild<FoodCard>(container);

                    if (foodCardVisual != null)
                    {
                        var item = itemsControl.Items[i] as MenuItem;
                        foodCardVisual.btnOrder.Click += (s, g) => AddToCart(item);
                    }
                }
            }
        }


        //Needs to differenciate between different carItems using some sort of ID
        private void CartItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < cartItemControl.Items.Count; i++)
            {
                // Get the ContentPresenter for the item
                ContentPresenter container = cartItemControl.ItemContainerGenerator.ContainerFromIndex(i) as ContentPresenter;

                if (container != null)
                {
                    // If you need to access the FoodCard, you might need to traverse the visual tree
                    CartContent cartCardVisual = FindVisualChild<CartContent>(container);

                    if (cartCardVisual != null)
                    {
                        // Create a local copy of the cartElement to avoid closure issue
                        CartElement item = cartItemControl.Items[i] as CartElement;
                        cartCardVisual.DecreaseAmountRequested += CartCardVisual_DecreaseAmountRequested;
                        cartCardVisual.RemoveItemRequested += CartCardVisual_RemoveItemRequested;
                    }
                }
            }
        }

        // Helper method to find the visual child of a specified type
        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                {
                    return (T)child;
                }
                else
                {
                    var childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }

        private void AddToCart(MenuItem item)
        {
            //This updates the available fooditems, so their buttons can be watched
            cartElementCounter++;
            UpdateCartTotal(item.Price, true);
            for (int i = 0; i != CartItems.Count; i++)
            {
                if (CartItems[i].Item.Name == item.Name)
                {
                    CartItems[i].Amount++;
                    return;
                }
            }
            CartItems.Add(new CartElement(cartElementCounter, item, 1));
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CartItemsControl_Loaded(cartItemControl, new RoutedEventArgs());
            }), System.Windows.Threading.DispatcherPriority.Render);
        }

        private void UpdateCartTotal(string price, bool add)
        {
            double.TryParse(price.Substring(1), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double realPrice);
            if (add)
            {
                total += realPrice;
            }
            else
            {
                MessageBox.Show($"{total} - {realPrice} = {total - realPrice}");
                total -= realPrice;
            }
            CartTotal = $"Total : {total}";
        }

        // Event handler in MainWindow to handle the item removal
        private void CartCardVisual_RemoveItemRequested(object sender, CartElement e)
        {
            UpdateCartTotal(e.Item.Price, false);
            CartItems.Remove(e);
        }

        private void CartCardVisual_DecreaseAmountRequested(object sender, CartElement e)
        {
            if (e.Amount > 1)
            {
                e.Amount--;
            } else if (e.Amount == 1)
            {
                //for (int i = 0; i != e.Amount; i++)
                //{
                //    UpdateCartTotal(e.Item.Price, false);
                //}
                CartItems.Remove(e);
            }
            UpdateCartTotal(e.Item.Price, false);
        }
    }
}
