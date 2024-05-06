using UnityEngine;

public class MakeBoard : MonoBehaviour
{
    public int rows;
    public int cols;

    public GameObject yellowCube;
    public GameObject redCube;

    public Mover mover;

    void Start()
    {
        Vector3 place = new Vector3(0, -.5f, 0);

        for (int row = 0; row < rows; ++row)
        {
            place.z = row - (rows - 1) / 2f;

            for (int col = 0; col < cols; ++col)
            {
                place.x = col - (cols - 1) / 2f;

                GameObject cube = (row + col) % 2 == 1 ? redCube : yellowCube;
                Instantiate<GameObject>(cube, place, Quaternion.identity);
            }
        }

        mover.rows = rows;
        mover.cols = cols;

        Vector3 pos = mover.transform.localPosition;

        pos.x = -(cols / 2f) + 0.5f;
        pos.z = -(rows / 2f) + 0.5f;

        mover.transform.localPosition = pos;
    }
}
