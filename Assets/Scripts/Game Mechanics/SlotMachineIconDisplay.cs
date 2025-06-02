using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PrizeIconAndEffect
{
    public Sprite icon;
    public StatusEffectData effectData;
}

public class SlotMachineIconDisplay : MonoBehaviour
{
    [Header("Ícones disponíveis")]
    public List<PrizeIconAndEffect> icons = new List<PrizeIconAndEffect>();

    private List<SpriteRenderer> faceRenderers = new List<SpriteRenderer>();
    private PrizeIconAndEffect finalPrize;
    private Coroutine spinningCoroutine;
    private bool isSpinning = false;

    void Awake()
    {

        faceRenderers = new List<SpriteRenderer>();

        var cilindros = GameObject.Find("Prize").transform;

        foreach (Transform cilindro in cilindros)
        {
            if (!cilindro.name.StartsWith("Cilindro")) continue;

            foreach (Transform face in cilindro)
            {
                var sr = face.GetComponent<SpriteRenderer>();
                if (sr != null)
                    faceRenderers.Add(sr);
            }
        }
    }

    void Start()
    {
        AssignRandomIcons();
    }

    public void AssignRandomIcons()
    {
        if (icons.Count == 0 || faceRenderers.Count == 0)
        {
            Debug.LogWarning("Ícones ou faces não encontrados.");
            return;
        }

        List<PrizeIconAndEffect> shuffled = new List<PrizeIconAndEffect>(icons);
        ShuffleList(shuffled);

        for (int i = 0; i < faceRenderers.Count; i++)
        {
            int index = i % shuffled.Count;
            faceRenderers[i].sprite = shuffled[index].icon;
        }
    }

    public void StartSpinning(float interval = 0.5f)
    {
        if (isSpinning) return;

        isSpinning = true;
        spinningCoroutine = StartCoroutine(SpinIcons(interval));
    }

    public void StopSpinningAndChooseFinal()
    {
        if (!isSpinning) return;

        StopCoroutine(spinningCoroutine);
        isSpinning = false;

        int chosenIndex = Random.Range(0, icons.Count);
        finalPrize = icons[chosenIndex];

        foreach (var renderer in faceRenderers)
        {
            if (renderer.name.StartsWith("MainSquare")) continue; // Evita sobrescrever os que já foram aplicados no SlotMachineController

            var randomIcon = icons[Random.Range(0, icons.Count)];
            renderer.sprite = randomIcon.icon;
        }
    }

    public PrizeIconAndEffect GetFinalPrize()
    {
        return finalPrize;
    }

    private IEnumerator SpinIcons(float interval)
    {
        isSpinning = true;

        while (true)
        {
            AssignRandomIcons();
            yield return new WaitForSeconds(interval);
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
