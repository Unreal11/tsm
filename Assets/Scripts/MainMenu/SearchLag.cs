using UnityEngine;

namespace MainMenu
{
    public class SearchLag : MonoBehaviour
    {
        public float timeLag;

        private float _elapsedTime;
        private bool _isEnable;

        public void EnableLag()
        {
            _elapsedTime = 0;
            _isEnable = true;
        }

        private void FixedUpdate()
        {
            if (_isEnable)
            {
                _elapsedTime += Time.fixedDeltaTime;

                if (_elapsedTime >= timeLag)
                {
                    _elapsedTime = 0;
                    _isEnable = false;
                }
            }
        }
    }
}