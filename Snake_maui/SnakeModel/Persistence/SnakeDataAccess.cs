using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using SnakeModel.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace SnakeModel.Persistence
{
    public class SnakeDataAccess : ISnakeDataAccess
    {
        

        private String? _basePath = String.Empty;

        public SnakeDataAccess(String? basePath = null)
        {
            _basePath = basePath;
        }
        
        public async Task<Field[,]> LoadAsync(String path)
        {
            if (!String.IsNullOrEmpty(_basePath))
                path = Path.Combine(_basePath, path);

            try
            {
                using(StreamReader reader = new StreamReader(path))
                {
                    String line = await reader.ReadLineAsync() ?? String.Empty;
                    String[] columns;
                    Int32 tableSize = Int32.Parse(line);
                    Field[,] table = new Field[tableSize, tableSize];

                    for(int i = 0; i < tableSize; i++)
                    {
                        line = await reader.ReadLineAsync() ?? String.Empty;
                        columns = line.Split(' ');
                        for (int j = 0; j < tableSize; j++)
                        {
                            switch (Int32.Parse(columns[j]))
                            {
                                case 0:
                                    table[i, j] = Field.Empty;
                                    break;
                                case 1:
                                    table[i, j] = Field.Body;
                                    break;
                                case 2:
                                    table[i, j] = Field.Head;
                                    break;
                                case 3:
                                    table[i, j] = Field.Wall;
                                    break;
                                case 4:
                                    table[i, j] = Field.Egg;
                                    break;
                                default:
                                    throw new Exception("Invalid field type");
                            }
                        }

                    }

                    return table;
                }
            }
            catch
            {
                throw new SnakeDataException();
            }
        }

        public async Task SaveAsync(String path, Field[,] fields)
        {
            if (!String.IsNullOrEmpty(_basePath))
                path = Path.Combine(_basePath, path);


            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    Int32 tableSize = fields.GetLength(0);
                    await writer.WriteLineAsync(tableSize.ToString());
                    for (int i = 0; i < tableSize; i++)
                    {
                        for (int j = 0; j < tableSize; j++)
                        {
                            switch (fields[i, j])
                            {
                                case Field.Empty:
                                    await writer.WriteAsync("0 ");
                                    break;
                                case Field.Body:
                                    await writer.WriteAsync("1 ");
                                    break;
                                case Field.Head:
                                    await writer.WriteAsync("2 ");
                                    break;
                                case Field.Wall:
                                    await writer.WriteAsync("3 ");
                                    break;
                                case Field.Egg:
                                    await writer.WriteAsync("4 ");
                                    break;
                                default:
                                    throw new Exception("Invalid field type");
                            }
                        }
                        await writer.WriteLineAsync();
                    }
                }
            }
            catch
            {
                throw new SnakeDataException();
            }
        }

        

    }
}
