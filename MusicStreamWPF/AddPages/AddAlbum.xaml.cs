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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicStreamWPF.AddPages
{
    /// <summary>
    /// Логика взаимодействия для AddAlbum.xaml
    /// </summary>
    public partial class AddAlbum : Page
    {
        int creatorid;
        string imagepath;
        public AddAlbum(int nowcreatorid)
        {
            InitializeComponent();
            creatorid = nowcreatorid;
        }

        private async void AddButton(object sender, RoutedEventArgs e)
        {

            HttpClient client1 = new HttpClient();
            HttpResponseMessage responseproduct = await client1.GetAsync($"https://localhost:7004/User/GetAllUsers");

            responseproduct.EnsureSuccessStatusCode();

            string responseBody = await responseproduct.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<List<AlbumDTO>>(responseBody);


            if (NameTextBox.Text == null)
            {
                ErrorLabel.Content = "Введите название альбома";
            }
            else if (imagepath == null)
            {
                ErrorLabel.Content = "Выберите превью альбома";
            }
            else
            {
                using (HttpClient client2 = new HttpClient())
                {
                    var album = new AlbumDTO
                    {
                        Name = NameTextBox.Text,
                        Duration = "1:11",
                        Date = DateTime.Now,
                        Creatorid = creatorid,
                        Imagesource = imagepath
                    };
                    if (data.Contains(album))
                    {
                        ErrorLabel.Content = "Альбом уже существует";
                    }
                    else
                    {
                        await AddAlbumLol(album);
                    }
                }
            }
        }
        private async Task AddAlbumLol(AlbumDTO album)
        {

            var json = JsonConvert.SerializeObject(album);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync("https://localhost:7004/Album/AddAlbum", data);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Альбом успешно добавлен!");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Ошибка добавления альбома: {errorContent}");
                }
            }
        }

        private void ChooseImage(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = "c:\\";
            dlg.Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*";
            dlg.FilterIndex = 1;
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() == true)
            {
                string selectedFileName = dlg.FileName;

                string path = "C:\\Users\\kosty\\source\\repos\\MusicStreamWPF\\MusicStreamWPF\\images\\" + System.IO.Path.GetFileName(selectedFileName);

                if (!System.IO.File.Exists(path))
                {
                    System.IO.File.Copy(selectedFileName, path, true);
                }

                imagepath = path;
            }
        }
    }
}