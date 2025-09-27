using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace TWGSRussifier.Runtime
{
    public class ModSceneManager : MonoBehaviour
    {
        public static ModSceneManager instance;
        public UnityAction onMenuSceneLoad;
        public UnityAction onLogoSceneLoad;
        public UnityAction onWarningsSceneLoad;
        public UnityAction onGameSceneLoad;
        public UnityAction onCreditsSceneLoad;
        public UnityAction onAnySceneLoad;
        public UnityAction onMenuSceneLoadOnce;
        private bool MenuWasLoaded = false;
        private string lastScene;
        
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
