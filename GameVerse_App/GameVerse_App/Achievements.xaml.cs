using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MySqlConnector;

namespace GameVerse_App
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Achievements : ContentPage
	{
		public Achievements ()
		{
			InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void Achievements_Appearing(object sender, EventArgs e)
        {
            GameAchievements.Children.Clear();
            try
            {
                ((App)Application.Current).dr.Close();
                ((App)Application.Current).cmd = new MySqlCommand("SELECT Image FROM achievements INNER JOIN user_has_game ON achievements.Game_idGame = user_has_game.Game_idGame WHERE user_has_game.User_idUser = '" + ((App)Application.Current).UserID + "' and user_has_game.IsPurchased = 1", ((App)Application.Current).cn);
                ((App)Application.Current).dr = ((App)Application.Current).cmd.ExecuteReader();
            }
            catch
            {
                ((App)Application.Current).cmd = new MySqlCommand("SELECT Image FROM achievements INNER JOIN user_has_game ON achievements.Game_idGame = user_has_game.Game_idGame WHERE user_has_game.User_idUser = '" + ((App)Application.Current).UserID + "' and user_has_game.IsPurchased = 1", ((App)Application.Current).cn);
                ((App)Application.Current).dr = ((App)Application.Current).cmd.ExecuteReader();
            }

            while (((App)Application.Current).dr.Read())
            {
                Image image = new Image()
                {
                    Aspect = Aspect.AspectFill,
                    WidthRequest = 70,
                    HeightRequest = 70,
                    Margin = new Thickness(0, 10, 15, 0)
                };

                string ServerImagePath = ((App)Application.Current).dr["Image"].ToString();
                byte[] imageByte = new byte[999999];
                imageByte = ((App)Application.Current).DownloadServerImageFromPath(ServerImagePath);
                image.Source = ImageSource.FromStream(() => new MemoryStream(imageByte));

                GameAchievements.Children.Add(image);
            }
        }
    }
}