using System.Text;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class CodeLineNumbers : MonoBehaviour
{
    [SerializeField] TMP_InputField input;
    [SerializeField] TMP_Text numbers;
    string previous;
    int previousLines = -1;
    void LateUpdate() { Refresh(); }
    public void Refresh()
    {
        if (input == null || numbers == null) return;
        input.textComponent.ForceMeshUpdate();
        var info = input.textComponent.textInfo;
        if (previous != input.text || previousLines != info.lineCount || string.IsNullOrEmpty(numbers.text))
        {
            var text = new StringBuilder(); int sourceLine = 1;
            for (int row=0;row<Mathf.Max(10,info.lineCount);row++)
            {
                bool newLine = row==0 || row>=info.lineCount;
                if (row>0 && row<info.lineCount)
                {
                    int first=info.lineInfo[row].firstCharacterIndex;
                    newLine=first>0 && info.characterInfo[first-1].character=='\n';
                }
                if(newLine)text.Append(sourceLine++);
                text.Append('\n');
            }
            numbers.text=text.ToString(); previous=input.text; previousLines=info.lineCount;
        }
        numbers.fontSize=input.textComponent.fontSize;
        numbers.lineSpacing=input.textComponent.lineSpacing;
        var pos=numbers.rectTransform.anchoredPosition;
        pos.y=input.textComponent.rectTransform.anchoredPosition.y;
        numbers.rectTransform.anchoredPosition=pos;
    }
}
