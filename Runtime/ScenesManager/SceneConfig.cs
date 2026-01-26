using UnityEngine.SceneManagement;

namespace TG.Core
{
    /// <summary>
    /// Parameters for Scene operations
    /// </summary>
    [System.Serializable]
    public class SceneConfig
    {
        public int sceneBuildIndex = 0;
        public bool usesFade = true;
        public bool unloadActiveScene = true;
        public ScenesManager.UnloadCondition unloadCondition = ScenesManager.UnloadCondition.AfterTransitionFadedIn;

        public static SceneConfig GetDefaultSceneConfig(int sceneBuildIndex) 
        {
            SceneConfig sceneConfig = new SceneConfig();
            sceneConfig.sceneBuildIndex = sceneBuildIndex;
            return sceneConfig;
        }
    }
}