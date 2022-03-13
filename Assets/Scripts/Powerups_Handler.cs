using System.Collections.Generic;
using UnityEngine;

public class Powerups_Handler : MonoBehaviour
{
    #region Variables
    [SerializeField] Powerups_ScriptableObject powerups_Data;

    [Header("Powerups for this level")]
    [SerializeField] private int multiBall_Count;
    [SerializeField] private int ballGenerator_Count, damageRandomizer_Count;

    private List<Brick> bricks;
    #endregion

    private void Start()
    {
        bricks = GameHandler.instance.bricksList;
        Scatter_Powerups();
    }

    private void Scatter_Powerups()
    {
        int total_Powerups_Count = multiBall_Count + ballGenerator_Count + damageRandomizer_Count;
        List<int> powerups_Spawn_Indexes = new List<int>();
        int maxIndex = bricks.Count;
        int randomInt = Random.Range(0, maxIndex);

        while (total_Powerups_Count-- > 0)
        {
            while (powerups_Spawn_Indexes.Contains(randomInt))
                randomInt = Random.Range(0, maxIndex);

            powerups_Spawn_Indexes.Add(randomInt);
        }

        foreach (int spawnIndex in powerups_Spawn_Indexes)
        {
            if (multiBall_Count > 0)
            {
                SpawnPowerup(Powerups.MultiBall, spawnIndex);
                multiBall_Count--;
            }
            else if (ballGenerator_Count > 0)
            {
                SpawnPowerup(Powerups.BallGenerator, spawnIndex);
                ballGenerator_Count--;
            }
            else if (damageRandomizer_Count > 0)
            {
                SpawnPowerup(Powerups.DamageRandomizer, spawnIndex);
                damageRandomizer_Count--;
            }
        }

        powerups_Spawn_Indexes.Clear();
    }

    private void SpawnPowerup(Powerups target_Powerup, int brickIndex)
    {
        Brick targetBrick = bricks[brickIndex];
        GameObject powerup_Prefab = GetPrefab_Powerup(target_Powerup);

        GameObject spawned_Powerup = Instantiate(powerup_Prefab);
        spawned_Powerup.transform.parent = targetBrick.transform;
        spawned_Powerup.transform.localPosition = Vector3.zero;
        spawned_Powerup.SetActive(false);
    }

    private GameObject GetPrefab_Powerup(Powerups targetPowerup)
    {
        foreach (var item in powerups_Data.powerups_Data)
        {
            if (item.powerup == targetPowerup)
                return item.powerUp_Prefab;
        }

        return null;
    }
}
