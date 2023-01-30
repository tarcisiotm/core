using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModuleBase : MonoBehaviour
{
    public bool HasInitialized { get; private set; }

    public virtual IEnumerator Initialize()
    {
        HasInitialized = true;
        yield break;
    }

    public virtual void Destroy() { }
}