using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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
    [SerializeField] private GameObject LaserPrefab;

    [Header("Misc")]
    [SerializeField] private float Health = 20.0f;
    [SerializeField] private GameMenuManager gameMenuManager;
    [SerializeField] private Slider healthBar;
    [SerializeField] private GameObject deadSprite;

    private NavMeshAgent playerAgent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        CurrentEnemySpawnCountdown = Random.Range(MinTimeToSpawnEnemies, MaxTimeToSpawnEnemies);
        CurrentLazerEvaluationTime = Random.Range(MinLaserEvaluationTime, MaxLaserEvaluationTime);

        gameMenuManager = GameObject.FindGameObjectWithTag("Game Menu Manager").GetComponent<GameMenuManager>();
    }

    private void FireLaser(Vector3 startPos, Vector3 targetPos)
    {
        Vector3 direction = (targetPos - startPos).normalized;

        float yaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        GameObject laser = Instantiate(
            LaserPrefab,
            startPos,
            Quaternion.Euler(-90, yaw, 0)
        );

        BossLaser laserScript = laser.GetComponent<BossLaser>();
        laserScript.DamagePerSecond = DamagePerSecond;
        laserScript.Direction = direction;
    }

    // Update is called once per frame
    private void Update()
    {
        CurrentEnemySpawnCountdown -= Time.deltaTime;
        CurrentLazerEvaluationTime -= Time.deltaTime;

        if (CurrentEnemySpawnCountdown < 0)
        {
            if (healthBar != null)
            {
                healthBar.gameObject.SetActive(true);
            }

            CurrentEnemySpawnCountdown = Random.Range(MinTimeToSpawnEnemies, MaxTimeToSpawnEnemies);
            int EnemiesToSpawn = Random.Range(MinNumOfEnemiesToSpawn, MaxNumOfEnemiesToSpawn + 1);

            for (int i = 0; i < EnemiesToSpawn; i++)
            {
                float X_Position = Random.Range(-43.0f, 43.0f);
                float Z_Position = Random.Range(-136.0f, -100.0f);
                Instantiate(EnemyPrefab[Random.Range(0, EnemyPrefab.Length)], new Vector3(X_Position, 1.228f, Z_Position), transform.rotation);
            }
        }

        if (CurrentLazerEvaluationTime < 0 && LazerDuration <= 0)
        {
            LazerDuration = Random.Range(MinLazerDuration, MaxLazerDuration);
        }

        if (LazerDuration > 0)
        {
            LazerDuration -= Time.deltaTime;

            Vector3 startPos = transform.position;
            Vector3 targetPos = Registry.PlayerObject.transform.position;

            float distanceToPlayer = Vector3.Distance(startPos, targetPos);

            if (distanceToPlayer <= Range)
            {
                if (healthBar != null)
                {
                    healthBar.gameObject.SetActive(true);
                }

                FireLaser(startPos, targetPos);
            }

            if (LazerDuration <= 0)
            {
                CurrentLazerEvaluationTime = Random.Range(MinLaserEvaluationTime, MaxLaserEvaluationTime);
            }
        }
    }

    public void DealDamage(float Damage)
    {
        Health -= Damage;

        if (healthBar != null)
        {
            healthBar.value = Health;
        }

        if (Health <= 0)
        {
            if (Registry.CoreGameInfrastructureObject != null)
            {
                Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
                    DeathSounds[Random.Range(0, DeathSounds.Count)],
                    Registry.SFX_Volume * DeathAmplitude * Registry.Master_Volume,
                    0,
                    Random.Range(1.0f - DeathPitchRange, 1.0f + DeathPitchRange));
            }

            Debug.Log("Boss dead");

            if (gameMenuManager != null)
            {
                gameMenuManager.ShowWinScreen();
            }
            Instantiate(deadSprite, transform.position + -transform.up, transform.rotation);
            Destroy(gameObject);
        }
        else
        {
            if (Registry.CoreGameInfrastructureObject != null)
            {
                Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
                    HurtSounds[Random.Range(0, HurtSounds.Count)],
                    Registry.SFX_Volume * HurtAmplitude * Registry.Master_Volume,
                    0,
                    Random.Range(1.0f - HurtPitchRange, 1.0f + HurtPitchRange));
            }

            Debug.Log("Boss hurt");
        }
    }
}