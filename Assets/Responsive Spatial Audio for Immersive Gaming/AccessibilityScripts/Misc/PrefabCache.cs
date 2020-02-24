using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System;
using UnityEngine;

public class PrefabCache {
    private static Dictionary<int, GameObject> cache = new Dictionary<int, GameObject>();
    
    public static GameObject getPrefab(string key)
    {
        if(key == null)
        {
            throw new System.NullReferenceException("Passed null to getPrefab");
        }
        if(cache.ContainsKey(MD5toInt(key)))
        {
            return cache[MD5toInt(key)];
        }
        else
        {
            var prefab = Resources.Load(key) as GameObject;
            if (prefab == null)
            {
                throw new System.NullReferenceException("Invalid prefab key ("+ key + ")");
            }
            cache[MD5toInt(key)] = prefab;
            return prefab;
        }
    }

    public static int MD5toInt(string s)
    {
        MD5 md5Hasher = MD5.Create();
        var hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(s));
        var ivalue = BitConverter.ToInt32(hashed, 0);
        return ivalue;
    }

}
