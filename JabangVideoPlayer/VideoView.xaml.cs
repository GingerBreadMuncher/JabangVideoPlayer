﻿using Microsoft.Win32;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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
        bool _isFullScreen = false;
        bool _isTitlePopUpEnabled = true;
        bool _isFullScreenPopUpEnabled = true;

        public VideoView(string filePath, string fileName)
        {
            InitializeComponent();
            _videoPlayer = new VideoPlayer(vPlayer);
            _video = new Video { FilePath = filePath, FileName = fileName};
            _controller = new Controller(_videoPlayer.Player, _videoPlayer);
            this.KeyDown += new KeyEventHandler(Window_KeyDown);
            PreLoadVideo();
            _videoPlayer.PositionChanged += UpdateTimeline;
            TitlePopUp(_video.FileName);
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
            MaximizeApp();
        }

        private void MaximizeApp()
        {
            if (this.WindowState != WindowState.Maximized)
            {
                this.SizeToContent = SizeToContent.Manual;
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.SizeToContent = SizeToContent.Width;
                this.WindowState = WindowState.Normal;
            }
        }

        private void MinimizeApp_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void FullScreen_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState != WindowState.Maximized)
            {
                MaximizeApp();
                _isFullScreen = true;
                grid.RowDefinitions[0].Height = new GridLength(0);
                grid.RowDefinitions[1].Height = new GridLength(0);
                grid.RowDefinitions[3].Height = new GridLength(0);
                FullScreenPopUp();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (_isFullScreen == true && e.Key == Key.F11)
            {
                MaximizeApp();
                _isFullScreen = false;
                fullScreenPopUpText.Visibility = Visibility.Collapsed;
                grid.RowDefinitions[0].Height = new GridLength(30);
                grid.RowDefinitions[1].Height = new GridLength(20);
                grid.RowDefinitions[3].Height = new GridLength(50);
            }
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
            this.Height = 600;
            this.Width = 800;
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
                _video.FileName = openFileDialog.SafeFileName;
                PreLoadVideo();
                TitlePopUp(_video.FileName);
            }
        }

        private void TitlePopUp(string fileName)
        {
            if (_isTitlePopUpEnabled) 
            {
                titlePopUpText.Text = fileName;
                Task.Delay(3000).ContinueWith(_ =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        titlePopUpText.Text = "";
                    });
                });
            }
        }

        private void FullScreenPopUp()
        {
            if (_isFullScreenPopUpEnabled)
            {
                fullScreenPopUpText.Visibility = Visibility.Visible;
                Task.Delay(4000).ContinueWith(_ =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        fullScreenPopUpText.Visibility = Visibility.Collapsed;
                    });
                });
            }
        }
        
        private void TitlePopUp_Switch(object sender, RoutedEventArgs e)
        {
            if (_isTitlePopUpEnabled)
            {
                titlePopUp.Header = "Enable title pop-up";
                _isTitlePopUpEnabled = false;
            }
            else
            {
                titlePopUp.Header = "Disable title pop-up";
                _isTitlePopUpEnabled = true;
            }
        }

        private void FullScreenPopUp_Switch(object sender, RoutedEventArgs e)
        {
            if (_isFullScreenPopUpEnabled)
            {
                fullScreenPopUp.Header = "Enable full screen pop-up";
                _isFullScreenPopUpEnabled = false;
            }
            else
            {
                fullScreenPopUp.Header = "Disable full screen pop-up";
                _isFullScreenPopUpEnabled = true;
            }
        }
    }
}
