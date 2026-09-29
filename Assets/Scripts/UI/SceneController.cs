using TMPro;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField] private TMP_Text speechText;

    public void MostrarFala(string texto)
    {
        speechText.text = texto;
    }
}