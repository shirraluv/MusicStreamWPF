using MusicStreamWPF.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace MusicStreamWPF.UserControls
{
    /// <summary>
    /// Логика взаимодействия для PlaylistItem.xaml
    /// </summary>
    public partial class PlaylistItem : UserControl


    {
        public PlaylistItem()
        {
            InitializeComponent();

            DataContext = this;
        }
        
       
        public string playlistname { get; set; }
        public string playlistimage { get; set; }

        public void SetData()
        {
            PlaylistName.Text = playlistname;
            PlaylistImage.Source = new BitmapImage(new Uri(playlistimage));
        }
    }
}