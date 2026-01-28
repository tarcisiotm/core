using System.Collections;
using UnityEngine;

namespace TG.Core.Audio
{
    [CreateAssetMenu]
    public class AudioDelayBeforePlaybackBehaviour : AudioPlaybackBehaviour
    {
        [SerializeField] private AudioPlayBehaviour audioPlayBehaviour;

        [Header("Optional Settings")]
        [SerializeField] private float delayInSeconds = 0f;
        protected override void HandlePlayback(AudioBase audioBase)
        {
            audioBase.StartCoroutine(DelayToPlay(audioBase, delayInSeconds));
        }

        private IEnumerator DelayToPlay(AudioBase audioBase, float delayInSeconds)
        {
            var timer = delayInSeconds;
            while (timer > 0) 
            {
                timer -= Time.deltaTime;
                yield return null;
            }

            audioPlayBehaviour.OnPlayback(audioBase);
        }
    }
}