using UnityEngine;

public class DebugLine : MonoBehaviour
{
    public LineRenderer linePrefab;

    public void DrawLine(Vector3 start, Vector3 end, float duration = 1f)
    {
        LineRenderer lineInstance = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        lineInstance.positionCount = 2;
        lineInstance.SetPosition(0, start);
        lineInstance.SetPosition(1, end);
        Destroy(lineInstance.gameObject, duration);
    }
}