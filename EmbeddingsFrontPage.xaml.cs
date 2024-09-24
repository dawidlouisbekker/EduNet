
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using static Maui.app1.BackgroundModelLoad;
using static System.Net.Mime.MediaTypeNames;

namespace Maui.app1;

public partial class EmbeddingsFrontPage : ContentPage
{
    FilesmodelView.MainViewModel BindingContext { get; set; }

    public EmbeddingsFrontPage()
	{
		InitializeComponent();
        BindingContext = FilesmodelView.MainViewModel.Instance;
        FileCollection.ItemsSource = BindingContext.PickedFiles;

    }

    List<string> paths = new List<string>();


    private async void OpenFiles(object sender, EventArgs e)
    {
        //await FilesmodelView.MainViewModel.Instance.PickFilesAsync();
        await BindingContext.PickFilesAsync();
    }

    public ObservableCollection<PickedFile> PickedFiles { get; set; } = new ObservableCollection<PickedFile>();
    

    private void ImageButton_Clicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is PickedFile pickedFile)
        {
            var viewModel = BindingContext as FilesmodelView.MainViewModel;
            viewModel?.PickedFiles.Remove(pickedFile);
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

    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        btnProcess.Background = Colors.Purple;
        BindingContext.SendPickedFiles();
        HttpClient _httpClient = new HttpClient();
        var url = "http://127.0.0.1:8060/collection-name";
        var content = new StringContent(colecName.Text, Encoding.UTF8, "text/plain");
        var response = await _httpClient.PostAsync(url, content);
        btnProcess.BackgroundColor = Colors.Blue;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new SharedProcessPage());
    }
}