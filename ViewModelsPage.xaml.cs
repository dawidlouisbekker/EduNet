using System.Collections.Generic;
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui.Animations;
using System.ComponentModel;
using System.Text;
using System.Net.Http;
using CommunityToolkit.Maui.Storage;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Web;
using static System.Net.WebRequestMethods;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.ObjectModel;
using static Maui.app1.BackgroundModelLoad;
using System;
using Microsoft.Maui.Controls.Shapes;
using System.Runtime.ConstrainedExecution;
using System.ComponentModel.Design;

namespace Maui.app1
{

    public partial class ViewModelsPage : ContentPage
    {

        //private readonly HttpClient _httpClient;
        public ViewModelsPage() {

            InitializeComponent();
            mylistview.ItemsSource = BackgroundModelLoad.StringDataStore.Instance.Items;
        }
        private void GoToEmbeddings(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EmbeddingsFrontPage());
        }

        private async void get_response()
        {
            SendButton.IsEnabled = false;
            loadIndicator.IsRunning = true;
            loadIndicator.IsVisible = true;
            try
            {
                // Instantiate your Http1 client
                var httpClient = new OllamaClienthttp2();

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

       // private async void OnCallHttp1ApiClicked()
       // {
       //     try
       //     {
                // Instantiate your Http1 client
       //         var httpClient = new OllamaClienthttp1();

                // URL of the API or endpoint
       //         string url = "http://localhost:8060/"; // Replace with the actual URL

                // Make the asynchronous call to the API
        //        string response = await httpClient.GetAsync(url);

                // Display the response in a Label or other UI element
        //    }
        //    catch (Exception ex)
        //    {
                // Handle any exceptions
        //        await DisplayAlert("Error", ex.Message, "OK");
        //    }
       // }


        private async void ItemClicked(object sender, SelectedItemChangedEventArgs e)
        {
            ChooseLabel.IsVisible = false;
            Loader(true);

            if (e.SelectedItem == null)
            {
                await DisplayAlert("nope", "ok", "ok");
                return;
            }
                 

            // Cast the selected item to your model (MyItem)
            var selectedItem = e.SelectedItem as BackgroundModelLoad.Item;

            if (selectedItem != null)
            {
                // Access the Name property
                string? selectedName = selectedItem.Name;
                if (selectedName == null)
                {
                    await DisplayAlert("Not found", "", "ok");
                    return;
                }
                await PostModelAsync(selectedName);
                FilesmodelView.MainViewModel.Instance.model = selectedName;
                // You can now use the selectedName variable or perform further actions
            }
            QueryEntry.IsEnabled = true;
            QueryEntry.Placeholder = "Ask a Question";

            Loader(false);

           // mylistview.ItemsSource = BackgroundModelLoad.StringDataStore.Instance.Items;



        }
        private void Loader(bool on)
        {
            if (on)
            {
                loadIndicator.IsRunning = true;
                loadIndicator.IsVisible = true;
            }
            else 
            { 
                loadIndicator.IsVisible = false;
                loadIndicator.IsVisible = false;

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
            MainGrid.SetRow(QueryEntry, 4);
            MainGrid.SetRowSpan(QueryEntry, 5);
            MainGrid.SetRow(QueryBorder, 4);
            MainGrid.SetRowSpan(QueryBorder, 5);
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
        private List<Label> labelList = new List<Label>();
        private int CreateText(string Text, bool bot)
        {
            if (bot)
            {
                Label dynamicLabel = new Label
                {
                    Text = Text,

                    BackgroundColor = Colors.Transparent,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Fill,
                    FontSize = 16,
                    TextColor = Colors.Ivory,
                    Margin = 5,
                };
                TextstackLayout.Children.Add(dynamicLabel);
                return TextstackLayout.Children.Count - 1;
            }
            else 
            {


                Label dynamicLabel = new Label
                {
                    Text = Text,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Fill,
                    FontSize = 16,
                    TextColor = Colors.Ivory,
                    Margin = 5,
                };

                var frame = new Frame
                {

                    BackgroundColor = Color.FromArgb("#4b4bce"),// Border color
                    BorderColor = Colors.Transparent,
                    CornerRadius = 10,
                    Padding = 0.4,   // Padding inside the border
                    HorizontalOptions = LayoutOptions.End,

                    VerticalOptions = LayoutOptions.Start,
                    Content = dynamicLabel,
                    Margin = 5
                };
                TextstackLayout.Children.Add(frame);

                
            }


            // Add the Border (containing the Label) to the StackLayout
            return 0;
        }
      

        private async Task ReceiveStreamAsync(string url, string query)
        {
            var uriBuilder = new UriBuilder(url);
            var queryParams = HttpUtility.ParseQueryString(uriBuilder.Query);
            queryParams["param1"] = query;
            //queryParams["param2"] = FilesmodelView.MainViewModel.Instance.model;
            uriBuilder.Query = queryParams.ToString();
            string finalUrl = uriBuilder.ToString();

            using (var client = new HttpClient())
            {
                try
                {
                    using (HttpResponseMessage response = await client.GetAsync(finalUrl,HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();
                        
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        using (var reader = new System.IO.StreamReader(stream))
                        {
                            char[] buffer = new char[1024];
                            int bytesRead;

                            int Pos = CreateText("", true);
                            var label = TextstackLayout.Children[Pos] as Label;
                            if (label == null)
                            {
                                await DisplayAlert("Error", "Label not found in the Frame", "OK");
                                return;
                            }
                            while ((bytesRead = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                

                                 string tokens = new string(buffer, 0, bytesRead);
                                 label.Text += tokens;
                                 //await TextCollection.ScrollToAsync(0, TextCollection.ContentSize.Height, true);

                            }
                        }
                    }
                }
                catch (HttpRequestException e)
                {
                    await DisplayAlert("Something Went Wrong", e.Message, "Okey");
                }
            
            }


            Loader(false);
            SendButton.IsEnabled = true;
            SendButton.BackgroundColor = Color.FromArgb("#2B0B98");
   
        }
 


        private async void SendQuery(object sender, EventArgs e)
                {
                    int Pos = CreateText(QueryEntry.Text, false);
                    string Url = "http://localhost:8060/stream";
                    string Query = QueryEntry.Text;
                    QueryEntry.IsEnabled = false;
                    QueryEntry.Text = "Answering...";
                    QueryBorder.IsVisible = false;
                    
                    QueryEntry.VerticalOptions = LayoutOptions.End;
                    EmbedButton.IsEnabled = false;
                    EmbedButton.IsVisible = false;
                    SendButton.IsEnabled = false;
                    Stopper.IsVisible = true;
                    Stopper.IsEnabled = true;
                    Loader(true);
                    await ReceiveStreamAsync(Url, Query);
                    QueryEntry.IsEnabled = true;
                    QueryEntry.Text = "";
                    MainGrid.SetRow(QueryEntry, 5);
                    QueryBorder.IsVisible = false;
                    MainGrid.SetRow(QueryBorder, 5);
                    EmbedButton.IsVisible = true;
                    EmbedButton.IsEnabled = true;
        }
        private async Task PostModelAsync(string model)
        {
          var client = new HttpClient();

                    // FastAPI URL to send the model text
          string? url = "http://127.0.0.1:8060/model/";

                    // Prepare the string content
          var content = new StringContent(model, Encoding.UTF8, "text/plain");

            // Post the request
            try
            {
                var response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Console.WriteLine("Error: " + response.StatusCode);
                }

                await FilesmodelView.MainViewModel.Instance.GetCollectionsAsync();
                FilesmodelView.MainViewModel.Instance.model = model;
                EmbedButton.IsEnabled = true;
                EmbedButton.IsVisible = true;
            }
            catch (Exception ex)
            {
              await DisplayAlert("Not Running", "Server is not running", "Ok");
              return;
            }
           
         }

        private async void StopModel(object sender, EventArgs e)
        {
            string? url = "http://127.0.0.1:8060/stop-server";
            var content = new StringContent("none", Encoding.UTF8, "text/plain");
            var client = new HttpClient();
            var response = await client.PostAsync(url,content);
            Stopper.IsVisible = false;
            Stopper.IsEnabled = false;
            EmbedButton.IsEnabled = true;
        }
            
     }

}