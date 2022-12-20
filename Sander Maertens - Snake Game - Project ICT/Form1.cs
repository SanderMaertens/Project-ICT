using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Snakes_Spel;
using System.IO.Ports;
using System.Data.SqlClient;
using System.Web.Services.Description;

namespace WindowsFormsApp10
{

    public partial class Form1 : Form
    {

        private List<Cirkel> Snake = new List<Cirkel>();
        private Cirkel eten = new Cirkel();

        int maxWidth;
        int maxHeight;

        int score;
        int highScore;

        Random rand = new Random();

        bool goLeft, goRight, goDown, goUp;


        public Form1()
        {
            InitializeComponent();
            new Instellingen();
        }
        static SerialPort port = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
        private SerialDataReceivedEventHandler port_datarecieved;
        
        //port.Handshake = Handshake.None;
        //    po.ReadTimeout = 5000;
        //    _serialPort.Open();
        //    _continue = true;
        //    _serialPort.DataReceived += SerialPort_DataReceived;


        void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine(port.ReadLine());
        }
        private void infinteClass()
        {
            for (int a = 0; a < 50; a++)
            {
                port.DataReceived += SerialPort_DataReceived;
            }
        }

        //private void Login_load(object sender, EventArgs e)
        //{
        //    port.RtsEnable = true;
        //    port.DtrEnable = true;
        //    port.DataReceived += new SerialDataReceivedEventHandler(port_datarecieved);

        //}
        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            //new SerialPort();
            //SerialPort port = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);


            if (e.KeyCode == Keys.Left && Instellingen.directions != "right")
            {
                goLeft = true;
            }
            if (e.KeyCode == Keys.Right && Instellingen.directions != "left")
            {
                goRight = true;
            }
            if (e.KeyCode == Keys.Up && Instellingen.directions != "down")
            {
                goUp = true;
            }
            if (e.KeyCode == Keys.Down && Instellingen.directions != "up")
            {
                goDown = true;
            }



        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }
            if (e.KeyCode == Keys.Up)
            {
                goUp = false;
            }
            if (e.KeyCode == Keys.Down)
            {
                goDown = false;
            }
        }

        private void StartGame(object sender, EventArgs e)
        {
            RestartGame();
            infinteClass();
        }

        private void TakeSnapShot(object sender, EventArgs e)
        {
            Label caption = new Label();
            caption.Text = "Ik Scoorde: " + score + " en mijn High Score is " + highScore + " op de Snake Game!";
            caption.Font = new Font("Ariel", 12, FontStyle.Bold);
            caption.ForeColor = Color.OrangeRed;
            caption.AutoSize = false;
            caption.Width = picCanvas.Width;
            caption.Height = 30;
            caption.TextAlign = ContentAlignment.MiddleCenter;
            picCanvas.Controls.Add(caption);

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = "";
            dialog.DefaultExt = "jpg";
            dialog.Filter = " | *.jpg";
            dialog.ValidateNames = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                int width = Convert.ToInt32(picCanvas.Width);
                int height = Convert.ToInt32(picCanvas.Height);
                Bitmap bmp = new Bitmap(width, height);
                picCanvas.DrawToBitmap(bmp, new Rectangle(0, 0, width, height));
                bmp.Save(dialog.FileName, ImageFormat.Jpeg);
                picCanvas.Controls.Remove(caption);
            }
        }

        private void GameTimerEvent(object sender, EventArgs e)
        {

            if (goLeft)
            {
                Instellingen.directions = "left";
            }
            if (goRight)
            {
                Instellingen.directions = "right";
            }
            if (goDown)
            {
                Instellingen.directions = "down";
            }
            if (goUp)
            {
                Instellingen.directions = "up";
            }

            for (int i = Snake.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {

                    switch (Instellingen.directions)
                    {
                        case "left":
                            Snake[i].X--;
                            break;
                        case "right":
                            Snake[i].X++;
                            break;
                        case "down":
                            Snake[i].Y++;
                            break;
                        case "up":
                            Snake[i].Y--;
                            break;
                    }

                    if (Snake[i].X < 0)
                    {
                        Snake[i].X = maxWidth;
                    }
                    if (Snake[i].X > maxWidth)
                    {
                        Snake[i].X = 0;
                    }
                    if (Snake[i].Y < 0)
                    {
                        Snake[i].Y = maxHeight;
                    }
                    if (Snake[i].Y > maxHeight)
                    {
                        Snake[i].Y = 0;
                    }


                    if (Snake[i].X == eten.X && Snake[i].Y == eten.Y)
                    {
                        EatFood();
                    }

                    for (int j = 1; j < Snake.Count; j++)
                    {

                        if (Snake[i].X == Snake[j].X && Snake[i].Y == Snake[j].Y)
                        {
                            GameOver();
                        }

                    }


                }
                else
                {
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                }
            }


            picCanvas.Invalidate();

        }

        private void UpdatePictureBoxGraphics(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            Brush snakeColour;

            for (int i = 0; i < Snake.Count; i++)
            {
                if (i == 0)
                {
                    snakeColour = Brushes.Black;
                }
                else
                {
                    snakeColour = Brushes.MidnightBlue;
                }

                canvas.FillEllipse(snakeColour, new Rectangle
                    (
                    Snake[i].X * Instellingen.Width,
                    Snake[i].Y * Instellingen.Height,
                    Instellingen.Width, Instellingen.Height
                    ));
            }


            canvas.FillEllipse(Brushes.DarkRed, new Rectangle
            (
            eten.X * Instellingen.Width,
            eten.Y * Instellingen.Height,
            Instellingen.Width, Instellingen.Height
            ));
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void RestartGame()
        {
            maxWidth = picCanvas.Width / Instellingen.Width - 1;
            maxHeight = picCanvas.Height / Instellingen.Height - 1;

            Snake.Clear();

            startButton.Enabled = false;
            snapButton.Enabled = false;
            score = 0;
            txtScore.Text = "Score: " + score;

            Cirkel head = new Cirkel { X = 10, Y = 5 };
            Snake.Add(head);

            for (int i = 0; i < 10; i++)
            {
                Cirkel body = new Cirkel();
                Snake.Add(body);
            }

            eten = new Cirkel { X = rand.Next(2, maxWidth), Y = rand.Next(2, maxHeight) };

            gameTimer.Start();

        }

        private void EatFood()
        {
            score += 1;

            txtScore.Text = "Score: " + score;

            Cirkel body = new Cirkel
            {
                X = Snake[Snake.Count - 1].X,
                Y = Snake[Snake.Count - 1].Y
            };

            Snake.Add(body);

            eten = new Cirkel { X = rand.Next(2, maxWidth), Y = rand.Next(2, maxHeight) };


        }

        private void GameOver()
        {
            gameTimer.Stop();
            startButton.Enabled = true;
            snapButton.Enabled = true;

            if (score > highScore)
            {
                highScore = score;

                txtHighScore.Text = "High Score: " + Environment.NewLine + highScore;
                txtHighScore.ForeColor = Color.Maroon;
                txtHighScore.TextAlign = ContentAlignment.MiddleCenter;
            }
        }


    }
}
