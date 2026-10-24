using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Presentation only: phase rules and Python execution remain in GameManager.
public class StoryPresentation : MonoBehaviour
{
    [SerializeField] private TMP_Text chapter;
    [SerializeField] private TMP_Text progress;
    [SerializeField] private TMP_Text example;
    [SerializeField] private Image storyBackground;
    [SerializeField] private Image illustration;
    [SerializeField] private Sprite nursery;
    [SerializeField] private Sprite childhood;
    [SerializeField] private Color babyColor = new Color(1f, .86f, .77f);
    [SerializeField] private Color childhoodColor = new Color(.79f, .91f, .85f);

    public void ShowPhase(int phase)
    {
        chapter.text = phase == 1 ? "CAPITULO 01 :  PRIMEIRAS PALAVRAS" : "CAPITULO 02 :  PEQUENAS DESCOBERTAS";
        progress.text = phase + " / 2";
        if(example!=null)example.text=phase==1
            ? "Experimente: <color=#7943CA>print</color>(<color=#B04337>\"Ola, mundo!\"</color>)"
            : "quantidade = 3; <color=#7943CA>print</color>(quantidade)";
        storyBackground.color = phase == 1 ? babyColor : childhoodColor;
        illustration.sprite = phase == 1 || childhood == null ? nursery : childhood;
    }
}
