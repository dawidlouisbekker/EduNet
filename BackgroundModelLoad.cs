using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Maui.app1
{
    internal class BackgroundModelLoad
    {
        public class StringDataStore
        {
            // The List to hold strings
            public List<string> Data { get; private set; } = new List<string>();

            public ObservableCollection<Item> Items { get; private set; }

            // Singleton instance
            private static StringDataStore? _instance;

            // Constructor is private to prevent instantiation
            private StringDataStore()
            {
              //  Items = new ObservableCollection<Item>();
              //     for (int i = 0; i < Data.Count; i++) 
              //      {

              //       Items.Add(new Item { Name = Data[i] });

              //      }
               
            } 
        
                

     

        // Public method to get the singleton instance
            public static StringDataStore Instance => _instance ??= new StringDataStore();

            // Method to populate the string list (you can call this on app start)
            public async Task PopulateDataAsync()
            {
                Items = new ObservableCollection<Item>();
                // Simulate background data loading
                await Task.Run(() =>
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = $"/C ollama list",
                            UseShellExecute = false, // Must be false to redirect output
                            RedirectStandardOutput = true, // Redirect the standard output (stdout)
                            RedirectStandardError = false, // Redirect the standard error (stderr)
                            CreateNoWindow = true, // Run without creating a console window
                        }
                    };

                    // Start the process
                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();


                    // Loop to generate buttons dynamically
                    using (StringReader reader = new StringReader(output))
                    {
                        string line;
                        int num = 0;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (num > 1)
                            {
                                Data.Add(line);
                                Items.Add(new Item { Name=line});
                            }
                            num++;
                        }
                    };

                    


                });
        }


        }
        public class Item
        {
            public string? Name { get; set; }
        };
    }
}
