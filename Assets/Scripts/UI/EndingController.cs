
using TMPro;
using UnityEngine;

public class EndingController : MonoBehaviour
{
    [SerializeField] private GameObject endingPanel;
    [SerializeField] private TMP_Text endingTitle;
    [SerializeField] private TMP_Text endingDescription;

    private void Start()
    {
        OcultarFinal();
    }

    public void MostrarFinal(string titulo, string descricao)
    {
        endingTitle.text = titulo;
        endingDescription.text = descricao;
        endingPanel.SetActive(true);
    }

    public void OcultarFinal()
    {
        endingPanel.SetActive(false);
    }
}
