using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TG.Core {
    /// <summary>
    /// Handles Pooling of Objects
    /// </summary>
    public class PoolingController : MonoBehaviour {
        [Header("Settings")]
        [SerializeField] private bool _onStart = true;

        [Tooltip("If true, will only create one object per frame")]
        [SerializeField] private bool _incrementalInitialization = true;
        [SerializeField] private bool _canIncreaseInSize = true;

        [SerializeField]
        private List<PoolingSet> _poolingSets = default;

        private GameObject _go;

        private bool _hasInit = false;
        private bool _hasFinishedCreatingSet = false;

        private int _amount;

        public bool HasInit => _hasInit;

        #region Initialization
        private void Start() {
            if (_onStart) { InitializePool(); }
        }

        public void InitializePool() {
            StartCoroutine(DoInit());
        }

        private IEnumerator DoInit() {
            for (int i = 0; i < _poolingSets.Count; i++) {
                yield return DoInitSet(_poolingSets[i]);
            }
            _hasInit = true;
        }

        private IEnumerator DoInitSet(PoolingSet poolingSet) {
            if (!poolingSet.IsValid()) { yield break; }

            _amount = poolingSet.AmountToPool;
            poolingSet.Objects = new List<GameObject>(_amount);

            _hasFinishedCreatingSet = false;

            for (int j = 0; j < _amount; j++) {
                AddObjectToPoolingSet(poolingSet, j.ToString());
                if (_incrementalInitialization) { yield return null; }
            }

            if (!_poolingSets.Contains(poolingSet)) {
                _poolingSets.Add(poolingSet);
            }

            _hasFinishedCreatingSet = true;
        }
        #endregion Initialization


        #region Object Handling
        private GameObject AddObjectToPoolingSet(PoolingSet p_poolingSet, string p_suffix) {
            _go = Instantiate(p_poolingSet.Prefab);
            _go.name += p_poolingSet.Prefab.name + " " + p_suffix;

            if (p_poolingSet.ParentTransform != null) {
                _go.transform.SetParent(p_poolingSet.ParentTransform);
            }

            _go.SetActive(false);
            p_poolingSet.Objects.Add(_go);
            return _go;
        }

        public void AddPoolingSet(PoolingSet poolingSet) {
            if (!poolingSet.IsValid()) { Debug.LogWarning("Set not valid!"); return; }
            if (!_hasInit || !_hasFinishedCreatingSet) { Debug.LogWarning("Another operation is still taking place."); return; }

            StartCoroutine(DoInitSet(poolingSet));
        }

        public bool ContainsPrefab(GameObject p_gameObject) {
            for (int i = 0; i < _poolingSets.Count; i++) {
                if (p_gameObject == _poolingSets[i].Prefab) {
                    return true;
                }
            }
            return false;
        }

        private PoolingSet GetSet(GameObject p_gameObject) {
            for (int i = 0; i < _poolingSets.Count; i++) {
                if (p_gameObject == _poolingSets[i].Prefab) {
                    return _poolingSets[i];
                }
            }
            return null;
        }

        private PoolingSet GetSet(string p_name) {
            for (int i = 0; i < _poolingSets.Count; i++) {
                if (p_name == _poolingSets[i].Prefab.name) {
                    return _poolingSets[i];
                }
            }
            return null;
        }

        public GameObject GetPooledObject(string p_prefabName) {
            PoolingSet set = GetSet(p_prefabName);

            if (set == null) {
                Debug.LogWarning("No set contains the needed prefab: " + p_prefabName);
                return null;
            }

            return GetPooledObject(set);
        }

        public T GetPooledObject<T>(GameObject p_prefab) {
            _go = GetPooledObject(p_prefab);
            return _go.GetComponent<T>();
        }

        public GameObject GetPooledObject(GameObject p_prefab) {
            PoolingSet set = GetSet(p_prefab);

            if (set == null) {
                Debug.LogWarning("No set contains the needed prefab: " + p_prefab.name);
                return null;
            }

            return GetPooledObject(set);
        }

        private GameObject GetPooledObject(PoolingSet p_set) {
            List<GameObject> p_collection = p_set.Objects;
            for (int i = 0; i < p_collection.Count; i++) {
                if (!p_collection[i].activeInHierarchy) {
                    //Debug.Log("Got: " + p_collection[i].name);
                    if (p_set.ParentTransform != null) {
                        p_collection[i].transform.SetParent(p_set.ParentTransform);
                    }
                    p_collection[i].GetComponent<IPoolingItem>()?.Reset();
                    return p_collection[i];
                }
            }

            if (!_canIncreaseInSize) { return null; }
            Debug.LogWarning("No object would be available, but we made more of: " + p_set.Prefab.name);
            return AddObjectToPoolingSet(p_set, p_collection.Count.ToString());
        }
        #endregion Object Handling


        #region Destruction
        public void ClearSet(GameObject go) {
            var poolingSet = GetSet(go);
            ClearSet(poolingSet);
        }

        public void ClearSet(PoolingSet poolingSet) {
            if (poolingSet == null) { return; }

            var objs = poolingSet.Objects;
            if (objs == null || objs.Count == 0) { return; }

            for (int i = poolingSet.Objects.Count; i >= 0; i--) {
                Destroy(objs[i]);
            }
            _poolingSets.Remove(poolingSet);

        }
        #endregion Destruction
    }
}