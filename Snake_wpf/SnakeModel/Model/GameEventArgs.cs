using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SnakeModel.Model
{
    public class GameEventArgs
    {
        public int x;
        public int y;
        public Brush? color;

        public int score;
        public string? gameovertext;

        public string? text;

        public GameEventArgs(int x, int y, Brush color)
        {
            this.x = x;
            this.y = y;
            this.color = color;
        }
        public GameEventArgs(int score, string? gameovertext)
        {
            this.score = score;
            this.gameovertext = gameovertext;
        }

        public GameEventArgs(int score)
        {
            this.score = score;
        }

        public GameEventArgs(int x, int y, string text)
        {
            this.x = x;
            this.y = y;
            this.text = text;
        }
    }
}
