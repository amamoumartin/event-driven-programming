using Game.SnakeGame.SnakeGameModel;
using Game.SnakeGame.SnakeGamePersistence;
using System.IO;
namespace Game.SnakeGame.View
{
    public partial class GameForm : Form
    {

        private Button[,] buttons = null!;
        private SnakeGameModel.SnakeGameModel model = null!;
        private System.Windows.Forms.Timer timer = null!;
        public GameForm()
        {
            InitializeComponent();

            model = new SnakeGameModel.SnakeGameModel(new DataAccess());
            
            this.KeyPreview = true;
            this.KeyUp += new KeyEventHandler(KeyboardPressed);

            model.DataToDraw += new EventHandler<GameEventArgs>(DrawOnTable);
            model.ScoreWriter += new EventHandler<GameEventArgs>(DrawScore);
            model.OnGameOver += new EventHandler<GameEventArgs>(GameOver);
            model.SpeedChange += new EventHandler<GameEventArgs>(SetTimerIntervalLower);

            timer = new System.Windows.Forms.Timer();
            timer.Tick += new EventHandler(MoveSnake);
            timer.Enabled = false;

            MakeTable(15);
        }
        //tábla létrehozása
        private void MakeTable(int size)
        {
            if (buttons != null) RemoveTable();
            buttons = new Button[size, size];
            for(int i = 0; i < size; i++)
            {
                for(int j = 0; j < size; j++)
                {
                    buttons[i, j] = new Button();
                    buttons[i, j].Location = new System.Drawing.Point(i * 30,30+j*30);
                    buttons[i, j].Width = 30;
                    buttons[i, j].Height = 30;
                    buttons[i, j].FlatStyle = FlatStyle.Popup;
                    buttons[i, j].BackColor = Color.LightGray;
                    buttons[i, j].Enabled = false;
                    Controls.Add(buttons[i, j]);
                }
            }
            model.StartNewGame(size);
        }
        //tábla eltávolítása
        private void RemoveTable()
        {
            int size = model.TableSize;
            for(int i = 0; i < size; i++)
            {
                for(int j=0;j<size;j++)
                {
                    Controls.Remove(buttons[i, j]);
                }
            }
        }
        //szinezés
        private void DrawOnTable(object? sender, GameEventArgs e)
        {
            buttons[e.x, e.y].BackColor = e.color;
        }
        //játék vége
        private void GameOver(object? sender, GameEventArgs e)
        {
            timer.Enabled = false;
            MessageBox.Show($"{e.gameovertext}\nNumber of eggs eaten: {e.score}");
            scoreMenuItem.Text = "Score: 0";
            MakeTable(model.TableSize);
        }
        //kígyó mozgatása a modellben
        private void MoveSnake(object? sender, EventArgs e)
        {
            model.Move();
        }
        //pont kiirása
        private void DrawScore(object? sender, GameEventArgs e)
        {
            scoreMenuItem.Text = $"Score: {e.score}";
        }
        //billentyűkezelés
        private void KeyboardPressed(object? sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Down || e.KeyCode == Keys.S)
            {
                model.SetDirection(Direction.Down);
            } 
            else if (e.KeyCode == Keys.Up || e.KeyCode == Keys.W)
            {
                model.SetDirection(Direction.Up);
            } 
            else if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A)
            {
                model.SetDirection(Direction.Left);
            } 
            else if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D)
            {
                model.SetDirection(Direction.Right);
            } 
            else if (e.KeyCode == Keys.Enter)
            {
                ToggleTimerState();
                model.ToggleGameState(timer.Enabled);
            }
            e.SuppressKeyPress = true;
        }
        //szünet
        private void ToggleTimerState()
        {
            if (timer.Enabled) timer.Enabled = false;
            else timer.Enabled = true;
        }

        private void x10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeTable(10);
        }

        private void x15ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeTable(15);
        }

        private void x20ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeTable(20);
        }

        private void ℹToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            MessageBox.Show("The Game can be started and paused by hitting ENTER, the direction of the snake can be changed using the arrow keys.");
        }

        private void fájlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }
        //sebesség változtatása a pont alapján
        private void SetTimerIntervalLower(object? sender, GameEventArgs e)
        {
            
            timer.Interval = e.score;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to quit?","Exit",MessageBoxButtons.YesNo);
            if(dialogResult == DialogResult.Yes)
            {
                this.Close();
            }
            else

            {
                timer.Enabled = true;
            }
        }

        private void GameForm_Load(object sender, EventArgs e)
        {

        }
    }
}