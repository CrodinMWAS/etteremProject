using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace Frontend
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Register(object sender, RoutedEventArgs e)
        {
            this.Close();
            Registration registration = new Registration();
            registration.Show(); 
        }

        private async void BtnLoginClick(object sender, RoutedEventArgs e)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseAddress"]);
            if (tb_name.Text.Length > 0 || pb_password.Password.Length > 0)
            {

                var data = new 
                {
                    Username = tb_name.Text,
                    Password = pb_password.Password
                };
                var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("login", content);
                    HttpResponseMessage resp = httpResponseMessage;
                    var jsonResp = await resp.Content.ReadAsStringAsync();
                    MessageBox.Show(jsonResp.ToString());
                }
                
                catch
                {
                    MessageBox.Show("Login failure");
                }
                
                
            } 
        }
    }
}
