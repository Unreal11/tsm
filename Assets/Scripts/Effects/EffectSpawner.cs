using UnityEngine;

namespace Effects
{
    public class EffectSpawner : MonoBehaviour
    {
        public static EffectSpawner instance;

        [Header("Sounds effects")]
        public AudioClip startFatigueSound;
        public AudioClip damageFatigueSound;

        [Header("Particle effects")]
        public EffectObject effectHealFlying;
        public EffectObject effectDamageFlying;

        public EffectObject effectUpResourceFlying;
        public EffectObject effectDownResourceFlying;

        public ParticleSystem effectDropCard;

        public ParticleSystem myTowerDamage;
        public ParticleSystem myWallDamage;
        public ParticleSystem myTowerHeal;
        public ParticleSystem myWallHeal;
        public ParticleSystem enemyTowerDamage;
        public ParticleSystem enemyWallDamage;
        public ParticleSystem enemyTowerHeal;
        public ParticleSystem enemyWallHeal;

        public ParticleSystem myUpResource_1;
        public ParticleSystem myUpResource_2;
        public ParticleSystem myUpResource_3;
        public ParticleSystem myDownResource_1;
        public ParticleSystem myDownResource_2;
        public ParticleSystem myDownResource_3;
        public ParticleSystem myUpIncome_1;
        public ParticleSystem myUpIncome_2;
        public ParticleSystem myUpIncome_3;
        public ParticleSystem myDownIncome_1;
        public ParticleSystem myDownIncome_2;
        public ParticleSystem myDownIncome_3;
        public ParticleSystem enemyUpResource_1;
        public ParticleSystem enemyUpResource_2;
        public ParticleSystem enemyUpResource_3;
        public ParticleSystem enemyDownResource_1;
        public ParticleSystem enemyDownResource_2;
        public ParticleSystem enemyDownResource_3;
        public ParticleSystem enemyUpIncome_1;
        public ParticleSystem enemyUpIncome_2;
        public ParticleSystem enemyUpIncome_3;
        public ParticleSystem enemyDownIncome_1;
        public ParticleSystem enemyDownIncome_2;
        public ParticleSystem enemyDownIncome_3;
       
        public ParticleSystem mainLightFatigue;
        public ParticleSystem myLightFatigue;
        public ParticleSystem enemyLightFatigue;
        public GameObject fatigueEffect;

        private AudioSource _audioSource;

        private void Start()
        {
            instance = this;

            _audioSource = GetComponent<AudioSource>();
        }

        public static void SpawnEffect(EffectObject effectObject, Vector3 spawnPosition, Vector3 target)
        {
            if (effectObject != null)
            {
                EffectObject spawnedEffect = Instantiate(effectObject, spawnPosition, Quaternion.identity);

                spawnedEffect.SetTarget(target);
            }
        }

        public static void StartFatigueEffect()
        {
            instance.fatigueEffect.SetActive(true);
            instance._audioSource.PlayOneShot(instance.startFatigueSound);
        }

        public static void ApplyFatigueEffect()
        {
            instance.mainLightFatigue.Play();
            instance.myLightFatigue.Play();
            instance.enemyLightFatigue.Play();
            instance._audioSource.PlayOneShot(instance.damageFatigueSound);
        }
    }
}