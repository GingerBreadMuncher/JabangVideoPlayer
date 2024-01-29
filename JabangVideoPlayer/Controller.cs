using System;
using System.Windows.Controls;

namespace JabangVideoPlayer
{
    public class Controller
    {
        VideoPlayer _videoPlayer;
        MediaElement _mediaElement;

        public Controller(MediaElement mediaElement, VideoPlayer videoPlayer)
        {
            _mediaElement = mediaElement;
            _videoPlayer = videoPlayer;
        }
        public string PlayOrPause(string pausedOrNah)
        {
            if (_videoPlayer.VideosPlaying)
            {
                _videoPlayer.Player.Pause();
                _videoPlayer.VideosPlaying = false;
                return "▶";
            }
            else
            {
                _videoPlayer.Player.Play();
                _videoPlayer.VideosPlaying = true;
                return "⏸";
            }
        }

        public void UpdatePosition(double value)
        {
            if (_mediaElement != null && _mediaElement.NaturalDuration.HasTimeSpan && _videoPlayer.VideosPlaying)
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
