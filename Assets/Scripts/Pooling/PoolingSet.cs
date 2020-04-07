using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines sets of an object to be pooled
/// </summary>
[System.Serializable]
public class PoolingSet {

	public string Name;
	public GameObject Prefab;
    //amount alive?
	public int AmountToPool = 10;
	public Transform ParentTransform;

    public List<GameObject> Objects;

	public bool IsValid(){
		if(Prefab == null || AmountToPool < 1){
			return false;
		}
		return true;
	}

	public int GetLastValidIndex(){
        for (int i = 0; i < Objects.Count; i++)
		{
			if(Objects[i].activeInHierarchy){
				if(i == 0){
					return -1;
				}else{
					return i - 1;
				}
			}
		}

        //All are active
		return -1;
	}

}
