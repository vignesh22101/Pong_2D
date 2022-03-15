using System.Collections;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(KeepLiving_Routine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator KeepLiving_Routine()
    {
        yield return null;

        while (true)
        {
            if (!GameHandler.instance.isBallMoving && transform.parent == null)//to make sure we don't destroy the powerups inside the bricks
                Destroy(gameObject);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
