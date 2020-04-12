using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TG.Core {
    /// <summary>
    /// Manager for loading / unloading scenes
    /// </summary>
    public class ScenesManager : Singleton<ScenesManager> {
        [Header("Settings")]
        [Tooltip("For loading times smaller than this value, will stall for the remainder.")]
        [SerializeField] float minLoadTime = 0f;

        [Header("Optional Settings")]
        [SerializeField] int mainMenuSceneBuildIndex = 0;

        public bool IsLoadingScene { get; private set; }
        public float LoadingProgress { get; private set; }

        //Delegates
        public delegate void SceneLoadEvent();
        public delegate void SceneLoadEventParams(int activeSceneBuildIndex, int newSceneBuildIndex);
        public delegate void SceneProgressUpdate(float loadingProgress);

        public static SceneLoadEvent OnTransitionFadedIn;
        public static SceneLoadEvent OnTransitionIsGoingToFadeOut;
        public static SceneLoadEventParams OnSceneIsGoingToLoad;
        public static SceneLoadEventParams OnSceneLoaded;

        public static SceneProgressUpdate OnSceneProgressUpdated;

        public enum UnloadCondition {
            DoNotUnload,
            BeforeNewSceneLoads,
            AfterTransitionFadedIn,
            AfterNewSceneHasLoaded
        }

        #region Core Methods

        public void LoadScene(int sceneBuildIndex) {
            LoadScene(GetSceneNameFromIndex(sceneBuildIndex));
        }

        public void LoadScene(SceneConfig sceneConfig) {
        }

        public void LoadScene(
            string sceneName,
            bool usesFade = false,
            UnloadCondition unloadCondition = UnloadCondition.AfterNewSceneHasLoaded) {

            if (IsLoadingScene) { return; }

            if (!IsSceneInBuild(sceneName, out int index)) {
                Debug.LogError($"Could not find scene {sceneName}. Make sure it is included inside Build Settings.");
                return;
            }

            StartCoroutine(Internal_LoadScene(sceneName, usesFade, unloadCondition));
        }

        IEnumerator Internal_LoadScene(
                string sceneName,
                bool usesFade = true,
                UnloadCondition unloadCondition = UnloadCondition.AfterNewSceneHasLoaded
            ) {

            float initialTime = Time.realtimeSinceStartup;
            Scene activeScene = SceneManager.GetActiveScene();
            IsLoadingScene = true;

            IsSceneInBuild(sceneName, out int newSceneIndex);

            OnSceneIsGoingToLoad?.Invoke(activeScene.buildIndex, newSceneIndex);

            SceneTransition sceneTransition = FindObjectOfType<SceneTransition>();

            bool fadeConditionsMet = usesFade && sceneTransition != null;

            if (unloadCondition == UnloadCondition.BeforeNewSceneLoads) {
                yield return SceneManager.UnloadSceneAsync(activeScene);
            }

            var asyncScene = SceneManager.LoadSceneAsync(sceneName);

            asyncScene.allowSceneActivation = false;

            if (fadeConditionsMet) {
                sceneTransition.FadeIn();
                yield return new WaitForSeconds(sceneTransition.TransitionDuration);
                OnTransitionFadedIn?.Invoke();
            }

            if (fadeConditionsMet && unloadCondition == UnloadCondition.AfterTransitionFadedIn) {
                yield return SceneManager.UnloadSceneAsync(activeScene);
            }

            while (!asyncScene.isDone) {
                LoadingProgress = asyncScene.progress;

                OnSceneProgressUpdated?.Invoke(LoadingProgress);

                if (asyncScene.progress >= 0.9f && Time.timeSinceLevelLoad - initialTime >= minLoadTime
                    ) { asyncScene.allowSceneActivation = true; }

                yield return null;
            }

            OnSceneProgressUpdated?.Invoke(1f);

            if (fadeConditionsMet) {
                OnTransitionIsGoingToFadeOut?.Invoke();
                yield return new WaitForSeconds(sceneTransition.BeforeFadeStallDuration);
                sceneTransition.FadeOut();
                yield return new WaitForSeconds(sceneTransition.TransitionDuration);
            }

            IsLoadingScene = false;
            OnSceneLoaded?.Invoke(activeScene.buildIndex, newSceneIndex);

            if (unloadCondition == UnloadCondition.AfterNewSceneHasLoaded) {
                SceneManager.UnloadSceneAsync(activeScene);
            }
        }

        public void LoadMainMenu(bool usesFade = true) {
            LoadScene(GetSceneNameFromIndex(mainMenuSceneBuildIndex), usesFade, UnloadCondition.AfterTransitionFadedIn);
        }

        public void ReloadScene(bool usesFade = true) {
            LoadScene(GetSceneNameFromIndex(SceneManager.GetActiveScene().buildIndex), usesFade, UnloadCondition.AfterTransitionFadedIn);
        }

        #endregion Core Methods


        #region Util
        string GetSceneNameFromIndex(int buildIndex) {
            return System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(buildIndex));
        }

        public bool IsSceneInBuild(string sceneName, out int sceneIndex) {
            int sceneCount = SceneManager.sceneCountInBuildSettings;

            for (int i = 0; i < sceneCount; i++) {
                if (sceneName == GetSceneNameFromIndex(i)) {
                    sceneIndex = i;
                    return true;
                }
            }
            sceneIndex = -1;
            return false;
        }

        public void LoadSceneWithFade(int index) {
            //LoadScene(SceneManager.GetSceneByBuildIndex(index), true, true);
        }

        public void LoadNextSceneWithFade() {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            LoadScene(GetSceneNameFromIndex(nextSceneIndex), true, UnloadCondition.AfterTransitionFadedIn);
        }
        #endregion Util
    }
}