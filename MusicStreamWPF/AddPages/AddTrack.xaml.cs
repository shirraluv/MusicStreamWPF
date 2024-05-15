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
    /// Логика взаимодействия для AddTrack.xaml
    /// </summary>
    public partial class AddTrack : Page
    {

        int creatoridd;
        string imagepath;
        string filepath;
        public List<AlbumDTO> albums;
        public AddTrack(int nowcreatorid)
        {
            InitializeComponent();
            creatoridd = nowcreatorid;
            DataContext = this;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadData();
        }
        public async Task LoadData()
        {
            try
            {
                HttpClient client = new HttpClient();
                int creatorid = creatoridd;
                HttpResponseMessage responseproduct = await client.GetAsync($"https://localhost:7004/Album/GetPerformerAlbums?creatorid={creatorid}");

                if (responseproduct.IsSuccessStatusCode)
                {
                    string responseBody = await responseproduct.Content.ReadAsStringAsync();


                    albums = JsonConvert.DeserializeObject<List<AlbumDTO>>(responseBody);
                    foreach (AlbumDTO album in albums)
                    {

                        AlbumListView.Items.Add(album);


                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
        }
        

        private async void AddButton(object sender, RoutedEventArgs e)
        {

            HttpClient client1 = new HttpClient();
            HttpResponseMessage responseproduct = await client1.GetAsync($"https://localhost:7004/Track/GetAllTracks");
            HttpResponseMessage responseproduct2 = await client1.GetAsync($"https://localhost:7004/Album/GetPerformerAlbums?creatorid={creatoridd}");

            responseproduct.EnsureSuccessStatusCode();
            responseproduct2.EnsureSuccessStatusCode();

            string responseBody = await responseproduct.Content.ReadAsStringAsync();
            string responseBody2 = await responseproduct2.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<List<TrackDTO>>(responseBody);
            albums = JsonConvert.DeserializeObject<List<AlbumDTO>>(responseBody2);
            AlbumDTO selectedAlbum = AlbumListView.SelectedItem as AlbumDTO;
            
            if (selectedAlbum == null)
            {
                ErrorLabel.Content = "Выберите альбом";
            }
            if (NameTextBox.Text == null)
            {
                ErrorLabel.Content = "Введите название трека";
            }
            else if (imagepath == null)
            {
                ErrorLabel.Content = "Выберите превью трека";
            }
            else if (filepath == null)
            {
                ErrorLabel.Content = "Выберите трек";
            }
            else
            {
                
                using (HttpClient client2 = new HttpClient())
                {
                    var track = new TrackDTO
                    {
                        Name = NameTextBox.Text,
                        Duration = "1:11",
                        Date = DateTime.Now,
                        Albumid = selectedAlbum.Id,
                        Imagesource = imagepath,
                        Auditions = 0,
                        Filename = filepath,
                    };
                    if (data.Contains(track))
                    {
                        ErrorLabel.Content = "Трек уже существует";
                    }
                    else
                    {
                        await AddTrackLol(track);
                    }
                }
            }
        }
        private async Task AddTrackLol(TrackDTO track)
        {

            var json = JsonConvert.SerializeObject(track);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync("https://localhost:7004/Track/AddTrack", data);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Трек успешно добавлен!");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Ошибка добавления трека: {errorContent}");
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

        private void ChoosePath(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = "c:\\";
            dlg.Filter = "MP3 files (*.mp3)|*.mp3";
            dlg.FilterIndex = 1;
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() == true)
            {
                string selectedFileName = dlg.FileName;

                string path = "C:\\Users\\kosty\\source\\repos\\MusicStreamWPF\\MusicStreamWPF\\musicfolder\\" + System.IO.Path.GetFileName(selectedFileName);
                if (!System.IO.File.Exists(path))
                {
                    System.IO.File.Copy(selectedFileName, path, true);
                }

                filepath = path;
            }
        }
    }
}
   