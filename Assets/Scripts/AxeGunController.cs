using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeGunController : MonoBehaviour
{
    #region Tentativa Raycasts
    [Header("AxeGun Settings")]
    [SerializeField, Tooltip("Number of pellets per shot (should equal gridRows * gridColumns)")]
    private int pelletCount = 30;
    [SerializeField, Tooltip("Maximum horizontal spread angle in degrees")]
    private float spreadAngleX = 50f;
    [SerializeField, Tooltip("Maximum vertical spread angle in degrees")]
    private float spreadAngleY = 25f;
    [SerializeField, Tooltip("Maximum range for each pellet")]
    private float maxRange = 15f;

    [Header("Muzzle Settings")]
    [SerializeField, Tooltip("Transform representing the muzzle point of the weapon")]
    private Transform muzzleTransform;
    [SerializeField, Tooltip("Particle system for the muzzle flash")]
    private ParticleSystem muzzleFlash;

    [Header("AxeGun SFX")]
    [SerializeField]
    private AudioSource gunFire;

    [SerializeField]
    private DebugLine debugLine;

    public void Shoot()
    {
        // Play the muzzle flash particle system
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        Vector3 origin = muzzleTransform.position;

        // Cast a ray from the center of the screen
        Ray centerRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));

        // Determine a target point along the center ray.
        // You might want to use your maxRange, or perform a Physics.Raycast from the camera to see what you hit.
        Vector3 targetPoint = centerRay.origin + centerRay.direction * maxRange;

        // Calculate the center direction from the muzzle to the target point
        Vector3 centerDirection = (targetPoint - origin).normalized;

        // Fire the center pellet ensuring it goes exactly where the crosshair is pointing
        {
            float currentRange = maxRange;
            Vector3 pelletDirection = centerDirection;
            Vector3 endPoint = origin + pelletDirection * currentRange;

            debugLine.DrawLine(origin, endPoint, 1f);
            Debug.DrawRay(origin, pelletDirection * currentRange, Color.red, 1f);

            if (Physics.Raycast(origin, pelletDirection, out RaycastHit hit, currentRange))
            {
                if (hit.transform.CompareTag("Enemy"))
                {
                    Debug.Log($"Center pellet hit: {hit.transform.name} at distance {hit.distance}");
                }
                // Process hit (damage, effects, etc.) here.
            }
        }

        // Fire remaining pellets randomly around the center
        for (int pelletIndex = 1; pelletIndex < pelletCount; pelletIndex++)
        {
            // Randomize angles for yaw (horizontal) and pitch (vertical)
            float randomYaw = Random.Range(-spreadAngleX, spreadAngleX);
            float randomPitch = Random.Range(-spreadAngleY, spreadAngleY);

            // Apply the random rotation to the center direction
            Vector3 pelletDirection = Quaternion.Euler(randomPitch, randomYaw, 0) * centerDirection;

            float currentRange = maxRange;
            Vector3 endPoint = origin + pelletDirection * currentRange;

            debugLine.DrawLine(origin, endPoint, 1f);
            Debug.DrawRay(origin, pelletDirection * currentRange, Color.red, 1f);

            if (Physics.Raycast(origin, pelletDirection, out RaycastHit hit, currentRange))
            {
                if (hit.transform.CompareTag("Enemy"))
                {
                    Debug.Log($"Pellet {pelletIndex} hit: {hit.transform.name} at distance {hit.distance}");
                    Destroy(hit.transform.gameObject);
                }
                // Process hit (damage, effects, etc.) here.
            }
        }

        if (gunFire != null)
        {
            gunFire.Play();
        }
    }


    #endregion

    #region Tentativa Cone Mesh
    //    [Header("Shotgun Settings")]
    //    [SerializeField] private float coneAngleX = 20f; // Horizontal cone angle (wider)
    //    [SerializeField] private float coneAngleY = 10f; // Vertical cone angle (narrower)
    //    [SerializeField] private float maxRange = 25f;
    //    [SerializeField] private float flatDamage = 50f; // Flat damage amount applied to all hits
    //    [SerializeField] private int maxTargetsPerShot = 10;
    //    [SerializeField] private LayerMask targetLayers;
    //    [SerializeField] private bool debugRays = true; // Draw debug rays to visualize detection

    //    [Header("Optional Settings")]
    //    [SerializeField] private bool visualizeShot = true;
    //    [SerializeField] private float visualDuration = 0.2f;
    //    [SerializeField] private Material coneMaterial;
    //    [SerializeField] private bool useDistanceFalloff = false; // Optional distance-based falloff

    //    [Header("Components")]
    //    [SerializeField] private Transform muzzleTransform;
    //    [SerializeField] private AudioSource gunFireAudio;

    //    // Optional: for visualizing the cone
    //    private GameObject coneVisual;

    //    private void Start()
    //    {
    //        if (visualizeShot && coneMaterial != null)
    //        {
    //            // Create a simple cone mesh for visualization
    //            coneVisual = CreateEllipticalConeMesh();
    //            coneVisual.SetActive(false);
    //        }
    //    }

    //    public void Shoot()
    //    {
    //        // Get the origin and direction of the shot
    //        Vector3 origin = muzzleTransform.position;
    //        Vector3 direction = Camera.main.transform.forward;

    //        // Use a slightly larger sphere check to ensure we catch everything in the cone
    //        float checkRadius = maxRange * 1.1f;

    //        // Find all potential targets in range using a non-allocating approach
    //        Collider[] hitColliders = new Collider[maxTargetsPerShot * 2]; // Buffer for potential hits
    //        int numHits = Physics.OverlapSphereNonAlloc(origin, checkRadius, hitColliders, targetLayers);

    //        // Track which objects we've already hit to avoid duplicates
    //        HashSet<GameObject> hitObjects = new HashSet<GameObject>();

    //        // Debug visualization of the sphere check
    //        if (debugRays)
    //        {
    //            Debug.DrawLine(origin, origin + Vector3.up * checkRadius, Color.yellow, 1.0f);
    //            Debug.DrawLine(origin, origin + Vector3.down * checkRadius, Color.yellow, 1.0f);
    //            Debug.DrawLine(origin, origin + Vector3.left * checkRadius, Color.yellow, 1.0f);
    //            Debug.DrawLine(origin, origin + Vector3.right * checkRadius, Color.yellow, 1.0f);
    //            Debug.DrawLine(origin, origin + Vector3.forward * checkRadius, Color.yellow, 1.0f);
    //            Debug.DrawLine(origin, origin + Vector3.back * checkRadius, Color.yellow, 1.0f);
    //        }

    //        for (int i = 0; i < numHits; i++)
    //        {
    //            Collider collider = hitColliders[i];
    //            if (collider == null) continue;

    //            GameObject targetObject = collider.gameObject;

    //            // Skip if we've already processed this game object
    //            if (hitObjects.Contains(targetObject)) continue;

    //            // Use multiple points on the collider for more accurate detection
    //            Vector3[] checkPoints = GetCheckPointsOnCollider(collider);
    //            bool isHit = false;
    //            Vector3 hitPoint = Vector3.zero;
    //            float hitDistance = 0f;

    //            foreach (Vector3 point in checkPoints)
    //            {
    //                // Calculate direction to check point
    //                Vector3 targetDir = (point - origin).normalized;

    //                // --- METHOD 1: Using forward direction and angle between vectors ---
    //                float angle = Vector3.Angle(direction, targetDir);

    //                // Calculate horizontal and vertical components of the angle
    //                Vector3 horizontalTargetDir = new Vector3(targetDir.x, 0, targetDir.z).normalized;
    //                Vector3 horizontalForward = new Vector3(direction.x, 0, direction.z).normalized;
    //                float horizontalAngle = Vector3.Angle(horizontalForward, horizontalTargetDir);

    //                Vector3 verticalPlane = Vector3.Cross(horizontalForward, Vector3.up);
    //                float verticalAngle = Vector3.SignedAngle(direction, targetDir, verticalPlane);

    //                // Check if target direction is within our elliptical cone bounds
    //                bool inEllipticalCone = (horizontalAngle <= coneAngleX / 2) && (Mathf.Abs(verticalAngle) <= coneAngleY / 2);

    //                if (inEllipticalCone)
    //                {
    //                    // Cast a ray to check for obstacles
    //                    RaycastHit hit;
    //                    if (Physics.Raycast(origin, targetDir, out hit, maxRange))
    //                    {
    //                        // If we hit our target object directly
    //                        if (hit.collider.gameObject == targetObject)
    //                        {
    //                            Debug.DrawLine(origin, hit.point, Color.green, 1.0f);
    //                            isHit = true;
    //                            hitPoint = hit.point;
    //                            hitDistance = hit.distance;
    //                            break; // We found a hit point, no need to check others
    //                        }
    //                        else if (debugRays)
    //                        {
    //                            // We hit something else blocking our target
    //                            Debug.DrawLine(origin, hit.point, Color.red, 1.0f);
    //                        }
    //                    }
    //                    else if (debugRays)
    //                    {
    //                        // No hit at all, ray went into empty space
    //                        Debug.DrawRay(origin, targetDir * maxRange, Color.blue, 1.0f);
    //                    }
    //                }
    //                else if (debugRays)
    //                {
    //                    // Point is outside our cone
    //                    Debug.DrawLine(origin, point, Color.grey, 1.0f);
    //                }
    //            }

    //            // Process the hit if we found one
    //            if (isHit)
    //            {
    //                hitObjects.Add(targetObject);

    //                // Calculate damage (flat by default)
    //                float damage = flatDamage;

    //                // Optional: Apply distance-based falloff
    //                if (useDistanceFalloff)
    //                {
    //                    float distanceFactor = 1f - (hitDistance / maxRange);
    //                    damage *= distanceFactor;
    //                }

    //                // Apply damage to the target...

    //                Debug.Log($"Hit: {targetObject.name} at distance {hitDistance:F1} - Damage: {damage:F1}");
    //            }
    //        }

    //        // Play audio
    //        if (gunFireAudio != null)
    //        {
    //            gunFireAudio.Play();
    //        }

    //        // Visualize the cone (optional)
    //        if (visualizeShot && coneVisual != null)
    //        {
    //            StartCoroutine(ShowConeVisual(origin, direction));
    //        }

    //        Debug.Log($"Shotgun fired - Hit {hitObjects.Count} targets");
    //    }

    //    // Get multiple points to check on a collider for more accurate detection
    //    private Vector3[] GetCheckPointsOnCollider(Collider collider)
    //    {
    //        if (collider is BoxCollider || collider is SphereCollider || collider is CapsuleCollider)
    //        {
    //            // For primitive colliders, use bounds
    //            Bounds bounds = collider.bounds;

    //            return new Vector3[]
    //            {
    //                bounds.center,                                          // Center
    //                bounds.center + new Vector3(bounds.extents.x, 0, 0),    // Right
    //                bounds.center - new Vector3(bounds.extents.x, 0, 0),    // Left
    //                bounds.center + new Vector3(0, bounds.extents.y, 0),    // Top
    //                bounds.center - new Vector3(0, bounds.extents.y, 0),    // Bottom
    //                bounds.center + new Vector3(0, 0, bounds.extents.z),    // Front
    //                bounds.center - new Vector3(0, 0, bounds.extents.z)     // Back
    //            };
    //        }
    //        else if (collider is MeshCollider)
    //        {
    //            // For mesh colliders, just use the center for now (could be expanded)
    //            return new Vector3[] { collider.bounds.center };
    //        }
    //        else
    //        {
    //            // Default fallback
    //            return new Vector3[] { collider.bounds.center };
    //        }
    //    }

    //    // Optional visualization methods
    //    private System.Collections.IEnumerator ShowConeVisual(Vector3 origin, Vector3 direction)
    //    {
    //        if (coneVisual != null)
    //        {
    //            coneVisual.transform.position = origin;
    //            coneVisual.transform.rotation = Quaternion.LookRotation(direction);

    //            // Calculate scales for X and Y based on their respective angles
    //            float scaleX = maxRange * 2 * Mathf.Tan(coneAngleX * 0.5f * Mathf.Deg2Rad);
    //            float scaleY = maxRange * 2 * Mathf.Tan(coneAngleY * 0.5f * Mathf.Deg2Rad);

    //            coneVisual.transform.localScale = new Vector3(scaleX, scaleY, maxRange);

    //            coneVisual.SetActive(true);
    //            yield return new WaitForSeconds(visualDuration);
    //            coneVisual.SetActive(false);
    //        }
    //    }

    //    private GameObject CreateEllipticalConeMesh()
    //    {
    //        GameObject cone = new GameObject("ShotgunConeVisual");
    //        cone.transform.parent = transform;

    //        MeshFilter meshFilter = cone.AddComponent<MeshFilter>();
    //        MeshRenderer meshRenderer = cone.AddComponent<MeshRenderer>();

    //        meshFilter.mesh = CreateEllipticalConicalMesh();
    //        meshRenderer.material = coneMaterial;

    //        return cone;
    //    }

    //    private Mesh CreateEllipticalConicalMesh()
    //    {
    //        Mesh mesh = new Mesh();

    //        int segments = 24; // More segments for smoother elliptical shape
    //        Vector3[] vertices = new Vector3[segments + 1];
    //        int[] triangles = new int[segments * 3];

    //        // Apex of the cone
    //        vertices[0] = Vector3.zero;

    //        // Create elliptical base
    //        for (int i = 0; i < segments; i++)
    //        {
    //            float angle = 2 * Mathf.PI * i / segments;
    //            // Create unit ellipse (will be scaled by transform)
    //            vertices[i + 1] = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 1f);
    //        }

    //        // Create triangles
    //        for (int i = 0; i < segments; i++)
    //        {
    //            triangles[i * 3] = 0; // Apex
    //            triangles[i * 3 + 1] = i + 1;
    //            triangles[i * 3 + 2] = (i + 1) % segments + 1;
    //        }

    //        mesh.vertices = vertices;
    //        mesh.triangles = triangles;
    //        mesh.RecalculateNormals();

    //        return mesh;
    //    }
    //}
    #endregion
}
