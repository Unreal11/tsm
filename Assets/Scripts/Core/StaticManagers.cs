using UnityEngine;

namespace Core
{
    public class StaticManagers : MonoBehaviour
    {
        private static StaticManagers instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }
    }
}