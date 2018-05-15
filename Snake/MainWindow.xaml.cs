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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using System.Media;
using System.Threading;

namespace Snake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SnakeBody snake = new SnakeBody("images/snake_head.png");
        
        List<SnakeBody> body = new List<SnakeBody>();
        List<TipForTurns> listOfTurns = new List<TipForTurns>();
        int lenghthOfSnake = 0;


        SnakeBody snake2 = new SnakeBody("images/snake_head2.png");
        List<SnakeBody> body2 = new List<SnakeBody>();
        List<TipForTurns> listOfTurns2 = new List<TipForTurns>();
        int lenghthOfSnake2 = 0;


        List<Fruit> fruits = new List<Fruit>();
        List<Poison> poisons = new List<Poison>();

        DispatcherTimer timer = new DispatcherTimer();
        DispatcherTimer timer2 = new DispatcherTimer();

        int thatTips; // to Search Next Turns
        int increament = 0;

        // Game's variables
        double timeInterval = 15; 
        int snakeSpeed = 5;
        bool twoPlayer;
        
        
        bool GameEffectSound= true;

        
        SoundPlayer eatSound = new SoundPlayer(@"./sounds/eat.wav");
        SoundPlayer clickSound = new SoundPlayer(@"./sounds/click.wav");
        SoundPlayer failkSound = new SoundPlayer(@"./sounds/fail.wav");

        string NewBestScore;
        string Best_name, Best_score;

      

        public MainWindow()
        {
            InitializeComponent();
            timer.Interval = TimeSpan.FromMilliseconds(timeInterval);
            timer2.Interval = TimeSpan.FromMilliseconds(timeInterval);

            snake.Name = "snake";
            snake2.Name = "snake2";
            
            ReadBestScore();
            nameLabel.Content = Best_name;
            scoreLabel.Content = Best_score;
        }



        public void AddToSnakeBody(List<SnakeBody> _body, SnakeBody _snake, int _snake_length)
        {
            string _url= "images/snake_body.png";
            SnakeBody lastElement = new SnakeBody("images/snake_head.png");

            if (_snake.Name == "snake")
                _url = "images/snake_body.png";
            if (_snake.Name == "snake2")
                _url = "images/snake_body2.png";

            if (_body.Count != 0)
            {
                lastElement = _body.ElementAt<SnakeBody>(_body.Count - 1);
                if(lastElement.YMOVE >0)
                    _body.Add(new SnakeBody(lastElement.Margin.Left, lastElement.Margin.Top - (lastElement.Height), lastElement.XMOVE, lastElement.YMOVE, lastElement.DoneTips, _url));// down
                if (lastElement.YMOVE < 0)
                    _body.Add(new SnakeBody(lastElement.Margin.Left, lastElement.Margin.Top + (lastElement.Height), lastElement.XMOVE, lastElement.YMOVE, lastElement.DoneTips, _url)); // top
                if (lastElement.XMOVE < 0)
                    _body.Add(new SnakeBody(lastElement.Margin.Left + (lastElement.Width), lastElement.Margin.Top, lastElement.XMOVE, lastElement.YMOVE, lastElement.DoneTips, _url)); // left
                if (lastElement.XMOVE > 0)
                    _body.Add(new SnakeBody(lastElement.Margin.Left - (lastElement.Width), lastElement.Margin.Top, lastElement.XMOVE, lastElement.YMOVE, lastElement.DoneTips, _url)); // right
                DisplayElements();
                if(_snake.Name== "snake")
                    lenghthOfSnake++;
                
                if(_snake.Name == "snake2")
                    lenghthOfSnake2++;
            }
            else
            {
                if (_snake.YMOVE > 0)
                    _body.Add(new SnakeBody(_snake.Margin.Left, _snake.Margin.Top - (_snake.Height), _snake.XMOVE, _snake.YMOVE, _snake.DoneTips, _url)); // down
                if (_snake.YMOVE < 0)
                    _body.Add(new SnakeBody(_snake.Margin.Left, _snake.Margin.Top + (_snake.Height), _snake.XMOVE, _snake.YMOVE, _snake.DoneTips, _url)); // top
                if (_snake.XMOVE < 0)
                    _body.Add(new SnakeBody(_snake.Margin.Left + (_snake.Width), _snake.Margin.Top, _snake.XMOVE, _snake.YMOVE, _snake.DoneTips, _url)); //left
                if (_snake.XMOVE > 0)
                    _body.Add(new SnakeBody(_snake.Margin.Left - (_snake.Width), _snake.Margin.Top, _snake.XMOVE, _snake.YMOVE, _snake.DoneTips, _url));// right


                DisplayElements();
                if (_snake.Name == "snake")
                    lenghthOfSnake++;

                if (_snake.Name == "snake2")
                    lenghthOfSnake2++;
            }

            
            



        }

        public void DisplayElements()
        {
            gameArea.Children.Clear();
            

            foreach (var item in body)
            {
                gameArea.Children.Add(item);
            }

            if (twoPlayer == true)
            {
                foreach (var item in body2)
                {
                    gameArea.Children.Add(item);
                }

            }
            
            foreach (var item in fruits)
            {
                gameArea.Children.Add(item);
            }
            foreach (var item in poisons)
            {
                gameArea.Children.Add(item);
            }
            gameArea.Children.Add(snake);
            if (twoPlayer == true)
                gameArea.Children.Add(snake2);


        }

        public void GameOver(string _loserSnakeName)
        {
            timer.Stop();
            FailSoundEffect();
            if (CmpScores(_loserSnakeName) == true)
            {
                //ReadBestScore();

                againLabel.IsEnabled = true;
                TwoPlayer.IsEnabled = true;

                if(twoPlayer==true)
                {
                    if(_loserSnakeName== "snake2")
                    {
                        finalLength.Foreground = Brushes.Green;
                        finalLength.Content = "Green snake win! Final lenght:" + lenghthOfSnake.ToString();
                        winnerLabelBest.Content = "Green snake win and have the best score!";
                        NewBestScore = lenghthOfSnake.ToString();
                    }
                    else
                    {
                        finalLength.Foreground = Brushes.DarkBlue;
                        finalLength.Content = "Blue snake win! Final length:" + lenghthOfSnake2.ToString();
                        winnerLabelBest.Content = "Blue snake win and have the best score!";
                        NewBestScore = lenghthOfSnake2.ToString();
                    }
                    
                }
                else
                {
                    finalLength.Foreground = Brushes.Red;
                    finalLength.Content = "Final length:" + lenghthOfSnake.ToString();
                    NewBestScore = lenghthOfSnake.ToString();
                }

                
                BestScore.Visibility = Visibility.Visible;

            }
            else
            {
                ReadBestScore();

                againLabel.IsEnabled = true;
                TwoPlayer.IsEnabled = true;


                if (twoPlayer == true)
                {
                    if (_loserSnakeName == "snake2")
                    {
                        finalLength.Foreground = Brushes.Green;
                        finalLength.Content = "Green snake win! Final lenght:" + lenghthOfSnake.ToString();
                    }
                    else
                    {
                        finalLength.Foreground = Brushes.DarkBlue;
                        finalLength.Content = "Blue snake win! Final length:" + lenghthOfSnake2.ToString();
                    }

                }
                else
                {
                    finalLength.Foreground = Brushes.Red;
                    finalLength.Content = "Final length:" + lenghthOfSnake.ToString();
                    NewBestScore = lenghthOfSnake.ToString();
                }


               
                
               

                finalLength.Visibility = Visibility.Visible;
                InfoTable.Visibility = Visibility.Visible;
                
                nameLabel.Content = Best_name;
                scoreLabel.Content = Best_score;


                gameArea.Children.Clear();
            }
            


        }

        public void ReadBestScore()
        {
            string line;
            int i = 0;
            try
            {
                using (StreamReader strR = new StreamReader("./gameTextFiles/theBest.txt"))
                {

                    while ((line = strR.ReadLine()) != null)
                    {
                        if (i == 0)
                            Best_name = line;
                        if (i == 1)
                            Best_score = line;
                        i++;
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        public bool CmpScores(string _loserSnakeName)
        {
            if(_loserSnakeName== "snake")
            {
                if (twoPlayer == false)
                {
                    if (lenghthOfSnake > Convert.ToInt32(Best_score))
                    {

                        return true;
                    }
                }
                    
                if (lenghthOfSnake2 > Convert.ToInt32(Best_score))
                {

                    return true;
                }
                return false;
            }
            else if (_loserSnakeName == "snake2")
            {
                if (lenghthOfSnake > Convert.ToInt32(Best_score))
                {

                    return true;
                }
                return false;
            }
            else
                return false;
        }

        public void SearchCollisionWithBody(List<SnakeBody> _body, SnakeBody _snake)
        {
            foreach (var item in _body)
            {
                if(_snake.XMOVE >0 ) // toRight
                {
                    if(_snake.Margin.Left + _snake.Width > item.Margin.Left && _snake.Margin.Left + _snake.Width < item.Margin.Left + (_snake.Width*0.75))
                    {
                        if(_snake.Margin.Top  > item.Margin.Top - _snake.Height && _snake.Margin.Top < item.Margin.Top + item.Height)
                        {
                            GameOver(_snake.Name);

                        }
                    }
                }
                if (_snake.XMOVE < 0) // toLeft
                {
                    if (_snake.Margin.Left < item.Margin.Left + item.Width && _snake.Margin.Left + (_snake.Width*0.75) > item.Margin.Left + item.Width)
                    {
                        if (_snake.Margin.Top > item.Margin.Top - _snake.Height && _snake.Margin.Top < item.Margin.Top + item.Height)
                        {
                            GameOver(_snake.Name);

                        }
                    }
                }
                if (_snake.YMOVE < 0) // toUp
                {
                    if (_snake.Margin.Left < item.Margin.Left + item.Width && _snake.Margin.Left  > item.Margin.Left - _snake.Width)
                    {
                        if (_snake.Margin.Top < item.Margin.Top + item.Height && _snake.Margin.Top > item.Margin.Top + item.Height - (_snake.Height*0.75))
                        {
                            GameOver(_snake.Name);

                        }
                    }
                }
                if (_snake.YMOVE > 0) // toDown
                {
                    if (_snake.Margin.Left < item.Margin.Left + item.Width && _snake.Margin.Left > item.Margin.Left - _snake.Width)
                    {
                        if (_snake.Margin.Top  + _snake.Height < item.Margin.Top + (_snake.Height*0.75) && _snake.Margin.Top > item.Margin.Top - _snake.Height)
                        {
                            GameOver(_snake.Name);
                        }
                    }
                }
            }
        }
        public void SearchCollisionWithPoison(SnakeBody _snake)
        {
            foreach (var item in poisons)
            {
                if (_snake.XMOVE > 0) // toRight
                {
                    if (_snake.Margin.Left + _snake.Width > item.Margin.Left && _snake.Margin.Left + _snake.Width < item.Margin.Left + (_snake.Width * 0.75))
                    {
                        if (_snake.Margin.Top > item.Margin.Top - _snake.Height && _snake.Margin.Top < item.Margin.Top + item.Height)
                        {
                            GameOver(_snake.Name);

                        }
                    }
                }
                if (_snake.XMOVE < 0) // toLeft
                {
                    if (_snake.Margin.Left < item.Margin.Left + item.Width && _snake.Margin.Left + (_snake.Width * 0.75) > item.Margin.Left + item.Width)
                    {
                        if (_snake.Margin.Top > item.Margin.Top - _snake.Height && _snake.Margin.Top < item.Margin.Top + item.Height)
                        {
                            GameOver(_snake.Name);

                        }
                    }
                }
                if (_snake.YMOVE < 0) // toUp
                {
                    if (_snake.Margin.Left < item.Margin.Left + item.Width && _snake.Margin.Left > item.Margin.Left - _snake.Width)
                    {
                        if (_snake.Margin.Top < item.Margin.Top + item.Height && _snake.Margin.Top > item.Margin.Top + item.Height - (_snake.Height * 0.75))
                        {
                            GameOver(_snake.Name);

                        }
                    }
                }
                if (_snake.YMOVE > 0) // toDown
                {
                    if (_snake.Margin.Left < item.Margin.Left + item.Width && _snake.Margin.Left > item.Margin.Left - _snake.Width)
                    {
                        if (_snake.Margin.Top + _snake.Height < item.Margin.Top + (_snake.Height * 0.75) && _snake.Margin.Top > item.Margin.Top - _snake.Height)
                        {
                            GameOver(_snake.Name);

                        }
                    }
                }
            }
        }


        public void TwoSnakesCollision(SnakeBody _mySnake, SnakeBody _opponentSnake, List<SnakeBody> _opponentBody)
        {

            #region Collision With Opponent Head

            if (_mySnake.XMOVE > 0) // toRight
            {
                if (_mySnake.Margin.Left + _mySnake.Width > _opponentSnake.Margin.Left && _mySnake.Margin.Left + _mySnake.Width < _opponentSnake.Margin.Left + (_mySnake.Width * 0.75))
                {
                    if (_mySnake.Margin.Top > _opponentSnake.Margin.Top - _mySnake.Height && _mySnake.Margin.Top < _opponentSnake.Margin.Top + _opponentSnake.Height)
                    {
                        GameOver(_mySnake.Name);

                    }
                }
            }
            if (_mySnake.XMOVE < 0) // toLeft
            {
                if (_mySnake.Margin.Left < _opponentSnake.Margin.Left + _opponentSnake.Width && _mySnake.Margin.Left + (_mySnake.Width * 0.75) > _opponentSnake.Margin.Left + _opponentSnake.Width)
                {
                    if (_mySnake.Margin.Top > _opponentSnake.Margin.Top - _mySnake.Height && _mySnake.Margin.Top < _opponentSnake.Margin.Top + _opponentSnake.Height)
                    {
                        GameOver(_mySnake.Name);

                    }
                }
            }
            if (_mySnake.YMOVE < 0) // toUp
            {
                if (_mySnake.Margin.Left < _opponentSnake.Margin.Left + _opponentSnake.Width && _mySnake.Margin.Left > _opponentSnake.Margin.Left - _mySnake.Width)
                {
                    if (_mySnake.Margin.Top < _opponentSnake.Margin.Top + _opponentSnake.Height && _mySnake.Margin.Top > _opponentSnake.Margin.Top + _opponentSnake.Height - (_mySnake.Height * 0.75))
                    {
                        GameOver(_mySnake.Name);

                    }
                }
            }
            if (_mySnake.YMOVE > 0) // toDown
            {
                if (_mySnake.Margin.Left < _opponentSnake.Margin.Left + _opponentSnake.Width && _mySnake.Margin.Left > _opponentSnake.Margin.Left - _mySnake.Width)
                {
                    if (_mySnake.Margin.Top + _mySnake.Height < _opponentSnake.Margin.Top + (_mySnake.Height * 0.75) && _mySnake.Margin.Top > _opponentSnake.Margin.Top - _mySnake.Height)
                    {
                        GameOver(_mySnake.Name);

                    }
                }
            }

            #endregion

            foreach (var item in _opponentBody)
            {
                if (_mySnake.XMOVE > 0) // toRight
                {
                    if (_mySnake.Margin.Left + _mySnake.Width > item.Margin.Left && _mySnake.Margin.Left + _mySnake.Width < item.Margin.Left + (_mySnake.Width * 0.75))
                    {
                        if (_mySnake.Margin.Top > item.Margin.Top - _mySnake.Height && _mySnake.Margin.Top < item.Margin.Top + item.Height)
                        {
                            GameOver(_mySnake.Name);

                        }
                    }
                }
                if (_mySnake.XMOVE < 0) // toLeft
                {
                    if (_mySnake.Margin.Left < item.Margin.Left + item.Width && _mySnake.Margin.Left + (_mySnake.Width * 0.75) > item.Margin.Left + item.Width)
                    {
                        if (_mySnake.Margin.Top > item.Margin.Top - _mySnake.Height && _mySnake.Margin.Top < item.Margin.Top + item.Height)
                        {
                            GameOver(_mySnake.Name);

                        }
                    }
                }
                if (_mySnake.YMOVE < 0) // toUp
                {
                    if (_mySnake.Margin.Left < item.Margin.Left + item.Width && _mySnake.Margin.Left > item.Margin.Left - _mySnake.Width)
                    {
                        if (_mySnake.Margin.Top < item.Margin.Top + item.Height && _mySnake.Margin.Top > item.Margin.Top + item.Height - (_mySnake.Height * 0.75))
                        {
                            GameOver(_mySnake.Name);

                        }
                    }
                }
                if (_mySnake.YMOVE > 0) // toDown
                {
                    if (_mySnake.Margin.Left < item.Margin.Left + item.Width && _mySnake.Margin.Left > item.Margin.Left - _mySnake.Width)
                    {
                        if (_mySnake.Margin.Top + _mySnake.Height < item.Margin.Top + (_mySnake.Height * 0.75) && _mySnake.Margin.Top > item.Margin.Top - _mySnake.Height)
                        {
                            GameOver(_mySnake.Name);
                        }
                    }
                }
            }
        }

        public void IsSnakeEatFruit(List<SnakeBody> _body, SnakeBody _snake, int _snake_length)
        {
            foreach (var item in fruits)
            {
                if(item.IsEnabled== true)
                {
                    if (_snake.XMOVE > 0) // toRight
                    {
                        if (_snake.Margin.Left + _snake.Width > item.Margin.Left && _snake.Margin.Left + _snake.Width < item.Margin.Left + (_snake.Width * 0.75))
                        {
                            if (_snake.Margin.Top > item.Margin.Top - _snake.Height && _snake.Margin.Top < item.Margin.Top + item.Height)
                            {
                                item.IsEnabled = false;
                                item.Visibility = Visibility.Hidden;
                                EatSoundEffect();

                                for (int i = 0; i < 2; i++)
                                {
                                    
                                    AddToSnakeBody(_body, _snake, _snake_length);
                                }
                                



                            }
                        }
                    }
                    if (_snake.XMOVE < 0) // toLeft
                    {
                        if (_snake.Margin.Left < item.Margin.Left + item.Width && _snake.Margin.Left + (_snake.Width * 0.75) > item.Margin.Left + item.Width)
                        {
                            if (_snake.Margin.Top > item.Margin.Top - _snake.Height && _snake.Margin.Top < item.Margin.Top + item.Height)
                            {
                                item.IsEnabled = false;
                                item.Visibility = Visibility.Hidden;
                                EatSoundEffect();
                                for (int i = 0; i < 2; i++)
                                {
                                    AddToSnakeBody(_body, _snake, _snake_length);
                                }


                            }
                        }
                    }
                    if (_snake.YMOVE < 0) // toUp
                    {
                        if (_snake.Margin.Left < item.Margin.Left + item.Width && _snake.Margin.Left > item.Margin.Left - _snake.Width)
                        {
                            if (_snake.Margin.Top < item.Margin.Top + item.Height && _snake.Margin.Top > item.Margin.Top + item.Height - (_snake.Height * 0.75))
                            {
                                item.IsEnabled = false;
                                item.Visibility = Visibility.Hidden;
                                EatSoundEffect();

                                for (int i = 0; i < 2; i++)
                                {
                                    AddToSnakeBody(_body, _snake, _snake_length);
                                }
                            }
                        }
                    }
                    if (_snake.YMOVE > 0) // toDown
                    {
                        if (_snake.Margin.Left < item.Margin.Left + item.Width && _snake.Margin.Left > item.Margin.Left - _snake.Width)
                        {
                            if (_snake.Margin.Top + _snake.Height < item.Margin.Top + (_snake.Height * 0.75) && _snake.Margin.Top > item.Margin.Top - _snake.Height)
                            {
                                item.IsEnabled = false;
                                item.Visibility = Visibility.Hidden;
                                EatSoundEffect();

                                for (int i = 0; i < 2; i++)
                                {
                                    AddToSnakeBody(_body, _snake, _snake_length);
                                }

                            }
                        }
                    }
                }
               
            }
        }

       
        public void EatSoundEffect()
        {
            if (GameEffectSound == true)
            {

                try
                {
                    eatSound.Play();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Sounds Effect Error!\n" + ex.Message);
                }
                

                
            }

            
        }

        public void ClickSoundEffect()
        {
            if (GameEffectSound == true)
            {


                try
                {
                    clickSound.Play();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Sounds Effect Error!\n"+ ex.Message);
                }


            }


        }
        public void FailSoundEffect()
        {
            if (GameEffectSound == true)
            {


                try
                {
                    failkSound.Play();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Sounds Effect Error!\n" + ex.Message);
                }


            }


        }


        public void OnlySnakeMoveEvent()
        {
            

            timer.Tick += OnlySnakeMove;
            timer.Start();
        }

        public void OnlySnakeMove(object sender, EventArgs e)
        {
            snake.StepOfObjectMove();

            if (twoPlayer == true)
            {
                snake2.StepOfObjectMove();
            }



            #region Move Elements of Snake's body (with Tips)

            foreach (var item in body)
            {
                thatTips = 1;
                foreach (var i in listOfTurns)
                {
                    if (thatTips > item.DoneTips)
                    {
                        if (item.Margin.Left == i.XPosition && item.Margin.Top == i.YPosition)
                        {
                            if (i.Turn == "Up")
                            {
                                if (item.YMOVE == 0)
                                {
                                    item.YMOVE = -snakeSpeed;
                                    item.XMOVE = 0;
                                    item.DoneTips++;

                                }
                            }
                            if (i.Turn == "Down")
                            {
                                if (item.YMOVE == 0)
                                {
                                    item.YMOVE = snakeSpeed;
                                    item.XMOVE = 0;
                                    item.DoneTips++;
                                }
                            }
                            if (i.Turn == "Left")
                            {
                                if (item.XMOVE == 0)
                                {
                                    item.YMOVE = 0;
                                    item.XMOVE = -snakeSpeed;
                                    item.DoneTips++;
                                }
                            }
                            if (i.Turn == "Right")
                            {
                                if (item.XMOVE == 0)
                                {
                                    item.YMOVE = 0;
                                    item.XMOVE = snakeSpeed;
                                    item.DoneTips++;
                                }
                            }
                        }




                    }
                    thatTips++;
                }
                item.StepOfObjectMove();





            }

            #endregion

            if (twoPlayer == true)
            {
                #region Move Elements of Snake's2 body2 (with Tips2)

                foreach (var item in body2)
                {
                    thatTips = 1;
                    foreach (var i in listOfTurns2)
                    {
                        if (thatTips > item.DoneTips)
                        {
                            if (item.Margin.Left == i.XPosition && item.Margin.Top == i.YPosition)
                            {
                                if (i.Turn == "Up")
                                {
                                    if (item.YMOVE == 0)
                                    {
                                        item.YMOVE = -snakeSpeed;
                                        item.XMOVE = 0;
                                        item.DoneTips++;

                                    }
                                }
                                if (i.Turn == "Down")
                                {
                                    if (item.YMOVE == 0)
                                    {
                                        item.YMOVE = snakeSpeed;
                                        item.XMOVE = 0;
                                        item.DoneTips++;
                                    }
                                }
                                if (i.Turn == "Left")
                                {
                                    if (item.XMOVE == 0)
                                    {
                                        item.YMOVE = 0;
                                        item.XMOVE = -snakeSpeed;
                                        item.DoneTips++;
                                    }
                                }
                                if (i.Turn == "Right")
                                {
                                    if (item.XMOVE == 0)
                                    {
                                        item.YMOVE = 0;
                                        item.XMOVE = snakeSpeed;
                                        item.DoneTips++;
                                    }
                                }
                            }




                        }
                        thatTips++;
                    }
                    item.StepOfObjectMove();





                }

                #endregion

                #region Snake2/Snake's body get out of the area
                if (snake2.Margin.Left > gameArea.Width)
                    snake2.ChangeObjectPosition(Convert.ToInt32(0 - gameArea.Width - snake2.Width), Convert.ToInt32(snake2.Margin.Top));


                if (snake2.Margin.Left < 0 - gameArea.Width - snake2.Width)
                    snake2.ChangeObjectPosition(Convert.ToInt32(gameArea.Width), Convert.ToInt32(snake2.Margin.Top));


                if (snake2.Margin.Top < 0 - gameArea.Height - snake2.Height)
                    snake2.ChangeObjectPosition(Convert.ToInt32(snake2.Margin.Left), Convert.ToInt32(gameArea.Height));


                if (snake2.Margin.Top > gameArea.Height)
                    snake2.ChangeObjectPosition(Convert.ToInt32(snake2.Margin.Left), Convert.ToInt32(0 - gameArea.Height - snake2.Height));


                foreach (var item in body2)
                {
                    if (item.Margin.Left > gameArea.Width)
                        item.ChangeObjectPosition(Convert.ToInt32(0 - gameArea.Width - item.Width), Convert.ToInt32(item.Margin.Top));


                    if (item.Margin.Left < 0 - gameArea.Width - item.Width)
                        item.ChangeObjectPosition(Convert.ToInt32(gameArea.Width), Convert.ToInt32(item.Margin.Top));


                    if (item.Margin.Top < 0 - gameArea.Height - item.Height)
                        item.ChangeObjectPosition(Convert.ToInt32(item.Margin.Left), Convert.ToInt32(gameArea.Height));


                    if (item.Margin.Top > gameArea.Height)
                        item.ChangeObjectPosition(Convert.ToInt32(item.Margin.Left), Convert.ToInt32(0 - gameArea.Height - item.Height));
                }
                #endregion


            }

        }

        public void SnakeMoveEvent()
        {
            
            
            timer.Tick += SnakeMove;
            timer.Start();
        }


        public void SnakeMove(object sender, EventArgs e)
        {

            increament++;
            Random rnd = new Random();
            // step of the snake's move
            
            

            if (twoPlayer == true)
            {
                
                TwoSnakesCollision(snake, snake2, body2);
                TwoSnakesCollision(snake2, snake, body);
            }
            

            if (body.Count !=0 || body2.Count != 0)
            {
                SearchCollisionWithBody(body, snake);
                if (twoPlayer == true)
                    SearchCollisionWithBody(body2, snake2);
            }
                

            if (fruits.Count != 0)
            {
                IsSnakeEatFruit(body, snake, lenghthOfSnake);
                if (twoPlayer == true)
                    IsSnakeEatFruit(body2, snake2, lenghthOfSnake2);
                
            }
                

            if (poisons.Count != 0)
            {
                SearchCollisionWithPoison(snake);
                if (twoPlayer == true)
                    SearchCollisionWithPoison(snake2);
            }


            lenghtDisplay.Content = lenghthOfSnake;
            lenghtDisplay2.Content = lenghthOfSnake2;
            


            #region Snake/Snake's body get out of the area
            if (snake.Margin.Left > gameArea.Width)
                snake.ChangeObjectPosition(Convert.ToInt32( 0 - gameArea.Width - snake.Width), Convert.ToInt32(snake.Margin.Top));
                

            if (snake.Margin.Left < 0 - gameArea.Width - snake.Width)
                snake.ChangeObjectPosition(Convert.ToInt32(gameArea.Width), Convert.ToInt32(snake.Margin.Top));
           

            if (snake.Margin.Top < 0 - gameArea.Height - snake.Height)
                snake.ChangeObjectPosition(Convert.ToInt32(snake.Margin.Left), Convert.ToInt32(gameArea.Height));
          

            if (snake.Margin.Top > gameArea.Height)
                snake.ChangeObjectPosition(Convert.ToInt32(snake.Margin.Left), Convert.ToInt32(0 - gameArea.Height - snake.Height));


            foreach (var item in body)
            {
                if (item.Margin.Left > gameArea.Width)
                    item.ChangeObjectPosition(Convert.ToInt32(0 - gameArea.Width - item.Width), Convert.ToInt32(item.Margin.Top));


                if (item.Margin.Left < 0 - gameArea.Width - item.Width)
                    item.ChangeObjectPosition(Convert.ToInt32(gameArea.Width), Convert.ToInt32(item.Margin.Top));


                if (item.Margin.Top < 0 - gameArea.Height - item.Height)
                    item.ChangeObjectPosition(Convert.ToInt32(item.Margin.Left), Convert.ToInt32(gameArea.Height));


                if (item.Margin.Top > gameArea.Height)
                    item.ChangeObjectPosition(Convert.ToInt32(item.Margin.Left), Convert.ToInt32(0 - gameArea.Height - item.Height));
            }
            #endregion



            #region Onclick

            

                #region Snake1
                if (Keyboard.IsKeyDown(Key.Up))
            {
                if (snake.YMOVE == 0)
                {
                    if (body.Count != 0)
                        listOfTurns.Add(new TipForTurns(snake.Margin.Left, snake.Margin.Top, "Up"));
                    snake.XMOVE = 0;
                    snake.YMOVE = -snakeSpeed;

                }

            }
            if (Keyboard.IsKeyDown(Key.Down))
            {
                if (snake.YMOVE == 0)
                {
                    if (body.Count != 0)
                        listOfTurns.Add(new TipForTurns(snake.Margin.Left, snake.Margin.Top, "Down"));
                    snake.XMOVE = 0;
                    snake.YMOVE = snakeSpeed;

                }

            }
            if (Keyboard.IsKeyDown(Key.Left))
            {
                if (snake.XMOVE == 0)
                {
                    if (body.Count != 0)
                        listOfTurns.Add(new TipForTurns(snake.Margin.Left, snake.Margin.Top, "Left"));
                    snake.XMOVE = -snakeSpeed;
                    snake.YMOVE = 0;

                }

            }
            if (Keyboard.IsKeyDown(Key.Right))
            {
                if (snake.XMOVE == 0)
                {
                    if (body.Count != 0)
                        listOfTurns.Add(new TipForTurns(snake.Margin.Left, snake.Margin.Top, "Right"));
                    snake.XMOVE = snakeSpeed;
                    snake.YMOVE = 0;

                }

            }
            #endregion


            if (twoPlayer == true)
            {
                #region Snake2
                try
                {
                    if (Keyboard.IsKeyDown(Key.W))
                    {
                        if (snake2.YMOVE == 0)
                        {
                            if (body2.Count != 0)
                                listOfTurns2.Add(new TipForTurns(snake2.Margin.Left, snake2.Margin.Top, "Up"));
                            snake2.XMOVE = 0;
                            snake2.YMOVE = -snakeSpeed;

                        }

                    }
                    if (Keyboard.IsKeyDown(Key.S))
                    {
                        if (snake2.YMOVE == 0)
                        {
                            if (body2.Count != 0)
                                listOfTurns2.Add(new TipForTurns(snake2.Margin.Left, snake2.Margin.Top, "Down"));
                            snake2.XMOVE = 0;
                            snake2.YMOVE = snakeSpeed;

                        }

                    }
                    if (Keyboard.IsKeyDown(Key.A))
                    {
                        if (snake2.XMOVE == 0)
                        {
                            if (body2.Count != 0)
                                listOfTurns2.Add(new TipForTurns(snake2.Margin.Left, snake2.Margin.Top, "Left"));
                            snake2.XMOVE = -snakeSpeed;
                            snake2.YMOVE = 0;

                        }

                    }
                    if (Keyboard.IsKeyDown(Key.D))
                    {
                        if (snake2.XMOVE == 0)
                        {
                            if (body2.Count != 0)
                                listOfTurns2.Add(new TipForTurns(snake2.Margin.Left, snake2.Margin.Top, "Right"));
                            snake2.XMOVE = snakeSpeed;
                            snake2.YMOVE = 0;

                        }

                    }
                }
                catch (Exception ex)
                {

                }
                #endregion
            }


            #endregion

            #region AddFruit

            if(twoPlayer==true)
            {
                if (fruits.Count == (lenghthOfSnake / 2 + lenghthOfSnake2 / 2))
                {

                    fruits.Add(new Fruit(rnd.Next(Convert.ToInt32(-gameArea.Width), Convert.ToInt32(gameArea.Width) - 50), rnd.Next(Convert.ToInt32(-gameArea.Height), Convert.ToInt32(gameArea.Height) - 50)));
                    fruits.Add(new Fruit(rnd.Next(Convert.ToInt32(-gameArea.Width), Convert.ToInt32(gameArea.Width) - 50), rnd.Next(Convert.ToInt32(-gameArea.Height), Convert.ToInt32(gameArea.Height) - 50)));

                    DisplayElements();

                }
            }
            else
            {
                if (fruits.Count == lenghthOfSnake / 2)
                {

                    fruits.Add(new Fruit(rnd.Next(Convert.ToInt32(-gameArea.Width), Convert.ToInt32(gameArea.Width) - 50), rnd.Next(Convert.ToInt32(-gameArea.Height), Convert.ToInt32(gameArea.Height) - 50)));
                    fruits.Add(new Fruit(rnd.Next(Convert.ToInt32(-gameArea.Width), Convert.ToInt32(gameArea.Width) - 50), rnd.Next(Convert.ToInt32(-gameArea.Height), Convert.ToInt32(gameArea.Height) - 50)));

                    DisplayElements();

                }
            }
           

            #endregion

            #region Add Poison
            if (increament % 2000 == 0)
            {
                poisons.Add(new Poison(rnd.Next(Convert.ToInt32(-gameArea.Width) , Convert.ToInt32(gameArea.Width) - 50), rnd.Next(Convert.ToInt32(-gameArea.Height), Convert.ToInt32(gameArea.Height) - 50)));

                DisplayElements();

            }
            #endregion



        }


        #region Start game Label/Button
        private void againLabel_MouseEnter(object sender, MouseEventArgs e)
        {
            againLabel.Background = Brushes.YellowGreen;
        }

        private void againLabel_MouseLeave(object sender, MouseEventArgs e)
        {
            againLabel.Background = Brushes.Green;
        }

        private void againLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClickSoundEffect();
            twoPlayer = false;

            

            if (increament ==0)
            {
                snake.YMOVE = snakeSpeed;
                snake.XMOVE = 0;
                gameArea.Children.Add(snake);

                

                SnakeMoveEvent();
                OnlySnakeMoveEvent();
                againLabel.IsEnabled = false;
                TwoPlayer.IsEnabled = false;
                InfoTable.Visibility = Visibility.Hidden;
                lenghthOfSnake = 0;
                


            }
            else
            {
                fruits.Clear();
                poisons.Clear();
                gameArea.Children.Clear();
                increament = 0;

                listOfTurns.Clear();
                body.Clear();
                gameArea.Children.Add(snake);
                lenghthOfSnake = 0;

               
                

                timer.Start();
                timer2.Start();
                againLabel.IsEnabled = false;
                TwoPlayer.IsEnabled = false;
                InfoTable.Visibility = Visibility.Hidden;
                
            }
            lenghtDisplay2_label.Visibility = Visibility.Hidden;
            lenghtDisplay2.Visibility = Visibility.Hidden;
        }

        #endregion


        #region Options Label/Button
        private void Options_MouseEnter(object sender, MouseEventArgs e)
        {
            Options.Background = Brushes.YellowGreen;
        }

        private void Options_MouseLeave(object sender, MouseEventArgs e)
        {
            Options.Background = Brushes.Green;
        }


        private void Options_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClickSoundEffect();
            Options_Grid.Visibility = Visibility.Visible;
        }
        #endregion


        #region Exit Label/Button
        private void Exit_MouseEnter(object sender, MouseEventArgs e)
        {
            Exit.Background = Brushes.YellowGreen;
        }

        private void Exit_MouseLeave(object sender, MouseEventArgs e)
        {
            Exit.Background = Brushes.Green;
        }

       

        private void Exit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClickSoundEffect();

            Thread.Sleep(500);
            Environment.Exit(1);
        }
        #endregion


        #region Enter Name for the best player
        private void okName_MouseEnter(object sender, MouseEventArgs e)
        {
            okName.Background = Brushes.YellowGreen;
        }

        private void okName_MouseLeave(object sender, MouseEventArgs e)
        {
            okName.Background = Brushes.Green;
        }

        

        private void okName_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                using (StreamWriter strW = new StreamWriter("./gameTextFiles/theBest.txt", false))
                {
                    strW.WriteLine(newName_textBox.Text.ToString());
                    strW.WriteLine(NewBestScore);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            ReadBestScore();
            BestScore.Visibility = Visibility.Hidden;
            finalLength.Visibility = Visibility.Visible;
            InfoTable.Visibility = Visibility.Visible;

            nameLabel.Content = Best_name;
            scoreLabel.Content = Best_score;


            gameArea.Children.Clear();
        }
        #endregion

        #region Two Players Start

        private void TwoPlayer_MouseEnter(object sender, MouseEventArgs e)
        {
            TwoPlayer.Background = Brushes.YellowGreen;
        }

        private void TwoPlayer_MouseLeave(object sender, MouseEventArgs e)
        {
            TwoPlayer.Background = Brushes.Green;
        }

        

        private void TwoPlayer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            twoPlayer = true;
            ClickSoundEffect();

           

            if (increament == 0)
            {
                snake.YMOVE = snakeSpeed;
                snake.XMOVE = 0;
                gameArea.Children.Add(snake);

                snake2.YMOVE = 0;
                snake2.XMOVE = snakeSpeed;
                gameArea.Children.Add(snake2);

                snake.ChangeObjectPosition(Convert.ToInt32(snake.Margin.Left) -30, Convert.ToInt32(snake.Margin.Top) - 30);
                snake2.ChangeObjectPosition(Convert.ToInt32(snake.Margin.Left) + 100, Convert.ToInt32(snake.Margin.Top) +100);

                SnakeMoveEvent();
                OnlySnakeMoveEvent();
                againLabel.IsEnabled = false;
                TwoPlayer.IsEnabled = false;
                InfoTable.Visibility = Visibility.Hidden;
                lenghthOfSnake = 0;
                lenghthOfSnake2 = 0;
            }
                
            
           else
            {
                fruits.Clear();
                poisons.Clear();
                gameArea.Children.Clear();
                increament = 0;

                listOfTurns.Clear();
                body.Clear();

                snake.ChangeObjectPosition(Convert.ToInt32(snake.Margin.Left) - 30, Convert.ToInt32(snake.Margin.Top) - 30);
                snake2.ChangeObjectPosition(Convert.ToInt32(snake.Margin.Left) + 100, Convert.ToInt32(snake.Margin.Top) + 100);


                snake2.YMOVE = 0;
                snake2.XMOVE = snakeSpeed;

                snake.YMOVE = snakeSpeed;
                snake.XMOVE = 0;

                listOfTurns2.Clear();
                body2.Clear();
                gameArea.Children.Add(snake2);
                gameArea.Children.Add(snake);
                lenghthOfSnake = 0;
                lenghthOfSnake2 = 0;




                timer.Start();
                timer2.Start();
                againLabel.IsEnabled = false;
                TwoPlayer.IsEnabled = false;
                InfoTable.Visibility = Visibility.Hidden;
            }

            lenghtDisplay2_label.Visibility = Visibility.Visible;
            lenghtDisplay2.Visibility = Visibility.Visible;


        }

        #endregion

        #region Options Grid

        #region OK Options Label/Button

        private void OK_Options_MouseEnter(object sender, MouseEventArgs e)
        {
            OK_Options.Background = Brushes.YellowGreen;
        }

        private void OK_Options_MouseLeave(object sender, MouseEventArgs e)
        {
            OK_Options.Background = Brushes.Green;
        }



        private void OK_Options_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClickSoundEffect();
            if (ON_Effect.Background == Brushes.Gold)
                GameEffectSound = true;
            if (OFF_Effect.Background == Brushes.Gold)
                GameEffectSound = false;

            Options_Grid.Visibility = Visibility.Hidden;
            
        }



        #endregion


        private void ON_Effect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClickSoundEffect();
            ON_Effect.Background = Brushes.Gold;
            ON_Effect.Foreground = Brushes.Black;

            OFF_Effect.Foreground = Brushes.SlateBlue;
            OFF_Effect.Background = Brushes.Silver;
        }
        private void OFF_Effect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClickSoundEffect();
            OFF_Effect.Background = Brushes.Gold;
            OFF_Effect.Foreground = Brushes.Black;

            ON_Effect.Foreground = Brushes.SlateBlue;
            ON_Effect.Background = Brushes.Silver;
        }

        #endregion



    }

    public class SnakeBody: Image
    {
        private int Xmove;
        public int XMOVE
        {
            get
            {
                return Xmove;
            }
            set
            {
                Xmove = value;
            }
        }

        public int DoneTips;

        private int Ymove;
        public int YMOVE
        {
            get
            {
                return Ymove;
            }
            set
            {
                Ymove = value;
            }
        }

        private Thickness ObjectMargin;

        public void StepOfObjectMove()
        {
            ObjectMargin.Top += Ymove;
            ObjectMargin.Left += Xmove;
            Margin = ObjectMargin;
            
        }
        public void ChangeObjectPosition(int mL, int mT)
        {
            ObjectMargin.Top = mT;
            ObjectMargin.Left = mL;
            Margin = ObjectMargin;

        }


        public SnakeBody(string url)
        {
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri(url, UriKind.Relative);
            bi3.EndInit();
            Source = bi3;

            Width = 20;
             
            Height = 20;
            Margin = new Thickness(0, 0, 0, 0);
            ObjectMargin = new Thickness();
            ObjectMargin = Margin;
            DoneTips = 0;
        }
        public SnakeBody(double marginL, double marginT, int x_move, int y_move, int _doneTIPS, string url)
        {
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri(url, UriKind.Relative);
            bi3.EndInit();
            Source = bi3;

            Margin = new Thickness(marginL,marginT,0,0);
            Width = 20;
            Height = 20;
            ObjectMargin = new Thickness(marginL, marginT, 0, 0);

            Ymove = y_move;
            Xmove = x_move;
            DoneTips = _doneTIPS;
        }

        
    }
    public class TipForTurns
    {
        public double XPosition { get; set; }
        public double YPosition { get; set; }


        public string Turn;

        public TipForTurns(double x, double y, string _turn)
        {
            XPosition = x;
            YPosition = y;
            Turn = _turn;
        }

    }


    public class Fruit: Image
    {
        public Fruit(int marginL, int marginT)
        {
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("images/apple_snake.png", UriKind.Relative);
            bi3.EndInit();
            Source = bi3;

            Width = 20;
            Height = 20;
            Margin = new Thickness(marginL, marginT, 0, 0);
            
        }
    }

    public class Poison: Image
    {
        public Poison(int marginL, int marginT)
        {
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("images/poison.png", UriKind.Relative);
            bi3.EndInit();
            Source = bi3;

            Width = 30;
            Height = 40;
            Margin = new Thickness(marginL, marginT, 0, 0);

        }
    }
}
