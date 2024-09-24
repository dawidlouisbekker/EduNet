using System.Collections.Generic;
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui.Animations;
using System.ComponentModel;
using System.Text;
using System.Net.Http;

namespace Maui.app1
{

    public partial class ViewModelsPage : ContentPage
    {
        private readonly HttpClient _httpClient;
        public ViewModelsPage() {

            InitializeComponent();
            mylistview.ItemsSource = BackgroundModelLoad.StringDataStore.Instance.Items;
            _httpClient = new HttpClient();

        }

        private void GoToEmbeddings(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EmbeddingsFrontPage());
        }

        private OllamaClienthttp1 client { get; set; }
        private async void get_response()
        {
            SendButton.IsEnabled = false;
            loadIndicator.IsRunning = true;
            loadIndicator.IsVisible = true;
            try
            {
                // Instantiate your Http1 client
                var httpClient = new OllamaClienthttp1();

                // URL of the API or endpoint
                string url = "http://localhost:8060/"; // Replace with the actual URL

                // Make the asynchronous call to the API
                string response = await httpClient.GetAsync(url);

                // Display the response in a Label or other UI element
      
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                await DisplayAlert("Error", ex.Message, "OK");
            }
            loadIndicator.IsRunning = false;
            loadIndicator.IsVisible = false;
        }

        private async void OnCallHttp1ApiClicked()
        {
            try
            {
                // Instantiate your Http1 client
                var httpClient = new OllamaClienthttp1();

                // URL of the API or endpoint
                string url = "http://localhost:8060/"; // Replace with the actual URL

                // Make the asynchronous call to the API
                string response = await httpClient.GetAsync(url);

                // Display the response in a Label or other UI element
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private string? model;

        private async void ItemClicked(object sender, SelectedItemChangedEventArgs e)
        {
            QueryEntry.IsEnabled = true;
            QueryEntry.Placeholder = "Ask a Question";

            //if (e.SelectedItem == null)
            //     await DisplayAlert("nope", "ok", "ok");
            //    return;

            // Cast the selected item to your model (MyItem)
            var selectedItem = e.SelectedItem as BackgroundModelLoad.Item;

            if (selectedItem != null)
            {
                // Access the Name property
                string selectedName = selectedItem.Name;
                await PostModelAsync(selectedName);
                // You can now use the selectedName variable or perform further actions
            }



        }

        private void OnArrowClicked(object sender, EventArgs e)
        {

        }

        private void QueryFocused(object sender, EventArgs e)
        {
        }

        private void QueryEntry_TextChanged(object sender, TextChangedEventArgs e)
        {

            QueryEntry.VerticalOptions = LayoutOptions.Fill;
            QueryBorder.IsVisible = true;

        }

        private void QueryEntry_Unfocused(object sender, FocusEventArgs e)
        {
            if (QueryEntry.Text.Length <= 1)
            {
                QueryBorder.IsVisible = false;
                QueryEntry.VerticalOptions = LayoutOptions.End;
            }

        }
        //http://localhost:11434/api/generate
        private async Task ReceiveStreamAsync(string url, string query)
        {
            url = url + "?param1=" + query;

            using (var client = new HttpClient())
            {
                try
                {
                    using (HttpResponseMessage response = await client.GetAsync(url,HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();
                        
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        using (var reader = new System.IO.StreamReader(stream))
                        {
                            char[] buffer = new char[1024];
                            int bytesRead;

                            while ((bytesRead = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                string tokens = new string(buffer, 0, bytesRead);
                                StreamLabel.Text += tokens; 
                            }
                        }
                    }
                }
                catch (HttpRequestException e)
                {
                    DisplayAlert("Something Went Wrong", e.Message, "Okey");
                }
            
            }


            loadIndicator.IsEnabled = false;
            loadIndicator.IsRunning = false;
            loadIndicator.IsVisible = false;
            SendButton.IsEnabled = true;
            SendButton.BackgroundColor = Color.FromArgb("#2B0B98");
   
        }
 


        private void SendQuery(object sender, EventArgs e)
                {
                    string Url = "http://localhost:8060/stream";
                    string Query = QueryEntry.Text;
                    SendButton.IsEnabled = false;
                    loadIndicator.IsRunning = true;
                    loadIndicator.IsVisible = true;
                    ReceiveStreamAsync(Url, Query);


                }
                private async Task PostModelAsync(string model)
                {
                    var client = new HttpClient();

                    // FastAPI URL to send the model text
                    var url = "http://localhost:8060/model/";


                    // Prepare the string content
                    var content = new StringContent(model, Encoding.UTF8, "text/plain");

                    // Post the request
                    var response = await client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Response received: " + responseString);
                    }
                    else
                    {
                        Console.WriteLine("Error: " + response.StatusCode);
                    }
                
            
                }
            
         
         
        
     } 
}