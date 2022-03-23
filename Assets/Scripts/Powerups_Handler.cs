using System.Collections.Generic;
using UnityEngine;

public class Powerups_Handler : MonoBehaviour
{
    #region Variables
    internal static Powerups_Handler instance;

    [SerializeField] private Powerups_ScriptableObject powerups_Data;

    [Header("Powerups for this level")]
    [SerializeField] private int multiBall_Count;
    [SerializeField] private int damageRandomizer_Count;

    private List<Brick> bricks;
    #endregion

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        bricks = GameHandler.instance.bricksList;
        Scatter_Powerups();
    }

    /// <summary>
    /// a random powerup gets instantiate from the top
    /// </summary>
    internal void Spawn_RandomPowerup_OnTop()
    {
        int randomInt = Random.Range(0, 100);
        Powerups targetPowerup;

        //assigning different probabilities for the powerups
        if (randomInt <= 33)
            targetPowerup = Powerups.DamageRandomizer;
        else if (randomInt <= 66)
            targetPowerup = Powerups.BallGenerator;
        else
            targetPowerup = Powerups.MultiBall;

        GameObject powerupPrefab = GetPowerup_Prefab(targetPowerup);
        Vector3 topEdge_Pos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1f, 0f));
        Vector3 topLeftCorner_Pos = (Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, 0f))) + (Vector3.right * powerupPrefab.transform.localScale.x);
        Vector3 topRightCorner_Pos = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));

        float randomPos_X = Random.Range(topLeftCorner_Pos.x, topRightCorner_Pos.x);
        Vector3 powerupSpawn_Pos = new Vector3(randomPos_X, topEdge_Pos.y, 0f);

        Instantiate(powerupPrefab, powerupSpawn_Pos, Quaternion.identity);
    }

    private void Scatter_Powerups()
    {
        int total_Powerups_Count = multiBall_Count + damageRandomizer_Count;
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
                SpawnPowerup_InsideBrick(Powerups.MultiBall, spawnIndex);
                multiBall_Count--;
            }
            else if (damageRandomizer_Count > 0)
            {
                SpawnPowerup_InsideBrick(Powerups.DamageRandomizer, spawnIndex);
                damageRandomizer_Count--;
            }
        }

        powerups_Spawn_Indexes.Clear();
    }

    private void SpawnPowerup_InsideBrick(Powerups target_Powerup, int brickIndex)
    {
        Brick targetBrick = bricks[brickIndex];
        GameObject powerup_Prefab = GetPowerup_Prefab(target_Powerup);
        GameObject spawned_Powerup = Instantiate(powerup_Prefab);

        spawned_Powerup.transform.parent = targetBrick.transform;
        spawned_Powerup.transform.localPosition = Vector3.zero;
        spawned_Powerup.SetActive(false);
    }

    private GameObject GetPowerup_Prefab(Powerups targetPowerup)
    {
        foreach (var item in powerups_Data.powerups_Data)
        {
            if (item.powerup == targetPowerup)
                return item.powerUp_Prefab;
        }

        return null;
    }
}
