using System.Collections;
using UnityEngine;

namespace TG.Core.Audio
{
    [CreateAssetMenu]
    public class AudioSceneUnloadFadeOutBehaviour : AudioSceneUnloadBehaviour
    {
        [SerializeField] private float fadeOutDuration = .5f;

        protected override void DoOnSceneWillUnload(AudioBase audioBase, int activeSceneBuildIndex, int sceneBuildIndexToUnloadWith)
        {
            if (activeSceneBuildIndex != sceneBuildIndexToUnloadWith) { return; }

            audioBase.StartCoroutine(OnPlaybackEndedCoroutine(audioBase));
        }

        private IEnumerator OnPlaybackEndedCoroutine(AudioBase audioBase)
        {
            var audioSource = audioBase.AudioSource;
            float startVolume = audioSource.volume;

            var timer = 0f;

            while (timer < fadeOutDuration) 
            {
                timer += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeOutDuration);
                yield return null;
            }

            audioSource.volume = 0f;
            audioSource.Stop();

            audioBase.gameObject.SetActive(false);
        }
    }
}