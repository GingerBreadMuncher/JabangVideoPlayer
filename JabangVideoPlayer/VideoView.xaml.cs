using Microsoft.Win32;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace JabangVideoPlayer
{
    /// <summary>
    /// Interaction logic for VideoView.xaml
    /// </summary>
    public partial class VideoView : Window
    {
        MainWindow mainWindow = new MainWindow();
        private string _filePath;
        bool videosPlaying = true;
        bool videoEnded = false;

        public VideoView(string filePath)
        {
            InitializeComponent();
            _filePath = filePath;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.01);
            timer.Tick += Timer_Tick;
            PlayVideo();
            timer.Start();
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


        public void PlayVideo()
        {
            if (_filePath != null)
            {
                videosPlaying = true;
                Play.Content = "⏸";
                VPlayer.Source = new Uri(_filePath);
                VPlayer.Play();
            }
        }

        private void VPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            VPlayer.Position = TimeSpan.FromSeconds(0);
            VPlayer.Pause();
            VPlayer.Visibility = Visibility.Collapsed;
            Status.Visibility = Visibility.Visible;
            Select.Visibility = Visibility.Visible;
            videoEnded = true;
            videosPlaying = false;
            Play.Content = "▶";
            Application.Current.MainWindow.Height = 600;
            Application.Current.MainWindow.Width = 800;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (VPlayer.NaturalDuration.HasTimeSpan)
            {
                Timeline.Maximum = VPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                Timeline.Value = VPlayer.Position.TotalSeconds;
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (videosPlaying)
            {
                VPlayer.Pause();
                Play.Content = "▶";
                videosPlaying = false;
            }
            else
            {
                VPlayer.Play();
                Play.Content = "⏸";
                videosPlaying = true;
            }

            if (videoEnded) 
            {
                VideoEnded();
            }
        }

        private void VideoEnded()
        {
            videoEnded = false;
            VPlayer.Visibility = Visibility.Visible;
            Status.Visibility = Visibility.Collapsed;
            Select.Visibility = Visibility.Collapsed;
            PlayVideo();
        }

        private void Timeline_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VPlayer.Position = TimeSpan.FromSeconds(Timeline.Value);
        }

        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VPlayer.Volume = Volume.Value / 100;
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                _filePath = openFileDialog.FileName;
                VideoEnded();
                PlayVideo();
            }
        }
    }
}
