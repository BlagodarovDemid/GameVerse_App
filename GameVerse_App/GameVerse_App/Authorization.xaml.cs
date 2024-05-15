using System;
using System.Linq;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MySqlConnector;

namespace GameVerse_App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Authorization : ContentPage
    {
        public Authorization()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
        private async void AuthorizationButton_Pressed(object sender, EventArgs e)
        {
            string nickname = "User";
            string email = Login.Text;
            string password = Password.Text;

            if (email == null || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                await DisplayAlert("Ошибка", "Некорректный адрес электронной почты", "OK");
                return;
            }

            if (password == null || password.Length < 8 || password.Length > 15 || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Ошибка", "Пароль должен содержать как минимум 8 символов и максимум 15", "OK");
                return;
            }

            if (!Regex.IsMatch(password, "[a-zA-Z]"))
            {
                await DisplayAlert("Ошибка", "Пароль должен содержать хотя бы одну букву", "OK");
                return;
            }

            if (!Regex.IsMatch(password, "[0-9]"))
            {
                await DisplayAlert("Ошибка", "Пароль должен содержать хотя бы одну цифру", "OK");
                return;
            }

            if (password.Contains(' '))
            {
                await DisplayAlert("Ошибка", "Пароль не должен содержать пробелы", "OK");
                return;
            }

            ((App)Application.Current).cmd = new MySqlCommand("SELECT IdUser, Nickname FROM Users WHERE Login='" + email + "' and Password='" + ((App)Application.Current).GetHashString(password) + "'", ((App)Application.Current).cn);
            ((App)Application.Current).dr = ((App)Application.Current).cmd.ExecuteReader();
            if (!((App)Application.Current).dr.HasRows)
            {
                ((App)Application.Current).dr.Close();
                await DisplayAlert("Ошибка", "Такого пользователя не существует", "OK");
                return;
            }
            else
            {
                while (((App)Application.Current).dr.Read())
                {
                    ((App)Application.Current).UserID = (int)((App)Application.Current).dr["IdUser"];
                    nickname = ((App)Application.Current).dr["Nickname"].ToString();
                }
                ((App)Application.Current).dr.Close();
            }

            await DisplayAlert(null, "Добро пожаловать, " + nickname + "!", "OK");

            Application.Current.MainPage = new AppShell();
            await Navigation.PopToRootAsync();
        }

        private async void ForwardButton_Pressed(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Registration());
        }

        private void ForgotButton_Pressed(object sender, EventArgs e)
        {

        }
    }
}