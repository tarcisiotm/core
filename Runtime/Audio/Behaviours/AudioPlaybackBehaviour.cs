using UnityEngine;

namespace TG.Core.Audio
{
    public abstract class AudioPlaybackBehaviour : ScriptableObject, IAudioPlayback
    {
        public void OnPlayback(AudioBase audioBase)
        {
            HandlePlayback(audioBase);
        }

        protected abstract void HandlePlayback(AudioBase audioBase);
    }
}