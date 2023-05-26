using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace SnakeGame
{
    public partial class Form1 : Form
    {
        private const int GridSize = 20;
        private const int InitialSnakeLength = 50;
        private const int TimerInterval = 1; // miliseconds
        private int score = 0;
        private Timer gameTimer;
        private List<Point> snake;
        private Point food;
        private Direction direction;
        private int highScore = 0;

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
            KeyDown += Form1_KeyDown;
        }

        private void InitializeGame()
        {
            // Set up the game area
            tableLayoutPanel1.BackColor = Color.Black;
            tableLayoutPanel1.Paint += new PaintEventHandler(DrawGameArea);

            // Set up the timer
            gameTimer = new Timer();
            gameTimer.Interval = TimerInterval;
            gameTimer.Tick += new EventHandler(UpdateGame);
            gameTimer.Start();

            // Set up initial game state
            snake = new List<Point>();
            direction = Direction.Right;

            // Create the initial snake
            Point head = new Point(10, 10);
            for (int i = 0; i < InitialSnakeLength; i++)
            {
                snake.Add(new Point(head.X - i, head.Y));
            }

            // Generate initial food
            GenerateFood();
        }

        private void UpdateGame(object sender, EventArgs e)
        {
            MoveSnake();
            CheckCollision();
            tableLayoutPanel1.Invalidate(); // Redraw the game area
                                            // Obtener la cabeza de la serpiente
            Point head = snake[0];

            // Actualizar el texto del label con las coordenadas de la cabeza de la serpiente
            snakeCoordLabel.Text = "Snake Coordinates: X = " + head.X + ", Y = " + head.Y;
        }

        private void MoveSnake()
        {
            Point head = snake[0];
            Point newHead = new Point(head.X, head.Y);

            // Update the position of the snake's head based on the current direction
            switch (direction)
            {
                case Direction.Up:
                    newHead.Y--;
                    break;
                case Direction.Down:
                    newHead.Y++;
                    break;
                case Direction.Left:
                    newHead.X--;
                    break;
                case Direction.Right:
                    newHead.X++;
                    break;
            }

            // Move the snake by adding the new head and removing the tail
            snake.Insert(0, newHead);
            snake.RemoveAt(snake.Count - 1);
        }

        private void CheckCollision()
        {
            Point head = snake[0];

            // Check if the snake collides with itself
            for (int i = 1; i < snake.Count; i++)
            {
                if (snake[i] == head)
                {
                    EndGame();
                    return;
                }
            }

            // Check if the snake collides with the walls
            if (head.X < 0 || head.X >= 769|| head.Y < 0 || head.Y >= 544)
            {
                EndGame();
                return;
            }

            // Check if the snake collides with the food
            Rectangle headRectangle = new Rectangle(head.X, head.Y, GridSize, GridSize);
            Rectangle foodRectangle = new Rectangle(food.X, food.Y, GridSize, GridSize);

            if (headRectangle.IntersectsWith(foodRectangle))
            {
                // Increase the length of the snake
                snake.Add(new Point(-1, -1));
                snake.Add(new Point(-1, -1));
                snake.Add(new Point(-1, -1));
                snake.Add(new Point(-1, -1));
                snake.Add(new Point(-1, -1));

                // Generate new food
                GenerateFood();

                // Increase the score
                score++;
                scoreLabel.Text = "Score: " + score.ToString();
            }
        }



        private void GenerateFood()
        {
            Random random = new Random();

            // Generate a random position for the food that is not occupied by the snake
            do
            {
                int x = random.Next(tableLayoutPanel1.Width / GridSize) * GridSize;
                int y = random.Next(tableLayoutPanel1.Height / GridSize) * GridSize;
                food = new Point(x, y);
            }
            while (snake.Contains(food));
        }

        private void DrawGameArea(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;

            // Draw the snake
            foreach (Point segment in snake)
            {
                int segmentSize = GridSize / 2; // Tamaño de los segmentos
                int segmentOffset = (GridSize - segmentSize) / 2; // Offset para centrar los segmentos
                graphics.FillRectangle(Brushes.Green, segment.X + segmentOffset, segment.Y + segmentOffset, segmentSize, segmentSize);
            }

            // Draw the food if it's not (-1, -1)
            if (food.X != -1 && food.Y != -1)
            {
                int fruitSize = GridSize; // Cambia el tamaño de la fruta
                int fruitOffset = (GridSize - fruitSize) / 2; // Offset para centrar la fruta
                graphics.FillEllipse(Brushes.Red, food.X + fruitOffset, food.Y + fruitOffset, fruitSize, fruitSize);
            }
        }




        private void EndGame()
        {
            gameTimer.Stop();
            MessageBox.Show("Game Over", "Snake Game", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (score > highScore)
            {
                // Si el puntaje actual es mayor que el high score, actualizar el high score
                highScore = score;

                // Guardar el high score en el archivo
                File.WriteAllText("SnakeHighScore.txt", "HighScore = "+ highScore.ToString());
            }

            // Reiniciar el puntaje a cero
            score = 0;
            scoreLabel.Text = "Score: " + score.ToString();

            InitializeGame();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Update the direction based on the arrow keys
            switch (e.KeyCode)
            {
                case Keys.Up:
                    direction = Direction.Up;
                    break;
                case Keys.Down:
                    direction = Direction.Down;
                    break;
                case Keys.Left:
                    direction = Direction.Left;
                    break;
                case Keys.Right:
                    direction = Direction.Right;
                    break;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Verificar si el archivo existe
            if (!File.Exists("SnakeHighScore.txt"))
            {
                // Si el archivo no existe, crearlo y establecer el high score en 0
                File.WriteAllText("SnakeHighScore.txt", "HighScore = 0");
            }

            // Leer el contenido del archivo y actualizar el high score
            string highScoreText = File.ReadAllText("SnakeHighScore.txt");
            int.TryParse(highScoreText, out highScore);
        }
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}
