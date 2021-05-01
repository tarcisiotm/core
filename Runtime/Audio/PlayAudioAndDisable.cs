using UnityEngine;

namespace TG.Core.Audio {
    /// <summary>
    /// Disables the gammeobject after it finishes playing
    /// </summary>
    public class PlayAudioAndDisable : AudioBase {
        [SerializeField] private AudioSource _audioSource = default;

        private Transform _targetTransform = default;

        private float _duration;

        protected virtual void Update() {
            _duration -= Time.deltaTime;
            if (_duration < 0) { HandleDisableObject(); }

            if(_targetTransform != null) {
                transform.position = _targetTransform.position;
            }
        }

        public virtual void PlayAndDisable(AudioClip audioClip, float volume = 1f, Transform transformToFollow = null) {
            _audioSource.clip = audioClip;
            _audioSource.volume = volume;
            _audioSource.Play();
            _duration = audioClip.length + 1f;

            _targetTransform = transformToFollow;
        }

        public virtual void HandleDisableObject() {
            _targetTransform = null;
            gameObject.SetActive(false);
        }
    }
}