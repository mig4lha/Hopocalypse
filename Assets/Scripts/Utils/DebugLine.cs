using UnityEngine;

public class DebugLine : MonoBehaviour
{
    // Reference to the LineRenderer prefab
    public LineRenderer linePrefab;

    /// <summary>
    /// Draws a line from start to end that will disappear after 'duration' seconds.
    /// </summary>
    public void DrawLine(Vector3 start, Vector3 end, float duration = 1f)
    {
        // Instantiate a new LineRenderer instance from the prefab.
        LineRenderer lineInstance = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);

        // Set the positions for the line.
        lineInstance.positionCount = 2;
        lineInstance.SetPosition(0, start);
        lineInstance.SetPosition(1, end);

        // Destroy the line after the specified duration.
        Destroy(lineInstance.gameObject, duration);
    }
}