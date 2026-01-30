using TG.Core.Audio;

namespace TG.Core.Audio
{
    public interface IAudioPlaybackEndedBehaviour
    {
        void OnAudioPlaybackEnded(AudioBase audioBase);
    }
}
