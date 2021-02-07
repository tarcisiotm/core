using UnityEngine;

namespace TG.Core {
    /// <summary>
    /// A single instance object that holds objects through scenes.
    /// </summary>
    public class RequiredComponentsAllScenes : MonoBehaviour {
        public static RequiredComponentsAllScenes instance;

        private void Awake() {

            if (instance != null && instance != this) {
                Destroy(this.gameObject);
                return;
            }

            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
    }
}