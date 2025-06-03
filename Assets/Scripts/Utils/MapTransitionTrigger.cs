using UnityEngine;
using UnityEngine.SceneManagement;

public class MapTransitionTrigger : MonoBehaviour
{
    public Transform playerSpawnPoint; // Onde o jogador deve aparecer no mapa 2
    public string sceneToUnload = "Level1"; // Cena antiga para descarregar se quiser

    private bool hasTransitioned = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTransitioned)
        {
            other.transform.position = playerSpawnPoint.position;
            other.transform.rotation = playerSpawnPoint.rotation;

            // Opcional: descarregar cena antiga
            if (!string.IsNullOrEmpty(sceneToUnload))
            {
                SceneManager.UnloadSceneAsync(sceneToUnload);
            }

            hasTransitioned = true;
        }
    }
}
