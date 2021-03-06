﻿using UnityEngine;

namespace TG.Core.Audio {
    /// <summary>
    /// Base class for audio that will add itself to a list for global operations from the Audio Manager
    /// </summary>
    public class AudioBase : MonoBehaviour {
        protected virtual void OnEnable() {
            AudioManagerBase.I.AddToAudioList(this);
        }

        protected virtual void OnDisable() {
            if (AudioManagerBase.I != null) AudioManagerBase.I.RemoveFromAudioList(this);
        }
    }
}