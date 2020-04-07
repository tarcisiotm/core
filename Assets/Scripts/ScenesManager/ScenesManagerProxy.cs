using System.Collections;
using System.Collections.Generic;
using TG.Core;
using UnityEngine;

namespace TG.Core {
    public class ScenesManagerProxy : MonoBehaviour {
        ScenesManager scenesManager;

        void Start() {
            scenesManager = FindObjectOfType<ScenesManager>();
        }

        public void LoadMainMenu() {
            //scenesManager.LoadMainMenu();
        }

        public void LoadNextLevel() {
            //scenesManager.LoadNextLevel();
        }

        public void ReloadScene() {
            //scenesManager.ReloadScene();
        }


    }
}