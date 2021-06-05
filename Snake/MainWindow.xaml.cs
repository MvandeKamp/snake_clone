using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Snake
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //0 = empty field, 1 = fruit, 2 = snake body, 3 = snake head
        byte[,] gameBoard = new byte[40,40];
        int snakeDirection;
        public Vector snakeHeadPostion;
        public Vector lastSnakeHeadPostion;
        public Vector fruitPostion;
        public List<Vector> snakeBody = new List<Vector>();
        Thread thread;
        public int direction = 4;
        public List<UIElement> uIElements = new List<UIElement>();
        public MainWindow()
        {
            InitializeComponent();
            thread = new Thread(GameLoop);
            thread.Start();
            Random rnd = new Random();
            SpawnSnakeHead(20, 20, 4);
            SpawnFruit(rnd.Next(39), rnd.Next(39));
        }

        public void SpawnSnakeHead(int x, int y, int direction)
        {
            snakeDirection = direction;
            snakeHeadPostion.X = x;
            snakeHeadPostion.Y = y;
            this.snakeGame.Dispatcher.Invoke(new Action(() =>
            {
                System.Windows.Shapes.Rectangle rect;
                rect = new System.Windows.Shapes.Rectangle();
                rect.Stroke = new SolidColorBrush(Colors.Red);
                rect.Fill = new SolidColorBrush(Colors.White);
                rect.Width = 16;
                rect.Height = 16;
                rect.Uid = "a" + x + "_" + y;
                gameBoard[x, y] = 3;
                Canvas.SetLeft(rect, x * 16);
                Canvas.SetTop(rect, y * 16);
                uIElements.Add(rect);
                snakeGame.Children.Add(rect);
                System.Windows.Shapes.Rectangle leftEye;
                leftEye = new System.Windows.Shapes.Rectangle();
                leftEye.Fill = new SolidColorBrush(Colors.Blue);
                leftEye.Width = 4;
                leftEye.Height = 4;
                leftEye.Uid = "a" + x + "_" + y + "left";
                switch (direction)
                {
                    case 1:
                        Canvas.SetLeft(leftEye, (x) * 16 + 4);
                        Canvas.SetTop(leftEye, (y) * 16 + 12 - 2);
                        break;
                    case 2:
                        Canvas.SetLeft(leftEye, (x) * 16 + 4 - 2);
                        Canvas.SetTop(leftEye, (y) * 16 + 4);
                        break;
                    case 3:
                        Canvas.SetLeft(leftEye, (x) * 16 + 12 - 4);
                        Canvas.SetTop(leftEye, (y) * 16 + 4 - 2);
                        break;
                    case 4:
                        Canvas.SetLeft(leftEye, (x ) * 16 + 12 - 2);
                        Canvas.SetTop(leftEye, (y) * 16 + 12 - 2);
                        break;
                }
                uIElements.Add(leftEye);
                snakeGame.Children.Add(leftEye);
                System.Windows.Shapes.Rectangle rightEye;
                rightEye = new System.Windows.Shapes.Rectangle();
                rightEye.Fill = new SolidColorBrush(Colors.Blue);
                rightEye.Width = 4;
                rightEye.Height = 4;
                rightEye.Uid = "a" + x + "_" + y + "right";
                //yay eye rotations in the most complicated way imagineable thanks wpf for not implementing rotations of objects in a good way
                switch (direction)
                {
                    case 1:
                        Canvas.SetTop(rightEye, (y) * 16 + 4 - 2);
                        Canvas.SetLeft(rightEye, (x) * 16 + 4);
                        break;
                    case 2:
                        Canvas.SetLeft(rightEye, (x) * 16 + 12 - 2);
                        Canvas.SetTop(rightEye, (y) * 16 + 4);
                        break;
                    case 3:
                        Canvas.SetTop(rightEye, (y) * 16 + 12 - 2);
                        Canvas.SetLeft(rightEye, (x) * 16 + 12 - 4);
                        break;
                    case 4:
                        Canvas.SetLeft(rightEye, (x) * 16 + 4 - 2);
                        Canvas.SetTop(rightEye, (y) * 16 + 12 - 2);
                        break;
                }
                uIElements.Add(rightEye);
                snakeGame.Children.Add(rightEye);
            }));
        }
        public void SpawnSnakeBody(int x, int y)
        {
            this.snakeGame.Dispatcher.Invoke(new Action(() =>
            {
                System.Windows.Shapes.Rectangle rect;
                rect = new System.Windows.Shapes.Rectangle();
                rect.Fill = new SolidColorBrush(Colors.White);
                rect.Width = 16;
                rect.Height = 16;
                rect.Uid = "a" + x + "_" + y + "_" + "body";
                gameBoard[x,y] = 2;
                Canvas.SetLeft(rect, x * 16);
                Canvas.SetTop(rect, y * 16);
                uIElements.Add(rect);
                snakeGame.Children.Add(rect);
            }));
        }
        public void SpawnFruit(int x, int y)
        {
            this.snakeGame.Dispatcher.Invoke(new Action(() =>
            {
                System.Windows.Shapes.Rectangle rect;
                rect = new System.Windows.Shapes.Rectangle();
                rect.Stroke = new SolidColorBrush(Colors.Green);
                rect.Fill = new SolidColorBrush(Colors.Yellow);
                rect.Width = 8;
                rect.Height = 8;
                rect.Uid = "a" + x + "_" + y + "_" + "2";
                gameBoard[x, y] = 2;
                fruitPostion.X = x;
                fruitPostion.Y = y;
                Canvas.SetLeft(rect, x * 16 + 4);
                Canvas.SetTop(rect, y * 16 + 4);
                uIElements.Add(rect);
                snakeGame.Children.Add(rect);
                foreach (Vector v in snakeBody)
                {
                    if (v == fruitPostion)
                    {
                        DeleteTile((int)fruitPostion.X, (int)fruitPostion.Y);
                        Random rnd = new Random();
                        SpawnFruit(rnd.Next(39), rnd.Next(39));
                    }
                }
            }));
        }
        public void DeleteTile(int x, int y)
        {
            this.snakeGame.Dispatcher.Invoke(new Action(() =>
            {
                List<UIElement> removeables = new List<UIElement>();
                lock (uIElements)
                {
                    foreach (UIElement ui in uIElements)
                    {
                        if (ui.Uid == ("a" + x + "_" + y + "right"))
                        {
                            removeables.Add(ui);
                        }
                        if (ui.Uid == ("a" + x + "_" + y + "left"))
                        {
                            removeables.Add(ui);
                        }
                        if (ui.Uid == ("a" + x + "_" + y))
                        {
                            removeables.Add(ui);
                        }
                        if (ui.Uid == ("a" + x + "_" + y + "_" + "2"))
                        {
                            removeables.Add(ui);
                        }
                        if (ui.Uid == ("a" + x + "_" + y + "_" + "body") & ui.Uid != ("a" + x + "_" + y + "_" + "2"))
                        {
                            removeables.Add(ui);
                        }
                    }
                    foreach (UIElement ui in removeables)
                    {
                        snakeGame.Children.Remove(ui);
                        uIElements.Remove(ui);
                    }
                }
            }));
        }
        public void GameLoop()
        {
            Stopwatch stopWatch = new Stopwatch();
            int i = 0;
            double lastTime;
            double now;
            double FPS = 4;
            double actualFrameTime;
            double wantedFrameTime = 1000 / FPS;
            stopWatch.Start();
            while (true)
            {
                now = (double)stopWatch.Elapsed.TotalMilliseconds;
                
                lastSnakeHeadPostion = snakeHeadPostion;
                switch (direction)
                {
                    case 1:
                        DeleteTile((int)snakeHeadPostion.X, (int)snakeHeadPostion.Y);
                        snakeHeadPostion.X = snakeHeadPostion.X - 1;
                        SnakeMove();
                        SpawnSnakeHead((int)snakeHeadPostion.X, (int)snakeHeadPostion.Y, 1);
                        break;
                    case 2:
                        DeleteTile((int)snakeHeadPostion.X, (int)snakeHeadPostion.Y);
                        snakeHeadPostion.Y = snakeHeadPostion.Y - 1;
                        SnakeMove();
                        SpawnSnakeHead((int)snakeHeadPostion.X, (int)snakeHeadPostion.Y, 2);
                        break;
                    case 3:
                        DeleteTile((int)snakeHeadPostion.X, (int)snakeHeadPostion.Y);
                        snakeHeadPostion.X = snakeHeadPostion.X + 1;
                        SnakeMove();
                        SpawnSnakeHead((int)snakeHeadPostion.X, (int)snakeHeadPostion.Y, 3);
                        break;
                    case 4:
                        DeleteTile((int)snakeHeadPostion.X, (int)snakeHeadPostion.Y);
                        snakeHeadPostion.Y = snakeHeadPostion.Y + 1;
                        SnakeMove();
                        SpawnSnakeHead((int)snakeHeadPostion.X, (int)snakeHeadPostion.Y, 4);
                        break;
                }
                if(fruitPostion == snakeHeadPostion)
                {
                    DeleteTile((int)fruitPostion.X, (int)fruitPostion.Y);
                    SpawnSnakeHead((int)snakeHeadPostion.X, (int)snakeHeadPostion.Y, direction);
                    SpawnSnakeBody((int)lastSnakeHeadPostion.X, (int)lastSnakeHeadPostion.Y);
                    Vector v = new Vector();
                    v.X = lastSnakeHeadPostion.X;
                    v.Y = lastSnakeHeadPostion.Y;
                    snakeBody.Add(v);
                    Random rnd = new Random();
                    SpawnFruit(rnd.Next(39), rnd.Next(39));
                    } else {
                    if(snakeBody.Count != 0)
                    {
                        SpawnSnakeBody((int)lastSnakeHeadPostion.X, (int)lastSnakeHeadPostion.Y);
                        DeleteTile((int)snakeBody[0].X, (int)snakeBody[0].Y);
                        snakeBody.RemoveAt(0);
                        Vector v = new Vector();
                        v.X = lastSnakeHeadPostion.X;
                        v.Y = lastSnakeHeadPostion.Y;
                        snakeBody.Add(v);
                    }
                }
                foreach(Vector v in snakeBody)
                {
                    if(v == snakeHeadPostion)
                    {
                        this.snakeGame.Dispatcher.Invoke(new Action(() =>
                        {
                            snakeGame.Children.Clear();
                            snakeBody.Clear();
                            uIElements.Clear();
                            Random rnd = new Random();
                            SpawnFruit(rnd.Next(39), rnd.Next(39));
                            SpawnSnakeHead(20, 20, 4);
                            
                        }));
                        break;
                    }
                }

                //Code for calculating frame time or FPS and beeing able to 
                //speed up the game if we didnt meet the required frametime for the last frames
                lastTime = (double)stopWatch.Elapsed.TotalMilliseconds;
                actualFrameTime = wantedFrameTime - (double)(lastTime - now);
                //Console.WriteLine(actualFrameTime);
                if (actualFrameTime > 0)
                {
                    Thread.Sleep((int)actualFrameTime);
                    if (i > (FPS / 10))
                    {
                        Console.WriteLine("FPS: " + 1000 / (actualFrameTime + (double)(lastTime - now)));
                        i = 0;
                    }
                    i++;
                } else
                {
                    Console.WriteLine("FPS: " + 1000 / ((actualFrameTime * -1) + wantedFrameTime));
                }
            }
        }
        public void SnakeMove()
        {
            if(snakeHeadPostion.X == 40)
            {
                snakeHeadPostion.X = 0;
            } else if (snakeHeadPostion.Y == 40)
            {
                snakeHeadPostion.Y = 0;
            }
            else if (snakeHeadPostion.X == -1)
            {
                snakeHeadPostion.X = 39;
            }
            else if (snakeHeadPostion.Y == -1)
            {
                snakeHeadPostion.Y = 39;
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            thread.Abort();
        }

        private void kek_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W)
            {
                direction = 2;
            }
            if (e.Key == Key.A)
            {
                direction = 1;
            }
            if (e.Key == Key.D)
            {
                direction = 3;
            }
            if (e.Key == Key.S)
            {
                direction = 4;
            }
        }
    }
}
