using UnityEngine;

public class AxeGunController : MonoBehaviour
{
    [SerializeField]
    private AudioSource gunFire;

    public void Shoot()
    {
        if (gunFire != null)
        {
            gunFire.Play();
        }
    }
}
