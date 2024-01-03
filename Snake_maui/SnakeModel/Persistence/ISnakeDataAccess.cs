using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SnakeModel.Model;

namespace SnakeModel.Persistence
{
    public interface ISnakeDataAccess
    {
        //public string[] LoadFile(string path);
        
        Task<Field[,]> LoadAsync(String path);

        Task SaveAsync(String path, Field[,] fields);


    }
}
