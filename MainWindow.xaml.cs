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
    public partial class MainWindow : Window
    {
        Menu menu;
        OnePlayerGame game1;
        TwoPlayerGame game2;

        public MainWindow()
        {
            InitializeComponent();

            game1 = new OnePlayerGame(this);
            game2 = new TwoPlayerGame(this);
            menu = new Menu(MainFrame, game1, game2);

            MainFrame.NavigationService.Navigate(menu);
        }

        public void ToMenu()
        {
            if (game1.started) { game1.gameTimer.Stop(); }
            if (game2.started) { game2.gameTimer.Stop(); }

            game1 = new OnePlayerGame(this);
            game2 = new TwoPlayerGame(this);
            menu = new Menu(MainFrame, game1, game2);

            MainFrame.NavigationService.Navigate(menu);

        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            if (game1.started) { game1.KeyIsDown(sender, e); }
            if (game2.started) { game2.KeyIsDown(sender, e); }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            if (game1.started) { game1.KeyIsUp(sender, e); }
            if (game2.started) { game2.KeyIsUp(sender, e); }
        }

        private void HomeButtonPress(object sender, RoutedEventArgs e)
        {
            ToMenu();
        }

        private void InstructionButtonPress(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(" Up Arrow: Fire\n Down Arrow: Shield (Disabled in single player mode)\n Left/Right Arrow: Movement\n (use WASD for Player 1 in multiplayer)\n\n The aim of the game is to be the last man standing. Players have limited ammo, which will regenerate at regular intervals. Hope you enjoy!\n\n\n\n\n\n\nCredits\nProgram Development: Author Brother Games\nArt: Iceator", "Instructions");
        }
    }
}
