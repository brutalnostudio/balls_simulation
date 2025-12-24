using UnityEngine;

public class ObstacleRotate : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 90f;           // stepeni po sekundi
    public bool clockwise = true;               // smer rotacije
    public Transform pivotPoint;                // tačka oko koje se rotira, ako je null rotira oko centra objekta
    public Vector3 rotationAxis = new Vector3(0, 0, 1); // osa rotacije (default Z)

    void Update()
    {
        float rotationStep = rotationSpeed * Time.deltaTime;
        if (!clockwise)
            rotationStep = -rotationStep;

        if (pivotPoint != null)
        {
            // Rotira oko pivot tačke
            transform.RotateAround(pivotPoint.position, rotationAxis, rotationStep);
        }
        else
        {
            // Rotira oko svog centra
            transform.Rotate(rotationAxis, rotationStep, Space.World);
        }
    }
}
