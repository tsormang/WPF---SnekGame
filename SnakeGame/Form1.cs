using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class Form1 : Form
    {
        private List<Circle> Snake = new List<Circle>();
        private Circle food = new Circle();


        public Form1()
        {
            InitializeComponent();

            // Link the Class 'Settings' to this Form
            new Settings();

            gameTimer.Interval = 1000 / Settings.Speed;
            gameTimer.Tick += updateScreen;
            gameTimer.Start();

            startGame();
        }

        private void keyisdown(object sender, KeyEventArgs e)
        {
            Input.changeState(e.KeyCode, true);
        }

        private void keyisup(object sender, KeyEventArgs e)
        {
            Input.changeState(e.KeyCode, false);
        }

        private void updateGraphics(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            if (Settings.GameOver == false)
            {
                Brush snakeColour;

                for (int i = 0; i < Snake.Count; i++)
                {
                    if (i == 0)
                    {
                        snakeColour = Brushes.White;
                    }
                    else
                    {
                        snakeColour = Brushes.WhiteSmoke;
                    }
                    // Render Snake Body + Head
                    canvas.FillEllipse(snakeColour,
                                        new Rectangle(
                                            Snake[i].X * Settings.Width,
                                            Snake[i].Y * Settings.Height,
                                            Settings.Width, Settings.Height
                                            ));
                    // Render food
                    canvas.FillEllipse(Brushes.Red,
                                        new Rectangle(
                                            food.X * Settings.Width,
                                            food.Y * Settings.Height,
                                            Settings.Width, Settings.Height
                                            ));
                }
            }
            else
            {
                // Game Over Text
                string GameOver = "Game Over \n" + "Final Score is " + Settings.Score + "\n Press Enter to Restart \n";
                label3.Text = GameOver;
                label3.Visible = true;
            }
        }

        private void updateScreen(object sender, KeyEventArgs e)
        {
            if (Settings.GameOver == true)
            {
                if (Input.KeyPress(Keys.Enter))
                {
                    startGame();
                }
            }
            else
            {
                if (Input.KeyPress(Keys.Right) && Settings.direction != Directions.Left)
                {
                    Settings.direction = Directions.Right;
                }
                else if (Input.KeyPress(Keys.Left) && Settings.direction != Directions.Right)
                {
                    Settings.direction = Directions.Left;
                }
                else if (Input.KeyPress(Keys.Up) && Settings.direction != Directions.Down)
                {
                    Settings.direction = Directions.Up;
                }
                else if (Input.KeyPress(Keys.Down) && Settings.direction != Directions.Up)
                {
                    Settings.direction = Directions.Down;
                }

                movePlayer();
            }
            pbCanvas.Invalidate();
        }

        private void movePlayer()
        {
            for (int i = Snake.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    switch (Settings.direction)
                    {
                        case Directions.Left:
                            Snake[i].X--;
                            break;
                        case Directions.Right:
                            Snake[i].X++;
                            break;
                        case Directions.Up:
                            Snake[i].Y--;
                            break;
                        case Directions.Down:
                            Snake[i].Y++;
                            break;
                    }

                    // Set the boundaries so as the Snake canot leave our canvas
                    int maxXpos = pbCanvas.Size.Width / Settings.Width;
                    int maxYpos = pbCanvas.Size.Height / Settings.Height;

                    // Collision with Boundaries
                    if (Snake[i].X < 0 || Snake[i].Y <0 || Snake[i].X > maxXpos || Snake[i].Y > maxYpos)
                    {
                        // Game Over if Snake touches the Boundaries
                        die();
                    }

                    // Collision with its Body
                    for (int j = 1; j < Snake.Count; j++)
                    {
                        if (Snake[i].X == Snake[j].X && Snake[i].Y == Snake[j].Y)
                        {
                            // Game Over if Snake touches its Body
                            die();
                        }
                    }

                    // Collision with Food
                    if (Snake[0].X == food.X && Snake[0].Y == food.Y)
                    {
                        // Grow Body
                        eat();
                    }

                }
                else
                {
                    // Continues Movement when there is no Collision
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                }
            }
        }

        private void startGame()
        {
            label3.Visible = false;
            new Settings();
            Snake.Clear();
            Circle head = new Circle { X = 10, Y = 5 };
            Snake.Add(head);
            label2.Text = Settings.Score.ToString();
            generateFood();
        }
        
        private void generateFood()
        {
            int maxXpos = pbCanvas.Size.Width / Settings.Width;
            int maxYpos = pbCanvas.Size.Height / Settings.Height;

            Random rnd = new Random();
            food = new Circle { X = rnd.Next(0, maxXpos), Y = rnd.Next(0, maxYpos) };
        }


        private void eat()
        {
            Circle body = new Circle
            {
                X = Snake[Snake.Count - 1].X,
                Y = Snake[Snake.Count - 1].Y
            };

            Snake.Add(body);
            Settings.Score += Settings.Points;
            label2.Text = Settings.Score.ToString();
            generateFood();
        }


        private void die()
        {
            Settings.GameOver = true;
        }






    }
}
