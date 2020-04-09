using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TG.Core.Audio {
    /// <summary>
    /// Base class for audio that will add itself to a list for global operations from the Audio Manager
    /// </summary>
    public class AudioBase : MonoBehaviour {

        protected virtual void OnEnable() {
            AudioManager.I.AddToAudioList(this);
        }

        protected virtual void OnDisable() {
            AudioManager.I.RemoveFromAudioList(this);
        }

    }
}
