using UnityEngine;

namespace Effects
{
    public class NecromancerText : MonoBehaviour
    {
        public float timeToDestroy;

        private void Start()
        {
            Destroy(gameObject, timeToDestroy);
        }
    }
}