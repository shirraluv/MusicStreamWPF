using MusicStreamWPF.AddPages;
using MusicStreamWPF.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
using System.Windows.Threading;

namespace MusicStreamWPF
{
    /// <summary>
    /// Логика взаимодействия для PerformerCard.xaml
    /// </summary>
    /// 
    public class PerformerToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int intValue = (int)value;
            return intValue == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class PerformerCard : Page
    {
        int nowperformerid;
        int logtrueorfalse = 0;
        public List<TrackDTO> tracks;
        public List<AlbumDTO> albums;
        private DispatcherTimer timer;
        string perfomname;
        int auditionss;
        string perfomnameimage;
        
        BitmapImage imagelol = new BitmapImage();
        public PerformerCard(int performerid, int Performer)
        {
            InitializeComponent();
            nowperformerid = performerid;
            logtrueorfalse = Performer;
            ShowButtonIfPerformerIsNotEmpty();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            OnPropertyChanged("ButtonVisibility");
        }
        public Visibility ButtonVisibility
        {
            get
            {
                return logtrueorfalse == 0 ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void ShowButtonIfPerformerIsNotEmpty()
        {
            if (logtrueorfalse != 0)
            {
                // Показываем кнопку
                albumm.Visibility = Visibility.Visible;
                trackk.Visibility = Visibility.Visible;
                

            }
            else
            {
                // Скрываем кнопку
                albumm.Visibility = Visibility.Collapsed;
                trackk.Visibility = Visibility.Collapsed;
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadData();
        }

        public async Task LoadData()
        {

            HttpClient client = new HttpClient();
            HttpResponseMessage responseproduct = await client.GetAsync($"https://localhost:7004/Album/GetPerformerAlbums?creatorid={nowperformerid}");

            if (responseproduct.IsSuccessStatusCode)
            {
                string responseBody = await responseproduct.Content.ReadAsStringAsync();

                albums = JsonConvert.DeserializeObject<List<AlbumDTO>>(responseBody);
                foreach (AlbumDTO album in albums)
                {
                    listboxalbums.Items.Add(album);
                    HttpClient client1 = new HttpClient();
                    HttpResponseMessage responseproduct1 = await client1.GetAsync($"https://localhost:7004/Track/GetAlbumTracks?albumid={album.Id}");

                    if (responseproduct1.IsSuccessStatusCode)
                    {
                        string responseBody1 = await responseproduct1.Content.ReadAsStringAsync();


                        
                        tracks = JsonConvert.DeserializeObject<List<TrackDTO>>(responseBody1);
                        foreach (TrackDTO track in tracks)
                        {

                            listboxtracks.Items.Add(track);


                        }

                    }
                }
            }
            HttpClient client2 = new HttpClient();
            HttpResponseMessage responseproduct2 = await client2.GetAsync($"https://localhost:7004/Performer/GetPerformer?id={nowperformerid}");

            if (responseproduct2.IsSuccessStatusCode)
            {
                string responseBody2 = await responseproduct2.Content.ReadAsStringAsync();

                PerformerDTO nowperfom = JsonConvert.DeserializeObject<PerformerDTO>(responseBody2);
                perfomname = nowperfom.Nick;
                sss.Text = perfomname;
                perfomnameimage = nowperfom.Imagesource;
                auditionss = nowperfom.Auditions;
                sss1.Text = auditionss.ToString();
                
                imagelol = new BitmapImage(new Uri(perfomnameimage, UriKind.RelativeOrAbsolute));
                ddd.ImageSource = imagelol;
                
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
                            mainWindow.playtrackpc(path, nowplayedtrack, nowplayedperformer, image, currentIndex, nowperformerid);
                        }
                    }

                }
            }
        }

        private void openalbum(object sender, SelectionChangedEventArgs e)
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
                            mainWindow.playtrackpc(path, nowplayedtrack, nowplayedperformer, image, currentIndex, nowperformerid);
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
                            mainWindow.playtrackpc(path, nowplayedtrack, nowplayedperformer, image, currentIndex, nowperformerid);
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

                            mainWindow.playtrackpc(path, nowplayedtrack, nowplayedperformer, image, currentIndex, nowperformerid);
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

                            mainWindow.playtrackpc(path, nowplayedtrack, nowplayedperformer, image, currentIndex, nowperformerid);
                        }

                    }
                }
            }
        }

        

        private void AddAlbum(object sender, RoutedEventArgs e)
        {
            int nowcreatorid = logtrueorfalse;
            AddAlbum addAlbum = new AddAlbum(nowcreatorid);
            Window mainWindow = Window.GetWindow(this);

            // Получаем Frame из MainWindow
            Frame mainFrame = mainWindow.FindName("mainFrame") as Frame;

            // Устанавливаем содержимое Frame на PerformerCard
            mainFrame.Navigate(addAlbum);
        }

        private void AddTrack(object sender, RoutedEventArgs e)
        {
            int nowcreatorid = logtrueorfalse;
            AddTrack addTrack = new AddTrack(nowcreatorid);
            Window mainWindow = Window.GetWindow(this);

            // Получаем Frame из MainWindow
            Frame mainFrame = mainWindow.FindName("mainFrame") as Frame;

            // Устанавливаем содержимое Frame на PerformerCard
            mainFrame.Navigate(addTrack);
        }

        private void EditSelectedAlbum(object sender, RoutedEventArgs e)
        {
            AlbumDTO selectedAlbum = (AlbumDTO)listboxalbums.SelectedItem;
            EditAlbum editAlbumPage = new EditAlbum(selectedAlbum);
            Window mainWindow = Window.GetWindow(this);
            Frame mainFrame = mainWindow.FindName("mainFrame") as Frame;

            mainFrame.Navigate(editAlbumPage);
        }

        private void EditSelectedTrack(object sender, RoutedEventArgs e)
        {
            TrackDTO selectedTrack = (TrackDTO)listboxtracks.SelectedItem;
            EditTrack editTrackPage = new EditTrack(selectedTrack);
            Window mainWindow = Window.GetWindow(this);
            Frame mainFrame = mainWindow.FindName("mainFrame") as Frame;

            mainFrame.Navigate(editTrackPage);

        }

        private async void DeleteSelectedAlbum(object sender, RoutedEventArgs e)
        {
            AlbumDTO selectedAlbum = (AlbumDTO)listboxalbums.SelectedItem;
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.DeleteAsync($"https://localhost:7004/Track/DeleteTracksByAlbumId?albumId={selectedAlbum.Id}");

            if (response.IsSuccessStatusCode)
            {
                HttpResponseMessage responseproduct = await client.DeleteAsync($"https://localhost:7004/Track/DeleteTrack?id={selectedAlbum.Id}");
                if (responseproduct.IsSuccessStatusCode)
                {
                    MessageBox.Show("Альбом успешно удален");
                }
            }


        }

        private async void DeleteSelectedTrack(object sender, RoutedEventArgs e)
        {
            TrackDTO selectedTrack = (TrackDTO)listboxtracks.SelectedItem;
            HttpClient client = new HttpClient();
            HttpResponseMessage responseproduct = await client.DeleteAsync($"https://localhost:7004/Track/DeleteTrack?id={selectedTrack.Id}");

            if (responseproduct.IsSuccessStatusCode)
            {
                MessageBox.Show("Трек успешно удален");
            }
        }
    }
}


