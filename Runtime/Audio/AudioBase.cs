using System;
using System.Collections;
using UnityEngine;

namespace TG.Core.Audio {
    /// <summary>
    /// Base class for audio that will add itself to a list for global operations from the Audio Manager.
    /// </summary>
    public class AudioBase : MonoBehaviour 
    {
        [Header("References")]
        [SerializeField] protected AudioSource audioSource;

        [Header("Config")]
        [SerializeField] private AudioBehaviorSet behaviorSet;
        [SerializeField] private bool fadesWhenSceneUnloads = true;
        [SerializeField] private bool fadeInVolume = false;

        public AudioSource AudioSource => audioSource;

        protected int sceneBuildIndexToUnloadWith;

        private bool isUnloading = false;

        private AudioManagerBase audioManager;

        protected float originalVolume;
        public float OriginalVolume => originalVolume;

        protected virtual void OnEnable() {
            ScenesManager.OnSceneIsGoingToLoad += CheckSceneIsGoingToUnload;
            GameStateManagerBase.OnGameStateChanged += OnGameStateChanged;
            //_audioManager.AddToAudioList(this);
        }

        protected virtual void OnDisable()
        {
            ScenesManager.OnSceneIsGoingToLoad -= CheckSceneIsGoingToUnload;
            GameStateManagerBase.OnGameStateChanged -= OnGameStateChanged;

            //_audioManager.RemoveFromAudioList(this);
        }

        private void OnGameStateChanged(GameState previousGameState, GameState newGameState)
        {
            var isPausing = newGameState == GameState.Paused;
            var isUnpausing = previousGameState == GameState.Paused;

            if (!isPausing && !isUnpausing) { return; }

            if (isPausing) { Pause(true); }
            // TODO: handle possible transitions that wouldn't make sense to restore audio
            // such as going to main menu, etc
            else if (isUnpausing/* && newGameState == GameState.*/) 
            {
                Pause(false);
            }
        }

        public void SetupAudio(AudioClip audioClip, float initialVolume, int sceneBuildIndexToUnloadWith) 
        {
            audioSource.clip = audioClip;
            audioSource.volume = initialVolume;
            this.sceneBuildIndexToUnloadWith = sceneBuildIndexToUnloadWith;
            originalVolume = initialVolume;

           // if (fadeInVolume) { FadeIn(); }
        }

        public void SetVolume(float volume, bool replacesOriginalValue = false) 
        {
            audioSource.volume = volume;

            if (replacesOriginalValue) 
            {
                originalVolume = volume;
            }
        }

        public virtual void Play() 
        {
            behaviorSet.playbackBehaviour.OnPlayback(this);

            if (behaviorSet.playbackEndedBehaviour != null)
            {
                StartCoroutine(CheckPlaybackEnded());
            }
        }

        protected virtual void Pause(bool paused) 
        {
            if (behaviorSet.onGamePausedBehaviour != null) 
            {
                behaviorSet.onGamePausedBehaviour.OnGamePausedEvent(this, paused);
            }

        }

        // This could be moved to the actual behaviour, but it would break the separation of responsibilities
        private IEnumerator CheckPlaybackEnded()
        {
            while (audioSource.isPlaying) { yield return null; }

            behaviorSet.playbackEndedBehaviour.OnAudioPlaybackEnded(this);
        }

        private void CheckSceneIsGoingToUnload(int activeSceneBuildIndex, int newSceneBuildIndex)
        {
            if (behaviorSet.sceneUnloadBehaviour == null) { return; }

            behaviorSet.sceneUnloadBehaviour.OnSceneWillUnload(this, activeSceneBuildIndex, sceneBuildIndexToUnloadWith);
        }
    }
}