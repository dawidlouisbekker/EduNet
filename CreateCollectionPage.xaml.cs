using Maui.app1.ViewModel;
using System.Text;

namespace Maui.app1;

public partial class CreateCollectionPage : ContentPage
{
	public CreateCollectionPage()
	{
		InitializeComponent();
	}
	private async void ChooseFolder(object sender, EventArgs e)
	{
		await FilesmodelView.MainViewModel.Instance.PickFolder();
		FolderPath.Text = FilesmodelView.MainViewModel.Instance.FolderPath;
    }
	private async void CreateCollection(object sender, EventArgs e)
	{
		if (colecName.Text == null || colecName.Text == " ")
		{
			await DisplayAlert("Error", "Please Enter a Collection Name", "Ok");
		}
		else
		{
			try
			{
				//HttpClient client = new HttpClient();
				//var url = "http://localhost:8060/collection-name";
				// Prepare the string content
				//var content = new StringContent(colecName.Text, Encoding.UTF8, "text/plain");

				// Post the request
				//var response = await client.PostAsync(url, content);
				FilesmodelView.MainViewModel.Instance.ListPdfsInDirectory(FolderPath.Text);
				await Navigation.PushAsync(new FileAdder());
            }
			catch (Exception ex)
			{
				await DisplayAlert("Oops", ex.Message, "Ok");
			}
		}
	
	}
}