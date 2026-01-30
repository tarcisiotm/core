using UnityEngine;

namespace TG.Core.Audio
{
    public abstract class AudioGamePauseEventBehaviour : ScriptableObject, IAudioGamePausedEvent
    {
        protected bool wasPlaying = false;

        public void OnGamePausedEvent(AudioBase audioBase, bool paused)
        {
            wasPlaying = audioBase.AudioSource.isPlaying;
            GamePausedEvent(audioBase, paused);
        }

        protected abstract void GamePausedEvent(AudioBase audioBase, bool paused);
    }
}