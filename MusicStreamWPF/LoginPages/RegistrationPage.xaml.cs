using MusicStreamWPF.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Window
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }


        private async void CreateAcc(object sender, RoutedEventArgs e)
        {
            HttpClient client1 = new HttpClient();
            HttpResponseMessage responseproduct = await client1.GetAsync($"https://localhost:7004/User/GetAllUsers");

            responseproduct.EnsureSuccessStatusCode();

            string responseBody = await responseproduct.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<List<LoginDTO>>(responseBody);
            if (PasswordTextBox.Text != SecondPasswordTextBox.Text)
            {
                ErrorLabel.Content = "Пароли не совпадают";
            }
            else
            {
                using (HttpClient client2 = new HttpClient())
                {
                    var login = new LoginDTO
                    {
                        Nick = LoginTextbox.Text,
                        Password = PasswordTextBox.Text,
                        Regdate = DateTime.Now
                    };
                    if (data.Contains(login))
                    {
                        ErrorLabel.Content = "Пользователь уже существует";
                    }
                    else
                    {
                        var json = JsonConvert.SerializeObject(login);
                        var data2 = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = await client2.PostAsync("https://localhost:7004/User/UserRegistration", data2);
                        HttpResponseMessage response1 = await client1.GetAsync($"https://localhost:7004/User/GetUserByName?Name={LoginTextbox.Text}");
                        if (response1.IsSuccessStatusCode)
                        {
                            string json1 = await response1.Content.ReadAsStringAsync();
                            UserDTO nowuser = JsonConvert.DeserializeObject<UserDTO>(json1);
                            int User = nowuser.Id;
                            MainWindow MAINWindow = new MainWindow(User);
                            MAINWindow.Show();
                            Window.GetWindow(this).Close();
                        }
                    }
                }
            }
        }
    }
}

