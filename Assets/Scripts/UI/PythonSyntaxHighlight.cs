using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

// A display layer only. TMP_InputField always contains literal Python, never markup.
[ExecuteAlways]
public class PythonSyntaxHighlight : MonoBehaviour
{
    [SerializeField] private TMP_InputField input;
    [SerializeField] private TMP_Text display;
    private string previous;
    private static readonly Regex Tokens = new Regex(@"(\#[^\r\n]*|'(?:\\.|[^'\\])*'|""(?:\\.|[^""\\])*""|\b(?:print|if|else|for|while|in|def|return|True|False|None|import)\b)");
    private void OnEnable() { if (display != null) display.enabled = true; Refresh(); }
    private void OnDisable()
    {
        if (display != null) display.enabled = false;
        if (input != null) input.textComponent.color = new Color(.16f, .15f, .33f, 1);
    }
    private void LateUpdate()
    {
        if (input == null || display == null) return;
        if (previous != input.text) Refresh();
        display.fontSize = input.textComponent.fontSize;
        display.textWrappingMode = input.textComponent.textWrappingMode;
    }
    public void Refresh()
    {
        if (input == null || display == null) return;
        previous = input.text;
        input.textComponent.color = new Color(0, 0, 0, 0);
        var result = new StringBuilder(); int start = 0;
        foreach (Match match in Tokens.Matches(previous))
        {
            result.Append(Literal(previous.Substring(start, match.Index - start)));
            var color = match.Value.StartsWith("#") ? "287968" : match.Value.StartsWith("\"") || match.Value.StartsWith("'") ? "B04337" : "7943CA";
            result.Append("<color=#").Append(color).Append('>').Append(Literal(match.Value)).Append("</color>");
            start = match.Index + match.Length;
        }
        result.Append(Literal(previous.Substring(start)));
        display.text = result.ToString();
    }
    private static string Literal(string value)
    {
        // Escape each '<' separately so user-entered </noparse> cannot close the span.
        return "<noparse>" + value.Replace("<", "</noparse><noparse><</noparse><noparse>") + "</noparse>";
    }
}
