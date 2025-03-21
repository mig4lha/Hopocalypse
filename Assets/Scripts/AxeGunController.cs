using UnityEngine;

public class AxeGunController : MonoBehaviour
{
    [Header("AxeGun Settings")]
    [SerializeField, Tooltip("Number of pellets per shot (should equal gridRows * gridColumns)")]
    private int pelletCount = 20;
    [SerializeField, Tooltip("Number of rows in the pellet grid")]
    private int gridRows = 5;
    [SerializeField, Tooltip("Number of columns in the pellet grid")]
    private int gridColumns = 5;
    [SerializeField, Tooltip("Maximum horizontal spread angle in degrees")]
    private float spreadAngleX = 10f;
    [SerializeField, Tooltip("Maximum vertical spread angle in degrees")]
    private float spreadAngleY = 5f;
    [SerializeField, Tooltip("Maximum range for each pellet")]
    private float maxRange = 25f;
    [SerializeField, Tooltip("Optional: Random range variability factor (0 means fixed range)")]
    private float rangeVariance = 0f;

    [Header("Muzzle Settings")]
    [SerializeField, Tooltip("Transform representing the muzzle point of the weapon")]
    private Transform muzzleTransform;

    [Header("AxeGun SFX")]
    [SerializeField]
    private AudioSource gunFire;

    public void Shoot()
    {
        // Use the muzzleTransform position as the origin.
        Vector3 origin = muzzleTransform.position;
        // Use the camera's forward for aiming.
        Vector3 forward = Camera.main.transform.forward;

        // Calculate the step between each pellet in the grid.
        float xStep = (gridColumns > 1) ? (spreadAngleX * 2f) / (gridColumns - 1) : 0;
        float yStep = (gridRows > 1) ? (spreadAngleY * 2f) / (gridRows - 1) : 0;

        // Starting offsets (the top-left of the cone, for example).
        float startX = -spreadAngleX;
        float startY = -spreadAngleY;

        int pelletIndex = 0;
        for (int row = 0; row < gridRows; row++)
        {
            float currentPitch = startY + yStep * row;
            for (int col = 0; col < gridColumns; col++)
            {
                if (pelletIndex >= pelletCount)
                    break;
                float currentYaw = startX + xStep * col;

                // Calculate the pellet's direction by applying the fixed pitch and yaw to the forward direction.
                Vector3 pelletDirection = Quaternion.Euler(currentPitch, currentYaw, 0) * forward;

                float currentRange = maxRange;
                if (rangeVariance > 0)
                {
                    currentRange += Random.Range(-rangeVariance, rangeVariance);
                }

                // For debugging: draw a ray from the muzzle.
                Debug.DrawRay(origin, pelletDirection * currentRange, Color.red, 1f);
                if (Physics.Raycast(origin, pelletDirection, out RaycastHit hit, currentRange))
                {
                    if (hit.transform.name == "Target Cube")
                    {
                        Debug.Log($"Pellet {pelletIndex} hit: {hit.transform.name} at distance {hit.distance}");
                    }

                    // Process hit (damage, effects, etc.) here.
                }
                pelletIndex++;
            }
        }

        Debug.Log("------------------------------------------------");

        if (gunFire != null)
        {
            gunFire.Play();
        }
    }
}
