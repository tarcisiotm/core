using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TG.Core
{
    public interface IModule
    {
        bool HasInitialized { get; }

        IEnumerator Initialize();
        void Destroy();
    }
}