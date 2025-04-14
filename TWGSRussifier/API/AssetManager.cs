using System.Collections.Generic;
using UnityEngine;

namespace TWGSRussifier.API
{
    public class AssetManager
    {
        private Dictionary<string, Object> assets = new Dictionary<string, Object>();
        public void Add<T>(string id, T asset) where T : Object
        {
            if (assets.ContainsKey(id))
            {
                assets[id] = asset;
            }
            else
            {
                assets.Add(id, asset);
            }
        }
        public T Get<T>(string id) where T : Object
        {
            if (assets.TryGetValue(id, out Object asset))
            {
                return asset as T;
            }
            return null;
        }
        public List<T> GetAll<T>() where T : Object
        {
            List<T> result = new List<T>();

            foreach (var asset in assets.Values)
            {
                if (asset is T typedAsset)
                {
                    result.Add(typedAsset);
                }
            }

            return result;
        }
        public bool Contains(string id)
        {
            return assets.ContainsKey(id);
        }
    }
}