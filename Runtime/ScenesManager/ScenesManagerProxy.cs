using UnityEngine;

namespace TG.Core {
    /// <summary>
    /// Scenes Manager Proxy for operations that require a reference
    /// </summary>
    public class ScenesManagerProxy : MonoBehaviour {

        public virtual void LoadMainMenu() {
            ScenesManager.I.LoadMainMenu();
        }

        public virtual void LoadNextLevel() {
            ScenesManager.I.LoadNextSceneWithFade();
        }

        public virtual void ReloadScene() {
            ScenesManager.I.ReloadScene();
        }

    }
}