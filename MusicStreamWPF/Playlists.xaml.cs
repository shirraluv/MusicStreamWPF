using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using MusicStreamWPF.UserControls;
using System.Text;
using System.Threading.Tasks;
using MusicStreamWPF.DTO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Runtime.Remoting.Contexts;
using System.Linq.Expressions;
using System.IO;

namespace MusicStreamWPF
{
    /// <summary>
    /// Логика взаимодействия для Playlists.xaml
    /// </summary>
    public partial class Playlists : Page
    {



        List<PlaylistDTO> playlists = new List<PlaylistDTO>();

        int nowuserid;
        public Playlists(int userid)
        {
            InitializeComponent();
            nowuserid = userid;
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
                HttpResponseMessage response = await client.GetAsync($"https://localhost:7004/Album/GetAllAlbums");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    List<AlbumDTO> albums = JsonConvert.DeserializeObject<List<AlbumDTO>>(json);

                    foreach (AlbumDTO album in albums)
                    {
                        playlistlistbox.Items.Add(album);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
        }

        public PlaylistItem FindPlaylistItemById(int id)
        {
            string playlistItemName = "PlaylistItem" + id;
            return this.FindName(playlistItemName) as PlaylistItem;
        }

        private void playlistpick(object sender, SelectionChangedEventArgs e)
        {
            ListBox playlistlistbox = sender as ListBox;
            AlbumDTO selectedPlaylist = playlistlistbox.SelectedItem as AlbumDTO;
            int openalbum = selectedPlaylist.Id;
            MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow != null)
            {
                mainWindow.GoToPlaylistMusic(openalbum);
            }
        }
    }
}