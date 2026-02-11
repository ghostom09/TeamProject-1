using UnityEngine;

public class DifficultyLevelManager : MonoBehaviour
{
    private float timer = 0f;
    [SerializeField] private float interval = 15f;

    private int dificultyLevel = 0;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer = 0f;
            dificultyLevel++;
            
        }
    }

    private void DificultyLevelUp()
    {
        
    }
}
