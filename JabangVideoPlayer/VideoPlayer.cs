using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace JabangVideoPlayer
{
    public class VideoPlayer
    {
        public MediaElement Player { get; set; }
        public Video CurrentVideo { get; set; }

        public event Action<double, double, TimeSpan, TimeSpan> PositionChanged;

        public double MaxValue { get; private set; }
        public double Value { get; private set; }
        public TimeSpan TimeRemaining { get; private set; }
        public TimeSpan ElapsedTime { get; private set; }

        public bool VideosPlaying = true;

        public VideoPlayer(MediaElement player) 
        {
            CurrentVideo = new Video();
            Player = player;
            Player.MediaEnded += Player_MediaEnded;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.2);
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (Player.NaturalDuration.HasTimeSpan)
            {
                ElapsedTime = Player.Position;
                TimeRemaining = Player.NaturalDuration.TimeSpan - Player.Position;
                MaxValue = Player.NaturalDuration.TimeSpan.TotalSeconds;
                Value = Player.Position.TotalSeconds;
                PositionChanged?.Invoke(Value, MaxValue, TimeRemaining, ElapsedTime);
            }
        }

        public void ResetPlayer()
        {
            Player.Position = TimeSpan.FromSeconds(0);
            Player.Pause();
        }

        public void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            ResetPlayer();
        }

        public void LoadVideo(Video video)
        {
            if (video != null)
            {
                Player.Source = new Uri(video.FilePath);
                Player.Play();
            }
        }

        public void Play() { Player.Play(); }
        public void Pause() { Player.Pause();}
    }
}
