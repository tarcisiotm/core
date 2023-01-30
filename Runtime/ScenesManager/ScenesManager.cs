using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TG.Core
{
    /// <summary>
    /// Manager for loading / unloading scenes
    /// </summary>
    public class ScenesManager : ModuleBase
    {
        [Header("Settings")]
        [Tooltip("For loading times smaller than this value, will stall for the remainder.")]
        [SerializeField] float _minLoadTime = 0f;

        [Header("Optional Settings")]
        [SerializeField] int _mainMenuSceneBuildIndex = 0;

        public bool IsLoadingScene { get; private set; }
        public float LoadingProgress { get; private set; }

        protected bool _hasInitialized;

        public bool HasInitialized => _hasInitialized;

        //Delegates
        public delegate void SceneLoadEvent();
        public delegate void SceneLoadEventParams(int activeSceneBuildIndex, int newSceneBuildIndex);
        public delegate void SceneProgressUpdate(float loadingProgress);

        public static SceneLoadEvent OnTransitionFadedIn;
        public static SceneLoadEvent OnTransitionIsGoingToFadeOut;
        public static SceneLoadEventParams OnSceneIsGoingToLoad;
        public static SceneLoadEventParams OnSceneLoaded;

        public static SceneProgressUpdate OnSceneProgressUpdated;

        public enum UnloadCondition
        {
            DoNotUnload,
            BeforeNewSceneLoads,
            AfterTransitionFadedIn,
            AfterNewSceneHasLoaded
        }

        #region Core Methods

        /*public void LoadScene(int sceneBuildIndex)
        {
            LoadScene(GetSceneNameFromIndex(sceneBuildIndex));
        }*/

        public void LoadScene(SceneConfig sceneConfig)
        {
        }

        public void LoadScene(
            int sceneBuildIndex,
            bool usesFade = false,
            UnloadCondition unloadCondition = UnloadCondition.AfterNewSceneHasLoaded)
        {

            if (IsLoadingScene) { return; }

            if (!IsSceneInBuild(sceneBuildIndex))
            {
                Debug.LogError($"Could not find scene with index {sceneBuildIndex}. Make sure it is included inside Build Settings.");
                return;
            }

            StartCoroutine(Internal_LoadScene(sceneBuildIndex, usesFade, unloadCondition));
        }

        private IEnumerator Internal_LoadScene(
                int sceneBuildIndex,
                bool usesFade = true,
                UnloadCondition unloadCondition = UnloadCondition.AfterNewSceneHasLoaded
            )
        {
            float initialTime = Time.realtimeSinceStartup;
            Scene activeScene = SceneManager.GetActiveScene();
            IsLoadingScene = true;

            IsSceneInBuild(sceneBuildIndex);

            OnSceneIsGoingToLoad?.Invoke(activeScene.buildIndex, sceneBuildIndex);

            SceneTransition sceneTransition = FindObjectOfType<SceneTransition>();

            bool fadeConditionsMet = usesFade && sceneTransition != null;

            if (unloadCondition == UnloadCondition.BeforeNewSceneLoads)
            {
                yield return SceneManager.UnloadSceneAsync(activeScene);
            }

            var asyncScene = SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Additive);

            asyncScene.allowSceneActivation = false;

            if (fadeConditionsMet)
            {
                sceneTransition.FadeIn();
                yield return WaitForSecondsUnscaled(sceneTransition.TransitionDuration);
                OnTransitionFadedIn?.Invoke();
            }

            //if (fadeConditionsMet && unloadCondition == UnloadCondition.AfterTransitionFadedIn) {
            //yield return SceneManager.UnloadSceneAsync(activeScene);
            //}

            while (!asyncScene.isDone)
            {
                LoadingProgress = asyncScene.progress;

                OnSceneProgressUpdated?.Invoke(LoadingProgress);

                if (asyncScene.progress >= 0.9f && Time.realtimeSinceStartup - initialTime >= _minLoadTime
                    ) { asyncScene.allowSceneActivation = true; }

                yield return null;
            }

            OnSceneProgressUpdated?.Invoke(1f);

            if (fadeConditionsMet)
            {
                OnTransitionIsGoingToFadeOut?.Invoke();
                yield return WaitForSecondsUnscaled(sceneTransition.BeforeFadeStallDuration);
                sceneTransition.FadeOut();
                yield return WaitForSecondsUnscaled(sceneTransition.TransitionDuration);
            }

            IsLoadingScene = false;
            OnSceneLoaded?.Invoke(activeScene.buildIndex, sceneBuildIndex);

            if (unloadCondition == UnloadCondition.AfterNewSceneHasLoaded)
            {
                SceneManager.UnloadSceneAsync(activeScene);
            }
        }

        private IEnumerator WaitForSecondsUnscaled(float seconds)
        {
            float startTime = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - startTime < seconds)
            {
                yield return null;
            }
        }

        public void LoadMainMenu(bool usesFade = true)
        {
            LoadScene(_mainMenuSceneBuildIndex, usesFade, UnloadCondition.AfterTransitionFadedIn);
        }

        public void ReloadScene(bool usesFade = true)
        {
            LoadScene(SceneManager.GetActiveScene().buildIndex, usesFade, UnloadCondition.AfterTransitionFadedIn);
        }

        #endregion Core Methods


        #region Util
        /*string GetSceneNameFromIndex(int buildIndex)
        {
            return System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(buildIndex));
        }*/

        public bool IsSceneInBuild(int sceneBuildIndex)
        {
            return sceneBuildIndex < SceneManager.sceneCountInBuildSettings;
           /* int sceneCount = SceneManager.sceneCountInBuildSettings;

            for (int i = 0; i < sceneCount; i++)
            {
                if (sceneName == GetSceneNameFromIndex(i))
                {
                    sceneIndex = i;
                    return true;
                }
            }
            sceneIndex = -1;
            return false;*/
        }

        public void LoadSceneWithFade(int index)
        {
            throw new NotImplementedException("LoadSceneWithFade: To be implemented.");
            //LoadScene(SceneManager.GetSceneByBuildIndex(index), true, true);
        }

        public void LoadNextSceneWithFade()
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            LoadScene(nextSceneIndex, true, UnloadCondition.AfterTransitionFadedIn);
        }

        public virtual IEnumerator Initialize()
        {
            _hasInitialized = true;
            yield break;
        }

        public virtual void Destroy()
        {
        }
        #endregion Util
    }
}