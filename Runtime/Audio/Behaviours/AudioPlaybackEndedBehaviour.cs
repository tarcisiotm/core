using UnityEngine;

namespace TG.Core.Audio
{
    public abstract class AudioPlaybackEndedBehaviour : ScriptableObject, IAudioPlaybackEndedBehaviour
    {
        public void OnAudioPlaybackEnded(AudioBase audioBase)
        {
            DoOnAudioPlaybackEnded(audioBase);
        }

        protected abstract void DoOnAudioPlaybackEnded(AudioBase audioBase);
    }
}
