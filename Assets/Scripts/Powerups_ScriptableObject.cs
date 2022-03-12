using UnityEngine;

[CreateAssetMenu(fileName = "Powerups_Info", menuName = "ScriptableObjects/Powerups_Info")]
public class Powerups_ScriptableObject : ScriptableObject
{
    public Powerups_Data[] powerups_Data;
}

[System.Serializable]
public class Powerups_Data
{
    public GameObject powerUp_Prefab;
    public Powerups powerup;
}
