using Frontend.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// AdminModify.xaml の相互作用ロジック
    /// </summary>
    public partial class AdminModify : Window
    {
        public AdminModify(Stock st)
        {
            InitializeComponent();
            tbAmount.Text = st.Amount.ToString();
            tbStockName.Text = st.StockName.ToString();
        }

        private void tbAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text); // Block input if it's not numeric
        }
        private void tbAmount_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Tab ||
                e.Key == Key.Enter || e.Key == Key.Left || e.Key == Key.Right)
            {
                e.Handled = false; 
            }
            else
            {
                e.Handled = !IsTextNumeric(KeyToChar(e.Key));
            }
        }
        private void tbAmount_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextNumeric(text))
                {
                    e.CancelCommand(); 
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private bool IsTextNumeric(string text)
        {
            Regex regex = new Regex("[^0-9]+"); 
            return !regex.IsMatch(text);
        }

        private string KeyToChar(Key key)
        {
            if (key >= Key.D0 && key <= Key.D9)
            {
                return (key - Key.D0).ToString();
            }
            if (key >= Key.NumPad0 && key <= Key.NumPad9)
            {
                return (key - Key.NumPad0).ToString();
            }
            return string.Empty;
        }
    }
}
