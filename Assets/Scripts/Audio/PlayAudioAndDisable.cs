using UnityEngine;

namespace TG.Core.Audio {
    public class PlayAudioAndDisable : AudioBase {

        [SerializeField] AudioSource audioSource;
        Transform targetTransform;

        float duration;

        protected virtual void Update() {
            duration -= Time.deltaTime;
            if (duration < 0) { HandleDisableObject(); }

            if(targetTransform != null) {
                transform.position = targetTransform.position;
            }
        }

        public virtual void PlayAndDisable(AudioClip audioClip, float volume = 1f, Transform transformToFollow = null) {
            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.Play();
            duration = audioClip.length + 1f;

            targetTransform = transformToFollow;
        }

        public virtual void HandleDisableObject() {
            targetTransform = null;
            gameObject.SetActive(false);
        }
    }
}