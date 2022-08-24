using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Space_Game_1
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Page
    {
        Frame Main;
        OnePlayerGame game1;
        TwoPlayerGame game2;

        public Menu(Frame frame, OnePlayerGame game1in, TwoPlayerGame game2in)
        {
            Main = frame;
            game1 = game1in;
            game2 = game2in;

            InitializeComponent();

            //Background
            ImageBrush bg = new ImageBrush();

            bg.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/space.png"));
            MyGrid.Background = bg;
        }

        private void StartOnePlayerGame(object sender, RoutedEventArgs e)
        {
            Main.NavigationService.Navigate(game1);
            game1.Start();
        }

        private void StartTwoPlayerGame(object sender, RoutedEventArgs e)
        {
            Main.NavigationService.Navigate(game2);
            game2.Start();
        }
    }
}
