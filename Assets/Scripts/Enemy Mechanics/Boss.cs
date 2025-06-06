using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    [SerializeField] private float beamCooldown = 3f;
    [SerializeField] private float beamDuration = 0.5f;
    [SerializeField] private float beamWidth = 0.4f;
    [SerializeField] private Color beamColor = Color.yellow;
    [SerializeField] private float beamDamage = 30f;
    [SerializeField] private float beamTargetDelay = 0.3f; // How far back in time to target (seconds)
    private Queue<(float time, Vector3 position)> playerPositionHistory = new Queue<(float, Vector3)>();
    [SerializeField] private AudioSource beamAudioSource;
    [SerializeField] private Material beamMaterial;

    private float beamTimer = 0f;
    private Vector3 lastPlayerPosition;
    private GameObject currentBeam;

    protected override void Start()
    {
        base.Start();
        isBoss = true;
    }

    protected override void Update()
    {
        if (player == null) return;

        playerPositionHistory.Enqueue((Time.time, player.transform.position));
        while (playerPositionHistory.Count > 0 && Time.time - playerPositionHistory.Peek().time > 2f)
        {
            playerPositionHistory.Dequeue();
        }

        lastPlayerPosition = GetDelayedPlayerPosition();
        Vector3 direction = lastPlayerPosition - transform.position;
        direction.y = 0;
        if (direction.magnitude > 0)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        beamTimer += Time.deltaTime;
        if (beamTimer >= beamCooldown)
        {
            Vector3 delayedPosition = GetDelayedPlayerPosition();
            ShootBeamAt(delayedPosition);
            beamTimer = 0f;
        }

    }

    private Vector3 GetDelayedPlayerPosition()
    {
        Vector3 fallback = player.transform.position;
        foreach (var entry in playerPositionHistory)
        {
            if (Time.time - entry.time >= beamTargetDelay)
                fallback = entry.position;
            else
                break;
        }
        return fallback;
    }


    private void ShootBeamAt(Vector3 targetPosition)
    {
        if (beamAudioSource != null && beamAudioSource.clip != null)
        {
            beamAudioSource.PlayOneShot(beamAudioSource.clip);
        }

        if (currentBeam != null)
            Destroy(currentBeam);

        Vector3 start = transform.position + Vector3.up * 3f;
        Vector3 dir = (targetPosition - start).normalized;
        Vector3 end = start + dir * 1000f;

        RaycastHit hit;
        if (Physics.Raycast(start, dir, out hit, Mathf.Infinity))
        {
            end = hit.point;
            Player hitPlayer = hit.collider.GetComponent<Player>();
            if (hitPlayer != null)
            {
                hitPlayer.TakeDamage(beamDamage);
            }
        }

        currentBeam = new GameObject("BossBeam");
        currentBeam.transform.position = start;

        var lr = currentBeam.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startWidth = beamWidth;
        lr.endWidth = beamWidth;
        lr.material = beamMaterial;
        lr.material.color = beamColor;
        lr.startColor = beamColor;
        lr.endColor = beamColor;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows = false;



        var light = currentBeam.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = beamColor;
        light.range = 5f;
        light.intensity = 8f;

        StartCoroutine(FadeBeam(lr, light, beamDuration));
    }


    private IEnumerator FadeBeam(LineRenderer lr, Light light, float duration)
    {
        float fadeInTime = duration * 0.2f;
        float fadeOutTime = duration * 0.3f;
        float holdTime = duration - fadeInTime - fadeOutTime;

        // Fade in
        for (float t = 0; t < fadeInTime; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0f, 1f, t / fadeInTime);
            SetBeamAlpha(lr, light, alpha);
            yield return null;
        }
        SetBeamAlpha(lr, light, 1f);

        // Hold
        yield return new WaitForSeconds(holdTime);

        // Fade out
        for (float t = 0; t < fadeOutTime; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(1f, 0f, t / fadeOutTime);
            SetBeamAlpha(lr, light, alpha);
            yield return null;
        }
        SetBeamAlpha(lr, light, 0f);

        Destroy(lr.gameObject);
    }

    private void SetBeamAlpha(LineRenderer lr, Light light, float alpha)
    {
        Color c = beamColor;
        c.a = alpha;
        lr.startColor = c;
        lr.endColor = c;
        if (lr.material != null)
            lr.material.color = c;
        if (light != null)
            light.intensity = 8f * alpha;
    }
}
