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
        [SerializeField] private int _mainMenuSceneBuildIndex = 1;

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

        public void LoadScene(int sceneBuildIndex)
        {
            LoadScene(SceneConfig.GetDefaultSceneConfig(sceneBuildIndex));
        }

        public void LoadScene(SceneConfig sceneConfig)
        {
            LoadScene(sceneConfig.sceneBuildIndex, sceneConfig.usesFade, sceneConfig.unloadCondition);
        }

        public void LoadScene(
            int sceneBuildIndex,
            bool usesFade = true,
            UnloadCondition unloadCondition = UnloadCondition.AfterTransitionFadedIn)
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
            var initialTime = Time.realtimeSinceStartup;

            var previouslyActiveScene = SceneManager.GetActiveScene();
            IsLoadingScene = true;

            OnSceneIsGoingToLoad?.Invoke(previouslyActiveScene.buildIndex, sceneBuildIndex);

            // TODO: add virtual method for custom hooks?

            SceneManager.SetActiveScene(gameObject.scene);

            if (unloadCondition == UnloadCondition.BeforeNewSceneLoads)
            {
                yield return SceneManager.UnloadSceneAsync(previouslyActiveScene);
            }

            yield return SceneTransitionFadeIn(usesFade, unloadCondition, previouslyActiveScene);

            if (unloadCondition == UnloadCondition.AfterTransitionFadedIn) // or no transition present
            {
                yield return SceneManager.UnloadSceneAsync(previouslyActiveScene);
            }

            var asyncLoadScene = SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Additive);
            asyncLoadScene.allowSceneActivation = false;

            while (!asyncLoadScene.isDone)
            {
                LoadingProgress = asyncLoadScene.progress;

                OnSceneProgressUpdated?.Invoke(LoadingProgress);

                if (asyncLoadScene.progress >= 0.9f && Time.realtimeSinceStartup - initialTime >= _minLoadTime)
                { 
                    asyncLoadScene.allowSceneActivation = true; 
                }

                yield return null;
            }

            OnSceneProgressUpdated?.Invoke(1f);

            yield return StartCoroutine(SceneTransitionFadeOut(usesFade));
           
            IsLoadingScene = false;

            OnSceneLoaded?.Invoke(previouslyActiveScene.buildIndex, sceneBuildIndex);

            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneBuildIndex));

            if (unloadCondition == UnloadCondition.AfterNewSceneHasLoaded)
            {
                SceneManager.UnloadSceneAsync(previouslyActiveScene);
            }
        }

        private IEnumerator SceneTransitionFadeIn(
            bool usesFade, 
            UnloadCondition unloadCondition, 
            Scene previouslyActiveScene) 
        {
            // todo grab reference!
            var sceneTransition = FindFirstObjectByType<SceneTransition>();
            if (!usesFade || sceneTransition == null) yield break;

            sceneTransition.FadeIn();
            yield return WaitForSecondsUnscaled(sceneTransition.TransitionDuration);
            OnTransitionFadedIn?.Invoke();
        }

        private IEnumerator SceneTransitionFadeOut(bool usesFade)
        {
            // todo grab reference!
            var sceneTransition = FindFirstObjectByType<SceneTransition>();
            if (!usesFade || sceneTransition == null) yield break;

            OnTransitionIsGoingToFadeOut?.Invoke();
            yield return WaitForSecondsUnscaled(sceneTransition.BeforeFadeStallDuration);
            sceneTransition.FadeOut();
            yield return WaitForSecondsUnscaled(sceneTransition.TransitionDuration);
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
        public static bool IsSceneInBuild(int sceneBuildIndex)
        {
            return sceneBuildIndex < SceneManager.sceneCountInBuildSettings;
        }

        public void LoadSceneWithFade(int index)
        {
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