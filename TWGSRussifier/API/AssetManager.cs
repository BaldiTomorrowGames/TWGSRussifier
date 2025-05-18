using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TWGSRussifier.API
{
    public class AssetManager
    {
        protected Dictionary<Type, Dictionary<string, object>> data = new Dictionary<Type, Dictionary<string, object>>();

        public void Add<T>(string key, T value)
        {
            Type type = value.GetType();
            if (!data.ContainsKey(type))
            {
                data[type] = new Dictionary<string, object>();
            }
            data[type][key] = value;
        }

        public void AddRange<T>(T[] range) where T : UnityEngine.Object
        {
            foreach (var obj in range)
            {
                Add(obj.name, obj);
            }
        }

        public T Get<T>(string key)
        {
            Type requestedType = typeof(T);
            foreach (var typeDict in data)
            {
                if (requestedType.IsAssignableFrom(typeDict.Key) && typeDict.Value.ContainsKey(key))
                {
                    return (T)typeDict.Value[key];
                }
            }
            return default(T);
        }

        public bool Remove<T>(string key)
        {
            Type type = typeof(T);
            if (data.ContainsKey(type) && data[type].ContainsKey(key))
            {
                data[type].Remove(key);
                return true;
            }
            return false;
        }

        public T[] GetAll<T>()
        {
            Type requestType = typeof(T);
            List<T> results = new List<T>();

            foreach (var typeDict in data)
            {
                if (requestType.IsAssignableFrom(typeDict.Key))
                {
                    foreach (var value in typeDict.Value.Values)
                    {
                        results.Add((T)value);
                    }
                }
            }

            return results.ToArray();
        }

        public void ClearAll<T>()
        {
            Type type = typeof(T);
            if (data.ContainsKey(type))
            {
                data[type].Clear();
            }
        }
    }
} 