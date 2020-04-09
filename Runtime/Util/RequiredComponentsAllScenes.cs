using UnityEngine;

namespace TG.Core {
    public class RequiredComponentsAllScenes : MonoBehaviour {
        public static RequiredComponentsAllScenes instance;

        void Awake() {

            if (instance != null && instance != this) {
                Destroy(this.gameObject);
                return;
            }

            DontDestroyOnLoad(this.gameObject);
        }
    }
}