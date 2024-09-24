namespace Maui.app1
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            //Tab Bar should change after login in
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SocialGroupPage());
        }
        private void SignUpClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Signup());
        }
    }

}
