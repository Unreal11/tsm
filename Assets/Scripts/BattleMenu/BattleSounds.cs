using System.Collections.Generic;
using UnityEngine;

namespace BattleMenu
{
    public class BattleSounds : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private List<AudioClip> audioClips;

        private int clipNumber;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            clipNumber = Random.Range(0, audioClips.Count);
        }

        private void Update()
        {
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(audioClips[clipNumber]);

                clipNumber++;

                if (clipNumber >= audioClips.Count)
                {
                    clipNumber = 0;
                }
            }
        }
    }
}