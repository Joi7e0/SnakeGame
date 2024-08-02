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

namespace Snake
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int SnakeSquareSize = 30;
        private readonly ImageBrush _snakeImage = new ImageBrush(new BitmapImage(new Uri("C:\\Maksym Havrylov\\C#\\SnakeGame-C#\\Snake\\Images\\snake.png")));
        private enum Direction {Left, Right, Top, Bottom}
        private Direction _direction = Direction.Right;
        private const int TimerInterval = 150;
        private DispatcherTimer _timer;
        private Rectangle _snakeHead;
        private Point _foodPosition;
        private static readonly Random randomPositionFood = new Random();

        private List<Rectangle> _snake = new List<Rectangle>();

        private int _score = 0;


        public MainWindow()
        {
            InitializeComponent();
     
        }
        private void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            InitialGame();
        }
        private void InitialGame()
        {
            _snakeHead = CreateSnakeSegment(new Point(5, 5));
            _snake.Add(_snakeHead);
            GameCanvas.Children.Add(_snakeHead);

            PlaceFood();

            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
            _timer.Interval = TimeSpan.FromMilliseconds(TimerInterval);
            _timer.Start();

        }

        private Rectangle CreateSnakeSegment(Point position)
        {
            Rectangle rectangle = new Rectangle
            {
                Width = SnakeSquareSize,
                Height = SnakeSquareSize,
                Fill = _snakeImage
            };

            Canvas.SetLeft(rectangle, position.X * SnakeSquareSize);
            Canvas.SetTop(rectangle, position.Y * SnakeSquareSize);

            return rectangle;
        }

        private void PlaceFood()
        {
            int maxX = (int)(GameCanvas.ActualWidth / SnakeSquareSize);
            int maxY = (int)(GameCanvas.ActualHeight / SnakeSquareSize);

            int foodX = randomPositionFood.Next(0, maxX);
            int foodY = randomPositionFood.Next(0, maxY);

            _foodPosition = new Point(foodX, foodY);

            Image foodImage = new Image
            {
                Width = SnakeSquareSize,
                Height = SnakeSquareSize,
                Source = new BitmapImage(new Uri("C:\\Maksym Havrylov\\C#\\SnakeGame-C#\\Snake\\Images\\Melon.png"))
            };
            

            Canvas.SetLeft(foodImage, foodX * SnakeSquareSize);
            Canvas.SetTop(foodImage, foodY * SnakeSquareSize);
            GameCanvas.Children.Add(foodImage);
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            Point newHeadPosition = CalcuteNewPosition();

            if(newHeadPosition == _foodPosition)
            {
                EatFood();
                PlaceFood();
            }
             
            if(newHeadPosition.X < 0 || newHeadPosition.Y < 0
                || newHeadPosition.X >= GameCanvas.ActualWidth / SnakeSquareSize
                || newHeadPosition.Y >= GameCanvas.ActualHeight / SnakeSquareSize)
            {
                EndGame();
                return;
            }

            if(_snake.Count >= 4)
            {
                for(int i = 0; i < _snake.Count; i++)
                {
                    Point currentPos = new Point(Canvas.GetLeft(_snake[i]), Canvas.GetTop(_snake[i]));

                    for(int j = i + 1; j < _snake.Count; j++)
                    {
                        Point nextPos = new Point(Canvas.GetLeft(_snake[j]), Canvas.GetTop(_snake[j]));

                        if (currentPos == nextPos)
                        {
                            EndGame();
                            return;
                        }
                    }
                }
            }

            for (int i = _snake.Count - 1; i > 0; i--)
            {
                Canvas.SetLeft(_snake[i], Canvas.GetLeft(_snake[i - 1]));
                Canvas.SetTop(_snake[i], Canvas.GetTop(_snake[i - 1]));
            }

            Canvas.SetLeft(_snakeHead, newHeadPosition.X * SnakeSquareSize);
            Canvas.SetTop(_snakeHead, newHeadPosition.Y * SnakeSquareSize);
        }

        private void EndGame()
        {
            _timer.Stop();
            RestartButton.Visibility = Visibility.Visible;
        }
        private void EatFood()
        {
            _score++;
            ScoreTextBlock.Text = "Score: " + _score.ToString();


            GameCanvas.Children.Remove(GameCanvas.Children.OfType<Image>().FirstOrDefault());
            Rectangle newSnake = CreateSnakeSegment(_foodPosition);
            _snake.Add(newSnake);
            GameCanvas.Children.Add(newSnake);
        }

        private Point CalcuteNewPosition()
        {
            double left = Canvas.GetLeft(_snakeHead) / SnakeSquareSize;
            double top = Canvas.GetTop(_snakeHead) / SnakeSquareSize;

            Point headCurrentPosition =  new Point(left, top);
            Point newHeadPostion = new Point();

            switch (_direction)
            {
                case Direction.Left:
                    newHeadPostion = new Point(headCurrentPosition.X - 1, headCurrentPosition.Y);
                    break;
                case Direction.Right:
                    newHeadPostion = new Point(headCurrentPosition.X + 1, headCurrentPosition.Y);
                    break;
                case Direction.Top:
                    newHeadPostion = new Point(headCurrentPosition.X, headCurrentPosition.Y -1);
                    break;
                case Direction.Bottom:
                    newHeadPostion = new Point(headCurrentPosition.X, headCurrentPosition.Y + 1);
                    break;

            }
            return newHeadPostion;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    if(_direction != Direction.Bottom)
                    _direction = Direction.Top;
                    break;
                case Key.Down:
                    if (_direction != Direction.Top)
                        _direction = Direction.Bottom;
                    break;
                case Key.Left:
                    if (_direction != Direction.Right)
                        _direction = Direction.Left;
                    break;
                case Key.Right:
                    if (_direction != Direction.Left)
                        _direction = Direction.Right;
                    break;

            }
        }
        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            _score = 0;
            ScoreTextBlock.Text = "Score: 0";
            GameCanvas.Children.Clear();
            _snake.Clear();
            RestartButton.Visibility = Visibility.Collapsed;
            InitialGame();

        }
        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
         
            InitialGame();
            StartButton.Visibility = Visibility.Collapsed;
            ScoreTextBlock.Visibility = Visibility.Visible;

        }
    }
}
