using UnityEngine;

namespace TG.Core.Audio {
    /// <summary>
    /// Base class for audio that will add itself to a list for global operations from the Audio Manager
    /// </summary>
    public class AudioBase : MonoBehaviour {
        /*private AudioManagerBase _audioManager;

        protected virtual void OnEnable() {
            _audioManager.AddToAudioList(this);
        }

        protected virtual void OnDisable() {
            _audioManager.RemoveFromAudioList(this);
        }*/
    }
}