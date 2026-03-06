using UnityEngine;

public class DifficultyLevelManager : MonoBehaviour
{
    [SerializeField] private float interval = 15f;
    [SerializeField] private int difficultyLevel = 1;

    [SerializeField] private EnemySpawnerManager spawnManager;

    private float timer;

    private void Start()
    {
        timer = 0f;

        if (spawnManager != null)
        {
            spawnManager.ApplyDifficulty(difficultyLevel);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer -= interval;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        difficultyLevel++;

        if (spawnManager != null)
        {
            spawnManager.ApplyDifficulty(difficultyLevel);
        }

        Debug.Log($"Difficulty Level Up → {difficultyLevel}");
    }
}