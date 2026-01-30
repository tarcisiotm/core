using UnityEngine;

namespace TG.Core.Audio
{
    [CreateAssetMenu(menuName = "TG Core/Audio/Behaviours/AudioPausePlaybackBehaviour")]
    public class AudioPausePlaybackBehaviour : AudioGamePauseEventBehaviour
    {
        protected override void GamePausedEvent(AudioBase audioBase, bool paused)
        {
            if (paused)
            {
                audioBase.AudioSource.Pause(); 
            }
            else if (wasPlaying) 
            {
                audioBase.AudioSource.UnPause();
            }
        }
    }
}
