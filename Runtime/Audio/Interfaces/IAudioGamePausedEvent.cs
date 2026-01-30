namespace TG.Core.Audio
{
    public interface IAudioGamePausedEvent
    {
        public void OnGamePausedEvent(AudioBase audioBase, bool paused);
    }
}