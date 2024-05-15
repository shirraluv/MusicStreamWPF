using MusicStreamWPF.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Controls.Primitives;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace MusicStreamWPF
{
    /// <summary>
    /// Логика взаимодействия для PlaylistMusic.xaml
    /// </summary>
    public partial class PlaylistMusic : Page
    {
        int thisopenalbum;
        string albumname;
        string albumimage;
        string perfomname;
        public List<TrackDTO> tracks;
        public PlaylistMusic(int openalbum)
        {
            InitializeComponent();
            thisopenalbum = openalbum;
            DataContext = this;
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadData();
        }
        BitmapImage imagelol = new BitmapImage();
        public async Task LoadData()
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage responseproduct = await client.GetAsync($"https://localhost:7004/Album/GetAlbum?id={thisopenalbum}");

                if (responseproduct.IsSuccessStatusCode)
                {
                    string responseBody = await responseproduct.Content.ReadAsStringAsync();

                    AlbumDTO nowalbum = JsonConvert.DeserializeObject<AlbumDTO>(responseBody);
                    albumname = nowalbum.Name;
                    albumimage = nowalbum.Imagesource;
                    sss.Text = albumname;
                    int performeridd = nowalbum.Creatorid;
                    imagelol = new BitmapImage(new Uri(albumimage, UriKind.RelativeOrAbsolute));
                    ddd.ImageSource = imagelol;
                    int albumidss = nowalbum.Id;

                    HttpClient client1 = new HttpClient();
                    HttpResponseMessage responseproduct1 = await client1.GetAsync($"https://localhost:7004/Performer/GetPerformer?id={performeridd}");

                    if (responseproduct1.IsSuccessStatusCode)
                    {
                        string responseBody1 = await responseproduct1.Content.ReadAsStringAsync();

                        PerformerDTO nowperfom = JsonConvert.DeserializeObject<PerformerDTO>(responseBody1);
                        perfomname = nowperfom.Nick;
                        sss1.Text = perfomname;

                        HttpClient client2 = new HttpClient();
                        HttpResponseMessage responseproduct2 = await client2.GetAsync($"https://localhost:7004/Track/GetAlbumTracks?albumid={albumidss}");

                        if (responseproduct2.IsSuccessStatusCode)
                        {
                            string responseBody2 = await responseproduct2.Content.ReadAsStringAsync();


                            listboxtracks.ItemsSource = tracks;
                            tracks = JsonConvert.DeserializeObject<List<TrackDTO>>(responseBody2);
                            foreach (TrackDTO track in tracks)
                            {

                                listboxtracks.Items.Add(track);


                            }

                        }


                    }


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
        }

        private async void trackchange(object sender, SelectionChangedEventArgs e)
        {
            if (listboxtracks.SelectedItem != null)
            {
                ListBox listboxtracks1 = sender as ListBox;
                TrackDTO selectedTrack = listboxtracks1.SelectedItem as TrackDTO;
                string path = selectedTrack.Filename;
                string nowplayedtrack = selectedTrack.Name;
                string image = selectedTrack.Imagesource;
                int albumid = selectedTrack.Albumid;
                int currentIndex = listboxtracks.SelectedIndex;
                int index2 = listboxtracks.SelectedIndex;
                int index = listboxtracks.Items.IndexOf(selectedTrack);


                HttpClient client = new HttpClient();
                HttpResponseMessage responseproduct = await client.GetAsync($"https://localhost:7004/Album/GetAlbum?id={albumid}");

                if (responseproduct.IsSuccessStatusCode)
                {
                    string responseBody = await responseproduct.Content.ReadAsStringAsync();

                    AlbumDTO albumtrack = JsonConvert.DeserializeObject<AlbumDTO>(responseBody);

                    int creatoridd = albumtrack.Creatorid;

                    HttpClient client1 = new HttpClient();
                    HttpResponseMessage responseproduct1 = await client1.GetAsync($"https://localhost:7004/Performer/GetPerformer?id={creatoridd}");

                    if (responseproduct1.IsSuccessStatusCode)
                    {
                        string responseBody1 = await responseproduct1.Content.ReadAsStringAsync();

                        PerformerDTO performeralbum = JsonConvert.DeserializeObject<PerformerDTO>(responseBody1);
                        string nowplayedperformer = performeralbum.Nick;
                        MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                        if (mainWindow != null)
                        {
                            mainWindow.playtrackpm(path, nowplayedtrack, nowplayedperformer, image, currentIndex, thisopenalbum);
                        }
                    }

                }
            }
        }
        public async void Next(int nowplayedindex)
        {
            HttpClient client3 = new HttpClient();
            HttpResponseMessage responseproduct3 = await client3.GetAsync($"https://localhost:7004/Track/GetAllTracks");

            if (responseproduct3.IsSuccessStatusCode)
            {
                string responseBody3 = await responseproduct3.Content.ReadAsStringAsync();


                tracks = JsonConvert.DeserializeObject<List<TrackDTO>>(responseBody3);
                foreach (TrackDTO track in tracks)
                {

                    listboxtracks.Items.Add(track);


                }

            }

            nowplayedindex++;
            if (nowplayedindex < listboxtracks.Items.Count)
            {
                listboxtracks.SelectedIndex = nowplayedindex;
                TrackDTO selectedTrack = (TrackDTO)listboxtracks.SelectedItem;
                string path = selectedTrack.Filename;
                string nowplayedtrack = selectedTrack.Name;
                string image = selectedTrack.Imagesource;
                int albumid = selectedTrack.Albumid;



                int currentIndex = listboxtracks.SelectedIndex;

                HttpClient client = new HttpClient();
                HttpResponseMessage responseproduct = await client.GetAsync($"https://localhost:7004/Album/GetAlbum?id={albumid}");

                if (responseproduct.IsSuccessStatusCode)
                {
                    string responseBody = await responseproduct.Content.ReadAsStringAsync();

                    AlbumDTO albumtrack = JsonConvert.DeserializeObject<AlbumDTO>(responseBody);

                    int creatoridd = albumtrack.Creatorid;

                    HttpClient client1 = new HttpClient();
                    HttpResponseMessage responseproduct1 = await client1.GetAsync($"https://localhost:7004/Performer/GetPerformer?id={creatoridd}");

                    if (responseproduct1.IsSuccessStatusCode)
                    {
                        string responseBody1 = await responseproduct1.Content.ReadAsStringAsync();

                        PerformerDTO performeralbum = JsonConvert.DeserializeObject<PerformerDTO>(responseBody1);
                        string nowplayedperformer = performeralbum.Nick;
                        MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                        if (mainWindow != null)
                        {
                            mainWindow.playtrackpm(path, nowplayedtrack, nowplayedperformer, image, currentIndex, thisopenalbum);
                        }

                    }
                }
            }
            else
            {
                int firstitem = 0;
                var item = listboxtracks.Items[firstitem];
                TrackDTO selectedTrack = listboxtracks.SelectedItem as TrackDTO;
                string path = selectedTrack.Filename;
                string nowplayedtrack = selectedTrack.Name;
                string image = selectedTrack.Imagesource;
                int albumid = selectedTrack.Albumid;


                int currentIndex = listboxtracks.SelectedIndex;

                HttpClient client = new HttpClient();
                HttpResponseMessage responseproduct = await client.GetAsync($"https://localhost:7004/Album/GetAlbum?id={albumid}");

                if (responseproduct.IsSuccessStatusCode)
                {
                    string responseBody = await responseproduct.Content.ReadAsStringAsync();

                    AlbumDTO albumtrack = JsonConvert.DeserializeObject<AlbumDTO>(responseBody);

                    int creatoridd = albumtrack.Creatorid;

                    HttpClient client1 = new HttpClient();
                    HttpResponseMessage responseproduct1 = await client1.GetAsync($"https://localhost:7004/Performer/GetPerformer?id={creatoridd}");

                    if (responseproduct1.IsSuccessStatusCode)
                    {
                        string responseBody1 = await responseproduct1.Content.ReadAsStringAsync();

                        PerformerDTO performeralbum = JsonConvert.DeserializeObject<PerformerDTO>(responseBody1);
                        string nowplayedperformer = performeralbum.Nick;
                        MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                        if (mainWindow != null)
                        {
                            mainWindow.playtrackpm(path, nowplayedtrack, nowplayedperformer, image, currentIndex, thisopenalbum);
                        }

                    }
                }
            }
        }
        public async void Previous(int nowplayedindex)
        {
            HttpClient client3 = new HttpClient();
            HttpResponseMessage responseproduct3 = await client3.GetAsync($"https://localhost:7004/Track/GetAllTracks");

            if (responseproduct3.IsSuccessStatusCode)
            {
                string responseBody3 = await responseproduct3.Content.ReadAsStringAsync();


                tracks = JsonConvert.DeserializeObject<List<TrackDTO>>(responseBody3);
                foreach (TrackDTO track in tracks)
                {

                    listboxtracks.Items.Add(track);


                }

            }

            nowplayedindex--;
            if (nowplayedindex >= 1)
            {
                listboxtracks.SelectedIndex = nowplayedindex;
                TrackDTO selectedTrack = (TrackDTO)listboxtracks.SelectedItem;
                string path = selectedTrack.Filename;
                string nowplayedtrack = selectedTrack.Name;
                string image = selectedTrack.Imagesource;
                int albumid = selectedTrack.Albumid;



                int currentIndex = listboxtracks.SelectedIndex;

                HttpClient client = new HttpClient();
                HttpResponseMessage responseproduct = await client.GetAsync($"https://localhost:7004/Album/GetAlbum?id={albumid}");

                if (responseproduct.IsSuccessStatusCode)
                {
                    string responseBody = await responseproduct.Content.ReadAsStringAsync();

                    AlbumDTO albumtrack = JsonConvert.DeserializeObject<AlbumDTO>(responseBody);

                    int creatoridd = albumtrack.Creatorid;

                    HttpClient client1 = new HttpClient();
                    HttpResponseMessage responseproduct1 = await client1.GetAsync($"https://localhost:7004/Performer/GetPerformer?id={creatoridd}");

                    if (responseproduct1.IsSuccessStatusCode)
                    {
                        string responseBody1 = await responseproduct1.Content.ReadAsStringAsync();

                        PerformerDTO performeralbum = JsonConvert.DeserializeObject<PerformerDTO>(responseBody1);
                        string nowplayedperformer = performeralbum.Nick;
                        MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                        if (mainWindow != null)
                        {

                            mainWindow.playtrackpm(path, nowplayedtrack, nowplayedperformer, image, currentIndex, thisopenalbum);
                        }

                    }
                }
            }
            else
            {
                int lastitem = listboxtracks.Items.Count - 1;
                listboxtracks.SelectedIndex = lastitem;
                TrackDTO selectedTrack = (TrackDTO)listboxtracks.SelectedItem;
                string path = selectedTrack.Filename;
                string nowplayedtrack = selectedTrack.Name;
                string image = selectedTrack.Imagesource;
                int albumid = selectedTrack.Albumid;


                int currentIndex = listboxtracks.SelectedIndex;

                HttpClient client = new HttpClient();
                HttpResponseMessage responseproduct = await client.GetAsync($"https://localhost:7004/Album/GetAlbum?id={albumid}");

                if (responseproduct.IsSuccessStatusCode)
                {
                    string responseBody = await responseproduct.Content.ReadAsStringAsync();

                    AlbumDTO albumtrack = JsonConvert.DeserializeObject<AlbumDTO>(responseBody);

                    int creatoridd = albumtrack.Creatorid;

                    HttpClient client1 = new HttpClient();
                    HttpResponseMessage responseproduct1 = await client1.GetAsync($"https://localhost:7004/Performer/GetPerformer?id={creatoridd}");

                    if (responseproduct1.IsSuccessStatusCode)
                    {
                        string responseBody1 = await responseproduct1.Content.ReadAsStringAsync();

                        PerformerDTO performeralbum = JsonConvert.DeserializeObject<PerformerDTO>(responseBody1);
                        string nowplayedperformer = performeralbum.Nick;
                        MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                        if (mainWindow != null)
                        {

                            mainWindow.playtrackpm(path, nowplayedtrack, nowplayedperformer, image, currentIndex, thisopenalbum);
                        }

                    }
                }
            }
        }
    }
}
