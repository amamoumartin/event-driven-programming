using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SnakeModel.Model;
namespace SnakeModel.Persistence
{
    public class SnakeStore : IStore
    {
        public async Task<IEnumerable<String>> GetFilesAsync()
        {
            return await Task.Run(() => Directory.GetFiles(FileSystem.AppDataDirectory)
            .Select(Path.GetFileName)
            .Where(name => name?.EndsWith(".stl") ?? false)
            .OfType<String>());
        }
        public async Task<DateTime> GetSaveTimeAsync(String path)
        {
            var info = new FileInfo(Path.Combine(FileSystem.AppDataDirectory, path));

            return await Task.Run(() => info.LastWriteTime);
        }

    }
}
