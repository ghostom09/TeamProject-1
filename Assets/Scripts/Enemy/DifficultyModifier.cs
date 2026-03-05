using UnityEngine;

public struct DifficultyModifier
{
    public float healthMultiplier;
    public float damageMultiplier;

    public float tankerSpawnPercent;
    public float rangedSpawnPercent;
    public float supportSpawnPercent;
    public float specialSpawnPercent;
    
    public float enemySpawnPercent;

    public int enemyLimit;
}

public static class DifficultyCalculator
{
    public static DifficultyModifier Calculate(int level)
    {
        DifficultyModifier result = new();

        result.healthMultiplier = 1f;
        result.damageMultiplier = 1f;
        result.enemyLimit = 4;

        // 2~5
        if (level >= 2)
        {
            int count = Mathf.Min(level, 5) - 1;
            result.healthMultiplier *= Mathf.Pow(1.03f, count);
            result.damageMultiplier *= Mathf.Pow(1.025f, count);
        }
        
        // 3 스파이크
        if (level >= 3)
        {
            result.rangedSpawnPercent += 0.04f;
            result.tankerSpawnPercent += 0.06f;
            result.enemyLimit += 2;
        }

        // 5 스파이크
        if (level >= 5)
        {
            result.healthMultiplier *= 1.08f;
            result.damageMultiplier *= 1.07f;
            result.enemySpawnPercent += 0.0003f;
            result.enemyLimit += 1;
        }

        // 6~13
        if (level >= 6)
        {
            int count = Mathf.Min(level, 13) - 5;
            result.healthMultiplier *= Mathf.Pow(1.06f, count);
            result.damageMultiplier *= Mathf.Pow(1.06f, count);
            result.specialSpawnPercent += count * 0.015f;
            result.enemySpawnPercent += 0.0001f;
            result.enemyLimit += 1 * count;
        }

        // 13 스파이크
        if (level >= 13)
        {
            result.healthMultiplier *= 1.25f;
            result.damageMultiplier *= 1.08f;
            result.supportSpawnPercent += 0.05f;
            result.enemySpawnPercent += 0.0005f;
            result.enemyLimit += 2;
        }

        // 14~21
        if (level >= 14)
        {
            int count = Mathf.Min(level, 21) - 13;
            result.healthMultiplier *= Mathf.Pow(1.08f, count);
            result.damageMultiplier *= Mathf.Pow(1.07f, count);
            result.specialSpawnPercent += count * 0.02f;
            result.enemySpawnPercent += 0.0001f;
            result.enemyLimit += 1 * count;
        }

        // 21 스파이크
        if (level >= 21)
        {
            result.healthMultiplier *= 1.35f;
            result.damageMultiplier *= 1.12f;
            result.enemySpawnPercent += 0.0007f;
            result.enemyLimit += 2;
        }

        // 22~
        if (level >= 22)
        {
            int extra = level - 21;
            result.healthMultiplier *= Mathf.Pow(1.04f, extra);
            result.damageMultiplier *= Mathf.Pow(1.04f, extra);
            result.specialSpawnPercent += extra * 0.03f;
            result.enemySpawnPercent += 0.0001f;
            result.enemyLimit += 2 * extra;
        }

        return result;
    }
}
