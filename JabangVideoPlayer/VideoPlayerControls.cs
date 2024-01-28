using System;
using System.Windows.Controls;

namespace JabangVideoPlayer
{
    public class VideoPlayerControls
    {
        private VideoPlayer _videoPlayer;
        public bool videosPlaying = true;
        private MediaElement _mediaElement;

        public VideoPlayerControls(MediaElement mediaElement, VideoPlayer videoPlayer)
        {
            _mediaElement = mediaElement;
            _videoPlayer = videoPlayer;
        }
        public string PlayOrPause(string pausedOrNah)
        {
            if (videosPlaying)
            {
                _videoPlayer.Player.Pause();
                videosPlaying = false;
                return "▶";
            }
            else
            {
                _videoPlayer.Player.Play();
                videosPlaying = true;
                return "⏸";
            }
        }

        public void UpdatePosition(double value)
        {
            if (_mediaElement != null && _mediaElement.NaturalDuration.HasTimeSpan && videosPlaying)
            {
                _mediaElement.Position = TimeSpan.FromSeconds(value);
            }
        }

        public void Volume_ValueChanged(double value)
        {
            if (_videoPlayer != null && _videoPlayer.Player != null)
            {
                _videoPlayer.Player.Volume = value / 100;
            }
        }
    }
}
