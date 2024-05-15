using System;
using System.IO;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MySqlConnector;

namespace GameVerse_App
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GameCenter : ContentPage
	{
        private int ID;
        private int imageIndex = 1;

        private byte[] Image1 = new byte[999999];
        private byte[] Image2 = new byte[999999];
        private byte[] Image3 = new byte[999999];
        public GameCenter(int GameID)
		{
			InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            ID = GameID;

            GameInfo_Appearing(GameID);

            Device.StartTimer(TimeSpan.FromSeconds(2), () =>
            {
                if (imageIndex > 3)
                {
                    imageIndex = 1;
                }

                if (imageIndex == 1) { GameImage.Source = ImageSource.FromStream(() => new MemoryStream(Image1)); }
                if (imageIndex == 2) { GameImage.Source = ImageSource.FromStream(() => new MemoryStream(Image2)); }
                if (imageIndex == 3) { GameImage.Source = ImageSource.FromStream(() => new MemoryStream(Image3)); }

                imageIndex++;

                return true;
            });
        }

        private void GameInfo_Appearing(int GameID)
        {
            startMark:
            try
            {
                ((App)Application.Current).dr.Close();
                ((App)Application.Current).cmd = new MySqlCommand("SELECT game.Title, game.TitleDescription, game.Developers, game.Category, game.PublicDate, sysrequirement.OS, sysrequirement.CPUmin, sysrequirement.RAMmin, sysrequirement.GPUartmin, sysrequirement.Space, image.Pos2, image.Pos3, image.Pos4, user_has_game.IsFavourite FROM image INNER JOIN user_has_game ON image.Game_idGame = user_has_game.Game_idGame INNER JOIN game ON game.idGame = user_has_game.Game_idGame INNER JOIN sysrequirement ON sysrequirement.Game_idGame = user_has_game.Game_idGame WHERE user_has_game.Game_idGame = '" + GameID + "' and user_has_game.User_idUser = '" + ((App)Application.Current).UserID + "'", ((App)Application.Current).cn);
                ((App)Application.Current).dr = ((App)Application.Current).cmd.ExecuteReader();
            }
            catch
            {
                ((App)Application.Current).cmd = new MySqlCommand("SELECT game.Title, game.TitleDescription, game.Developers, game.Category, game.PublicDate, sysrequirement.OS, sysrequirement.CPUmin, sysrequirement.RAMmin, sysrequirement.GPUmin, sysrequirement.Space, image.Pos2, image.Pos3, image.Pos4, user_has_game.IsFavourite FROM image INNER JOIN user_has_game ON image.Game_idGame = user_has_game.Game_idGame INNER JOIN game ON game.idGame = user_has_game.Game_idGame INNER JOIN sysrequirement ON sysrequirement.Game_idGame = user_has_game.Game_idGame WHERE user_has_game.Game_idGame = '" + GameID + "' and user_has_game.User_idUser = '" + ((App)Application.Current).UserID + "'", ((App)Application.Current).cn);
                ((App)Application.Current).dr = ((App)Application.Current).cmd.ExecuteReader();
            }

            if (((App)Application.Current).dr.HasRows)
            {
                while (((App)Application.Current).dr.Read())
                {
                    GameTitle.Text = ((App)Application.Current).dr["Title"].ToString();
                    GameTitleDescription.Text = ((App)Application.Current).dr["TitleDescription"].ToString();
                    GameTitleDescription.Text = ((App)Application.Current).dr["TitleDescription"].ToString();
                    GameDevelopers.Text = "Разработчик: " + ((App)Application.Current).dr["Developers"].ToString();
                    GamePublicDate.Text = "Дата выхода: " + DateTime.Parse(((App)Application.Current).dr["PublicDate"].ToString()).ToString("D", new CultureInfo("ru-RU"));
                    FavouriteGame.IsChecked = Convert.ToBoolean(((App)Application.Current).dr["IsFavourite"].ToString()) == true ? true : false;
                    
                    GameOS.Text = "• ОС: " + ((App)Application.Current).dr["OS"].ToString();
                    GameCPUmin.Text = "• Процессор: " + ((App)Application.Current).dr["CPUmin"].ToString();
                    GameRAMmin.Text = "• Оперативная память: " + ((App)Application.Current).dr["RAMmin"].ToString();
                    GameGPUmin.Text = "• Видеокарта: " + ((App)Application.Current).dr["GPUmin"].ToString();
                    GameSpacing.Text = "• Место на диске: " + ((App)Application.Current).dr["Space"].ToString();

                    Image1 = ((App)Application.Current).DownloadServerImageFromPath(((App)Application.Current).dr["Pos2"].ToString());
                    Image2 = ((App)Application.Current).DownloadServerImageFromPath(((App)Application.Current).dr["Pos3"].ToString());
                    Image3 = ((App)Application.Current).DownloadServerImageFromPath(((App)Application.Current).dr["Pos4"].ToString());
                }
            }
            else
            {
                try
                {
                    ((App)Application.Current).cmd = new MySqlCommand("INSERT INTO user_has_game VALUES('" + ((App)Application.Current).UserID + "','" + GameID + "',default,default,default)", ((App)Application.Current).cn);
                    ((App)Application.Current).cmd.ExecuteNonQuery();

                }
                catch
                {
                    ((App)Application.Current).dr.Close();
                    ((App)Application.Current).cmd = new MySqlCommand("INSERT INTO user_has_game VALUES('" + ((App)Application.Current).UserID + "','" + GameID + "',default,default,default)", ((App)Application.Current).cn);
                    ((App)Application.Current).cmd.ExecuteNonQuery();
                }

                goto startMark;
            }
        }

        private async void GameCenter_Appearing(object sender, EventArgs e)
        {
            GameAchievements.Children.Clear();
            try
            {
                ((App)Application.Current).dr.Close();
                ((App)Application.Current).cmd = new MySqlCommand("SELECT Image FROM achievements WHERE achievements.Game_idGame = '" + ID + "'", ((App)Application.Current).cn);
                ((App)Application.Current).dr = ((App)Application.Current).cmd.ExecuteReader();
            }
            catch
            {
                ((App)Application.Current).cmd = new MySqlCommand("SELECT Image FROM achievements WHERE achievements.Game_idGame = '" + ID + "'", ((App)Application.Current).cn);
                ((App)Application.Current).dr = ((App)Application.Current).cmd.ExecuteReader();
            }

            while (((App)Application.Current).dr.Read())
            {
                Image image = new Image()
                {
                    Aspect = Aspect.AspectFill,
                    WidthRequest = 70,
                    HeightRequest = 70,
                    Margin = new Thickness(0, 0, 15, 0)
                };

                string ServerImagePath = ((App)Application.Current).dr["Image"].ToString();
                byte[] imageByte = new byte[999999];
                imageByte = ((App)Application.Current).DownloadServerImageFromPath(ServerImagePath);
                image.Source = ImageSource.FromStream(() => new MemoryStream(imageByte));

                GameAchievements.Children.Add(image);
            }
        }

        private void FavouriteGame_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (FavouriteGame.IsChecked = true)
            {
                FavouriteGame.IsEnabled = false;
            }
            else
            {
                try
                {
                    ((App)Application.Current).cmd = new MySqlCommand("UPDATE user_has_game SET user_has_game.IsFavourite = CASE WHEN user_has_game.IsFavourite = '" + ID + "' THEN 1 ELSE 0 END WHERE user_has_game.User_idUser = '" + ((App)Application.Current).UserID + "'", ((App)Application.Current).cn);
                    ((App)Application.Current).cmd.ExecuteNonQuery();
                }
                catch
                {
                    ((App)Application.Current).dr.Close();
                    ((App)Application.Current).cmd = new MySqlCommand("UPDATE user_has_game SET user_has_game.IsFavourite = CASE WHEN user_has_game.IsFavourite = '" + ID + "' THEN 1 ELSE 0 END WHERE user_has_game.User_idUser = '" + ((App)Application.Current).UserID + "'", ((App)Application.Current).cn);
                    ((App)Application.Current).cmd.ExecuteNonQuery();
                }
            }
        }
    }
}