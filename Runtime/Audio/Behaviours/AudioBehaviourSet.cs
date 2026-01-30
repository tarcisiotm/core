using UnityEngine;

namespace TG.Core.Audio
{
    [CreateAssetMenu(menuName = "TG Core/Audio/Behaviours/AudioBehaviorSet")]
    public class AudioBehaviorSet : ScriptableObject
    {
        [Tooltip("Handles Play behaviour, such as regular or delayed playback.")]
        public AudioPlaybackBehaviour playbackBehaviour;

        [Tooltip("Handles what happens when the game is paused.")]
        public AudioGamePauseEventBehaviour onGamePausedBehaviour;

        [Tooltip("Triggers after audio finishes playing. Useful for SFX and pooling.")]
        public AudioPlaybackEndedBehaviour playbackEndedBehaviour;
        
        [Tooltip("Triggers when the \"owner\" scene is about to be unloaded. Useful for fading out the volume.")]
        public AudioSceneUnloadBehaviour sceneUnloadBehaviour;

        // Game pause behaviour
    }
}