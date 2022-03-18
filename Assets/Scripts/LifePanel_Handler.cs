using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePanel_Handler : MonoBehaviour
{
    #region Variables
    internal static Transform _transform;
    #endregion

    private void Awake()
    {
        _transform = transform;
        gameObject.SetActive(false);
    }

    internal static void UpdateLives_UI(int playerLives)
    {
        for (int i = 0; i < _transform.childCount; i++)
        {
            _transform.GetChild(i).gameObject.SetActive(i < playerLives);
        }
    }
}
