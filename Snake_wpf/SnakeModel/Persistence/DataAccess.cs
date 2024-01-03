using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeModel.Persistence
{
    public class DataAccess : IDataAccess
    {
        public string[] LoadFile(string path)
        {
            return File.ReadAllLines(Environment.CurrentDirectory + "../../../../../SnakeModel/Persistence/" + path);
        }
    }
}
