using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Effects
{
    public class BattleEndEffect : MonoBehaviour
    {
        public float animationTime;

        private void Start()
        {
            WinAnimation();
        }

        public void WinAnimation()
        {
            LeanTween.cancel(gameObject);

            transform.localScale = Vector3.one;

            LeanTween.scale(gameObject, Vector3.one * 2, animationTime).setEasePunch();
        }
    }
}