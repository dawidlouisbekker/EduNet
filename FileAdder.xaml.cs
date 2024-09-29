using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Maui.Storage;

namespace Maui.app1;

public partial class FileAdder : ContentPage
{
    public FileAdder()
	{
		InitializeComponent();
        BindingContext = FilesmodelView.MainViewModel.Instance;
        FileCollection.ItemsSource = FilesmodelView.MainViewModel.Instance.PickedFiles;


    }
    List<string> paths = new List<string>();
    //public ObservableCollection<PickedFile> PickedFiles { get; set; } = new ObservableCollection<PickedFile>();



    private async void OpenFiles(object sender, EventArgs e)
    {
        await FilesmodelView.MainViewModel.Instance.PickFilesAsync();
      //  await BindingContext.PickFilesAsync();
    }



    private void ImageButton_Clicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is PickedFile pickedFile)
        {   
            var viewModel = BindingContext as FilesmodelView.MainViewModel;
            viewModel?.PickedFiles.Remove(pickedFile);
        }
        else
        {
            DisplayAlert("", "", "");
        }

    }

    private readonly HttpClient _httpClient;

    public void ApiClient()
    {
        HttpClient _httpClient = new HttpClient();
    }

    public async Task<HttpResponseMessage> SendItemsAsync(IEnumerable<PickedFile> pickedFiles)
    {
        var url = "http://127.0.0.1/8060/local-embed";
        var response = await _httpClient.PostAsJsonAsync(url, pickedFiles);
        return response;
    }

    private async void JustProcess(object sender, EventArgs e)
    {
        //  btnProcess.Background = Colors.Purple;
        FilesmodelView.MainViewModel.Instance.SendPickedFiles();
       // btnProcess.BackgroundColor = Colors.Blue;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new SharedProcessPage());
    }
}

