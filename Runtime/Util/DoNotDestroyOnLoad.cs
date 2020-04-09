using UnityEngine;
/// <summary>
/// Attach this component to make an object persist through scenes
/// </summary>
public class DoNotDestroyOnLoad : MonoBehaviour
{
    private void Awake() {
        DontDestroyOnLoad(this);
    }
}
