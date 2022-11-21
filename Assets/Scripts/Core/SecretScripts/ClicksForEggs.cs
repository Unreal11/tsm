using System.Collections;
using UnityEngine;

namespace Core.SecretScripts
{
    public class ClicksForEggs : MonoBehaviour
    {
        public int coundClicks = 27;
        public int showTime = 3;
        public GameObject textEgg;
        public ParticleSystem clickParticle;

        private int _elapsedClick;

        private void OnMouseDown()
        {
            _elapsedClick++;

            clickParticle.Play();

            if (_elapsedClick >= coundClicks)
            {
                YoYo();

                _elapsedClick = 0;
            }
        }

        private void YoYo()
        {
            StartCoroutine(TimeShowEggs());
        }

        private IEnumerator TimeShowEggs()
        {
            textEgg.SetActive(true);

            yield return new WaitForSeconds(showTime);

            textEgg.SetActive(false);
        }
    }
}