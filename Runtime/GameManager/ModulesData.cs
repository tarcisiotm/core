using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TG.Core {
    [CreateAssetMenu(fileName = "Modules Data", menuName = "TG Core/Modules Data")]
    public class ModulesData : ScriptableObject
    {
        [SerializeField] private List<GameObject> _modulePrefabs;
        public List<GameObject> ModulePrefabs => _modulePrefabs;

        private void OnValidate()
        {
            for (int i = _modulePrefabs.Count - 1; i >= 0; i--)
            {
                if (_modulePrefabs[i].GetComponent<IModule>() == null)
                {
                    Debug.LogError($"Removed {_modulePrefabs[i].name} since it does not implement IModule!");
                    _modulePrefabs.RemoveAt(i);
                }
            }
        }
    }
}