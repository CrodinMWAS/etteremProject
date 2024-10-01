using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Frontend
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Index : Window
    {
        public Index()
        { 
            InitializeComponent(); 
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard fadeInStoryboard = (Storyboard)this.FindResource("FadeInAnimation");
            fadeInStoryboard.Begin(this);
        }

        private void btn_login_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.ShowDialog();
        }

        private void ResponsiveText(object sender, SizeChangedEventArgs e)
        {
            double newFontSize = Math.Min(e.NewSize.Width / 20, e.NewSize.Height / 10)/40; 
            txt1.FontSize = 40 * newFontSize;
            txt2.FontSize = 20 * newFontSize;
            txt3.FontSize = 14 * newFontSize;
            txt4.FontSize = 15 * newFontSize;
            btnLogin.FontSize = 15 * newFontSize;
            lblNavbar.FontSize = 20 * newFontSize;
            rotateTransform.CenterX = 125 * newFontSize;
            rotateTransform.CenterY = 125 * newFontSize;
        }

        private void btn_exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void lblNavbar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            fadeOutAnimation.Completed += (s, _) =>
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                Thread closeThread = new Thread(() =>
                {
                    Thread.Sleep(1000); // Delay for 1 second
                                        // Use Dispatcher to close the window on the UI thread
                    Dispatcher.Invoke(() =>
                    {
                        this.Close();
                    });
                });
                closeThread.Start();
            };
            this.BeginAnimation(Window.OpacityProperty, fadeOutAnimation);
        }
    }
}
    