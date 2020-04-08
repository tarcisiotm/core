using UnityEngine.SceneManagement;

[System.Serializable]
public class SceneConfig{
    public Scene scene;
    public bool usesFade = false;
    public bool unloadActiveScene = true;
}