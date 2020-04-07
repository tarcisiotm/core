using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TG.Core
{
    public class ScenesManager : Singleton<ScenesManager>
    {
        [Header("Settings")]
        [SerializeField] bool usesFade = false;
        [Tooltip("If loading is too fast, will wait for at least this value.")]
        [SerializeField] float minLoadSceneTime = 1f;

        public bool IsLoadingScene { get; private set; }
        public float LoadingProgress { get; private set; }

        //Delegates
        public delegate void SceneLoadEvent();
        public delegate void SceneProgressUpdate(float loadingProgress);

        public static SceneLoadEvent OnSceneIsGoingToLoad;
        public static SceneLoadEvent OnSceneLoaded;

        public static SceneProgressUpdate OnSceneProgressUpdated;

        #region Core Methods
        public void LoadScene(string sceneName) {
            LoadScene(SceneManager.GetSceneByName(sceneName));
        }

        public void LoadScene(int sceneBuildIndex) {
            LoadScene(SceneManager.GetSceneByBuildIndex(sceneBuildIndex));
        }

        public void LoadScene(SceneConfig sceneConfig) {
        }

        public void LoadScene(
            Scene sceneToLoad,
            bool unloadActiveSceneOnComplete = true){

            if (IsLoadingScene){ return; }


            IsLoadingScene = true;
            StartCoroutine(Internal_LoadScene(sceneToLoad, unloadActiveSceneOnComplete));
        }

        IEnumerator Internal_LoadScene(Scene sceneToLoad, bool usesFade = false, bool unloadActiveSceneOnComplete = false) {
            //test if scene exists
            Debug.Log("Build index: " + sceneToLoad.buildIndex);

            OnSceneIsGoingToLoad?.Invoke();

            SceneTransition sceneTransition = FindObjectOfType<SceneTransition>();

            var asyncScene = SceneManager.LoadSceneAsync(sceneToLoad.buildIndex);

            asyncScene.allowSceneActivation = false;

            if (sceneTransition != null) {
                sceneTransition.FadeIn();
                yield return new WaitForSeconds(sceneTransition.TransitionDuration);
            }

            while (!asyncScene.isDone){
                LoadingProgress = Mathf.Clamp01(asyncScene.progress / 0.9f) * 100;

                OnSceneProgressUpdated?.Invoke(LoadingProgress);

                if (asyncScene.progress >= 0.9f){ asyncScene.allowSceneActivation = true; }

                yield return null;
            }

            OnSceneProgressUpdated?.Invoke(1f);

            if (sceneTransition != null) {
                //TODO test if necessary - Time to read completed text
                yield return new WaitForSeconds(.3f);
                sceneTransition.FadeOut();
                yield return new WaitForSeconds(sceneTransition.TransitionDuration);
            }

            IsLoadingScene = false;

            OnSceneLoaded?.Invoke();

            Scene activeScene = SceneManager.GetActiveScene();
            if (unloadActiveSceneOnComplete) { SceneManager.UnloadSceneAsync(activeScene); }
        }


        #endregion Core Methods
    }
}