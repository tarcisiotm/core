using System.Collections;
using UnityEngine;

namespace TG.Core.Audio
{
    public abstract class AudioSceneUnloadBehaviour : ScriptableObject, IAudioSceneWillUnloadBehaviour
    {
        public void OnSceneWillUnload(AudioBase audioBase, int activeSceneBuildIndex, int sceneBuildIndexToUnloadWith)
        {
            DoOnSceneWillUnload(audioBase, activeSceneBuildIndex, sceneBuildIndexToUnloadWith);  
        }

        protected abstract void DoOnSceneWillUnload(AudioBase audioBase, int activeSceneBuildIndex, int sceneBuildIndexToUnloadWith);
    }
}