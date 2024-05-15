using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using MySqlConnector;
using SkiaSharp;

namespace GameVerse_App
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Profile : ContentPage
	{
        private int ID;
        private string UserServerDictionaryName;
        private string avatarPath;
        private byte[] scaledImageBytes = new byte[999999];
        public Profile(int UserId)
		{
			InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            ID = UserId;

            ProfileInfo_Appearing();
            Achievements_Appearing(UserId);
        }

        private void ProfileInfo_Appearing()
        {
            startMark:
            try
            {
                ((App)Application.Current).cmd = new MySqlCommand("SELECT users.Login, users.NickName, users.Region, users.AboutMe, users.Image, socialnet.Twitch, socialnet.FaceBook, socialnet.Youtube FROM users INNER JOIN socialnet ON socialnet.User_idUser = users.idUser WHERE users.idUser = '" + ID + "'", ((App)Application.Current).cn);
                ((App)Application.Current).dr = ((App)Application.Current).cmd.ExecuteReader();
            }
            catch
            {
                ((App)Application.Current).dr.Close();
                ((App)Application.Current).cmd = new MySqlCommand("SELECT users.Login, users.NickName, users.Region, users.AboutMe, users.Image, socialnet.Twitch, socialnet.FaceBook, socialnet.Youtube FROM users INNER JOIN socialnet ON socialnet.User_idUser = users.idUser WHERE users.idUser = '" + ID + "'", ((App)Application.Current).cn);
                ((App)Application.Current).dr = ((App)Application.Current).cmd.ExecuteReader();
            }

            if (((App)Application.Current).dr.HasRows)
            {
                while (((App)Application.Current).dr.Read())
                {
                    Nickname.Text = ((App)Application.Current).dr["NickName"].ToString();
                    UserServerDictionaryName = ID + "_" + ((App)Application.Current).dr["Login"].ToString();
                    Login.Text = "Login: " + ((App)Application.Current).dr["Login"].ToString();
                    AboutMe.Text = ((App)Application.Current).dr["AboutMe"].ToString();

                    string region = ((App)Application.Current).dr["Region"].ToString();
                    if (region == "Не выбран") { Region.Source = "region_default_icon.png"; }
                    if (region == "США") { Region.Source = "flag_usa.png"; }
                    if (region == "Франция") { Region.Source = "flag_french.png"; }
                    if (region == "Германия") { Region.Source = "flag_germany.png"; }
                    if (region == "Российская Федерация") { Region.Source = "flag_russia.png"; }

                    Twitch.ClassId = ((App)Application.Current).dr["Twitch"].ToString();
                    Facebook.ClassId = ((App)Application.Current).dr["FaceBook"].ToString();
                    YouTube.ClassId = ((App)Application.Current).dr["Youtube"].ToString();

                    string ServerImagePath = ((App)Application.Current).dr["Image"].ToString();
                    if (ServerImagePath.Contains("Avatar.png"))
                    {
                        Avatar.Source = ImageSource.FromStream(() => new MemoryStream(((App)Application.Current).DownloadServerImageFromPath(ServerImagePath)));
                    }
                    else 
                    {
                        if (ServerImagePath.Length <= 1)
                        {
                            Avatar.Source = "user_avatar_default.png";
                            ServerImagePath = "";
                        }
                        else
                        {
                            if (((App)Application.Current).IsDirectoryInServerExists(@"\GameVerse\Avatars\" + UserServerDictionaryName))
                            {
                                try
                                {
                                    Avatar.Source = ImageSource.FromStream(() => new MemoryStream(((App)Application.Current).DownloadServerImageFromPath(ServerImagePath)));
                                }
                                catch (Exception ex)
                                {
                                    ((App)Application.Current).sftpClient.Disconnect();
                                    Avatar.Source = "user_avatar_default.png";
                                    DisplayAlert("Предупреждение", ex.Message, "OK");
                                }
                            }
                            else
                            {
                                Avatar.Source = "user_avatar_default.png";
                            }
                        }
                    }
                }
            }
            else
            {
                try
                {
                    ((App)Application.Current).cmd = new MySqlCommand("INSERT INTO socialnet VALUES(default,default,default,default,'" + ID + "')", ((App)Application.Current).cn);
                    ((App)Application.Current).cmd.ExecuteNonQuery();
                }
                catch
                {
                    ((App)Application.Current).dr.Close();
                    ((App)Application.Current).cmd = new MySqlCommand("INSERT INTO socialnet VALUES(default,default,default,default,'" + ID + "')", ((App)Application.Current).cn);
                    ((App)Application.Current).cmd.ExecuteNonQuery();
                }
                goto startMark;
            }
        }

        private void Achievements_Appearing(int UserID)
        {
            GameAchievements.Children.Clear();

            try
            {
                ((App)Application.Current).cmd = new MySqlCommand("SELECT Image FROM achievements INNER JOIN user_has_game ON achievements.Game_idGame = user_has_game.Game_idGame WHERE user_has_game.User_idUser = '" + UserID + "' and user_has_game.IsPurchased = 1 ORDER BY RAND() LIMIT 8", ((App)Application.Current).cn);
                ((App)Application.Current).dr = ((App)Application.Current).cmd.ExecuteReader();
            }
            catch
            {
                ((App)Application.Current).dr.Close();
                ((App)Application.Current).cmd = new MySqlCommand("SELECT Image FROM achievements INNER JOIN user_has_game ON achievements.Game_idGame = user_has_game.Game_idGame WHERE user_has_game.User_idUser = '" + UserID + "' and user_has_game.IsPurchased = 1 ORDER BY RAND() LIMIT 8", ((App)Application.Current).cn);
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
        
        private void Profile_Appearing(object sender, EventArgs e)
        {
            try
            {
                ((App)Application.Current).cmd = new MySqlCommand("SELECT game.idGame, game.Title, game.Price, image.Pos1, user_has_game.IsFavourite FROM game INNER JOIN image ON image.Game_idGame = game.idGame INNER JOIN user_has_game ON user_has_game.User_idUser = " + ID + " WHERE user_has_game.IsFavourite = 1 LIMIT 1", ((App)Application.Current).cn);
                ((App)Application.Current).dr = ((App)Application.Current).cmd.ExecuteReader();
            }
            catch
            {
                ((App)Application.Current).dr.Close();
                ((App)Application.Current).cmd = new MySqlCommand("SELECT game.idGame, game.Title, game.Price, image.Pos1, user_has_game.IsFavourite FROM game INNER JOIN image ON image.Game_idGame = game.idGame INNER JOIN user_has_game ON user_has_game.User_idUser = " + ID + " WHERE user_has_game.IsFavourite = 1 LIMIT 1", ((App)Application.Current).cn);
                ((App)Application.Current).dr = ((App)Application.Current).cmd.ExecuteReader();
            }

            var items = new ObservableCollection<GameBlockItem> { };
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

            foreach (var item in items)
            {
                GameBlock gameBlock = new GameBlock(item);
                gameBlock.MinimumHeightRequest = 120;
                GameFavourite.Children.Add(gameBlock);
            }
        }

        private async void ProfileImage_Pressed(object sender, EventArgs e)
        {
            var action = await DisplayActionSheet("Выберите изображение:", "Отмена", null, "Сделать новое изображение", "Выбрать существующее изображение");

            if (action == "Сделать новое изображение")
            {
                try
                {
                    var photo = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                    {
                        Title = $"xamarin.{DateTime.Now.ToString("dd.MM.yyyy_hh.mm.ss")}.png"
                    });

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        await photo.OpenReadAsync();
                        var newFile = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
                        using (var stream = await photo.OpenReadAsync())
                        using (var newStream = File.OpenWrite(newFile))
                            await stream.CopyToAsync(newStream);

                    }

                    using (var stream = await photo.OpenReadAsync())
                    {
                        var memoryStream = new MemoryStream();
                        await stream.CopyToAsync(memoryStream);

                        scaledImageBytes = await CropAndScaleImageAsync(memoryStream.ToArray(), 512, 512);
                        Avatar.Source = ImageSource.FromStream(() => new MemoryStream(scaledImageBytes));
                    }


                    var remotePath = @"\GameVerse\Avatars\" + UserServerDictionaryName;
                    avatarPath = remotePath + @"\Avatar.png";
                    if (((App)Application.Current).IsDirectoryInServerExists(remotePath))
                    {
                        try
                        {
                            ((App)Application.Current).sftpClient.Connect();
                            ((App)Application.Current).sftpClient.UploadFile(new MemoryStream(scaledImageBytes), avatarPath, true);
                            ((App)Application.Current).sftpClient.Disconnect();
                        }
                        catch { ((App)Application.Current).sftpClient.Disconnect(); }
                    }
                    else
                    {
                        try
                        {
                            ((App)Application.Current).sftpClient.Connect();
                            ((App)Application.Current).sftpClient.CreateDirectory(remotePath);
                            ((App)Application.Current).sftpClient.UploadFile(new MemoryStream(scaledImageBytes), avatarPath, true);

                            try
                            {
                                ((App)Application.Current).dr.Close();
                                ((App)Application.Current).cmd = new MySqlCommand("UPDATE users SET users.Image='" + @"\\GameVerse\\Avatars\\" + UserServerDictionaryName + @"\\Avatar.png" + "' WHERE users.idUser='" + ID + "'", ((App)Application.Current).cn);
                                await ((App)Application.Current).cmd.ExecuteNonQueryAsync();
                            } catch
                            {
                                ((App)Application.Current).cmd = new MySqlCommand("UPDATE users SET users.Image='" + @"\\GameVerse\\Avatars\\" + UserServerDictionaryName + @"\\Avatar.png" + "' WHERE users.idUser='" + ID + "'", ((App)Application.Current).cn);
                                await ((App)Application.Current).cmd.ExecuteNonQueryAsync();
                            }

                            ((App)Application.Current).sftpClient.Disconnect();
                        }
                        catch { ((App)Application.Current).sftpClient.Disconnect(); }
                    }
                    ((App)Application.Current).sftpClient.Disconnect();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Не удалось загрузить изображение:", ex.Message, "OK");
                    ((App)Application.Current).sftpClient.Disconnect();
                    scaledImageBytes = new byte[0];
                }
            }
            else if (action == "Выбрать существующее изображение")
            {
                try
                {
                    var photo = await MediaPicker.PickPhotoAsync();

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        await photo.OpenReadAsync();
                        var newFile = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
                        using (var stream = await photo.OpenReadAsync())
                        using (var newStream = File.OpenWrite(newFile))
                            await stream.CopyToAsync(newStream);

                    }

                    using (var stream = await photo.OpenReadAsync())
                    {
                        var memoryStream = new MemoryStream();
                        await stream.CopyToAsync(memoryStream);

                        scaledImageBytes = await CropAndScaleImageAsync(memoryStream.ToArray(), 512, 512);
                        Avatar.Source = ImageSource.FromStream(() => new MemoryStream(scaledImageBytes));
                    }

                    var remotePath = @"\GameVerse\Avatars\" + UserServerDictionaryName;
                    avatarPath = remotePath + @"\Avatar.png";
                    if (((App)Application.Current).IsDirectoryInServerExists(remotePath))
                    {
                        try
                        {
                            ((App)Application.Current).sftpClient.Connect();
                            ((App)Application.Current).sftpClient.UploadFile(new MemoryStream(scaledImageBytes), avatarPath, true);
                            ((App)Application.Current).sftpClient.Disconnect();
                        }
                        catch { ((App)Application.Current).sftpClient.Disconnect(); }
                    }
                    else
                    {
                        try
                        {
                            ((App)Application.Current).sftpClient.Connect();
                            ((App)Application.Current).sftpClient.CreateDirectory(remotePath);
                            ((App)Application.Current).sftpClient.UploadFile(new MemoryStream(scaledImageBytes), avatarPath, true);

                            try
                            {
                                ((App)Application.Current).dr.Close();
                                ((App)Application.Current).cmd = new MySqlCommand("UPDATE users SET users.Image='" + @"\\GameVerse\\Avatars\\" + UserServerDictionaryName + @"\\Avatar.png" + "' WHERE users.idUser='" + ID + "'", ((App)Application.Current).cn);
                                await ((App)Application.Current).cmd.ExecuteNonQueryAsync();
                            }
                            catch
                            {
                                ((App)Application.Current).cmd = new MySqlCommand("UPDATE users SET users.Image='" + @"\\GameVerse\\Avatars\\" + UserServerDictionaryName + @"\\Avatar.png" + "' WHERE users.idUser='" + ID + "'", ((App)Application.Current).cn);
                                await ((App)Application.Current).cmd.ExecuteNonQueryAsync();
                            }
                            ((App)Application.Current).sftpClient.Disconnect();
                        }
                        catch { ((App)Application.Current).sftpClient.Disconnect(); }
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Не удалось загрузить изображение:", ex.Message, "OK");
                    ((App)Application.Current).sftpClient.Disconnect();
                    scaledImageBytes = new byte[0];
                }
            }
        }

        private async void ChangeInfo_Pressed(object sender, EventArgs e)
        {
            var NickName = await DisplayPromptAsync("Введите новый ник:",Nickname.Text);
            if (NickName == null || NickName == "") { return; }
            Nickname.Text = NickName;

            var AboutText = await DisplayPromptAsync("Напишите о себе:", AboutMe.Text);
            if (AboutText == null || AboutText == "") { return; }
            AboutMe.Text = AboutText;

            try
            {
                ((App)Application.Current).cmd = new MySqlCommand("UPDATE users SET users.NickName = '" + NickName + "', users.AboutMe = '" + AboutMe + "' WHERE users.idUser = '" + ID + "'", ((App)Application.Current).cn);
                ((App)Application.Current).cmd.ExecuteNonQuery();
            }
            catch
            {
                ((App)Application.Current).dr.Close();
                ((App)Application.Current).cmd = new MySqlCommand("UPDATE users SET users.NickName = '" + NickName + "', users.AboutMe = '" + AboutMe + "' WHERE users.idUser = '" + ID + "'", ((App)Application.Current).cn);
                ((App)Application.Current).cmd.ExecuteNonQuery();
            }

            var TwitchPath = await DisplayPromptAsync("Введите ссылку на твич:", Twitch.ClassId.ToString());
            if (TwitchPath == null || TwitchPath == "") { return; }
            Twitch.ClassId = TwitchPath;

            var FacebookPath = await DisplayPromptAsync("Введите ссылку на фейсбук:", Facebook.ClassId.ToString());
            if (FacebookPath == null || FacebookPath == "") { return; }
            Facebook.ClassId = FacebookPath;

            var YouTubePath = await DisplayPromptAsync("Введите ссылку на ютуб:", YouTube.ClassId.ToString());
            if (YouTubePath == null || YouTubePath == "") { return; }
            YouTube.ClassId = YouTubePath;

            try
            {
                ((App)Application.Current).cmd = new MySqlCommand("UPDATE socialnet SET socialnet.Twitch = '" + TwitchPath + "', socialnet.FaceBook = '" + FacebookPath + "', socialnet.YouTube = '" + YouTubePath + "' WHERE socialnet.User_idUser = '" + ID + "'", ((App)Application.Current).cn);
                ((App)Application.Current).cmd.ExecuteNonQuery();
            }
            catch
            {
                ((App)Application.Current).dr.Close();
                ((App)Application.Current).cmd = new MySqlCommand("UPDATE socialnet SET socialnet.Twitch = '" + TwitchPath + "', socialnet.FaceBook = '" + FacebookPath + "', socialnet.YouTube = '" + YouTubePath + "' WHERE socialnet.User_idUser = '" + ID + "'", ((App)Application.Current).cn);
                ((App)Application.Current).cmd.ExecuteNonQuery();
            }
        }

        private async void ChangeRegion_Pressed(object sender, EventArgs e)
        {
            var region = await DisplayActionSheet("Выберите новый регион:", "Не выбран", "Отмена", "США", "Франция", "Германия", "Российская Федерация");
            if (region == "Не выбран") { Region.Source = "region_default_icon.png"; }
            if (region == "США") { Region.Source = "flag_usa.png"; }
            if (region == "Франция") { Region.Source = "flag_french.png"; }
            if (region == "Германия") { Region.Source = "flag_germany.png"; }
            if (region == "Российская Федерация") { Region.Source = "flag_russia.png"; }

            if(region != "Отмена")
            {
                try
                {
                    ((App)Application.Current).cmd = new MySqlCommand("UPDATE users SET users.Region = '" + region + "' WHERE users.idUser = '" + ID + "'", ((App)Application.Current).cn);
                    ((App)Application.Current).cmd.ExecuteNonQuery();
                }
                catch
                {
                    ((App)Application.Current).dr.Close();
                    ((App)Application.Current).cmd = new MySqlCommand("UPDATE users SET users.Region = '" + region + "' WHERE users.idUser = '" + ID + "'", ((App)Application.Current).cn);
                    ((App)Application.Current).cmd.ExecuteNonQuery();
                }
            }
        }

        private async void Twitch_Pressed(object sender, EventArgs e)
        {
            try
            {
                var url = Twitch.ClassId;

                await Browser.OpenAsync(url);
            }
            catch { }
        }

        private async void Facebook_Pressed(object sender, EventArgs e)
        {
            try
            {
                var url = Facebook.ClassId;

                await Browser.OpenAsync(url);
            }
            catch { }
        }

        private async void YouTube_Pressed(object sender, EventArgs e)
        {
            try
            {
                var url = YouTube.ClassId;

                await Browser.OpenAsync(url);
            }
            catch { }
        }

        private async Task<byte[]> CropAndScaleImageAsync(byte[] imageBytes, int maxWidth, int maxHeight)
        {
            using (SKBitmap imageBitmap = SKBitmap.Decode(imageBytes))
            {
                SKImageInfo info = new SKImageInfo(maxWidth, maxHeight);

                using (SKSurface surface = SKSurface.Create(info))
                {
                    using (SKCanvas canvas = surface.Canvas)
                    {
                        SKPaint paint = new SKPaint();
                        paint.IsAntialias = true;
                        paint.FilterQuality = SKFilterQuality.High;
                        paint.IsDither = true;
                        SKRect destRect = new SKRect(0, 0, maxWidth, maxHeight);
                        canvas.DrawBitmap(imageBitmap, destRect, paint);
                    }

                    using (SKImage scaledImage = surface.Snapshot())
                    {
                        using (SKImage image = scaledImage)
                        using (SKData data = image.Encode())
                        {
                            return data.ToArray();
                        }
                    }
                }
            }
        }
    }
}