using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeModel.Persistence
{
    public interface IStore
    {
        Task<IEnumerable<String>> GetFilesAsync();

        Task<DateTime> GetSaveTimeAsync(String path);
    }
}
