using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Game.SnakeGame.SnakeGamePersistence
{
    public class DataAccess : IDataAccess
    {

        public string[] LoadFile(string path)
        {
            return File.ReadAllLines(Environment.CurrentDirectory + "../../../../../SnakeGamePersistence/" + path);
        }
    }
    
}
