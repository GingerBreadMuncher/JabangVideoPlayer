using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System;

namespace JabangVideoPlayer
{
    public class Timeline : Slider
    {
        VideoPlayer _videoPlayer;
        MediaElement _mediaElement;
        public bool IsDragging = false;

        public void Initialize(MediaElement mediaElement, VideoPlayer videoPlayer)
        {
            _videoPlayer = new VideoPlayer(mediaElement);
            _mediaElement = mediaElement;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var thumb = GetTemplateChild("PART_Track") as Track;
            if (thumb != null)
            {
                thumb.Thumb.DragStarted += Thumb_DragStarted;
                thumb.Thumb.DragCompleted += Thumb_DragCompleted;
            }
        }

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            _videoPlayer.Pause();
            IsDragging = true;
        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            _videoPlayer.Player.Position = TimeSpan.FromSeconds(this.Value);
            _videoPlayer.Play();
            IsDragging = false;
        }

        public void UpdatePosition(double value)
        {
            if (!IsDragging && _mediaElement != null && _mediaElement.NaturalDuration.HasTimeSpan && _videoPlayer.VideosPlaying)
            {
                _mediaElement.Position = TimeSpan.FromSeconds(value);
            }
        }
    }
}
