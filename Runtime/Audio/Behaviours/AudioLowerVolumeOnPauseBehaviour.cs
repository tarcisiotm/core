using UnityEngine;
namespace TG.Core.Audio
{
    [CreateAssetMenu(menuName = "TG Core/Audio/Behaviours/AudioLowerVolumeOnPauseBehaviour")]

    public class AudioLowerVolumeOnPauseBehaviour : AudioGamePauseEventBehaviour
    {
        [SerializeField] protected float pausedVolumeMult = 0.4f;

        //private float previousVolume;

        protected override void GamePausedEvent(AudioBase audioBase, bool paused)
        {
            if (paused)
            {
                //previousVolume = audioBase.AudioSource.volume;
                audioBase.SetVolume(audioBase.OriginalVolume * pausedVolumeMult);
            }
            else if (wasPlaying)
            {
                audioBase.SetVolume(audioBase.OriginalVolume);//previousVolume;
            }
        }
    }
}
