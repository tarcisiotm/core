using UnityEngine;

public class DoNotDestroy : MonoBehaviour
{
    [SerializeField]
    bool m_shouldKeepOne = true;

    void Start()
    {
        DontDestroyOnLoad(this);

        if (!m_shouldKeepOne) { return; }

        var goName = name;

        var doNotDestroyList = FindObjectsOfType<DoNotDestroy>();

        if(doNotDestroyList != null && doNotDestroyList.Length > 0){
            for (int i = 0; i < doNotDestroyList.Length; i++)
            {
                if(goName == doNotDestroyList[i].name && doNotDestroyList[i].GetInstanceID() != GetInstanceID()){
                    Debug.Log("Destroying clone");
                    Destroy(this.gameObject);
                    return;
                }
            }
        }

    }
}
