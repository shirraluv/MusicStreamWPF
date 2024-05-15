using MusicStreamWPF.DTO;
using MusicStreamWPF.LoginPages;
using MusicStreamWPF.UserControls;
using NAudio.Gui;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
namespace MusicStreamWPF
{

    public enum PlayMethod
    {
        PlayTrack,
        PlayTrackPM,
        PlayTrackPC
    }

    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility != Visibility.Visible;
            }
            return false;
        }
    }
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }
            return false;
        }
    }
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public PlayMethod currentPlayMethod { get; private set;  }
        int performerid;
        int openalbum;
        string nowplayedname1;
        string nowplayedcreator1;
        string nowplayedimage1;
        int userid;
        private DispatcherTimer timer;
        private MusicPage musicPage;
        private PlaylistMusic playlistMusic;
        private PerformerCard performerCard;
        int volumenow;
        public string nickname1;

        BitmapImage imagelol = new BitmapImage();
        public MainWindow(int User)
        {
            InitializeComponent();
            musicPage = new MusicPage();
            performerCard = new PerformerCard(performerid, User);
            playlistMusic = new PlaylistMusic(openalbum);
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromTicks(10);
            timer.Tick += Timer_Tick;
            timer.Start();
            userid = User;
            volumeSlider.Value = 50.0;
            
            

            mediaPlayer.MediaOpened += (sender, args) =>
            {
                IsPlaying = true;
            };

            mediaPlayer.MediaEnded += (sender, args) =>
            {
                IsPlaying = false;
            };
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
            HttpResponseMessage response = await client.GetAsync($"https://localhost:7004/User/GetUser?id={userid}");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                UserDTO nowuser = JsonConvert.DeserializeObject<UserDTO>(json);
                    nickname1 = nowuser.Nick;
                    sss22.Text = nickname1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
        }

      

        
        private void GoToMain(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new System.Uri("Main.xaml", System.UriKind.Relative));
        }

        public void GoToPlaylistMusic(int openalbum)
        {
            PlaylistMusic playlistsPage = new PlaylistMusic(openalbum);
            mainFrame.Navigate(playlistsPage);
        }

        private void GoToPlaylist(object sender, RoutedEventArgs e)
        {
            Playlists playlistsPage = new Playlists(userid);
            mainFrame.Navigate(playlistsPage);
        }

        private void GoToMusicPage(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new System.Uri("MusicPage.xaml", System.UriKind.Relative));
        }
        int nowplayedindex1;
        MediaPlayer mediaPlayer = new MediaPlayer();
        public void playtrack(string path, string nowplayedtrack, string nowplayedperformer, string image, int currentIndex)
        {
            if (mediaPlayer != null)
            {
                mediaPlayer.Stop();
                mediaPlayer.Close();
            }
            mediaPlayer = new MediaPlayer();
            mediaPlayer.Open(new Uri(path, UriKind.RelativeOrAbsolute));
            mediaPlayer.Play();
            IsPlaying = true;
            nowplayedname1 = nowplayedtrack;
            nowplayedcreator1 = nowplayedperformer;
            nowplayedimage1 = image;
            nowplayedindex1 = currentIndex;
            

            imagelol = new BitmapImage(new Uri(nowplayedimage1, UriKind.RelativeOrAbsolute));
            nowplayedimagee.ImageSource = imagelol;
            currentPlayMethod = PlayMethod.PlayTrack;
            
        }

        public class TimeSpanConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is double seconds)
                {
                    return TimeSpan.FromSeconds(seconds);
                }
                return TimeSpan.Zero;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return null;
            }
        }

        public void stoptrack(string path)
        {

            mediaPlayer.Stop();
        }


       

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Position = TimeSpan.FromSeconds(slider.Value);
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            
            slider.Value = mediaPlayer.Position.TotalSeconds;
            nowplayednamee.Text = nowplayedname1;
            nowplayedcreatorr.Text = nowplayedcreator1;
            wow.Text = volumenow.ToString();
            



        }

        private bool _isPlaying;
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                _isPlaying = value;
                OnPropertyChanged(nameof(IsPlaying));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void PauseClick(object sender, RoutedEventArgs e)
        {
            
            mediaPlayer.Pause();
            IsPlaying = false;
        }

        private void PlayClick(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
            IsPlaying = true;
        }

        private void SkipNext(object sender, RoutedEventArgs e)
        {
            PlayMethod currentmethod = GetCurrentPlayMethod();
            if (currentPlayMethod == PlayMethod.PlayTrack)
            {
                if (musicPage != null)
                {
                    int nowplayedindex = nowplayedindex1;
                    musicPage.Next(nowplayedindex);
                }
            }
            if (currentPlayMethod == PlayMethod.PlayTrackPM)
            {
                if (playlistMusic != null)
                {
                    int nowplayedindex = nowplayedindex1;
                    playlistMusic.Next(nowplayedindex);
                }
            }
            if (currentPlayMethod == PlayMethod.PlayTrackPC)
            {
                if (performerCard != null)
                {
                    int nowplayedindex = nowplayedindex1;
                    performerCard.Next(nowplayedindex);
                }
            }
        }

        private void SkipPrevious(object sender, RoutedEventArgs e)
        {
            PlayMethod currentmethod = GetCurrentPlayMethod();
            if (GetCurrentPlayMethod() == PlayMethod.PlayTrack)
            {
                if (musicPage != null)
                {
                    int nowplayedindex = nowplayedindex1;
                    musicPage.Previous(nowplayedindex);
                }
            }
            if (GetCurrentPlayMethod() == PlayMethod.PlayTrackPM)
            {
                if (playlistMusic != null)
                {
                    int nowplayedindex = nowplayedindex1;
                    playlistMusic.Previous(nowplayedindex);
                }
            }
            if (GetCurrentPlayMethod() == PlayMethod.PlayTrackPC)
            {
                if (performerCard != null)
                {
                    int nowplayedindex = nowplayedindex1;
                    performerCard.Previous(nowplayedindex);
                }
            }
        }

        private async void TextBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string nowperformername = nowplayedcreatorr.Text;
            HttpClient client1 = new HttpClient();
            HttpResponseMessage response1 = await client1.GetAsync($"https://localhost:7004/Performer/GetPerformerByName?Name={nowperformername}");
            if (response1.IsSuccessStatusCode)
            {
                string json1 = await response1.Content.ReadAsStringAsync();
                PerformerDTO nowperformer = JsonConvert.DeserializeObject<PerformerDTO>(json1);
                int performerid = nowperformer.Id;
                int Performer = 0;
                PerformerCard performercard = new PerformerCard(performerid, Performer);
                mainFrame.Navigate(performercard);
            }
        }

        private void TextBlock_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            if (textBlock != null)
            {
                SolidColorBrush brush1 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9cc9c9"));
                textBlock.TextDecorations = TextDecorations.Underline;
                textBlock.Foreground = brush1;
            }
        }

        private void TextBlock_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            if (textBlock != null)
            {
                SolidColorBrush brush2 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9cf9c9"));
                textBlock.TextDecorations = null;
                textBlock.Foreground = brush2;
            }
        }

        public void playtrackpm(string path, string nowplayedtrack, string nowplayedperformer, string image, int currentIndex, int thisopenalbum)
        {
            if (mediaPlayer != null)
            {
                mediaPlayer.Stop();
                mediaPlayer.Close();
            }
            mediaPlayer = new MediaPlayer();
            mediaPlayer.Open(new Uri(path, UriKind.RelativeOrAbsolute));
            mediaPlayer.Play();
            IsPlaying = true;
            nowplayedname1 = nowplayedtrack;
            nowplayedcreator1 = nowplayedperformer;
            nowplayedimage1 = image;
            nowplayedindex1 = currentIndex;
            currentPlayMethod = PlayMethod.PlayTrackPM;
            openalbum = thisopenalbum;
            
            imagelol = new BitmapImage(new Uri(nowplayedimage1, UriKind.RelativeOrAbsolute));
            nowplayedimagee.ImageSource = imagelol;
        }

        public void playtrackpc(string path, string nowplayedtrack, string nowplayedperformer, string image, int currentIndex, int nowperformerid)
        {
            if (mediaPlayer != null)
            {
                mediaPlayer.Stop();
                mediaPlayer.Close();
            }
            mediaPlayer = new MediaPlayer();
            mediaPlayer.Open(new Uri(path, UriKind.RelativeOrAbsolute));
            mediaPlayer.Play();
            IsPlaying = true;
            nowplayedname1 = nowplayedtrack;
            nowplayedcreator1 = nowplayedperformer;
            nowplayedimage1 = image;
            nowplayedindex1 = currentIndex;
            currentPlayMethod = PlayMethod.PlayTrackPC;
            performerid = nowperformerid;
            
            imagelol = new BitmapImage(new Uri(nowplayedimage1, UriKind.RelativeOrAbsolute));
            nowplayedimagee.ImageSource = imagelol;
        }
        public PlayMethod GetCurrentPlayMethod()
        {
            return currentPlayMethod;
        }

        private void openvolume(object sender, RoutedEventArgs e)
        {
            if (volumeSlider.Visibility == Visibility.Hidden)
            {
                volumeSlider.Visibility = Visibility.Visible;
            }
            else
            {
                volumeSlider.Visibility = Visibility.Hidden;
            }
        }

        private void slider_VolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Volume = (double)volumeSlider.Value / 100.0;
            volumenow = (int)volumeSlider.Value;
        }

        

        private void GoToArtistPage(object sender, RoutedEventArgs e)
        {
            PerformerLogin performerpage = new PerformerLogin();
            mainFrame.Navigate(performerpage);
        }

        private void Leave(object sender, RoutedEventArgs e)
        {
            LoginPage LoginWindow = new LoginPage();
            LoginWindow.Show();
            Window.GetWindow(this).Close();
        }
    }
}
