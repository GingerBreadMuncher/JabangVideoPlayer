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
        public string PlayOrPause()
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

        public void Volume_ValueChanged(double value)
        {
            if (_videoPlayer != null && _videoPlayer.Player != null)
            {
                _videoPlayer.Player.Volume = value / 100;
            }
        }
    }
}
