using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using SnakeModel.Persistence;
using Microsoft.VisualBasic.FileIO;
using System.Windows.Media;

namespace SnakeModel.Model
{
    public class SnakeGameModel
    {
        public int TableSize { get; set; }
        public Field[,] Table = null!;
        public Direction Direction { get; set; }
        public int Score { get; set; }
        public bool State { get; set; }
        public bool Alive { get; set; }
        
        private IDataAccess DataAccess;

        private Point? Egg = new Point();
        public List<Point>? Snake = new List<Point>();
        private bool ChangedDirection;

        public event EventHandler<GameEventArgs>? DataToDraw;
        public event EventHandler<GameEventArgs>? OnGameOver;
        public event EventHandler<GameEventArgs>? ScoreWriter;
        public SnakeGameModel(IDataAccess dataAccess)
        {
            DataAccess = dataAccess;
        }
        //új játék
        public void StartNewGame(int size)
        {
            ChangedDirection = false;
            TableSize = size;
            Table = new Field[TableSize, TableSize];
            Score = 0;
            State = false;
            Alive = true;
            Direction = Direction.Left;
            if (DataAccess == null)
            {
                GenerateTable();
            }
            else
            {
                switch (size)
                {
                    case 10:
                        ParseDataAccess(DataAccess.LoadFile("10x10.txt"));
                        break;
                    case 15:
                        ParseDataAccess(DataAccess.LoadFile("15x15.txt"));
                        break;
                    case 20:
                        ParseDataAccess(DataAccess.LoadFile("20x20.txt"));
                        break;
                    default:
                        break;
                }
            }
        }
        //inputfajl feldolgozasa
        private void ParseDataAccess(string[] readData)
        {
            Snake!.Clear();
            for (int i = 0; i < TableSize; i++)
            {
                string row = readData[i];
                for (int j = 0; j < TableSize; j++)
                {
                    char column = row[j];
                    switch (Int32.Parse(column.ToString()))
                    {
                        case 0:
                            //SetField(i, j, Field.Empty);
                            Table[i, j] = Field.Empty;
                            break;
                        case 1:
                            //SetField(i, j, Field.Body);
                            Table[i, j] = Field.Body;
                            Snake.Add(new Point { X = i, Y = j });
                            break;
                        case 2:
                            //SetField(i, j, Field.Head);
                            Table[i, j] = Field.Head;
                            Snake.Add(new Point { X = i, Y = j });
                            break;
                        case 3:
                            //SetField(i, j, Field.Wall);
                            Table[i, j] = Field.Wall;
                            break;
                        case 4:
                            //SetField(i, j, Field.Egg);
                            Table[i, j] = Field.Egg;
                            Egg!.X = i;
                            Egg!.Y = j;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        //tábla-kígyó-tojás-falak létrehozása
        private void GenerateTable()
        {
            NewTable();
            NewSnake();
            NewEgg();
            NewWalls();
        }
        //tábla készítése
        private void NewTable()
        {
            for (int i = 0; i < TableSize; i++)
            {
                for (int j = 0; j < TableSize; j++)
                {
                    //SetField(i, j, Field.Empty);
                    Table[i, j] = Field.Empty;
                }
            }
        }
        //mező beállítása + színezés ennek megfelelően
        public void SetField(int x, int y, Field fieldType)
        {
            Table[x, y] = fieldType;
            switch (fieldType)
            {
                case Field.Egg:
                    DataToDraw?.Invoke(this, new GameEventArgs(x, y, Brushes.Yellow));
                    break;
                case Field.Wall:
                    DataToDraw?.Invoke(this, new GameEventArgs(x, y, Brushes.Black));
                    break;
                case Field.Head:
                    DataToDraw?.Invoke(this, new GameEventArgs(x, y, Brushes.DarkGreen));
                    break;
                case Field.Body:
                    DataToDraw?.Invoke(this, new GameEventArgs(x, y, Brushes.LightGreen));
                    break;
                default:
                    DataToDraw?.Invoke(this, new GameEventArgs(x, y, Brushes.LightGray));
                    break;
            }
            
        }

        //kígyó mozgatása
        public void Move()
        {
            ChangedDirection = false;
            if (!State) return;
            MoveBody();
            switch (Direction)
            {
                case Direction.Up:
                    Snake![0].Y--;
                    break;
                case Direction.Down:
                    Snake![0].Y++;
                    break;
                case Direction.Left:
                    Snake![0].X--;
                    break;
                case Direction.Right:
                    Snake![0].X++;
                    break;
            }

            if (SnakeHitsSnake() || Snake![0].X < 0
                || Snake[0].X >= TableSize ||
                 Snake[0].Y < 0 || Snake[0].Y >= TableSize)
            {
                Die("You lost! Your snake collided!");
            }
            else if (Table[Snake[0].X, Snake[0].Y] == Field.Wall)
            {
                Die("You lost! Your snake collided with a wall!");
            }
            else
            {
                SetField(Snake[0].X, Snake[0].Y, Field.Head);
                if (Snake[0].X == Egg!.X && Snake[0].Y == Egg.Y)
                {
                    Eat();
                }
            }
        }
        //a kígyó testének mozgatása
        public void MoveBody()
        {
            for (int i = Snake!.Count - 1; i > 0; i--)
            {
                if (i == Snake.Count - 1)
                {
                    SetField(Snake[i].X, Snake[i].Y, Field.Empty);
                }
                if (Snake[i - 1].X < 0
                || Snake[i - 1].X > TableSize ||
                 Snake[i - 1].Y < 0 || Snake[i - 1].Y > TableSize)
                {
                    Die("You lost! Your snake collided!");
                }
                Snake[i].X = Snake[i - 1].X;
                Snake[i].Y = Snake[i - 1].Y;
                SetField(Snake[i].X, Snake[i].Y, Field.Body);
            }
        }
        //ütközik-e a kígyó magával
        private bool SnakeHitsSnake()
        {
            int i = 1;
            bool eatsItself = false;
            while (i < Snake!.Count && !eatsItself)
            {
                if (Snake[i].X == Snake[0].X && Snake[i].Y == Snake[0].Y)
                {
                    eatsItself = true;
                }
                ++i;
            }
            return eatsItself;
        }
        //pályán belüli-e a mező,ha igen akkor üres-e?
        private bool CanBePlaced(int x, int y)
        {
            if (x < 0 || y < 0) { return false; }
            if (x >= TableSize || y >= TableSize) { return false; }
            return Table[x, y] == Field.Empty;
        }

        //tojás készítése
        private void NewEgg()
        {
            Random rnd = new Random();
            int rndXTest = rnd.Next(0, TableSize);
            int rndYTest = rnd.Next(0, TableSize);
            while (!CanBePlaced(rndXTest, rndYTest))
            {
                rndXTest = rnd.Next(0, TableSize);
                rndYTest = rnd.Next(0, TableSize);
            }
            Egg = new Point { X = rndXTest, Y = rndYTest };
            SetField(Egg.X, Egg.Y, Field.Egg);
        }
        //kígyó létrehozása
        private void NewSnake()
        {
            Snake!.Clear();
            Point head = new Point { X = TableSize / 2, Y = TableSize / 2 };
            Snake.Add(head);
            SetField(head.X, head.Y, Field.Head);
            for (int i = 0; i < 4; i++)
            {
                Point body = new Point { X = Snake[i].X, Y = Snake[i].Y+1 };
                Snake.Add(body);
                SetField(body.X, body.Y, Field.Body);
            }
        }
        //falak elhelyezése
        private void NewWalls()
        {
            int wallcount = GetWallNumber();
            int c = 0;
            while (c < wallcount)
            {
                Random rnd = new Random();
                int rndXTest = rnd.Next(0, TableSize);
                int rndYTest = rnd.Next(0, TableSize);
                while (!CanBePlaced(rndXTest, rndYTest) || rndXTest == TableSize / 2)
                {
                    rndXTest = rnd.Next(0, TableSize);
                    rndYTest = rnd.Next(0, TableSize);
                }
                SetField(rndXTest, rndYTest, Field.Wall);
                c++;
            }
        }
        //tojás elfogyasztása
        private void Eat()
        {
            Point body = new Point { X = Snake![Snake.Count - 1].X, Y = Snake[Snake.Count - 1].Y };
            Snake.Add(body);
            SetField(body.X, body.Y, Field.Body);
            ++Score;
            NewEgg();
            ScoreWriter?.Invoke(this, new GameEventArgs(Score));
        }
        //meghal a kígyó
        private void Die(string text)
        {
            Alive = false;
            State = false;
            OnGameOver?.Invoke(this, new GameEventArgs(Score, text));
        }
        //játék megállítása
        public void ToggleGameState(bool state)
        {
            State = state;
        }
        //irány beállítása
        public void SetDirection(Direction d)
        {
            if (ChangedDirection) return;
            if (!State) return;
            if (Direction == Direction.Up && d == Direction.Down) return;
            if (Direction == Direction.Down && d == Direction.Up) return;
            if (Direction == Direction.Left && d == Direction.Right) return;
            if (Direction == Direction.Right && d == Direction.Left) return;
            Direction = d;
            ChangedDirection = true;
        }
        //falak,akadályok száma
        private int GetWallNumber()
        {
            return (TableSize switch
            {
                10 => 6,
                15 => 12,
                20 => 18,
                _ => 6
            });
        }
    }
}
