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
using System.Windows.Shapes;

namespace Frontend
{
    /// <summary>
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
        }

        private void Login(object sender, RoutedEventArgs e)
        {
            this.Close();
            Login login = new Login();
            login.Show(); 
        }

        private void btn_login_Click(object sender, RoutedEventArgs e)
        {
            if (tb_name.Text == "" || tb_email.Text == "" || pb_password.Password == "" || pb_confirmPassword.Password == "")
            {
                MessageBox.Show("Ki kell tölteni az összes mezőt");
                return;
            }else if (pb_confirmPassword != pb_password)
            {
                MessageBox.Show("A jelszó és annak megerősítése nem egyeznek");
                return;
            }
        }
    }
}
