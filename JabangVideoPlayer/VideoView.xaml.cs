using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace JabangVideoPlayer
{
    /// <summary>
    /// Interaction logic for VideoView.xaml
    /// </summary>
    public partial class VideoView : Window
    {
        Video _video;
        VideoPlayer _videoPlayer;
        Controller _controller;
        bool _isVideoEnded = false;

        public VideoView(string filePath)
        {
            InitializeComponent();
            _videoPlayer = new VideoPlayer(vPlayer);
            _video = new Video { FilePath = filePath};
            _controller = new Controller(_videoPlayer.Player, _videoPlayer);
            PreLoadVideo();
            _videoPlayer.LoadVideo(_video);
            _videoPlayer.PositionChanged += UpdateTimeline;
        }

        private void DragBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MaximizeApp_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState != WindowState.Maximized)
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            else
                Application.Current.MainWindow.WindowState = WindowState.Normal;
        }

        private void MinimizeApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void VPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            _videoPlayer.ResetPlayer();
            _videoPlayer.Player.Visibility = Visibility.Collapsed;
            status.Visibility = Visibility.Visible;
            select.Visibility = Visibility.Visible;
            _isVideoEnded = true;
            _videoPlayer.VideosPlaying = false;
            play.Content = "▶";
            Application.Current.MainWindow.Height = 600;
            Application.Current.MainWindow.Width = 800;
        }

        private void UpdateTimeline(double value, double maxValue)
        {
            timeline.Value = value;
            timeline.Maximum = maxValue;
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            play.Content = _controller.PlayOrPause(play.Content.ToString());

            if (_isVideoEnded) 
            {
                PreLoadVideo();
            }
        }

        private void PreLoadVideo()
        {
            _isVideoEnded = false;
            play.Content = "⏸";
            _videoPlayer.Player.Visibility = Visibility.Visible;
            status.Visibility = Visibility.Collapsed;
            select.Visibility = Visibility.Collapsed;
            _videoPlayer.LoadVideo(_video);
            _videoPlayer.VideosPlaying = true;
        }

        private void Timeline_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _controller.UpdatePosition(e.NewValue);
        }

        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_controller != null)
            {
                _controller.Volume_ValueChanged(e.NewValue);
            }
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                _video.FilePath = openFileDialog.FileName;
                PreLoadVideo();
            }
        }
    }
}
