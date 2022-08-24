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
using System.Windows.Threading;

namespace Space_Game_1
{
    public partial class TwoPlayerGame : Page
    {
        //player 1 fields
        ImageBrush p1Image = new ImageBrush();
        int p1LivesCounter = 20;
        int p1AmmoCounter = 15;
        bool p1Shield;

        //player fields
        ImageBrush p2Image = new ImageBrush();
        int p2LivesCounter = 20;
        int p2AmmoCounter = 15;
        bool p2Shield;

        double bulletSpeed = 20;
        double playerSpeed = 5;


        public DispatcherTimer gameTimer = new DispatcherTimer();
        Random rand = new Random();
        Rect enemyHitBox;
        List<Rectangle> itemRemover = new List<Rectangle>();
        ulong gameTick;
        MainWindow win;

        public bool started;

        public TwoPlayerGame(MainWindow home)
        {
            win = home;
        }

        public void Start()
        {
            started = true;
            InitializeComponent();

            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            MyCanvas.Focus();

            //Background
            ImageBrush bg = new ImageBrush();

            bg.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/space.png"));
            MyCanvas.Background = bg;

            //Player1
            ImageBrush player1Image = new ImageBrush();
            player1Image.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/enemy/angrySpaceship.png"));
            Player1.Fill = player1Image;

            //Player2
            ImageBrush player2Image = new ImageBrush();
            player2Image.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/player/playerSpaceship.png"));
            Player2.Fill = player2Image;
        }

        private void GameLoop(object sender, EventArgs e)
        {
            gameTick++;

            //update HUD
            p1LivesText.Content = "Player 1 Lives: " + p1LivesCounter;
            p2LivesText.Content = "Player 2 Lives: " + p2LivesCounter;
            p1AmmoText.Content = "Player 1 Ammo: " + p1AmmoCounter;
            p2AmmoText.Content = "Player 2 Ammo: " + p2AmmoCounter;

            //shield deploying
            if (p1Shield)
            {
                p1Image.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/enemy/angrySpaceshipShield.png"));
                Player1.Fill = p1Image;
            }

            else
            {
                p1Image.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/enemy/angrySpaceship.png"));
            }

            if (p2Shield)
            {
                p2Image.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/player/playerSpaceshipShield.png"));
                Player2.Fill = p2Image;
            }

            else
            {
                p2Image.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/player/playerSpaceship.png"));
                Player2.Fill = p2Image;
            }

            //sprite movement
            MovePlayer1();
            MovePlayer2();

            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                if (x is Rectangle)
                {
                    MoveP1Bullet(x);
                    MoveP2Bullet(x);
                }
            }

            //delete items in item remover
            foreach (Rectangle i in itemRemover)
            {
                MyCanvas.Children.Remove(i);
            }

            //check lives
            if (p1LivesCounter < 1)
            {
                p1LivesText.Content = "Player 1 Lives: 0";
                EndGame("Player 2");
            }

            else if (p2LivesCounter < 1)
            {
                p2LivesText.Content = "PLayer 2 Lives: 0";
                EndGame("Player 1");
            }

            //Grant player ammo
            if (gameTick % 250 == 0)
            {
                p1AmmoCounter += 10;
                p2AmmoCounter += 10;
            }

        }

        private void MovePlayer2()
        {
            if (p2moveLeft == true && Canvas.GetLeft(Player2) > 0)
            {
                Canvas.SetLeft(Player2, Canvas.GetLeft(Player2) - playerSpeed);
            }
            if (p2moveRight == true && Canvas.GetLeft(Player2) + 90 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(Player2, Canvas.GetLeft(Player2) + playerSpeed);
            }
        }

        private void MovePlayer1()
        {
            if (p1moveLeft == true && Canvas.GetLeft(Player1) > 0)
            {
                Canvas.SetLeft(Player1, Canvas.GetLeft(Player1) - playerSpeed);
            }
            if (p1moveRight == true && Canvas.GetLeft(Player1) + 90 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(Player1, Canvas.GetLeft(Player1) + playerSpeed);
            }

        }

