using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SceneConfig{
    public Scene scene;
    public bool usesFade = false;
    public bool unloadActiveScene = true;
}
