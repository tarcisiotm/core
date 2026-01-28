using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TG.Core.Audio
{
    /// <summary>
    /// Manager for creating OneShots, and globally handling audio
    /// </summary>
    public abstract class AudioManagerBase : ModuleBase
    {
        [Header("Base Settings")]
        [SerializeField] protected bool _fadeBGMOnSceneLoad = true;

        //[Header("References")]
        //[Tooltip("Optional BGM Audio Source Template")]
        //[SerializeField] protected AudioSource bgmAudioSourceTemplate;

        [Tooltip("Audio Prefabs for pooling sounds at runtime")]
        [SerializeField] protected GameObject _bgmAudioPrefab;
        [SerializeField] protected GameObject _sfxAudioPrefab;

        protected PoolingController _poolingController;

        protected PlayAudioAndDisable _plauAudioAndDisable;

        protected List<AudioBase> _audioList = new List<AudioBase>();

        protected bool _isReady = false;

        #region Unity's Callbacks
        protected void OnEnable()
        {
            ScenesManager.OnSceneIsGoingToLoad += OnSceneIsGoingToLoad;
            ScenesManager.OnSceneLoaded += OnSceneLoaded;
            OnEnableChild();

            if (_poolingController == null) _poolingController = GetComponentInChildren<PoolingController>();
        }

        protected virtual void OnSceneIsGoingToLoad(int activeSceneBuildIndex, int newSceneBuildIndex) { }
        protected virtual void OnSceneLoaded(int activeSceneBuildIndex, int newSceneBuildIndex) { }

        protected virtual void Start()
        {
            _isReady = true;
        }

        protected void OnDisable()
        {
            ScenesManager.OnSceneIsGoingToLoad -= OnSceneIsGoingToLoad;
            ScenesManager.OnSceneLoaded -= OnSceneLoaded;
            OnDisableChild();
        }

        protected virtual void OnEnableChild() { }

        protected virtual void OnDisableChild() { }

        #endregion Unity's Callbacks

        #region Object Creation
        private PlayAudioAndDisable GetOneShotAudioObject()
        {
            PlayAudioAndDisable audioObj = null;
            audioObj = _poolingController.GetPooledObject(_sfxAudioPrefab).GetComponent<PlayAudioAndDisable>();
            return audioObj;
        }

        private AudioBase GetBGMAudioObject()
        {
            AudioBase audioObj = null;
            audioObj = _poolingController.GetPooledObject(_bgmAudioPrefab).GetComponent<AudioBase>();
            return audioObj;
        }
        /// <summary>
        /// Creates a one shot sound that will follow target transform
        /// </summary>
        /// <param name="clip">The audio clip</param>
        /// <param name="targetTransform">The transform</param>
        /// <param name="vol">The volume</param>
        public virtual void CreateOneShotFollowTarget(AudioClip clip, Transform targetTransform, float vol)
        {
            _plauAudioAndDisable = GetOneShotAudioObject();

            if (_plauAudioAndDisable == null)
            {
                Debug.LogWarning("No component source was found...");
                return;
            }

            //Add it to list in order to properly pause the game
            _plauAudioAndDisable.gameObject.SetActive(true);

            _plauAudioAndDisable.PlayAndDisable(clip, vol, targetTransform);
        }

        /// <summary>
        /// Creates a one shot sound that will be stationary
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="pos"></param>
        /// <param name="vol"></param>
        public virtual void CreateOneShot(AudioClip clip, Vector3 pos, float vol = 1f)
        {
            _plauAudioAndDisable = GetOneShotAudioObject();

            if (_plauAudioAndDisable == null)
            {
                Debug.LogWarning("No component source was found...");
                return;
            }

            _plauAudioAndDisable.transform.position = pos;

            //Add it to list in order to properly pause the game
            _plauAudioAndDisable.gameObject.SetActive(true);
            _plauAudioAndDisable.PlayAndDisable(clip, vol);
        }

        #endregion Object Creation

        #region Global Config
        public void SetPoolingManager(PoolingController p_poolingController)
        {
            if (_poolingController != null) Debug.LogWarning("AudioManagerBase: Overwriting pooling manager!");
            _poolingController = p_poolingController;
        }

        //TODO future sprint to add pause all, etc
        /*public virtual void AddToAudioList(AudioBase p_audioObj)
        {
            _audioList.Add(p_audioObj);
        }

        public virtual void RemoveFromAudioList(AudioBase p_audioObj)
        {
            _audioList.Remove(p_audioObj);
        }*/

        #endregion Global Config

        #region Playback

        public virtual void PlayBGM(AudioClip audioClip, int buildSceneIndex, float volume = 1f, float delay = 0f)
        {
            var bgmAudio = GetBGMAudioObject();

            if (bgmAudio == null) 
            { 
                Debug.LogError("No available BGM audio sources!"); 
                return; 
            }

            bgmAudio.gameObject.SetActive(true);

            bgmAudio.SetupAudio(audioClip, volume, buildSceneIndex);
            bgmAudio.Play();
            //bgmAudioSource.clip = audioClip;
            //bgmAudioSource.volume = volume;

            //StartCoroutine(DelayBeforePlaying(bgmAudioSource, delay));
        }

        protected IEnumerator DelayBeforePlaying(AudioSource bgmAudioSource, float delay) 
        {
            var timer = 0f;
            while (timer < delay) 
            { 
                yield return null;
                timer += Time.deltaTime;
            }

            bgmAudioSource.Play();
        }
        #endregion Playback
    }
}