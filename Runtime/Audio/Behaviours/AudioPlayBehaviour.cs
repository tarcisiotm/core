using UnityEngine;

namespace TG.Core.Audio
{
    [CreateAssetMenu]
    public class AudioPlayBehaviour : AudioPlaybackBehaviour
    {
        protected override void HandlePlayback(AudioBase audioBase)
        {
            if (audioBase.AudioSource == null) 
            {
                Debug.LogError("AudioSource is null", audioBase);
                return;
            }

            if (audioBase.AudioSource.clip == null)
            {
                Debug.LogError("AudioSource's clip is null", audioBase);
                return;
            }

            audioBase.AudioSource.Play();
        }
    }
}