using System;using System.Collections;using System.Collections.Generic;using UnityEngine;namespace TG.Core.Audio {

    /// <summary>    /// Manager for creating OneShots, and globally handling audio    /// </summary>    public abstract class AudioManagerBase : Singleton<AudioManagerBase> {        [Header("Base Settings")]        [SerializeField] protected bool fadeBGMOnSceneLoad = true;        [Header("References")]        [Tooltip("Optional BGM Audio Source")]        [SerializeField] protected AudioSource bgmAudioSource;        [Tooltip("Audio Prefab for instantiating sounds at runtime")]        [SerializeField] protected GameObject audioPrefab;        protected PoolingManager poolingManager;        protected PlayAudioAndDisable plauAudioAndDisable;        protected List<AudioBase> audioList = new List<AudioBase>();        protected bool isReady = false;


        #region Unity's Callbacks
        protected override void OnEnable() {            base.OnEnable();            ScenesManager.OnSceneIsGoingToLoad += OnSceneIsGoingToLoad;            OnEnableChild();        }        protected virtual void OnSceneIsGoingToLoad(int activeSceneBuildIndex, int newSceneBuildIndex) {        }        protected virtual IEnumerator Start() {            while (PoolingManager.I == null) {                yield return null;            }            poolingManager = PoolingManager.I;            isReady = true;        }        protected override void OnDisable() {            base.OnDisable();            ScenesManager.OnSceneIsGoingToLoad -= OnSceneIsGoingToLoad;            OnDisableChild();        }        protected virtual void OnEnableChild() { }        protected virtual void OnDisableChild() { }

        #endregion Unity's Callbacks

        #region Object Creation        /// <summary>                                        /// Creates a one shot sound that will follow target transform                                        /// </summary>                                        /// <param name="clip">The audio clip</param>                                        /// <param name="targetTransform">The transform</param>                                        /// <param name="vol">The volume</param>        public virtual void CreateOneShotFollowTarget(AudioClip clip, Transform targetTransform, float vol) {            plauAudioAndDisable = poolingManager.GetPooledObject(audioPrefab).GetComponent<PlayAudioAndDisable>();            if (plauAudioAndDisable == null) {                Debug.LogWarning("No component source was found...");                return;            }

            //Add it to list in order to properly pause the game
            plauAudioAndDisable.gameObject.SetActive(true);            plauAudioAndDisable.PlayAndDisable(clip, vol, targetTransform);        }

        /// <summary>        /// Creates a one shot sound that will be stationary        /// </summary>        /// <param name="clip"></param>        /// <param name="pos"></param>        /// <param name="vol"></param>        public virtual void CreateOneShot(AudioClip clip, Vector3 pos, float vol = 1f) {            plauAudioAndDisable = poolingManager.GetPooledObject(audioPrefab).GetComponent<PlayAudioAndDisable>();            if (plauAudioAndDisable == null) {                Debug.LogWarning("No component source was found...");                return;            }            plauAudioAndDisable.transform.position = pos;

            //Add it to list in order to properly pause the game
            plauAudioAndDisable.gameObject.SetActive(true);            plauAudioAndDisable.PlayAndDisable(clip, vol);        }

        #endregion Object Creation

        #region Global Config        //TODO future sprint to add pause all, etc
        public virtual void AddToAudioList(AudioBase p_audioObj) {            audioList.Remove(p_audioObj);        }        public virtual void RemoveFromAudioList(AudioBase p_audioObj) {            audioList.Remove(p_audioObj);        }

        #endregion Global Config

        #region Playback
        public virtual void PlayBGM(AudioClip audioClip, float volume = 1) {            if (bgmAudioSource == null) { return; }            bgmAudioSource.clip = audioClip;            bgmAudioSource.volume = volume;            bgmAudioSource.Play();        }

        #endregion Playback    }}