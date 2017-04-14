using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObjectPool : MonoBehaviour {

    public static TileObjectPool instance = null;
    public bool isPersistant = true;
    public Dictionary<string, List<Transform>> tilePool;

#if UNITY_EDITOR
    public List<string> poolKeys = new List<string>();
    public List<int> poolCount = new List<int>();
    public List<int> poolActive = new List<int>();
#endif

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        if (isPersistant)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        tilePool = new Dictionary<string, List<Transform>>();
    }

    public Transform SpawnObject(Transform prefab)
    {
        Transform tileObject = null;
        if (tilePool.ContainsKey(prefab.name))
        {
            for (int i = 0; i < tilePool[prefab.name].Count; i++)
            {
                if (!tilePool[prefab.name][i].gameObject.activeInHierarchy)
                {
                    tileObject = tilePool[prefab.name][i];
                    break;
                }
            }
        }
        else
        {
            tilePool[prefab.name] = new List<Transform>();
        }

        if (tileObject == null)
        {
            tileObject = (Transform)Instantiate(prefab);
            tileObject.name = prefab.name;
            tilePool[prefab.name].Add(tileObject);
        }

#if UNITY_EDITOR
        poolKeys.Clear();
        poolCount.Clear();
        poolActive.Clear();
        foreach(var name in tilePool.Keys)
        {
            poolKeys.Add(name);
            poolCount.Add(tilePool[name].Count);
            int c = 0;
            for(int i = 0; i < tilePool[name].Count; i++)
            {
                if (!tilePool[name][i].gameObject.activeInHierarchy)
                    c++;
            }
            poolActive.Add(c);
        }
#endif
        tileObject.parent = null;
        return tileObject;
    }

    public void ReleaseObject(Transform instance)
    {
        instance.parent = transform;
        instance.gameObject.SetActive(false);
    }
}
