using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DoorOpener : MonoBehaviour
{
    public Animator doorAnimator;
    public string nextSceneName = "Level2";
    public Transform nextSceneRoot; // Um transform vazio onde o mapa 2 vai ser posicionado (alinhado)

    private AsyncOperation asyncLoad;
    private bool hasLoaded = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasLoaded)
        {
            doorAnimator.SetTrigger("OpenDoor");
            StartCoroutine(LoadNextSceneAdditive());
        }
    }

    private IEnumerator LoadNextSceneAdditive()
    {
        asyncLoad = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Opcional: mover o root da cena 2 para nextSceneRoot.position
        Scene loadedScene = SceneManager.GetSceneByName(nextSceneName);
        if (loadedScene.IsValid())
        {
            GameObject[] rootObjects = loadedScene.GetRootGameObjects();
            foreach (var obj in rootObjects)
            {
                obj.transform.position += nextSceneRoot.position;
                // Ajuste conforme necessário para alinhar exatamente
            }
        }

        hasLoaded = true;
        Debug.Log($"Cena {nextSceneName} carregada e posicionada.");
    }
}
