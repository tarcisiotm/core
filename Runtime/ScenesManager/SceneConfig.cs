using UnityEngine.SceneManagement;

/// <summary>
/// Parameters for Scene operations
/// </summary>
[System.Serializable]
public class SceneConfig{
    public Scene scene;
    public bool usesFade = false;
    public bool unloadActiveScene = true;
}