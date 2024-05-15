using System;
using System.IO;
using Xamarin.Forms;
using MySqlConnector;

namespace GameVerse_App
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            InfoRefresher();
        }
        private void InfoRefresher()
        {
            try
            {
                ((App)Application.Current).cmd = new MySqlCommand("SELECT users.Login, users.Image FROM users WHERE users.idUser = '" + ((App)Application.Current).UserID + "'", ((App)Application.Current).cn);
                ((App)Application.Current).dr = ((App)Application.Current).cmd.ExecuteReader();
            }
            catch
            {
                ((App)Application.Current).dr.Close();
                ((App)Application.Current).cmd = new MySqlCommand("SELECT users.Login, users.Image FROM users WHERE users.idUser = '" + ((App)Application.Current).UserID + "'", ((App)Application.Current).cn);
                ((App)Application.Current).dr = ((App)Application.Current).cmd.ExecuteReader();
            }
            while (((App)Application.Current).dr.Read())
            {
                UserLogin.Text = ((App)Application.Current).dr["Login"].ToString();
                string ServerImagePath = ((App)Application.Current).dr["Image"].ToString();
                if (ServerImagePath.Contains("Avatar.png"))
                {
                    UserAvatar.Source = ImageSource.FromStream(() => new MemoryStream(((App)Application.Current).DownloadServerImageFromPath(ServerImagePath)));
                }
            }
            ((App)Application.Current).dr.Close();
        }

        private async void UserAvatar_Pressed(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new Profile(((App)Application.Current).UserID));
        }

        private async void ExitButtonClicked(object sender, EventArgs e)
        {
            var result = await DisplayAlert("Подтверждение", "Вы точно хотите выйти из приложения?", "Да", "Нет");
            if (result == true)
            {
                System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            } else
            {
                return;
            }
        }
    }
}
