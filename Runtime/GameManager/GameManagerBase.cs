using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TG.Core
{
    public abstract class GameManagerBase : Singleton<GameManagerBase>, IModule
    {
        protected bool _hasInitialized;
        protected bool _hasInitializedModules;

        public bool HasInitialized => _hasInitialized;
        public bool HasInitializedModules => _hasInitializedModules;
        public bool HasFullyInitialized => HasInitialized && HasInitializedModules;

        [SerializeField] protected ModulesData _modulesData;

        protected HashSet<IModule> _modulesHashSet = new();

        public T GetModule<T>(T moduleType) where T : IModule
        {
            if (_modulesHashSet.TryGetValue(moduleType, out var result)) return (T)result;
            else throw new System.Exception($"Module: {moduleType} not found!");
        }

        protected virtual IEnumerator Start()
        {
            yield return Initialize();

            foreach (var moduleData in _modulesData.ModulePrefabs)
            {
                var go = Instantiate(moduleData, transform.parent);
                var iModule = go.GetComponent<IModule>();
                yield return iModule.Initialize();
                _modulesHashSet.Add(iModule);
            }

            _hasInitializedModules = true;
        }

        public virtual IEnumerator Initialize()
        {
            _hasInitialized = true;
            yield break;
        }

        public virtual void Destroy()
        {
        }
    }
}