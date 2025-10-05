using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace TWGSRussifier.Runtime
{
    public class ModSceneManager : MonoBehaviour
    {
        public static ModSceneManager instance = null!;
        public UnityAction onMenuSceneLoad = null!;
        public UnityAction onLogoSceneLoad = null!;
        public UnityAction onWarningsSceneLoad = null!;
        public UnityAction onGameSceneLoad = null!;
        public UnityAction onCreditsSceneLoad = null!;
        public UnityAction onAnySceneLoad = null!;
        public UnityAction onMenuSceneLoadOnce = null!;
        private bool MenuWasLoaded = false;
        private string lastScene = null!;
        
        public void Start()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Update()
        {
            if (lastScene != SceneManager.GetActiveScene().name)
            {
                lastScene = SceneManager.GetActiveScene().name;

                onAnySceneLoad?.Invoke();

                switch (lastScene)
                {
                    case "Logo":
                        onLogoSceneLoad?.Invoke();
                        break;
                    case "Warnings":
                        onWarningsSceneLoad?.Invoke();
                        break;
                    case "MainMenu":
                        onMenuSceneLoad?.Invoke();
                        if (!MenuWasLoaded)
                        {
                            onMenuSceneLoadOnce?.Invoke();
                            MenuWasLoaded = true;
                        }
                        break;
                    case "Game":
                        onGameSceneLoad?.Invoke();
                        break;
                    case "Credits":
                        onCreditsSceneLoad?.Invoke();
                        break;
                }
            }
        }
    }
}
