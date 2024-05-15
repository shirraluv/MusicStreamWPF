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
    /// Логика взаимодействия для EditAlbum.xaml
    /// </summary>
    public partial class EditAlbum : Page
    {
        string imagepath;
        public AlbumDTO SelectedAlbum { get; set; }
        public EditAlbum(AlbumDTO selectedAlbum)
        {
            InitializeComponent();
            SelectedAlbum = selectedAlbum;
            imagepath = selectedAlbum.Imagesource;
            DataContext = this;

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

        private async void EditButton(object sender, RoutedEventArgs e)
        {

            if (NameTextBox.Text == null)
            {
                ErrorLabel.Content = "Введите название трека";
            }
            else if (imagepath == null)
            {
                ErrorLabel.Content = "Выберите превью трека";
            }
            else
            {
                using (HttpClient client2 = new HttpClient())
                {
                    var album = new AlbumDTO
                    {
                        Id = SelectedAlbum.Id,
                        Name = NameTextBox.Text,
                        Duration = SelectedAlbum.Duration,
                        Date = SelectedAlbum.Date,
                        Creatorid = SelectedAlbum.Creatorid,
                        Imagesource = imagepath,
                    };
                    int id = SelectedAlbum.Id;
                    await EditAlbumLol(album);
                }
            }
        }
        private async Task EditAlbumLol(AlbumDTO album)
        {

            var json = JsonConvert.SerializeObject(album);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                var response = await client.PutAsync($"https://localhost:7004/Album/UpdateAlbum?id={album.Id}", data);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Альбом успешно изменен!");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Ошибка изменения альбома: {errorContent}");
                }
            }
        }
    }
}
