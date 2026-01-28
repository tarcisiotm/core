using UnityEngine;

namespace TG.Core.Audio
{
    [CreateAssetMenu]
    public class AudioBehaviorSet : ScriptableObject
    {
        [Tooltip("Handles Play behaviour, such as regular or delayed playback.")]
        public AudioPlaybackBehaviour playbackBehaviour;
        
        [Tooltip("Triggers after audio finishes playing. Useful for SFX and pooling.")]
        public AudioPlaybackEndedBehaviour playbackEndedBehaviour;
        
        [Tooltip("Triggers when the \"owner\" scene is about to be unloaded. Useful for fading out the volume.")]
        public AudioSceneUnloadBehaviour sceneUnloadBehaviour;
    }
}