using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    [Header("Hurt SFX")]
    public List<AudioClip> HurtSounds;

    [Range(0, 1.0f)] public float HurtAmplitude = 1.0f;
    [Range(0.0f, 0.15f)] public float HurtPitchRange;

    [Header("Death SFX")]
    public List<AudioClip> DeathSounds;

    [Range(0, 1.0f)] public float DeathAmplitude = 1.0f;
    [Range(0.0f, 0.15f)] public float DeathPitchRange;

    [Header("Enemy Spawning")]
    [SerializeField] private float MinTimeToSpawnEnemies = 20.0f; // seconds

    [SerializeField] private float MaxTimeToSpawnEnemies = 40.0f;
    private float CurrentEnemySpawnCountdown;
    [SerializeField] private int MinNumOfEnemiesToSpawn = 1;
    [SerializeField] private int MaxNumOfEnemiesToSpawn = 3;
    [SerializeField] private GameObject[] EnemyPrefab;

    [Header("Lazer Attack")]
    [SerializeField] private float Range = 30.0f;

    [SerializeField] private float DamagePerSecond = 1.0f;
    [SerializeField] private float MinLazerDuration = 1.0f;
    [SerializeField] private float MaxLazerDuration = 3.0f;
    [SerializeField] private float MinLaserEvaluationTime = 5.0f;
    [SerializeField] private float MaxLaserEvaluationTime = 10.0f;
    private float CurrentLazerEvaluationTime;
    private float LazerDuration;
    [SerializeField] private float LazerWidth = 0.2f;
    [SerializeField] private Material LazerMaterial;
    [SerializeField] private Sprite LazerTexture;
    private GameObject lazerObject;
    private SpriteRenderer lazerRenderer;

    [Header("Misc")]
    [SerializeField] private float Health = 20.0f;

    [SerializeField] private GameObject deadSprite;

    private NavMeshAgent playerAgent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        playerAgent = gameObject.GetComponent<NavMeshAgent>();
        playerAgent.updateRotation = true;

        CurrentEnemySpawnCountdown = Random.Range(MinTimeToSpawnEnemies, MaxTimeToSpawnEnemies);
        CurrentLazerEvaluationTime = Random.Range(MinLaserEvaluationTime, MaxLaserEvaluationTime);

        // initialize lazer
        lazerObject = new GameObject("BossLazer");
        lazerObject.transform.SetParent(transform);

        lazerRenderer = lazerObject.AddComponent<SpriteRenderer>();
        lazerRenderer.sprite = LazerTexture;
        lazerRenderer.material = LazerMaterial;

        lazerObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        CurrentEnemySpawnCountdown -= Time.deltaTime;
        CurrentLazerEvaluationTime -= Time.deltaTime;

        if (CurrentEnemySpawnCountdown < 0)
        {
            CurrentEnemySpawnCountdown = Random.Range(MinTimeToSpawnEnemies, MaxTimeToSpawnEnemies);
            int EnemiesToSpawn = Random.Range(MinNumOfEnemiesToSpawn, MaxNumOfEnemiesToSpawn + 1);

            for (int i = 0; i < EnemiesToSpawn; i++)
            {
                float X_Position = Random.Range(-44.0f, 44.0f);
                float Z_Position = Random.Range(-137.0f, -101.0f);
                Instantiate(EnemyPrefab[Random.Range(0, EnemyPrefab.Length)], new Vector3(X_Position, 1.228f, Z_Position), transform.rotation);
            }
        }

        if (CurrentLazerEvaluationTime < 0)
        {
            LazerDuration = Random.Range(MinLazerDuration, MaxLazerDuration);
        }

        if (LazerDuration >= 0)
        {
            LazerDuration -= Time.deltaTime;

            if (LazerDuration < 0)
            {
                lazerObject.SetActive(false);
                CurrentLazerEvaluationTime = Random.Range(MinLaserEvaluationTime, MaxLaserEvaluationTime);
            }

            Vector3 StartPos = transform.position;
            Vector3 EndPos = Registry.PlayerObject.transform.position;

            float distanceToPlayer = Vector3.Distance(StartPos, EndPos);

            if (distanceToPlayer <= Range)
            {
                lazerObject.SetActive(true);

                Vector3 direction = EndPos - StartPos;
                float distance = direction.magnitude;

                // Position halfway between start and end
                lazerObject.transform.position = StartPos + direction * 0.5f;

                // Rotate to face the player
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                lazerObject.transform.rotation = Quaternion.Euler(0, 0, angle);

                // Scale laser to match distance
                lazerObject.transform.localScale = new Vector3(distance, LazerWidth, 1);

                Registry.PlayerObject.DealDamage(DamagePerSecond * Time.deltaTime, true);
            }
            else
            {
                lazerObject.SetActive(false);
            }
        }
    }

    public void DealDamage(float Damage)
    {
        Health -= Damage;

        if (Health <= 0)
        {
            Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
                    DeathSounds[Random.Range(0, DeathSounds.Count)],
                    Registry.SFX_Volume * DeathAmplitude * Registry.Master_Volume,
                    0,
                    Random.Range(1.0f - DeathPitchRange, 1.0f + DeathPitchRange));

            Debug.Log("Enemy dead");
            Instantiate(deadSprite, transform.position + -transform.up, transform.rotation);
            Destroy(gameObject);
        }
        else
        {
            Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
                    HurtSounds[Random.Range(0, HurtSounds.Count)],
                    Registry.SFX_Volume * HurtAmplitude * Registry.Master_Volume,
                    0,
                    Random.Range(1.0f - HurtPitchRange, 1.0f + HurtPitchRange));

            Debug.Log("Enemy hurt");
        }
    }
}