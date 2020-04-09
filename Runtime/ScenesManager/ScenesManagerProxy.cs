using System.Collections;
using System.Collections.Generic;
using TG.Core;
using UnityEngine;

namespace TG.Core {
    /// <summary>
    /// Scenes Manager Proxy for operations that require a reference
    /// </summary>
    public class ScenesManagerProxy : MonoBehaviour {
        ScenesManager scenesManager;

        protected virtual void Start() {
            scenesManager = FindObjectOfType<ScenesManager>();
        }

        public virtual void LoadMainMenu() {
            scenesManager.LoadScene(0);
        }

        public virtual void LoadNextLevel() {
            scenesManager.LoadNextSceneWithFade();
        }

        public virtual void ReloadScene() {
            scenesManager.ReloadScene();
        }


    }
}