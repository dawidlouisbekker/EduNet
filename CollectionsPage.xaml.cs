
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using static Maui.app1.BackgroundModelLoad;
using static System.Net.Mime.MediaTypeNames;
using CommunityToolkit.Maui.Storage;
using System.Threading;

namespace Maui.app1;

public partial class EmbeddingsFrontPage : ContentPage
{
    FilesmodelView.MainViewModel BindingContext { get; set; }
    public EmbeddingsFrontPage()
	{
		InitializeComponent();
        BindingContext = FilesmodelView.MainViewModel.Instance;
        CollectionsList.ItemsSource = BindingContext.Collections;
    }
    

    List<string> paths = new List<string>();

    private async void OpenFiles(object sender, EventArgs e)
    {
        //await FilesmodelView.MainViewModel.Instance.PickFilesAsync();
        await BindingContext.PickFilesAsync();
    }
    

    private async void ImageButton_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Question?", "Would you like to proceed?", "Yes", "No");
        if (answer)
        {
            if (sender is ImageButton button && button.BindingContext is Collection collection)
            {
                var viewModel = BindingContext as FilesmodelView.MainViewModel;
                viewModel?.Collections.Remove(collection);
            }
        }
        else
        {
            // User pressed No
            //await DisplayAlert("Response", "You chose No", "OK");
        }
        

    }




    public async Task<HttpResponseMessage> SendItemsAsync(IEnumerable<PickedFile> pickedFiles)
    {
        HttpClient _httpClient = new HttpClient();
        var url = "http://127.0.0.1/8060/local-embed";
        var response = await _httpClient.PostAsJsonAsync(url, pickedFiles);
        return response;
    }



    private void AddFiles(object sender, EventArgs e)
    {
        Navigation.PushAsync(new FileAdder());
    }

    private async void CreateCollection (object sender, EventArgs e) 
    {
      await Navigation.PushAsync(new CreateCollectionPage());
    }

    private void ItemClicked(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is PickedFile selectedFile)
        {
            foreach (var cell in CollectionsList.TemplatedItems)
            {
                var viewCell = cell as ViewCell;
                var frame = viewCell?.View.FindByName<Frame>("ItemFrame");
                if (frame != null)
                {
                    frame.BackgroundColor = frame.BindingContext == selectedFile ? Colors.LightGray : Colors.White;
                }
            }
        }
    }
}   