using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private Camera _camera;

    void Start()
    {
        _camera = Camera.main;
    }
    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - _camera.transform.position);
    }

    public void UpdateHealthBar(float currentHP, float maxHP)
    {
        slider.value = currentHP / maxHP;
    }
}
