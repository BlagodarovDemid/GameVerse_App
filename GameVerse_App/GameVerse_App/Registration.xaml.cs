using System;
using System.Linq;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using MySqlConnector;

namespace GameVerse_App
{
    public partial class Registration : ContentPage
    {
        public Registration()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            RegionDisplay.Text = "Выберите свой регион";
        }

        private string SelectedRegion = "Не выбран";

        private async void RegistrationButton_Clicked(object sender, EventArgs e)
        {
            string email = Login.Text;
            string password = Password.Text;
            string confirm = ConfirmPassword.Text;

            if (RegionDisplay.Text == "Выберите свой регион")
            {
                SelectedRegion = "Не выбран";
            }

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

            if (password != confirm)
            {
                await DisplayAlert("Ошибка", "Пароли не совпадают", "OK");
                return;
            }

            ((App)Application.Current).cmd = new MySqlCommand("INSERT INTO Users VALUES (default,'" + email + "','" + ((App)Application.Current).GetHashString(password) + "',default,'" + SelectedRegion + "',default,default,default)", ((App)Application.Current).cn,null);
            ((App)Application.Current).dr = ((App)Application.Current).cmd.ExecuteReader();
            ((App)Application.Current).dr.Close();

            await DisplayAlert("Успешно!", "Вы успешно зарегистрировались!", "OK");
        }

        private void Region_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Region.SelectedIndex == 0)
            {
                SelectedRegion = "Российская Федерация";
            }
            if (Region.SelectedIndex == 1)
            {
                SelectedRegion = "США";
            }
            if (Region.SelectedIndex == 2)
            {
                SelectedRegion = "Франция";
            }
            if (Region.SelectedIndex == 3)
            {
                SelectedRegion = "Германия";
            }
            if (Region.SelectedIndex == 4)
            {
                SelectedRegion = "Не выбран";
            }
            RegionDisplay.Text = SelectedRegion;
        }

        private async void BackButton_Pressed(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }
}
