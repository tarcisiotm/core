using System.Collections;using System.Collections.Generic;using UnityEngine;namespace TG.Core.Audio{
    /// <summary>    /// Manager for creating OneShots, and globally handling audio    /// </summary>    public abstract class AudioManagerBase : Singleton<AudioManagerBase>    {        [Header("Base Settings")]        [SerializeField] protected bool _fadeBGMOnSceneLoad = true;        [Header("References")]        [Tooltip("Optional BGM Audio Source")]        [SerializeField] protected AudioSource _bgmAudioSource;        [Tooltip("Audio Prefab for instantiating sounds at runtime")]        [SerializeField] protected GameObject _audioPrefab;        protected PoolingController _poolingController;        protected PlayAudioAndDisable _plauAudioAndDisable;        protected List<AudioBase> _audioList = new List<AudioBase>();        protected bool _isReady = false;

        #region Unity's Callbacks        protected override void OnEnable()        {            base.OnEnable();            ScenesManager.OnSceneIsGoingToLoad += OnSceneIsGoingToLoad;            ScenesManager.OnSceneLoaded += OnSceneLoaded;            OnEnableChild();        }        protected virtual void OnSceneIsGoingToLoad(int activeSceneBuildIndex, int newSceneBuildIndex) { }        protected virtual void OnSceneLoaded(int activeSceneBuildIndex, int newSceneBuildIndex) { }        protected virtual void Start()        {            _isReady = true;        }        protected override void OnDisable()        {            base.OnDisable();            ScenesManager.OnSceneIsGoingToLoad -= OnSceneIsGoingToLoad;            ScenesManager.OnSceneLoaded -= OnSceneLoaded;            OnDisableChild();        }        protected virtual void OnEnableChild() { }        protected virtual void OnDisableChild() { }

        #endregion Unity's Callbacks
        #region Object Creation        private PlayAudioAndDisable GetOneShotObject()
        {
            PlayAudioAndDisable audioObj = null;

            if (_poolingController == null) audioObj = Instantiate(_audioPrefab).GetComponent<PlayAudioAndDisable>();
            else audioObj = _poolingController.GetPooledObject(_audioPrefab).GetComponent<PlayAudioAndDisable>();

            return audioObj;
        }

        /// <summary>        /// Creates a one shot sound that will follow target transform        /// </summary>        /// <param name="clip">The audio clip</param>        /// <param name="targetTransform">The transform</param>        /// <param name="vol">The volume</param>        public virtual void CreateOneShotFollowTarget(AudioClip clip, Transform targetTransform, float vol)        {            _plauAudioAndDisable = GetOneShotObject();            if (_plauAudioAndDisable == null)            {                Debug.LogWarning("No component source was found...");                return;            }

            //Add it to list in order to properly pause the game
            _plauAudioAndDisable.gameObject.SetActive(true);            _plauAudioAndDisable.PlayAndDisable(clip, vol, targetTransform);        }

        /// <summary>        /// Creates a one shot sound that will be stationary        /// </summary>        /// <param name="clip"></param>        /// <param name="pos"></param>        /// <param name="vol"></param>        public virtual void CreateOneShot(AudioClip clip, Vector3 pos, float vol = 1f)        {            _plauAudioAndDisable = GetOneShotObject();            if (_plauAudioAndDisable == null)            {                Debug.LogWarning("No component source was found...");                return;            }            _plauAudioAndDisable.transform.position = pos;

            //Add it to list in order to properly pause the game
            _plauAudioAndDisable.gameObject.SetActive(true);            _plauAudioAndDisable.PlayAndDisable(clip, vol);        }

        #endregion Object Creation
        #region Global Config        public void SetPoolingManager(PoolingController p_poolingController)        {            _poolingController = p_poolingController;        }

        //TODO future sprint to add pause all, etc
        public virtual void AddToAudioList(AudioBase p_audioObj)        {            _audioList.Remove(p_audioObj);        }        public virtual void RemoveFromAudioList(AudioBase p_audioObj)        {            _audioList.Remove(p_audioObj);        }
        #endregion Global Config
        #region Playback        public virtual void PlayBGM(AudioClip audioClip, float volume = 1)        {            if (_bgmAudioSource == null) { return; }            _bgmAudioSource.clip = audioClip;            _bgmAudioSource.volume = volume;            _bgmAudioSource.Play();        }
        #endregion Playback    }
}