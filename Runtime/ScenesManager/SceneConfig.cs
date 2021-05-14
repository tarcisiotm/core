using UnityEngine.SceneManagement;

namespace TG.Core
{
    /// <summary>
    /// Parameters for Scene operations
    /// </summary>
    [System.Serializable]
    public class SceneConfig
    {
        public Scene scene;
        public bool usesFade = false;
        public bool unloadActiveScene = true;
    }
}