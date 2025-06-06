using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEngine.UI.Image;

public class AxeGunController : MonoBehaviour
{
    public int currentAmmo;
    public float nextTimeToFire = 0f;

    private bool isReloading = false;

    [Header("Muzzle Settings")]
    [SerializeField, Tooltip("Transform da muzzle")]
    private Transform muzzleTransform;
    [SerializeField, Tooltip("Particle system para muzzle flash")]
    private ParticleSystem muzzleFlash;
    [SerializeField] private Material tracerMaterial;

    [Header("AxeGun SFX")]
    [SerializeField]
    private AudioSource gunFire;

    [SerializeField]
    private DebugLine debugLine;

    private UIController UIController;

    private WaveController waveController;

    private Player player;

    public PlayerStats stats;

    private void Start()
    {
        UIController = FindAnyObjectByType<UIController>();
        if (UIController == null)
        {
            Debug.LogError("UIController not found in the scene.");
        }

        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
        {
            player = playerGO.GetComponent<Player>();
            if (stats == null)
            {
                stats = player.stats;
            }
        }
        else
        {
            Debug.LogError("Player GameObject not found!");
        }

        GameObject waveControllerGO = GameObject.Find("WaveController");
        if (waveControllerGO != null)
        {
            waveController = waveControllerGO.GetComponent<WaveController>();
        }
        else
        {
            Debug.LogError("WaveController GameObject not found!");
        }

        currentAmmo = stats.maxAmmo;
        UIController.UpdateAmmoUI(currentAmmo);

        UIController.HideReloadElement();
    }

    // Corotina pra reload da arma
    public IEnumerator Reload()
    {
        isReloading = true;

        UIController.HideCrosshairShowReload();

        float reloadTimer = stats.reloadTime;

        //if (player.hasReloadBuff) {
        //    reloadTimer *= 0.5f;
        //} else
        //{
        //    reloadTimer = stats.reloadTime;
        //}

        while (reloadTimer > 0)
        {
            UIController.UpdateReloadTimer(reloadTimer);
            yield return null;
            reloadTimer -= Time.deltaTime;
        }

        currentAmmo = stats.maxAmmo;
        isReloading = false;

        UIController.UpdateAmmoUI(currentAmmo);
        UIController.ResetAmmoAndReloadUI();
    }

    private void CheckIfEnemyIsHitAndApplyDamage(Vector3 origin, Vector3 pelletDirection, float currentRange)
    {
        if (Physics.Raycast(origin, pelletDirection, out RaycastHit hit, currentRange))
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                Enemy enemyComponent = hit.transform.GetComponent<Enemy>();
                if (enemyComponent != null && !enemyComponent.isDefeated)
                {
                    enemyComponent.TakeDmg(stats.pelletDamage);
                }
            }
        }
    }


    public void Shoot()
    {
        // Não permitir tiros enquanto realoading
        if (isReloading)
            return;

        // Se sem ammo, começar corotina de reload
        if (currentAmmo <= 0)
        {
            if (!isReloading)

                StartCoroutine(Reload());
            return;
        }

        // Prevenir tiros antes do delay da fire rate
        if (Time.time < nextTimeToFire)
            return;

        nextTimeToFire = Time.time + stats.fireRate;

        currentAmmo--;
        UIController.UpdateAmmoUI(currentAmmo);

        // Se o último tiro foi disparado, iniciar reload automaticamente
        if (currentAmmo == 0 && !isReloading)
        {
            StartCoroutine(Reload());
        }

        // Muzzle flash effect do tiro
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        Vector3 origin = muzzleTransform.position;

        // Ray cast pro centro da crosshair (centro do ecrã)
        Ray centerRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));

        Vector3 targetPoint = centerRay.origin + centerRay.direction * stats.maxRange;

        Vector3 centerDirection = (targetPoint - origin).normalized;

        // Disparar a primeira bala no centro (assegurar que pelo menos uma das balas acerta onde a crosshair está)
        {
            float currentRange = stats.maxRange;
            Vector3 pelletDirection = centerDirection;
            Vector3 endPoint = origin + pelletDirection * currentRange;

            //debugLine.DrawLine(origin, endPoint, 1f);
            //Debug.DrawRay(origin, pelletDirection * currentRange, Color.red, 1f);
            SpawnTracer(origin, endPoint);

            CheckIfEnemyIsHitAndApplyDamage(origin, pelletDirection, currentRange);
        }

        // Disparar as restantes balas do tiro
        for (int pelletIndex = 1; pelletIndex < stats.pelletCount; pelletIndex++)
        {
            // Randomizar angulos X e Y pro spread
            float randomYaw = Random.Range(-stats.spreadAngleX, stats.spreadAngleX);
            float randomPitch = Random.Range(-stats.spreadAngleY, stats.spreadAngleY);

            // Aplicar pequena rotação random aos tiros
            Vector3 pelletDirection = Quaternion.Euler(randomPitch, randomYaw, 0) * centerDirection;

            float currentRange = stats.maxRange;
            Vector3 endPoint = origin + pelletDirection * currentRange;

            // Debug lines pra ver trajetoria das balas
            //debugLine.DrawLine(origin, endPoint, 1f);
            //Debug.DrawRay(origin, pelletDirection * currentRange, Color.red, 1f);
            SpawnTracer(origin, endPoint);

            // Verificar se os raycasts colidem com inimigos e processar interação
            CheckIfEnemyIsHitAndApplyDamage(origin, pelletDirection, currentRange);
        }

        // Som do tiro
        if (gunFire != null)
        {
            gunFire.Play();
        }
    }


    private void SpawnTracer(Vector3 start, Vector3 end, float width = 0.04f, float duration = 0.08f)
    {
        GameObject tracer = new GameObject("ShotgunTracer");
        var lr = tracer.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startWidth = width;
        lr.endWidth = width;
        lr.material = tracerMaterial;
        lr.startColor = lr.endColor = Color.white;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows = false;
        lr.numCapVertices = 4;

        StartCoroutine(FadeAndDestroyTracer(lr, duration));
    }

    private IEnumerator FadeAndDestroyTracer(LineRenderer lr, float duration)
    {
        float elapsed = 0f;
        Color startColor = lr.startColor;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            Color c = startColor;
            c.a = alpha;
            lr.startColor = c;
            lr.endColor = c;
            yield return null;
        }
        Destroy(lr.gameObject);
    }
}
