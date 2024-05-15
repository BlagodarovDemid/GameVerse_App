using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MySqlConnector;

namespace GameVerse_App
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ShopList : ContentPage
	{
		public ShopList ()
		{
			InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void Games_Appearing(string SearchText = null)
        {
            if (SearchText != null && SearchText != "") {  SearchText = " WHERE game.Title LIKE '%" + SearchText + "%'"; }
            GameList.Children.Clear();
            var items = new ObservableCollection<GameBlockItem> { };
            try
            {
                ((App)Application.Current).cmd = new MySqlCommand("SELECT game.idGame, game.Title, game.Price FROM game" + SearchText, ((App)Application.Current).cn);
                ((App)Application.Current).dr = ((App)Application.Current).cmd.ExecuteReader();

            }
            catch
            {
                ((App)Application.Current).dr.Close();
                ((App)Application.Current).cmd = new MySqlCommand("SELECT game.idGame, game.Title, game.Price FROM game" + SearchText, ((App)Application.Current).cn);
                ((App)Application.Current).dr = ((App)Application.Current).cmd.ExecuteReader();
            }
            while (((App)Application.Current).dr.Read())
            {
                items.Add(new GameBlockItem
                {
                    ID = Convert.ToInt32(((App)Application.Current).dr["idGame"]),
                    Title = ((App)Application.Current).dr["Title"].ToString(),
                    Price = Convert.ToInt32(((App)Application.Current).dr["Price"])
                });
            }
            ((App)Application.Current).dr.Close();

            int rowCount = 0;
            foreach (var item in items)
            {
                GameBlock gameBlock = new GameBlock(item);
                gameBlock.MinimumHeightRequest = 120;
                gameBlock.Margin = new Thickness(0, 20 + (rowCount * gameBlock.Height * 1.2), 0, 0);
                GameList.Children.Add(gameBlock);
                rowCount++;
            }
        }
        private async void ShopList_Appearing(object sender, EventArgs e)
        {
            Games_Appearing();
        }

        private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            Games_Appearing(SearchBar.Text);
        }
    }
}