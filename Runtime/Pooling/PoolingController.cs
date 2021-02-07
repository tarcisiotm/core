using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TG.Core {
    /// <summary>
    /// Handles Pooling of Objects
    /// </summary>
    public class PoolingController : MonoBehaviour {
        [Header("Settings")]
        [SerializeField] bool onStart = true;

        [Tooltip("If true, will only create one object per frame")]
        [SerializeField] bool incrementalInitialization = true;
        [SerializeField] bool canIncreaseInSize = true;

        [SerializeField]
        List<PoolingSet> poolingSets = default;

        GameObject go;

        bool hasInit = false;
        bool hasFinishedCreatingSet = false;

        int amount;

        public bool HasInit => hasInit;

        #region Initialization
        void Start() {
            if (onStart) { InitializePool(); }
        }

        public void InitializePool() {
            StartCoroutine(DoInit());
        }

        IEnumerator DoInit() {
            for (int i = 0; i < poolingSets.Count; i++) {
                yield return DoInitSet(poolingSets[i]);
            }
            hasInit = true;
        }

        IEnumerator DoInitSet(PoolingSet poolingSet) {
            if (!poolingSet.IsValid()) { yield break; }

            amount = poolingSet.AmountToPool;
            poolingSet.Objects = new List<GameObject>(amount);

            hasFinishedCreatingSet = false;

            for (int j = 0; j < amount; j++) {
                AddObjectToPoolingSet(poolingSet, j.ToString());
                if (incrementalInitialization) { yield return null; }
            }

            if (!poolingSets.Contains(poolingSet)) {
                poolingSets.Add(poolingSet);
            }

            hasFinishedCreatingSet = true;
        }
        #endregion Initialization


        #region Object Handling
        GameObject AddObjectToPoolingSet(PoolingSet p_poolingSet, string p_suffix) {
            go = Instantiate(p_poolingSet.Prefab);
            go.name += p_poolingSet.Prefab.name + " " + p_suffix;

            if (p_poolingSet.ParentTransform != null) {
                go.transform.SetParent(p_poolingSet.ParentTransform);
            }

            go.SetActive(false);
            p_poolingSet.Objects.Add(go);
            return go;
        }

        public void AddPoolingSet(PoolingSet poolingSet) {
            if (!poolingSet.IsValid()) { Debug.LogWarning("Set not valid!"); return; }
            if (!hasInit || !hasFinishedCreatingSet) { Debug.LogWarning("Another operation is still taking place."); return; }

            StartCoroutine(DoInitSet(poolingSet));
        }

        public bool ContainsPrefab(GameObject p_gameObject) {
            for (int i = 0; i < poolingSets.Count; i++) {
                if (p_gameObject == poolingSets[i].Prefab) {
                    return true;
                }
            }
            return false;
        }

        PoolingSet GetSet(GameObject p_gameObject) {
            for (int i = 0; i < poolingSets.Count; i++) {
                if (p_gameObject == poolingSets[i].Prefab) {
                    return poolingSets[i];
                }
            }
            return null;
        }

        PoolingSet GetSet(string p_name) {
            for (int i = 0; i < poolingSets.Count; i++) {
                if (p_name == poolingSets[i].Prefab.name) {
                    return poolingSets[i];
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
            go = GetPooledObject(p_prefab);
            return go.GetComponent<T>();
        }

        public GameObject GetPooledObject(GameObject p_prefab) {
            PoolingSet set = GetSet(p_prefab);

            if (set == null) {
                Debug.LogWarning("No set contains the needed prefab: " + p_prefab.name);
                return null;
            }

            return GetPooledObject(set);
        }

        GameObject GetPooledObject(PoolingSet p_set) {
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

            if (!canIncreaseInSize) { return null; }
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
            poolingSets.Remove(poolingSet);

        }
        #endregion Destruction


    }
}