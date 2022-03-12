using UnityEngine;

public class PersistentData : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
