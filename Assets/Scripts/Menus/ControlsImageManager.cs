using UnityEngine;

public class ControlsImageManager : MonoBehaviour
{
    [SerializeField] private GameObject controlsImagePanel;

    public void ShowControls()
    {
        controlsImagePanel.SetActive(true);
    }

    public void HideControls()
    {
        controlsImagePanel.SetActive(false);
    }
}
