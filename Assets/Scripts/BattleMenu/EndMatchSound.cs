using UnityEngine;

namespace BattleMenu
{
    public class EndMatchSound : MonoBehaviour
    {
        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private AudioClip endMatchAudioClip;

        [SerializeField]
        private BattleSounds battleSounds;

        [SerializeField]
        [Range(0f, 1f)]
        private float volume;

        private void Start()
        {
            battleSounds.enabled = false;
            audioSource.Stop();
            audioSource.volume = volume;
            audioSource.PlayOneShot(endMatchAudioClip);
        }
    }
}
