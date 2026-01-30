namespace TG.Core.Audio
{
    public interface IAudioSceneWillUnloadBehaviour
    {
        void OnSceneWillUnload(AudioBase audioBase, int sceneBeingUnloadedBuildIndex, int sceneBuildIndexToUnloadWith);
    }
}