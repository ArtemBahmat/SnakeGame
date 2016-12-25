using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SnakeGame.Model;

namespace SnakeGame
{
    public partial class GameForm : Form
    {
        List<SnakePart> snake = new List<SnakePart>();
        SnakePart food = new SnakePart();
        const int TILE_WIDTH = 16;
        const int TILE_HEIGHT = 16;
        const string SCORE_FILE = "score.txt";
        bool _gameOver = false;
        int _score = 0;
        int _timerInterval = 200;   //milliseconds 
        Direction direction;

        enum Direction
        {
            Down = 0,
            Left = 1, 
            Right = 2, 
            Up = 3
        }
               
        public GameForm()
        {
            InitializeComponent();
            gameTimer.Interval = _timerInterval;     
            gameTimer.Tick += new EventHandler(Update);
            gameTimer.Start();
            StartGame();
        }

        private void StartGame()
        {
            _gameOver = false;
            _score = 0;
            
            direction = Direction.Down;
            snake.Clear();
            SnakePart head = new SnakePart(10, 5);
            gameTimer.Interval = _timerInterval;
            snake.Add(head);
            food.GenerateFood(picBox.Size.Width / TILE_WIDTH, picBox.Size.Height / TILE_HEIGHT);
        }     

        private void Update(object sender, EventArgs e)
        {
            if (_gameOver)
            {
                if (Input.Pressed(Keys.Enter))
                    StartGame();
            }
            else
            {
                if (Input.Pressed(Keys.Right))
                {
                    if (snake.Count < 2 || snake[0].X == snake[1].X)
                        direction = Direction.Right;
                }
                else if (Input.Pressed(Keys.Left))
                {
                    if (snake.Count < 2 || snake[0].X == snake[1].X)
                        direction = Direction.Left;
                }
                else if (Input.Pressed(Keys.Up))
                {
                    if (snake.Count < 2 || snake[0].Y == snake[1].Y)
                        direction = Direction.Up;
                }
                else if (Input.Pressed(Keys.Down))
                {
                    if (snake.Count < 2 || snake[0].Y == snake[1].Y)
                        direction = Direction.Down;
                }
                UpdateSnake();
            }
            picBox.Invalidate();
        }

        private void UpdateSnake()
        {
            for (int i = snake.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    switch (direction)
                    {
                        case Direction.Down: 
                            snake[i].Y++;
                            break;
                        case Direction.Left: 
                            snake[i].X--;
                            break;
                        case Direction.Right: 
                            snake[i].X++;
                            break;
                        case Direction.Up: 
                            snake[i].Y--;
                            break;
                    }

                    int maxTileWidth = picBox.Width / TILE_WIDTH;
                    int maxTileHeight = picBox.Height / TILE_HEIGHT;

                    _gameOver = snake[i].X < 0 || snake[i].X >= maxTileWidth || snake[i].Y < 0 || snake[i].Y >= maxTileHeight;

                    for (int j = 1; j < snake.Count; j++)
                    {
                        if (snake[i].CompareTo(snake[j]) == 0)
                        {
                            _gameOver = true;
                        }
                    }

                    if(snake[i].CompareTo(food) == 0)
                    {
                        SnakePart part = new SnakePart(snake[snake.Count - 1].X, snake[snake.Count - 1].Y);
                        snake.Add(part);
                        food.GenerateFood(picBox.Size.Width / TILE_WIDTH, picBox.Size.Height / TILE_HEIGHT);
                        _score++;
                        gameTimer.Interval -= 10;
                    }
                }
                else
                {
                    snake[i].X = snake[i - 1].X;
                    snake[i].Y = snake[i - 1].Y;
                }
            }
        }

        private void PicBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;
            if (_gameOver)
            {
                Font font = this.Font;
                string gameoverMsg = "Gameover";
                string scoreMsg = "Score: " + _score.ToString();
                string newGameMsg = "Press Enter to Start Over";
                int centerWidth = picBox.Width / 2;
                SizeF msgSize = canvas.MeasureString(gameoverMsg, font);
                PointF msgPoint = new PointF(centerWidth - msgSize.Width / 2, 16);
                canvas.DrawString(gameoverMsg, font, Brushes.White, msgPoint);
                msgSize = canvas.MeasureString(scoreMsg, font);
                msgPoint = new PointF(centerWidth - msgSize.Width / 2, 32);
                canvas.DrawString(scoreMsg, font, Brushes.White, msgPoint);
                msgSize = canvas.MeasureString(newGameMsg, font);
                msgPoint = new PointF(centerWidth - msgSize.Width / 2, 48);
                canvas.DrawString(newGameMsg, font, Brushes.White, msgPoint);
            }
            else
            {
                for (int i = 0; i < snake.Count; i++)
                {
                    Brush snake_color = i == 0 ? Brushes.Red : Brushes.Black;
                    canvas.FillRectangle(snake_color, new Rectangle(snake[i].X * TILE_WIDTH, snake[i].Y * TILE_HEIGHT, TILE_WIDTH, TILE_HEIGHT));
                }

                canvas.FillRectangle(Brushes.Orange, new Rectangle(food.X * TILE_WIDTH, food.Y * TILE_HEIGHT, TILE_WIDTH, TILE_HEIGHT));
                canvas.DrawString("Score: " + _score.ToString(), this.Font, Brushes.White, new PointF(4, 4));
            }
        }

        private void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, false);
        }

        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {         
            Input.ChangeState(e.KeyCode, true);
            gameTimer.Stop();
            Update(null, null);
            gameTimer.Start();
        }

        private void SaveScoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string strToWrite = $"{DateTime.Now.ToString()} - scores: {_score.ToString()}";
            StreamWriter file;

            try
            {
                using (file = new StreamWriter(SCORE_FILE, true))
                {
                    file.WriteLine(strToWrite);
                    file.Close();
                }

                MessageBox.Show($"Score was saved to file: {SCORE_FILE}", "Info");              
            }
            catch (Exception)
            {
                MessageBox.Show($"Some error occured. Score wasn't saved to file", "Error");
            }
        }

        private void FinishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _gameOver = true;
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void ShowBestScoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int bestScore = GetBestScoreFromFile(SCORE_FILE);
            MessageBox.Show($"Best score: {bestScore.ToString()}", "Info");
        }

        private int GetBestScoreFromFile(string file)
        {
            List<int> scores = new List<int>();

            if (File.Exists(file))
            {
                var strings = File.ReadLines(file);
                scores = GetScores(strings);
            }

            return scores.Count() > 0 ? scores.Max() : 0;
        }

        private List<int> GetScores(IEnumerable<string> strings)
        {
            int scoreNumber = 0;
            List<int> result = new List<int>();

            foreach (var str in strings)
            {
                scoreNumber = int.Parse(str.Substring(str.LastIndexOf(' ') + 1));
                result.Add(scoreNumber);
            }

            return result;
        }
    }
}
