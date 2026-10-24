using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class StoryLayoutValidation
{
    [MenuItem("CodeStory/Validar layout e gerar prévias")]
    public static void RenderPreviews() { RenderPreviewSet("layout"); }

    public static void RenderPreviewSet(string prefix)
    {
        var canvas = UnityEngine.Object.FindAnyObjectByType<Canvas>();
        var scaler = canvas.GetComponent<CanvasScaler>();
        var mode = canvas.renderMode; var previousCamera = canvas.worldCamera; var distance = canvas.planeDistance;
        var scaleMode = scaler.uiScaleMode; var factor = scaler.scaleFactor;
        var cameraObject = new GameObject("LayoutValidationCamera", typeof(Camera));
        var camera = cameraObject.GetComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = Color.white;
        camera.cullingMask = 1 << 5; camera.orthographic = true; camera.nearClipPlane = .1f; camera.farClipPlane = 100;
        Directory.CreateDirectory("Temp/CodeStoryValidation");
        try
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = camera; canvas.planeDistance = 10;
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            foreach (var size in new[] { new Vector2Int(1600, 900), new Vector2Int(1280, 720), new Vector2Int(1024, 768) })
            {
                var rt = new RenderTexture(size.x, size.y, 24); var previousRT = RenderTexture.active;
                var texture = new Texture2D(size.x, size.y, TextureFormat.RGB24, false);
                try
                {
                    camera.targetTexture = rt;
                    scaler.scaleFactor = Mathf.Min(size.x / 1600f, size.y / 900f);
                    Canvas.ForceUpdateCanvases();
                    foreach (var text in canvas.GetComponentsInChildren<TMP_Text>()) text.ForceMeshUpdate();
                    Canvas.ForceUpdateCanvases(); camera.Render(); RenderTexture.active = rt;
                    texture.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0); texture.Apply();
                    File.WriteAllBytes("Temp/CodeStoryValidation/" + prefix + "-" + size.x + "x" + size.y + ".png", texture.EncodeToPNG());
                }
                finally { camera.targetTexture = null; RenderTexture.active = previousRT; UnityEngine.Object.DestroyImmediate(texture); rt.Release(); UnityEngine.Object.DestroyImmediate(rt); }
            }
        }
        finally
        {
            canvas.renderMode = mode; canvas.worldCamera = previousCamera; canvas.planeDistance = distance;
            scaler.uiScaleMode = scaleMode; scaler.scaleFactor = factor;
            UnityEngine.Object.DestroyImmediate(cameraObject); Canvas.ForceUpdateCanvases();
        }
    }

    [MenuItem("CodeStory/Testar Python e progressão (Play)")]
    public static void RunSmokeTest()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode || SceneManager.GetActiveScene().isDirty || SceneManager.GetActiveScene().path != "Assets/Scenes/MVP.unity")
            throw new InvalidOperationException("Abra e salve MVP, fora do Play, antes de testar.");
        SessionState.SetBool("CodeStory.SmokeTest", true);
        EditorApplication.playModeStateChanged -= OnPlayMode;
        EditorApplication.playModeStateChanged += OnPlayMode;
        EditorApplication.isPlaying = true;
    }
    [InitializeOnLoadMethod]
    static void Register()
    {
        EditorApplication.playModeStateChanged -= OnPlayMode; EditorApplication.playModeStateChanged += OnPlayMode;
        EditorApplication.delayCall += () =>
        {
            const string request="Temp/CodeStory-preview.request";
            if(!File.Exists(request))return;
            File.Delete(request);
            try
            {
                if(EditorApplication.isPlayingOrWillChangePlaymode || SceneManager.GetActiveScene().path!="Assets/Scenes/MVP.unity")
                    throw new InvalidOperationException("Abra MVP fora do Play para gerar as previas.");
                RenderPreviews();File.WriteAllText("Temp/CodeStory-preview.result","OK: previews rendered without rebuilding scene");
            }
            catch(Exception e){File.WriteAllText("Temp/CodeStory-preview.result",e.ToString());}
        };
    }
    static async void OnPlayMode(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.EnteredPlayMode || !SessionState.GetBool("CodeStory.SmokeTest", false)) return;
        SessionState.SetBool("CodeStory.SmokeTest", false);
        string report;
        try
        {
            await Task.Delay(700);
            var manager = UnityEngine.Object.FindAnyObjectByType<GameManager>();
            var phase = UnityEngine.Object.FindAnyObjectByType<PhaseManager>();
            var all = UnityEngine.Object.FindAnyObjectByType<Canvas>().GetComponentsInChildren<Transform>(true);
            var input = all.Single(t => t.name == "CodeInput").GetComponent<TMP_InputField>();
            var execute = all.Single(t => t.name == "ExecuteButton").GetComponent<Button>();
            var next = all.Single(t => t.name == "NextPhaseButton").GetComponent<Button>();
            var log = all.Single(t => t.name == "LogText").GetComponent<TMP_Text>();
            var bubble=all.Single(t=>t.name=="BabySpeechBubble").gameObject;
            var bubbleText=all.Single(t=>t.name=="BabySpeechText").GetComponent<TMP_Text>();
            var narrator=all.Single(t=>t.name=="SpeechText").GetComponent<TMP_Text>();
            if(input.text.Contains("print("))throw new Exception("Starter code already contains the answer");
            if(bubble.activeSelf)throw new Exception("Bubble visible before first output");
            input.text=string.Join("\n",Enumerable.Range(1,30).Select(i=>"# linha "+i));
            input.GetComponent<CodeLineNumbers>().Refresh();
            if(!all.Single(t=>t.name=="LineNumbers").GetComponent<TMP_Text>().text.Contains("30"))throw new Exception("Line number gutter did not follow 30 lines");
            async Task Run(string code)
            {
                var done = new TaskCompletionSource<bool>();
                void Changed(bool busy) { if (!busy) done.TrySetResult(true); }
                manager.ExecutionStateChanged += Changed;
                try
                {
                    input.text = code; execute.onClick.Invoke();
                    if (execute.interactable) throw new Exception("Execute button did not lock");
                    if (await Task.WhenAny(done.Task, Task.Delay(15000)) != done.Task) throw new TimeoutException("Python did not complete");
                    if (!execute.interactable) throw new Exception("Execute button did not unlock");
                }
                finally { manager.ExecutionStateChanged -= Changed; }
            }
            await Run("# apenas um comentario");
            if(bubble.activeSelf || next.gameObject.activeSelf)throw new Exception("Empty output celebrated or advanced phase");
            await Run("print(\"nao deve aparecer\")\nquantidade = 3");
            if(!log.text.Contains("ainda nao aprendeu") || !narrator.text.Contains("ainda nao aprendeu") || bubble.activeSelf || next.gameObject.activeSelf)
                throw new Exception("Phase 1 allowed another command or showed output before validation");
            await Run("print(\"Olá, mundo!\")");
            if(!bubble.activeSelf || bubbleText.text!="Olá, mundo!" || narrator.text!="Lia disse suas primeiras palavras!")throw new Exception("Baby speech/narration separation failed");
            await Task.Delay(200); // Allow the button color transition to finish before the visual check.
            RenderPreviewSet("first-words");
            if (!log.text.Contains("Olá, mundo!") || !next.gameObject.activeSelf) throw new Exception("Phase 1 output/progression failed");
            string savedCode=input.text;
            all.Single(t=>t.name=="HintButton").GetComponent<Button>().onClick.Invoke();
            if(!log.text.Contains("DICA") || input.text!=savedCode)throw new Exception("Hint changed code or did not show");
            await Run("print(\"<b>texto</b>\")");
            if (!log.text.Contains("<b>texto</b>")) throw new Exception("Literal Python markup was changed");
            await Run("print("); if (!log.text.Contains("sintaxe")) throw new Exception("Syntax error feedback failed");
            if(bubble.activeSelf)throw new Exception("Stale bubble after syntax error");
            next.onClick.Invoke();
            if(bubble.activeSelf)throw new Exception("Baby bubble leaked into phase 2");
            if (phase.FaseAtual != 2) throw new Exception("Phase transition failed");
            if(all.Single(t=>t.name=="StoryIllustration").GetComponent<Image>().sprite.name!="childhood-apples-approved")
                throw new Exception("Phase 2 artwork not assigned");
            await Run("print(\"Ola!\")");
            if(!bubble.activeSelf || bubbleText.text!="Ola!" || !log.text.Contains("variavel inteira quantidade"))
                throw new Exception("Phase 2 output without quantity did not preserve feedback/bubble");
            await Run("print(");
            if(bubble.activeSelf)throw new Exception("Phase 2 stale bubble on error");
            await Run("quantidade = 3");
            if(bubble.activeSelf)throw new Exception("Phase 2 bubble with no output");
            await Run("quantidade = 3\nprint(quantidade)");
            if(!bubble.activeSelf || bubbleText.text!="3")throw new Exception("Phase 2 printed quantity missing from bubble");
            if (!all.Single(t => t.name == "EndingPanel").gameObject.activeSelf) throw new Exception("Phase 2 ending failed");
            all.Single(t => t.name == "BackToStory").GetComponent<Button>().onClick.Invoke();
            if (all.Single(t => t.name == "EndingPanel").gameObject.activeSelf) throw new Exception("Dismiss ending failed");
            await Task.Delay(200);
            RenderPreviewSet("phase-two");
            all.Single(t=>t.name=="ClearButton").GetComponent<Button>().onClick.Invoke();
            if(input.text.Length!=0)throw new Exception("Clear did not clear the code");
            report = "PASS: phase 1 print-only restriction; starter without print; no bubble on empty output/errors in either phase; separate narration and speech; phase 2 approved artwork and print bubble (with/without quantity); 30-line gutter; clear and hint; live Python UTF-8; execute lock/unlock; progression; integer evaluation; ending dismiss.";
        }
        catch (Exception e) { report = "FAIL: " + e; }
        File.WriteAllText("Temp/CodeStoryValidation/play-tests.txt", report);
        Debug.Log(report); EditorApplication.isPlaying = false;
    }
}
