using UnityEditor;
using UnityEngine;

namespace MainMenu
{
    public class DragonMove : MonoBehaviour
    {
        [SerializeField]
        private float speed;
        [SerializeField]
        private int nextSpot;
        [SerializeField]
        private Transform[] moveSpots;

        [SerializeField] 
        private ParticleSystem fireParticle;
        [SerializeField]
        private GameObject fireLights;

        private void Start()
        {
            transform.position = moveSpots[nextSpot].transform.position;
            fireParticle = GetComponentInChildren<ParticleSystem>();
            fireParticle.Stop();
        }
        private void Update()
        {
            Move();
        }

        private void Move()
        {
            transform.position = Vector3.MoveTowards(transform.position, moveSpots[nextSpot].position, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, moveSpots[nextSpot].position) < 0.2f)
                nextSpot += 1;

            if (nextSpot == moveSpots.Length)
                nextSpot = 0;

            if (Vector3.Distance(transform.position, moveSpots[0].position) < 0.2f)
                EnableFireParticle();
            
            if (Vector3.Distance(transform.position, moveSpots[4].position) < 0.2f)
               DisableFireParticle();
        }

        private void EnableFireParticle()
        {
            fireParticle.Play();
            fireLights.SetActive(true);
        }

        private void DisableFireParticle()
        {
            fireParticle.Stop();
            fireLights.SetActive(false);
        }
    }
}