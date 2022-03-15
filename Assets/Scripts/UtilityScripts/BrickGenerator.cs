using UnityEngine;

public class BrickGenerator : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private Transform startingTransform;//bricks generation starts from this position
    [SerializeField] private int row, column;
    [SerializeField] private float spacingX, spacingY;
    [SerializeField] private Transform bricksParent_Transform;
    #endregion

    internal void GenerateBricks()
    {
        GameObject bricksCollection = new GameObject($"Bricks - {row},{column}");

        Vector3 position = startingTransform.localPosition;
        float startPos_X = startingTransform.localPosition.x;
        float startPos_Y = startingTransform.localPosition.y;

        for (int i = 0; i < row; i++)
        {
            position.y = startPos_Y - (i * spacingY);//moving downwards

            for (int j = 0; j < column; j++)
            {
                position.x = (j * spacingX) + startPos_X;//moving rightwards

                Instantiate(brickPrefab, position, Quaternion.identity, bricksCollection.transform).name = $"Brick {i} {j}";
            }
        }

        bricksCollection.transform.parent = bricksParent_Transform;
    }
}
