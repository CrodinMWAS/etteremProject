using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
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
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
        }

        private void LoadLogin()
        {
            this.Close();
            Login login = new Login();
            login.Show();
        }

        private void Login(object sender, RoutedEventArgs e)
        {
            LoadLogin();
        }

        private void ResetFields()
        {
            tb_name.BorderBrush = Brushes.Black;
            err_name.Content = "";
            tb_email.BorderBrush = Brushes.Black;
            err_email.Content = "";
            pb_password.BorderBrush = Brushes.Black;
            err_password.Content = "";
            pb_confirmPassword.BorderBrush = Brushes.Black;
            err_confirmPassword.Content = "";
        }

        private async void btn_login_Click(object sender, RoutedEventArgs e)
        {
            ResetFields();
            int[] errors = { 0, 0, 0, 0 };
            if (tb_name.Text == "")
            {
                errors[0] = 1;
            }
            if (tb_email.Text == "")
            {
                errors[1] = 1;
            }
            if (!Regex.IsMatch(tb_email.Text, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
            {
                errors[1] = 2;
            }
            if (pb_password.Password == "")
            {
                errors[2] = 1;
            }
            if (pb_confirmPassword.Password == "")
            {
                errors[3] = 1;
            }
            if (pb_confirmPassword.Password != pb_password.Password)
            {
                errors[3] = 2;
            }

            int errorCount = 0;
            for (int i = 0; i < errors.Length; i++)
            {
                if (errors[i] > 0)
                {
                    errorCount++;
                    switch (i)
                    {
                        case 0:
                            tb_name.BorderBrush = Brushes.Red;
                            err_name.Content = "Empty field";
                            break;
                        case 1:
                            tb_email.BorderBrush = Brushes.Red;
                            err_email.Content = errors[i] == 1 ? "Empty field" : "Not valid format";
                            break;
                        case 2:
                            pb_password.BorderBrush = Brushes.Red;
                            err_password.Content = "Empty field";
                            break;
                        case 3:
                            pb_confirmPassword.BorderBrush = Brushes.Red;
                            err_confirmPassword.Content = errors[i] == 1 ? "Empty field" : "Passwords don't match";
                            break;
                        default:
                            break;
                    }
                }
            }

            if (errorCount == 0)
            {
                HttpClient httpClient = new HttpClient();
                try
                {
                    httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseAddress"]);
                }
                
                catch {
                    MessageBox.Show("Server connection failure");
                }
                if (tb_name.Text.Length > 0 || pb_password.Password.Length > 0)
                {

                    var data = new
                    {
                        Username = tb_name.Text,
                        Email = tb_email.Text,
                        Password = pb_password.Password
                    };
                    var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

                    try
                    {
                        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("register", content);
                        HttpResponseMessage resp = httpResponseMessage;
                        var jsonResp = await resp.Content.ReadAsStringAsync();  
                        LoadLogin();
                    }

                    catch
                    {
                        MessageBox.Show("Registration failure");
                    }
                }
                else
                {
                    MessageBox.Show("Error");
                }
            }
        }
    }
}
