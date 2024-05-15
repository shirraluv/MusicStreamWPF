using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MusicStreamWPF.DTO;
using MusicStreamWPF.Properties;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MusicStreamWPF.LoginPages
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Window
    {
        public LoginPage()
        {
            InitializeComponent();
            AutoPlace();
        }

        public void AutoPlace()
        {
            UsernameTextBox.Text = Properties.Settings.Default.Username;
            PasswordTextBox.Password = Properties.Settings.Default.Password;
            RememberCheck.IsChecked = Properties.Settings.Default.Checked;
        }
        private async void ConfirmButton(object sender, RoutedEventArgs e)
        {

            var login = new LoginDTO
            {
                Nick = UsernameTextBox.Text,
                Password = PasswordTextBox.Password,
            };
            var user = await Login(login);
            if (user != null)
            {
                if (RememberCheck.IsChecked == true)
                {
                    Properties.Settings.Default.Username = login.Nick;
                    Properties.Settings.Default.Password = login.Password;
                    Properties.Settings.Default.Checked = true;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    Properties.Settings.Default.Username = string.Empty;
                    Properties.Settings.Default.Password = string.Empty;
                    Properties.Settings.Default.Checked = false;
                    Properties.Settings.Default.Save();
                }


                int User = user.Id;
                MainWindow MAINWindow = new MainWindow(User);
                MAINWindow.Show();
                Window.GetWindow(this).Close();
            }
            else
            {
                ErrorLabel.Content = "Неверный логин или пароль";
            }

        }
        private async Task<LoginDTO> Login(LoginDTO login)
        {
            var json = JsonConvert.SerializeObject(login);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync($"https://localhost:7004/User/UserLogin", data);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<LoginDTO>(content);
                }
                else
                {

                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorLabel.Content = errorContent;
                    return null;


                }
            }
        }

        private void Hyperlink2_Click(object sender, RoutedEventArgs e)
        {
            RegistrationPage reg = new RegistrationPage();
            reg.Show();
            Window.GetWindow(this).Close();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
