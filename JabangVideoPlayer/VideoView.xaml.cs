using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;

namespace JabangVideoPlayer
{
    /// <summary>
    /// Interaction logic for VideoView.xaml
    /// </summary>
    public partial class VideoView : Window
    {
        private Video _video;
        private VideoPlayer _videoPlayer;
        private VideoPlayerControls _videoPlayerControls;
        bool videoEnded = false;

        public VideoView(string filePath)
        {
            InitializeComponent();
            _videoPlayer = new VideoPlayer(VPlayer);
            _video = new Video { FilePath = filePath};
            _videoPlayer.LoadVideo(_video);
            _videoPlayerControls = new VideoPlayerControls(_videoPlayer.Player, _videoPlayer);
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
            Status.Visibility = Visibility.Visible;
            Select.Visibility = Visibility.Visible;
            videoEnded = true;
            _videoPlayerControls.videosPlaying = false;
            Play.Content = "▶";
            Application.Current.MainWindow.Height = 600;
            Application.Current.MainWindow.Width = 800;
        }

        private void UpdateTimeline(double value, double maxValue)
        {
            Timeline.Value = value;
            Timeline.Maximum = maxValue;
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            Play.Content = _videoPlayerControls.PlayOrPause(Play.Content.ToString());

            if (videoEnded) 
            {
                PreLoadVideo();
            }
        }

        private void PreLoadVideo()
        {
            videoEnded = false;
            Play.Content = "⏸";
            _videoPlayer.Player.Visibility = Visibility.Visible;
            Status.Visibility = Visibility.Collapsed;
            Select.Visibility = Visibility.Collapsed;
            _videoPlayer.LoadVideo(_video);
            _videoPlayerControls.videosPlaying = true;
        }

        private void Timeline_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _videoPlayerControls.UpdatePosition(e.NewValue);
        }

        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_videoPlayerControls != null)
            {
                _videoPlayerControls.Volume_ValueChanged(e.NewValue);
            }
        }

        private void Select_Click(object sender, RoutedEventArgs e)
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
