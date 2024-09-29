using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;


namespace Maui.app1
{
    internal class TextFiles
    {
        public async Task WriteTextToFile(string text, string fileName)
        {
            string filePath = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);
            using (FileStream outputStream = File.OpenWrite(filePath))
            using (StreamWriter streamWriter = new StreamWriter(outputStream))
            {
                await streamWriter.WriteAsync(text);
            }
        }

        public async Task<string> ReadTextFromFile(string fileName)
        {
            string filePath = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);
            using (FileStream inputStream = File.OpenRead(filePath))
            using (StreamReader reader = new StreamReader(inputStream))
            {
                return await reader.ReadToEndAsync();
            }
        }


    }
}
