using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TG.Core
{
    public abstract class GameManagerBase : Singleton<GameManagerBase>
    {
        public static event Action OnInitialized;
        public static event Action<ModuleBase> OnModulesInitialized;

        protected bool _hasInitialized;
        protected bool _hasInitializedModules;
        public bool Initialized => _hasInitialized;

        [SerializeField] protected ModulesData _modulesData;

        protected List<ModuleBase> _modules = new();

        private readonly List<Action> _pendingModuleRequests = new();

        public void GetModuleAsync<T>(Action<T> callback) where T : ModuleBase
        {
            if (Initialized)
            {
                callback?.Invoke(GetModule<T>());
            }
            else
            {
                OnModulesInitialized += (Action<ModuleBase>)callback;
            }
        }

        public T GetModule<T>() where T : ModuleBase
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
                var go = Instantiate(moduleData, transform);
                var module = go.GetComponent<ModuleBase>();
                yield return module.Initialize();

                go.name = moduleData.name; // removes "(clone)" suffix
                go.transform.parent = null; // root of managers scene
                _modules.Add(module);
            }

            _hasInitializedModules = true;
            OnInitialized?.Invoke();
            OnInitialized = null;
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