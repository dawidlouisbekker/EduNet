using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static Maui.app1.BackgroundModelLoad;

/* Unmerged change from project 'Maui.app1 (net8.0-windows10.0.19041.0)'
Added:
using Maui;
using Maui.app1;
using Maui.app1;
using Maui.app1.ViewModel;
*/

namespace Maui.app1
{
    internal class FilesmodelView
    {
        public class MainViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<PickedFile> PickedFiles { get; set; } = new ObservableCollection<PickedFile>();
            public ICommand DeleteItemCommand { get; }

            private static MainViewModel? _instance;
            public static MainViewModel Instance => _instance ??= new MainViewModel();

            List<string> paths = new List<string>();

            public void OnDeleteItem(PickedFile PickedFile)
            {
                if (PickedFiles.Contains(PickedFile))
                {
                    PickedFiles.Remove(PickedFile);
                }
            }


            public async Task PickFilesAsync()
            {
                var options = new PickOptions
                {
                    PickerTitle = "Please select PDF files",
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { "com.adobe.pdf" } }, 
                { DevicePlatform.Android, new[] { "application/pdf" } }, 
                { DevicePlatform.WinUI, new[] { ".pdf" } }, 
                { DevicePlatform.MacCatalyst, new[] { "com.adobe.pdf" } } 
            })
                };
                
                var result = await FilePicker.PickMultipleAsync(options);
                if (result != null)
                {
                    foreach (var file in result)
                    {
                        PickedFiles.Add(new PickedFile { FileName = file.FileName, FilePath = file.FullPath });
                        paths.Append(file.FullPath);
                    }
                }
            }

            public async void SendPickedFiles()
            {
                HttpClient _httpClient = new HttpClient();
                var url = "http://127.0.0.1:8060/local-embed";

                foreach (var pickedFile in PickedFiles)
                {
                    if (!string.IsNullOrEmpty(pickedFile.FilePath))
                    {
                        var contents = new StringContent(pickedFile.FilePath, Encoding.UTF8, "text/plain");
                        try
                        {
                            var response = await _httpClient.PostAsync(url, contents);
                            if (response.IsSuccessStatusCode)
                            {
                                Console.WriteLine("Success: File path sent successfully!");
                            }
                            else
                            {
                                Console.WriteLine($"Error: Failed to send file path. Status code: {response.StatusCode}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Exception: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: File path is null or empty.");
                    }
                }
              var content = new StringContent("done", Encoding.UTF8, "text/plain");
            }


            public event PropertyChangedEventHandler PropertyChanged;
        }

    }
    public class PickedFile
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }

}
