using TWGSRussifier.API;
using UnityEngine;

namespace TWGSRussifier.Runtime
{
    public class RussifierController : MonoBehaviour
    {
        private static RussifierController instance;
        public AssetManager ModAssets = new AssetManager();
        public static RussifierController Instance => instance;

        public void Start()
        {
            instance = this;
            ModAssets = new AssetManager();
            DontDestroyOnLoad(gameObject);
        }

        public void Load()
        {
            Singleton<LocalizationManager>.Instance.LoadLocalizedText("Subtitles_Russian.json", default(Language));
        }
    }
}
