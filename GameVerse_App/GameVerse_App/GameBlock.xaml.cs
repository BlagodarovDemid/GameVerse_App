using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using MySqlConnector;

namespace GameVerse_App
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GameBlock : ContentView
	{
        public GameBlock(GameBlockItem item)
		{
			InitializeComponent();

            GameID.Text = item.ID.ToString();
            GameTitle.Text = item.Title.ToString();
            if (item.Price > 0) { GameIsFree.IsVisible = false; GamePrice.Text = item.Price.ToString(); } else { GameIsFree.IsVisible = true; GamePrice.IsVisible = false; GamePriceIcon.IsVisible = false; }

            try
            {
                ((App)Application.Current).cmd = new MySqlCommand("SELECT image.Pos1 FROM image WHERE image.Game_idGame = '" + item.ID + "'", ((App)Application.Current).cn);
                ((App)Application.Current).dr = ((App)Application.Current).cmd.ExecuteReader();

            }
            catch
            {
                ((App)Application.Current).dr.Close();
                ((App)Application.Current).cmd = new MySqlCommand("SELECT image.Pos1 FROM image WHERE image.Game_idGame = '" + item.ID + "'", ((App)Application.Current).cn);
                ((App)Application.Current).dr = ((App)Application.Current).cmd.ExecuteReader();
            }

            while (((App)Application.Current).dr.Read())
            {
                Device.StartTimer(TimeSpan.FromSeconds(2), () =>
                {
                    return true;
                });
                string ServerImagePath = ((App)Application.Current).dr["Pos1"].ToString();
                GameImage.Source = ImageSource.FromStream(() => new MemoryStream(((App)Application.Current).DownloadServerImageFromPath(ServerImagePath)));
            }
            ((App)Application.Current).dr.Close();
        }

        private async void GameImage_Pressed(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new GameCenter(Convert.ToInt32(GameID.Text)));
        }
    }
}