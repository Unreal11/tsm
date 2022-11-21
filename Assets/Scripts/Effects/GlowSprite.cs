using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Effects
{
    public class GlowSprite : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;

        [Range(0, 1f)]
        public float maxGlow = 1f;
        [Range(0, 1f)]
        public float minGlow = 0;
        public float speedGlow = 0.01f;
        public float randomSpeedGlow = 0.45f;

        private bool _isUpGlow;
        private float _maxGlow;
        private float _minGlow;

        private void Start()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>()
                    ?? throw new NullReferenceException($"{nameof(SpriteRenderer)} in {name} not founded!");
            }
        }

        private void FixedUpdate()
        {
            Color color = spriteRenderer.color;

            color.a = _isUpGlow
                ? color.a + speedGlow
                : color.a - speedGlow;

            if (color.a >= _maxGlow)
            {
                _isUpGlow = false;
                _minGlow = Random.Range(minGlow, minGlow + randomSpeedGlow);
            }

            if (color.a <= _minGlow)
            {
                _isUpGlow = true;
                _maxGlow = Random.Range(maxGlow - randomSpeedGlow, maxGlow);
            }

            spriteRenderer.color = color;
        }
    }
}