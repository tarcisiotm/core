using System.Collections;
using UnityEngine;

namespace TG.Core.Audio {
    /// <summary>
    /// Base class for audio that will add itself to a list for global operations from the Audio Manager.
    /// </summary>
    public class AudioBase : MonoBehaviour 
    {
        public enum SceneUnloadBehaviour 
        {
            StopPlayingImmediately,
            FadeOut,
            KeepPlayingIndefinitely,
        }

        [Header("References")]
        [SerializeField] protected AudioSource audioSource;

        [Header("Config")]
        [SerializeField] private AudioBehaviorSet behaviorSet;
        [SerializeField] private bool fadesWhenSceneUnloads = true;
        [SerializeField] private bool fadeInVolume = false;
        [SerializeField] private SceneUnloadBehaviour sceneUnloadBehaviour;

        public AudioSource AudioSource => audioSource;

        protected int sceneBuildIndexToUnloadWith;

        private bool isUnloading = false;

        private AudioManagerBase audioManager;

        protected virtual void OnEnable() {
            ScenesManager.OnSceneIsGoingToLoad += CheckSceneIsGoingToUnload;
            //_audioManager.AddToAudioList(this);
        }

        protected virtual void OnDisable()
        {
            ScenesManager.OnSceneIsGoingToLoad -= CheckSceneIsGoingToUnload;
            //_audioManager.RemoveFromAudioList(this);
        }

        public void SetupAudio(AudioClip audioClip, float initialVolume, int sceneBuildIndexToUnloadWith) 
        {
            audioSource.clip = audioClip;
            audioSource.volume = initialVolume;
            this.sceneBuildIndexToUnloadWith = sceneBuildIndexToUnloadWith;

           // if (fadeInVolume) { FadeIn(); }
        }

        public virtual void Play() 
        {
            behaviorSet.playbackBehaviour.OnPlayback(this);

            if (behaviorSet.playbackEndedBehaviour != null)
            {
                StartCoroutine(CheckPlaybackEnded());
            }
        }

        // todo, move this to the actual behaviour, no sense being here...
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