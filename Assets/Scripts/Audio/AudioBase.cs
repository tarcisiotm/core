using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TG.Core.Audio {
    public class AudioBase : MonoBehaviour {

        protected virtual void OnEnable() {
            AudioManager.I.AddToAudioList(this);
        }

        protected virtual void OnDisable() {
            AudioManager.I.RemoveFromAudioList(this);
        }

    }
}
