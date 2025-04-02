using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace TWGSRussifier.Runtime
{
     public class AssetsReloader : MonoBehaviour
    {
        public static AssetsReloader Instance;

        public UnityAction onAssetsReload;

        public KeyCode reloadAssets = KeyCode.F5;

        public void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        public void Update()
        {
            if (Input.GetKeyDown(reloadAssets))
            {
                if (SceneManager.GetActiveScene().name != "MainMenu")
                {
                    ElevatorScreen screen = GameObject.FindObjectOfType<ElevatorScreen>();
                    if (screen != null)
                    {
                        GameObject.DestroyImmediate(screen.gameObject);
                    }
                    Singleton<CoreGameManager>.Instance.ReturnToMenu();
                }
                Debug.Log("Reloading assets...");
                onAssetsReload?.Invoke();
            }
        }
    }
}
