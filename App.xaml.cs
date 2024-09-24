using static Maui.app1.BackgroundModelLoad;

namespace Maui.app1
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            StartDataPopulation();
        }

        private async void StartDataPopulation()
        {
            // Start populating data asynchronously
            await StringDataStore.Instance.PopulateDataAsync();
        }

    }
}
