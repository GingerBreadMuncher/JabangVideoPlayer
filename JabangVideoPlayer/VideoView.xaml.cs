using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Animation;

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
        Timeline _timeline;
        DispatcherTimer _hideControlsTimer;
        DispatcherTimer _fullScreenPopUpTimer;
        DispatcherTimer _titlePopUpTimer;
        bool _isVideoEnded = false;
        bool _isFullScreen = false;
        bool _isFolderListEnabled = false;
        bool _isTitlePopUpEnabled = true;
        bool _isFullScreenPopUpEnabled = true;
        Dictionary<string, string> _listFilePaths;

        public VideoView(string filePath, string fileName)
        {
            InitializeComponent();
            _videoPlayer = new VideoPlayer(vPlayer);
            _video = new Video { FilePath = filePath, FileName = fileName };
            _controller = new Controller(_videoPlayer.Player, _videoPlayer);
            _listFilePaths = new Dictionary<string, string>();
            _timeline = new Timeline();
            timeline.Initialize(vPlayer, _videoPlayer);

            _hideControlsTimer = new DispatcherTimer();
            _hideControlsTimer.Interval = TimeSpan.FromSeconds(3);
            _hideControlsTimer.Tick += HideControlsTimer_Tick;

            _fullScreenPopUpTimer = new DispatcherTimer();
            _fullScreenPopUpTimer.Interval = TimeSpan.FromSeconds(4);
            _fullScreenPopUpTimer.Tick += FullScreenPopUpTimer_Tick;

            _titlePopUpTimer = new DispatcherTimer();
            _titlePopUpTimer.Interval = TimeSpan.FromSeconds(3);
            _titlePopUpTimer.Tick += TitlePopUpTimer_Tick;

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
            if (_isFullScreen == false) { EnterFullScreen(); }
            else { ExitFullScreen(); }
        }

        private void EnterFullScreen()
        {
            _isFullScreen = true;
            if (this.WindowState != WindowState.Maximized) { MaximizeApp(); }
            grid.RowDefinitions[0].Height = new GridLength(0);
            grid.RowDefinitions[1].Height = new GridLength(0);
            vPlayer.MouseMove += VPlayer_MouseMove;
            _hideControlsTimer.Stop();
            _hideControlsTimer.Start();
            _fullScreenPopUpTimer.Stop();
            _fullScreenPopUpTimer.Start();
            if (_isFullScreenPopUpEnabled) { fullScreenPopUpText.Visibility = Visibility.Visible; }
        }

        private void ExitFullScreen()
        {
            _isFullScreen = false;
            if (this.WindowState != WindowState.Normal) { MaximizeApp(); }
            fullScreenPopUpText.Visibility = Visibility.Collapsed;
            grid.RowDefinitions[0].Height = new GridLength(30);
            grid.RowDefinitions[1].Height = new GridLength(20);
            _hideControlsTimer.Stop();
            controlBar.Visibility = Visibility.Visible;
        }

        private void HideControlsTimer_Tick(object sender, EventArgs e)
        {
            HideControls();
        }

        private void HideControls()
        {
            if (_isFullScreen)
            {
                controlBar.Visibility = Visibility.Collapsed;
                Cursor = Cursors.None;
                if (_isFolderListEnabled) 
                {
                    grid.ColumnDefinitions[3].Width = new GridLength(0);
                    folderList.Visibility = Visibility.Collapsed;
                }
                if (_hideControlsTimer.IsEnabled) { _hideControlsTimer.Stop(); }
            }
        }

        private void FullScreenPopUpTimer_Tick(object sender, EventArgs e)
        {
            if (_isFullScreenPopUpEnabled)
            {
                fullScreenPopUpText.Visibility = Visibility.Collapsed;
                _hideControlsTimer.Stop();
            }
        }
        
        private void TitlePopUpTimer_Tick(object sender, EventArgs e)
        {
            if (_isTitlePopUpEnabled)
            {
                titlePopUpText.Text = "";
                _titlePopUpTimer.Stop();
            }
        }

        private void TitlePopUp(string fileName)
        {
            if (_isTitlePopUpEnabled)
            {
                titlePopUpText.Text = fileName;
                _titlePopUpTimer.Start();
            }
        }

        private void VPlayer_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isFullScreen)
            {
                controlBar.Visibility = Visibility.Visible;
                Cursor = Cursors.Arrow;
                if (_isFolderListEnabled)
                {
                    grid.ColumnDefinitions[3].Width = new GridLength(250);
                    folderList.Visibility = Visibility.Visible;
                }
                if (!_hideControlsTimer.IsEnabled) { _hideControlsTimer.Start(); }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F11: if (_isFullScreen == true) { ExitFullScreen(); } break;

                case Key.Right: if (vPlayer.NaturalDuration.HasTimeSpan && _videoPlayer.VideosPlaying) 
                    { 
                        vPlayer.Position = vPlayer.Position.Add(TimeSpan.FromSeconds(5)); status.Text = "⏩";
                        Storyboard storyboardRight = status.FindResource("FadeOutAnimation") as Storyboard;
                        if (storyboardRight != null) { storyboardRight.Begin(); };
                    } 
                    break;

                case Key.Left: if (vPlayer.NaturalDuration.HasTimeSpan && _videoPlayer.VideosPlaying)
                    {
                        vPlayer.Position = vPlayer.Position.Add(TimeSpan.FromSeconds(-5)); status.Text = "⏪";
                        Storyboard storyboardLeft = status.FindResource("FadeOutAnimation") as Storyboard;
                        if (storyboardLeft != null) { storyboardLeft.Begin(); };
                    }
                    break;
            }
        }

        private void VPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            SizeToContent = SizeToContent.Manual;
            _videoPlayer.ResetPlayer();
            _videoPlayer.Player.Visibility = Visibility.Collapsed;
            mediaEndedText.Visibility = Visibility.Visible;
            select.Visibility = Visibility.Visible;
            controlBar.Visibility = Visibility.Visible;
            if (_isFolderListEnabled) folderList.Visibility = Visibility.Visible;
            _isVideoEnded = true;
            _videoPlayer.VideosPlaying = false;
            play.Content = "▶";
            mediaEndedText.Text = "";
        }

        private void UpdateTimeline(double value, double maxValue)
        {
            if (!timeline.IsDragging)
            {
                timeline.Value = value;
                timeline.Maximum = maxValue;
            }
            else
            {
                play.Content = "⏸";
                _videoPlayer.VideosPlaying = true;
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            play.Content = _controller.PlayOrPause();

            if (_isVideoEnded)
            {
                PreLoadVideo();
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!controlBar.IsMouseOver && !folderList.IsMouseOver && !dragBorder.IsMouseOver && _isVideoEnded == false)
            {
                play.Content = _controller.PlayOrPause();
                status.Text = play.Content.ToString();
                Storyboard storyboard = status.FindResource("FadeOutAnimation") as Storyboard;
                if (storyboard != null) { storyboard.Begin(); }
            }
        }

        private void PreLoadVideo()
        {
            _isVideoEnded = false;
            play.Content = "⏸";
            _videoPlayer.Player.Visibility = Visibility.Visible;
            mediaEndedText.Visibility = Visibility.Collapsed;
            select.Visibility = Visibility.Collapsed;
            _videoPlayer.LoadVideo(_video);
            _videoPlayer.VideosPlaying = true;
        }

        private void Timeline_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _timeline.UpdatePosition(e.NewValue);
            vPlayer.Position = TimeSpan.FromSeconds(timeline.Value);
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
                SizeToContent = SizeToContent.Width;
                _video.FilePath = openFileDialog.FileName;
                _video.FileName = openFileDialog.SafeFileName;
                PreLoadVideo();
                TitlePopUp(_video.FileName);
            }
        }

        private void OpenFolder(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                _isFolderListEnabled = true;
                folderListBox.Items.Clear();
                grid.ColumnDefinitions[3].Width = new GridLength(250);
                folderList.Visibility = Visibility.Visible;
                string folderPath = dialog.FileName;
                string[] extensions = new string[] { ".mp4", ".avi", ".mp3", ".mkv" };
                IEnumerable<string> files = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories)
                    .Where(file => extensions.Contains(Path.GetExtension(file)));

                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    folderListBox.Items.Add(fileName);
                    _listFilePaths[fileName] = file;
                }
            }
        }

        private void FolderListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedFileName = folderListBox.SelectedItem as string;

            if (!string.IsNullOrEmpty(selectedFileName) && _listFilePaths.TryGetValue(selectedFileName, out string selectedFilePath))
            {
                _video.FilePath = selectedFilePath;
                _video.FileName = selectedFileName;
                PreLoadVideo();
                TitlePopUp(_video.FileName);
            }
        }

        private void CloseList_Click(object sender, RoutedEventArgs e)
        {
            _isFolderListEnabled = false;
            folderListBox.Items.Clear();
            folderList.Visibility = Visibility.Collapsed;
            grid.ColumnDefinitions[3].Width = new GridLength(0);
        }

        private void VPlayer_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_isFullScreen && !controlBar.IsMouseOver && !folderList.IsMouseOver)
            {
                controlBar.Visibility = Visibility.Collapsed;
                if (_isFolderListEnabled)
                {
                    grid.ColumnDefinitions[3].Width = new GridLength(0);
                    folderList.Visibility = Visibility.Collapsed;
                }
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
