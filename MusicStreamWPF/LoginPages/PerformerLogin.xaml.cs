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
    /// Логика взаимодействия для PerformerLogin.xaml
    /// </summary>
    public partial class PerformerLogin : Page
    {
        public PerformerLogin()
        {
            InitializeComponent();
            AutoPlace();
            DataContext = this;
        }

        public void AutoPlace()
        {
            UsernameTextBox.Text = Properties.Settings.Default.PerfUser;
            PasswordTextBox.Password = Properties.Settings.Default.PerfPassword;
            RememberCheck.IsChecked = Properties.Settings.Default.PerfChecked;
        }

        private async void ConfirmButton(object sender, RoutedEventArgs e)
        {

            var login = new LogPerfDTO
            {
                Nick = UsernameTextBox.Text,
                Password = PasswordTextBox.Password,
            };
            var user = await Login(login);
            if (user != null)
            {
                if (RememberCheck.IsChecked == true)
                {
                    Properties.Settings.Default.PerfUser = login.Nick;
                    Properties.Settings.Default.PerfPassword = login.Password;
                    Properties.Settings.Default.PerfChecked = true;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    Properties.Settings.Default.PerfUser = string.Empty;
                    Properties.Settings.Default.PerfPassword = string.Empty;
                    Properties.Settings.Default.PerfChecked = false;
                    Properties.Settings.Default.Save();
                }


                int User = user.Id;
                int Performer = user.Id;
                int performerid = user.Id;

                PerformerCard performerCard = new PerformerCard(performerid, Performer);
                Window mainWindow = Window.GetWindow(this);

                // Получаем Frame из MainWindow
                Frame mainFrame = mainWindow.FindName("mainFrame") as Frame;

                // Устанавливаем содержимое Frame на PerformerCard
                mainFrame.Navigate(performerCard);


            }
            else
            {
                ErrorLabel.Content = "Неверный логин или пароль";
            }

        }
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
        private async Task<LogPerfDTO> Login(LogPerfDTO login)
        {
            var json = JsonConvert.SerializeObject(login);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync($"https://localhost:7004/Performer/PerformerLogin", data);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<LogPerfDTO>(content);
                }
                else
                {

                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorLabel.Content = errorContent;
                    return null;


                }
            }
        }
    }
}
