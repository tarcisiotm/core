using UnityEngine;

namespace TG.Core.Audio
{
    [CreateAssetMenu(menuName = "TG Core/Audio/Behaviours/AudioDisableWhenDoneBehaviour")]
    public class AudioDisableWhenDoneBehaviour : AudioPlaybackEndedBehaviour
    {
        [SerializeField] private bool destroyWhenDone = false;

        protected override void DoOnAudioPlaybackEnded(AudioBase audioBase)
        {
            audioBase.gameObject.SetActive(false);

            if (destroyWhenDone) { Destroy(audioBase.gameObject); }
        }
    }
}