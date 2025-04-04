using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AxeGunController : MonoBehaviour
{
    #region Tentativa Raycasts
    [Header("AxeGun Settings")]
    [SerializeField, Tooltip("Número balas por tiro")]
    private int pelletCount = 30;
    [SerializeField, Tooltip("Ângulo máximo do spread em X")]
    private float spreadAngleX = 50f;
    [SerializeField, Tooltip("Ângulo máximo do spread em Y")]
    private float spreadAngleY = 25f;
    [SerializeField, Tooltip("Range máximo de cada bala do tiro")]
    private float maxRange = 15f;

    [Header("Ammo and Reload Settings")]
    [SerializeField, Tooltip("Max Ammo da AxeGun")]
    private int maxAmmo = 12;
    [SerializeField, Tooltip("Número de balas atuais da AxeGun")]
    private int currentAmmo;
    [SerializeField, Tooltip("AxeGun Reload Time")]
    private float reloadTime = 2f;
    private bool isReloading = false;

    [Header("Fire Rate Settings")]
    [SerializeField, Tooltip("Tempo de delay entre cada tiro")]
    private float fireRate = 0.3f; // Customizable time between shots
    private float nextTimeToFire = 0f;

    [Header("Muzzle Settings")]
    [SerializeField, Tooltip("Transform da muzzle")]
    private Transform muzzleTransform;
    [SerializeField, Tooltip("Particle system para muzzle flash")]
    private ParticleSystem muzzleFlash;

    [Header("AxeGun SFX")]
    [SerializeField]
    private AudioSource gunFire;

    [Header("UI Elements")]
    [SerializeField, Tooltip("TextMesh da Ammo Count")]
    private TMP_Text ammoTextMesh;
    [SerializeField, Tooltip("Crosshair GameObject")]
    private GameObject crosshair;
    [SerializeField, Tooltip("TextMesh do Reload Countdown")]
    private TMP_Text reloadCountdownText;

    [SerializeField]
    private DebugLine debugLine;

    private void Start()
    {
        // Inicializar ammmo = maxAmmo
        currentAmmo = maxAmmo;
        UpdateAmmoUI();

        // Esconder reload text
        if (reloadCountdownText != null)
            reloadCountdownText.gameObject.SetActive(false);
    }

    // Atualiza o UI de ammo a cada frame pro valor atual de ammo
    private void UpdateAmmoUI()
    {
        if (ammoTextMesh != null)
            ammoTextMesh.text = currentAmmo.ToString();
    }

    // Corotina pra reload da arma
    private IEnumerator Reload()
    {
        isReloading = true;

        // Tirar crosshair durante relaod e meter countdown de reload
        if (crosshair != null)
            crosshair.SetActive(false);
        if (reloadCountdownText != null)
            reloadCountdownText.gameObject.SetActive(true);

        float reloadTimer = reloadTime;
        while (reloadTimer > 0)
        {
            // Atualizar timer de reload no ecrã
            reloadCountdownText.text = reloadTimer.ToString("F2");
            yield return null;
            reloadTimer -= Time.deltaTime;
        }

        // Reset do UI de relaod e ammo
        reloadCountdownText.text = "";
        if (reloadCountdownText != null)
            reloadCountdownText.gameObject.SetActive(false);
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
        isReloading = false;

        if (crosshair != null)
            crosshair.SetActive(true);
    }


    public void Shoot()
    {
        // Não permitir tiros enquanto realoading
        if (isReloading)
            return;

        // Se sem ammo, começar corotina de reload
        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        // Prevenir tiros antes do delay da fire rate
        if (Time.time < nextTimeToFire)
            return;

        nextTimeToFire = Time.time + fireRate;

        currentAmmo--;
        UpdateAmmoUI();

        // Muzzle flash effect do tiro
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        Vector3 origin = muzzleTransform.position;

        // Ray cast pro centro da crosshair (centro do ecrã)
        Ray centerRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));

        Vector3 targetPoint = centerRay.origin + centerRay.direction * maxRange;

        Vector3 centerDirection = (targetPoint - origin).normalized;

        // Disparar a primeira bala no centro (assegurar que pelo menos uma das balas acerta onde a crosshair está)
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
            }
        }

        // Disparar as restantes balas do tiro
        for (int pelletIndex = 1; pelletIndex < pelletCount; pelletIndex++)
        {
            // Randomizar angulos X e Y pro spread
            float randomYaw = Random.Range(-spreadAngleX, spreadAngleX);
            float randomPitch = Random.Range(-spreadAngleY, spreadAngleY);

            // Aplicar pequena rotação random aos tiros
            Vector3 pelletDirection = Quaternion.Euler(randomPitch, randomYaw, 0) * centerDirection;

            float currentRange = maxRange;
            Vector3 endPoint = origin + pelletDirection * currentRange;

            // Debug lines pra ver trajetoria das balas
            debugLine.DrawLine(origin, endPoint, 1f);
            Debug.DrawRay(origin, pelletDirection * currentRange, Color.red, 1f);

            // Verificar se os raycasts colidem com inimigos e processar interação
            if (Physics.Raycast(origin, pelletDirection, out RaycastHit hit, currentRange))
            {
                if (hit.transform.CompareTag("Enemy"))
                {
                    Debug.Log($"Pellet {pelletIndex} hit: {hit.transform.name} at distance {hit.distance}");
                    Destroy(hit.transform.gameObject);
                }
            }
        }

        // Som do tiro
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
