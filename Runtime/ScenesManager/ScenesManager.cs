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
        [SerializeField] private float _minLoadTime = 0f;

        [Header("Optional Settings")]
        [SerializeField] private int _mainMenuSceneBuildIndex = 0;

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

        public enum UnloadCondition
        {
            DoNotUnload,
            BeforeNewSceneLoads,
            AfterTransitionFadedIn,
            AfterNewSceneHasLoaded
        }

        public override IEnumerator Initialize()
        {
            return base.Initialize();
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
            const string managersSceneName = "Managers Scene";

            var initialTime = Time.realtimeSinceStartup;

            var previouslyActiveScene = SceneManager.GetActiveScene();
            IsLoadingScene = true;

            OnSceneIsGoingToLoad?.Invoke(previouslyActiveScene.buildIndex, sceneBuildIndex);

            // TODO: grab reference
            var sceneTransition = FindFirstObjectByType<SceneTransition>();

            // TODO: rename for clarity
            var fadeConditionsMet = usesFade && sceneTransition != null;

            // TODO: add virtual method for custom hooks

            // make managers scene active
            Scene managersScene;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name != managersSceneName) continue;

                managersScene = SceneManager.GetSceneAt(i);
                SceneManager.SetActiveScene(managersScene);

                //SceneManager.SetActiveScene(SceneManager.GetSceneByName("MySceneName"));
                break;
            }

            if (unloadCondition == UnloadCondition.BeforeNewSceneLoads)
            {
                yield return SceneManager.UnloadSceneAsync(previouslyActiveScene);
            }

            if (fadeConditionsMet)
            {
                sceneTransition.FadeIn();
                yield return WaitForSecondsUnscaled(sceneTransition.TransitionDuration);
                OnTransitionFadedIn?.Invoke();
            }

            // this cannot be! we need to set the transition as the active scene before unloading it here
            if (fadeConditionsMet && unloadCondition == UnloadCondition.AfterTransitionFadedIn)
            {
                yield return SceneManager.UnloadSceneAsync(previouslyActiveScene);
            }

            var asyncScene = SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Additive);
            asyncScene.allowSceneActivation = false;

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
            OnSceneLoaded?.Invoke(previouslyActiveScene.buildIndex, sceneBuildIndex);

            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneBuildIndex));

            if (unloadCondition == UnloadCondition.AfterNewSceneHasLoaded)
            {
                SceneManager.UnloadSceneAsync(previouslyActiveScene);
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

        // todo: why?
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
            //throw new NotImplementedException("LoadSceneWithFade: To be implemented.");
            LoadScene(index, true, UnloadCondition.AfterTransitionFadedIn);
        }

        public void LoadNextSceneWithFade()
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            LoadScene(nextSceneIndex, true, UnloadCondition.AfterTransitionFadedIn);
        }
        #endregion Util
    }
}