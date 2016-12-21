﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class GameForm : Form
    {
        List<SnakePart> snake = new List<SnakePart>();
        const int TileWidth = 16;
        const int TileHeight = 16;
        const string ScoreFile = "score.txt";
        bool gameOver = false;
        int score = 0;
        int timerInterval = 150;  //milliseconds 
        int direction = 0;  // Down = 0, Left = 1, Right = 2, Up = 3
        SnakePart food = new SnakePart();
        
        public GameForm()
        {
            InitializeComponent();
            gameTimer.Interval = timerInterval;     
            gameTimer.Tick += new EventHandler(Update);
            gameTimer.Start();
            StartGame();
        }

        private void StartGame()
        {
            gameOver = false;
            score = 0;
            direction = 0;
            snake.Clear();
            SnakePart head = new SnakePart(10, 5);
            gameTimer.Interval = timerInterval;
            snake.Add(head);
            food.GenerateFood(picBox.Size.Width / TileWidth, picBox.Size.Height / TileHeight);
        }

       

        private void Update(object sender, EventArgs e)
        {
            if (gameOver)
            {
                if (Input.Pressed(Keys.Enter))
                    StartGame();
            }
            else
            {
                if (Input.Pressed(Keys.Right))
                {
                    if (snake.Count < 2 || snake[0].X == snake[1].X)
                        direction = 2;
                }
                else if (Input.Pressed(Keys.Left))
                {
                    if (snake.Count < 2 || snake[0].X == snake[1].X)
                        direction = 1;
                }
                else if (Input.Pressed(Keys.Up))
                {
                    if (snake.Count < 2 || snake[0].Y == snake[1].Y)
                        direction = 3;
                }
                else if (Input.Pressed(Keys.Down))
                {
                    if (snake.Count < 2 || snake[0].Y == snake[1].Y)
                        direction = 0;
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
                        case 0: // Down
                            snake[i].Y++;
                            break;
                        case 1: // Left
                            snake[i].X--;
                            break;
                        case 2: // Right
                            snake[i].X++;
                            break;
                        case 3: // Up
                            snake[i].Y--;
                            break;
                    }

                    int maxTileWidth = picBox.Width / TileWidth;
                    int maxTileHeight = picBox.Height / TileHeight;

                    gameOver = snake[i].X < 0 || snake[i].X >= maxTileWidth || snake[i].Y < 0 || snake[i].Y >= maxTileHeight;

                    for (int j = 1; j < snake.Count; j++)
                    {
                        if (snake[i].X == snake[j].X && snake[i].Y == snake[j].Y)
                            gameOver = true;
                    }

                    if (snake[i].X == food.X && snake[i].Y == food.Y)
                    {
                        SnakePart part = new SnakePart(snake[snake.Count - 1].X, snake[snake.Count - 1].Y);
                        snake.Add(part);
                        food.GenerateFood(picBox.Size.Width / TileWidth, picBox.Size.Height / TileHeight);
                        score++;
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

        private void picBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;
            if (gameOver)
            {
                Font font = this.Font;
                string gameoverMsg = "Gameover";
                string scoreMsg = "Score: " + score.ToString();
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
                    canvas.FillRectangle(snake_color, new Rectangle(snake[i].X * TileWidth, snake[i].Y * TileHeight, TileWidth, TileHeight));
                }

                canvas.FillRectangle(Brushes.Orange, new Rectangle(food.X * TileWidth, food.Y * TileHeight, TileWidth, TileHeight));
                canvas.DrawString("Score: " + score.ToString(), this.Font, Brushes.White, new PointF(4, 4));
            }
        }

        private void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, false);
        }

        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, true);
        }

        private void saveScoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string strToWrite = $"{DateTime.Now.ToString()} - scores: {score.ToString()}";
            StreamWriter file;

            try
            {
                using (file = new StreamWriter(ScoreFile, true))
                {
                    file.WriteLine(strToWrite);
                    file.Close();
                }

                MessageBox.Show($"Score was saved to file: {ScoreFile}", "Info");              
            }
            catch (Exception)
            {
                MessageBox.Show($"Score was not saved to file", "Error");
            }
        }

        private void finishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gameOver = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void showBestScoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int bestScore = GetBestScoreFromFile(ScoreFile);
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