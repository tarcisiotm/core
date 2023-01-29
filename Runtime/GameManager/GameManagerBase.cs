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

        protected List<IModule> _modules = new();

        public T GetModule<T>() where T : IModule
        {
            foreach(var module in _modules)
            {
                if (module is T) return (T)module;
            }

            throw new System.Exception("Module not found!");
        }

        protected virtual IEnumerator Start()
        {
            yield return Initialize();

            foreach (var moduleData in _modulesData.ModulePrefabs)
            {
                var go = Instantiate(moduleData, transform.parent);
                var iModule = go.GetComponent<IModule>();
                yield return iModule.Initialize();
                _modules.Add(iModule);
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