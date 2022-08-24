using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
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
    public partial class OnePlayerGame : Page
    {
        //enemy fields
        ImageBrush enemyImage = new ImageBrush();
        double enemySpeed = 5;
        int enemyLivesCounter = 20;
        int enemyMove;
        bool enemyHasMoved;
        string lastMove = "nothing";

        int initialPlayerMoveChance;
        int initialRandomMoveChance;
        int initialShootChance;
        int maxChance = 10000;

        int playerMoveChance;
        int randomMoveChance;
        int shootChance;

        //player fields
        ImageBrush playerImage = new ImageBrush();
        double playerSpeed = 5;
        int playerLivesCounter = 20;
        int ammoCounter = 15;

        double bulletSpeed = 20;


        public DispatcherTimer gameTimer = new DispatcherTimer();
        Random rand = new Random();
        Rect enemyHitBox;
        List<Rectangle> itemRemover = new List<Rectangle>();
        ulong gameTick;
        MainWindow win;

        public bool started;

        public OnePlayerGame(MainWindow home)
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

            //Enemy
            ImageBrush enemyImage = new ImageBrush();
            enemyImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/enemy/angrySpaceship.png"));
            Enemy.Fill = enemyImage;

            initialShootChance = maxChance * 10 / 100;
            initialPlayerMoveChance = initialShootChance + maxChance * 75 / 100;
            initialRandomMoveChance = initialPlayerMoveChance + maxChance * 15 / 100;

            playerMoveChance = initialPlayerMoveChance;
            randomMoveChance = initialRandomMoveChance;
            shootChance = initialShootChance;

            //Player
            ImageBrush playerImage = new ImageBrush();
            playerImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/player/playerSpaceship.png"));
            Player.Fill = playerImage;
        }

        private void GameLoop(object sender, EventArgs e)
        {
            gameTick++;

            if (gameTick !> 10)
            {

                enemyHitBox = new Rect(Canvas.GetLeft(Enemy), Canvas.GetTop(Enemy), Enemy.Width, Enemy.Height);

                //update HUD
                playerLivesText.Content = "PLayer Lives: " + playerLivesCounter;
                enemyLivesText.Content = "Enemy Lives: " + enemyLivesCounter;
                ammoText.Content = "Ammo: " + ammoCounter;

                //sprite movement
                MoveEnemy();
                MovePlayer();

                foreach (var x in MyCanvas.Children.OfType<Rectangle>())
                {
                    if (x is Rectangle)
                    {
                        MoveEnemyBullet(x);
                        MovePlayerBullet(x);
                    }
                }

                //delete items in item remover
                foreach (Rectangle i in itemRemover)
                {
                    MyCanvas.Children.Remove(i);
                }

                //check lives
                if (enemyLivesCounter < 1)
                {
                    enemyLivesText.Content = "Enemy Lives: 0";

                    EndGame("You Win!");
                }

                else if (playerLivesCounter < 1)
                {
                    playerLivesText.Content = "PLayer Lives: 0";

                    EndGame("The AI wins");

                }

                //Grant player ammo
                if (gameTick % 250 == 0)
                {
                    ammoCounter += 5;
                }
            }
        }

        private void MovePlayer()
        {
            if (moveLeft == true && Canvas.GetLeft(Player) > 0)
            {
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) - playerSpeed);
            }
            if (moveRight == true && Canvas.GetLeft(Player) + 90 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) + playerSpeed);
            }
        }

        private void MoveEnemy()
        {
            //enemy movement
            enemyHasMoved = false;

            do
            {
                enemyMove = rand.Next(0, maxChance);

                //Shoot
                if (enemyMove < shootChance)
                {
                    EnemyFire();
                    enemyHasMoved = true;
                }

                //move towards player
                else if (enemyMove < playerMoveChance)
                {
                    double distance = (Canvas.GetLeft(Player) - Canvas.GetLeft(Enemy)) / 10;
                    Canvas.SetLeft(Enemy, Canvas.GetLeft(Enemy) + distance);
                    enemyHasMoved = true;
                    lastMove = "onPlayer";
                }

                //RandomMove
                else if (enemyMove < randomMoveChance)
                {
                    //Move right if ship was already moving right
                    if (lastMove == "right")
                    {
                        //move right
                        if (Canvas.GetLeft(Enemy) + 90 < Application.Current.MainWindow.Width)
                        {
                            Canvas.SetLeft(Enemy, Canvas.GetLeft(Enemy) + enemySpeed);
                            enemyHasMoved = true;
                            lastMove = "right";
                        }

                        else
                        {
                            Canvas.SetLeft(Enemy, Canvas.GetLeft(Enemy) - enemySpeed);
                            enemyHasMoved = true;
                            lastMove = "left";
                        }
                    }

                    //Move left if ship was already moving left
                    else if (lastMove == "left")
                    {
                        //move left
                        if (Canvas.GetLeft(Enemy) > 10)
                        {
                            Canvas.SetLeft(Enemy, Canvas.GetLeft(Enemy) - enemySpeed);
                            enemyHasMoved = true;
                            lastMove = "left";
                        }

                        else
                        {
                            Canvas.SetLeft(Enemy, Canvas.GetLeft(Enemy) + enemySpeed);
                            enemyHasMoved = true;
                            lastMove = "right";
                        }
                    }

                    //Pick a direction to move if the ship wasn't already moving

                    //right
                    if (enemyMove < 5000)
                    {
                        if (Canvas.GetLeft(Enemy) + 90 < Application.Current.MainWindow.Width)
                        {
                            Canvas.SetLeft(Enemy, Canvas.GetLeft(Enemy) + enemySpeed);
                            enemyHasMoved = true;
                            lastMove = "right";
                        }
                    }

                    //left
                    else if (enemyMove < 10000)
                    {

                        if (Canvas.GetLeft(Enemy) > 10)
                        {
                            Canvas.SetLeft(Enemy, Canvas.GetLeft(Enemy) - enemySpeed);
                            enemyHasMoved = true;
                            lastMove = "left";
                        }
                    }
                }
                //else { throw new IndexOutOfRangeException("enemy move does not exist"); }
            }
            while (!enemyHasMoved);

        }

        private void MoveEnemyBullet(Rectangle x)
        {
            if (x is Rectangle && (string)x.Tag == "enemyBullet")
            {
                Canvas.SetTop(x, Canvas.GetTop(x) + bulletSpeed);

                Rect enemyBulletHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                if (Canvas.GetTop(x) > Canvas.GetTop(Player))
                {
                    itemRemover.Add(x);
                }

                //collision detection
                foreach (var y in MyCanvas.Children.OfType<Rectangle>())
                {
                    //for player ship
                    if (y is Rectangle && (string)y.Tag == "player")
                    {
                        Rect playerHitBox = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                        if (enemyBulletHitBox.IntersectsWith(playerHitBox))
                        {
                            itemRemover.Add(x);
                            playerLivesCounter--;
                        }
                    }
                }
            }
        }

        private void MovePlayerBullet(Rectangle x)
        {
            if (x is Rectangle && (string)x.Tag == "playerBullet")
            {
                Canvas.SetTop(x, Canvas.GetTop(x) - bulletSpeed);

                Rect playerBulletHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                if (Canvas.GetTop(x) < Canvas.GetTop(Enemy))
                {
                    itemRemover.Add(x);
                }

                foreach (var y in MyCanvas.Children.OfType<Rectangle>())
                {
                    //for enemy ship
                    if (y is Rectangle && (string)y.Tag == "enemy")
                    {
                        Rect enemyHitBox = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                        if (playerBulletHitBox.IntersectsWith(enemyHitBox))
                        {
                            itemRemover.Add(x);
                            enemyLivesCounter--;
                        }
                    }
                }
            }
        }

        private void EnemyFire()
        {
            Rectangle newBullet = new Rectangle
            {
                Tag = "enemyBullet",
                Height = 20,
                Width = 5,
                Fill = Brushes.White,
                Stroke = Brushes.Red
            };

            Canvas.SetLeft(newBullet, Canvas.GetLeft(Enemy) + Enemy.Width / 2);
            Canvas.SetTop(newBullet, Canvas.GetTop(Enemy) + newBullet.Height / 2);

            MyCanvas.Children.Add(newBullet);
        }

        private void PlayerFire()
        {
            Rectangle newBullet = new Rectangle
            {
                Tag = "playerBullet",
                Height = 20,
                Width = 5,
                Fill = Brushes.White,
                Stroke = Brushes.Red
            };

            Canvas.SetLeft(newBullet, Canvas.GetLeft(Player) + Player.Width / 2);
            Canvas.SetTop(newBullet, Canvas.GetTop(Player) - newBullet.Height / 2);

            MyCanvas.Children.Add(newBullet);

            ammoCounter--;
        }

        private void EndGame(string text) 
        {
            MessageBox.Show(text, "Winner");
            gameTimer.Stop();
            win.ToMenu();
        }

        //______________________________________CONTROLS_______________________________________________
        bool moveLeft;
        bool moveRight;

        public void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                moveLeft = true;
            }

            if (e.Key == Key.Right)
            {
                moveRight = true;
            }
        }

        public void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                moveLeft = false;
            }

            if (e.Key == Key.Right)
            {
                moveRight = false;
            }

            if (e.Key == Key.Up)
            {
                if (ammoCounter > 0)
                {
                    PlayerFire();
                }
            }

        }
    }
}