        private void MoveP1Bullet(Rectangle x)
        {
            if (x is Rectangle && (string)x.Tag == "p1Bullet")
            {
                Canvas.SetTop(x, Canvas.GetTop(x) + bulletSpeed);

                Rect p1BulletHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                if (Canvas.GetTop(x) > Canvas.GetTop(Player2))
                {
                    itemRemover.Add(x);
                }

                //collision detection
                foreach (var y in MyCanvas.Children.OfType<Rectangle>())
                {
                    if (y is Rectangle && (string)y.Tag == "player2")
                    {
                        Rect p2HitBox = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                        if (p1BulletHitBox.IntersectsWith(p2HitBox))
                        {
                            itemRemover.Add(x);

                            if (!p2Shield)
                            {
                                p2LivesCounter--;
                            }
                        }
                    }
                }
            }
        }

        private void MoveP2Bullet(Rectangle x)
        {
            if (x is Rectangle && (string)x.Tag == "p2Bullet")
            {
                Canvas.SetTop(x, Canvas.GetTop(x) - bulletSpeed);

                Rect p2BulletHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                if (Canvas.GetTop(x) < Canvas.GetTop(Player1))
                {
                    itemRemover.Add(x);
                }

                foreach (var y in MyCanvas.Children.OfType<Rectangle>())
                {
                    //for enemy ship
                    if (y is Rectangle && (string)y.Tag == "player1")
                    {
                        Rect p1HitBox = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                        if (p2BulletHitBox.IntersectsWith(p1HitBox))
                        {
                            itemRemover.Add(x);

                            if (!p1Shield)
                            {
                                p1LivesCounter--;
                            }
                        }
                    }
                }
            }
        }

        private void P1Fire()
        {
            Rectangle newBullet = new Rectangle
            {
                Tag = "p1Bullet",
                Height = 20,
                Width = 5,
                Fill = Brushes.White,
                Stroke = Brushes.Red
            };

            Canvas.SetLeft(newBullet, Canvas.GetLeft(Player1) + Player1.Width / 2);
            Canvas.SetTop(newBullet, Canvas.GetTop(Player1) + newBullet.Height / 2);

            MyCanvas.Children.Add(newBullet);

            p1AmmoCounter--;
        }

        private void P2Fire()
        {
            Rectangle newBullet = new Rectangle
            {
                Tag = "p2Bullet",
                Height = 20,
                Width = 5,
                Fill = Brushes.White,
                Stroke = Brushes.Red
            };

            Canvas.SetLeft(newBullet, Canvas.GetLeft(Player2) + Player2.Width / 2);
            Canvas.SetTop(newBullet, Canvas.GetTop(Player2) - newBullet.Height / 2);

            MyCanvas.Children.Add(newBullet);

            p2AmmoCounter--;
        }

        private void EndGame(string winner)
        {
            MessageBox.Show(winner + " wins!", "winner");
            gameTimer.Stop();
            win.ToMenu();
        }

        //______________________________________CONTROLS_______________________________________________
        public bool p1moveLeft, p2moveLeft;
        public bool p1moveRight, p2moveRight;

        public void KeyIsDown(object sender, KeyEventArgs e)
        {
            //player 1 controls
            if (e.Key == Key.A)
            {
                p1moveLeft = true;
            }

            if (e.Key == Key.D)
            {
                p1moveRight = true;
            }

            if (e.Key == Key.S)
            {
                p1Shield = true;
            }

            //player 2 controls
            if (e.Key == Key.Left)
            {
                p2moveLeft = true;
            }

            if (e.Key == Key.Right)
            {
                p2moveRight = true;
            }

            if (e.Key == Key.Down)
            {
                p2Shield = true;
            }
        }

        public void KeyIsUp(object sender, KeyEventArgs e)
        {
            //player 1 controls
            if (e.Key == Key.A)
            {
                p1moveLeft = false;
            }

            if (e.Key == Key.D)
            {
                p1moveRight = false;
            }

            if (e.Key == Key.W)
            {
                if (!p1Shield && p1AmmoCounter > 0)
                {
                    P1Fire();
                }
            }

            if (e.Key == Key.S)
            {
                p1Shield = false;
            }

            //player 2 controls
            if (e.Key == Key.Left)
            {
                p2moveLeft = false;
            }

            if (e.Key == Key.Right)
            {
                p2moveRight = false;
            }

            if (e.Key == Key.Up)
            {
                if (!p2Shield && p2AmmoCounter > 0)
                {
                    P2Fire();
                }
            }

            if (e.Key == Key.Down)
            {
                p2Shield = false;
            }
        }

    }
}
