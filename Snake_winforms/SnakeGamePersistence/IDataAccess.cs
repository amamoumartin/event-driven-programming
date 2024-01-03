using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Game.SnakeGame.SnakeGamePersistence
{
    public interface IDataAccess
    {
        
        public string[] LoadFile(string path);
    }
}
