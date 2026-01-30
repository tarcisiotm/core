using System.Collections;
using UnityEngine;

namespace TG.Core.Audio
{
    [CreateAssetMenu(menuName = "TG Core/Audio/Behaviours/AudioFadeOnPauseBehaviour")]
    public class AudioFadeOnPauseBehaviour : AudioGamePauseEventBehaviour
    {
        private bool fadesIn = true;
        private bool fadesOut = true;

        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private float fadeOutDuration = 0.3f;
        [Space]
        [SerializeField] private float targetPausedVolumeMult = 0.4f;

        Coroutine fadeCoroutine;

        protected override void GamePausedEvent(AudioBase audioBase, bool paused)
        {
            if (paused) { HandlePaused(audioBase); } 
            else { HandleUnpaused(audioBase); }
        }

        private void HandlePaused(AudioBase audioBase) 
        {
            if (!fadesOut) 
            {
                audioBase.SetVolume(audioBase.OriginalVolume * targetPausedVolumeMult);
                return;
            }

            if (fadeCoroutine != null) { audioBase.StopCoroutine(fadeCoroutine); }

            fadeCoroutine = audioBase.StartCoroutine(FadeVolumeCoroutine(
                audioBase, 
                audioBase.AudioSource.volume,
                audioBase.AudioSource.volume * targetPausedVolumeMult,
                fadeOutDuration));
        }

        private void HandleUnpaused(AudioBase audioBase) 
        {
            if (!fadesIn) 
            {
                audioBase.SetVolume(audioBase.OriginalVolume);
                return;
            }

            if (fadeCoroutine != null) { audioBase.StopCoroutine(fadeCoroutine); }

            fadeCoroutine = audioBase.StartCoroutine(FadeVolumeCoroutine(
                audioBase,
                audioBase.AudioSource.volume,
                audioBase.OriginalVolume,
                fadeInDuration));
        }

        private IEnumerator FadeVolumeCoroutine(
            AudioBase audioBase, 
            float initialVolume, 
            float targetVolume, 
            float fadeDuration)
        {
            var timer = 0f;

            while (timer < fadeDuration)
            {
                timer += Time.unscaledDeltaTime;
                audioBase.SetVolume(Mathf.Lerp(initialVolume, targetVolume, timer / fadeDuration));
                yield return null;
            }

            audioBase.SetVolume(targetVolume);

            fadeCoroutine = null;
        }
    }
}