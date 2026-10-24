using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Explicit, repeatable authoring tool. The one-shot request is local to Temp, never shipped.
[InitializeOnLoad]
public static class StoryLayoutBuilder
{
    const string Request = "Temp/CodeStory-layout.request";
    const string ScenePath = "Assets/Scenes/MVP.unity";
    static readonly Color Ink = Hex("292653"), Cream = Hex("FFF9EB"), Lavender = Hex("D4C9F2"), Blue = Hex("DDEFFC"), Peach = Hex("FFDBC5"), Mint = Hex("A1E7CA");
    static TMP_FontAsset font, pixelFont, codeFont;
    static Transform root;

    static StoryLayoutBuilder() { EditorApplication.delayCall += ProcessRequest; }
    static void ProcessRequest()
    {
        if (!File.Exists(Request)) return;
        File.Delete(Request);
        try { Apply(); StoryLayoutValidation.RenderPreviews(); EditorSceneManager.SaveScene(SceneManager.GetActiveScene()); File.WriteAllText("Temp/CodeStory-layout.result", "OK: layout saved and previews rendered"); EditorApplication.delayCall += StoryLayoutValidation.RunSmokeTest; }
        catch (Exception e) { File.WriteAllText("Temp/CodeStory-layout.result", e.ToString()); Debug.LogException(e); }
    }

    [MenuItem("CodeStory/Aplicar layout aprovado")]
    public static void Apply()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) throw new InvalidOperationException("Saia do Play antes de aplicar o layout.");
        var scene = SceneManager.GetActiveScene();
        if (scene.path != ScenePath || scene.isDirty) throw new InvalidOperationException("Abra e salve MVP antes de aplicar o layout.");
        root = scene.GetRootGameObjects().Single(g => g.name == "Canvas").transform.Find("MainPanel");
        pixelFont = ImportFont("PressStart2P"); codeFont = ImportFont("IBMPlexMonoMedium"); font = ImportFont("VT323");
        // Remove only the decorative objects authored by earlier versions of this tool.
        string[] legacy = { "FrameShine", "CornerFalseFalse", "CornerFalseTrue", "CornerTrueFalse", "CornerTrueTrue", "ToolbarSpace", "ToolbarCloud", "ToolbarStar", "HeaderCloud", "HeaderStar", "FooterStar" };
        foreach (var t in root.GetComponentsInChildren<Transform>(true).Reverse())
            if (legacy.Contains(t.name) || (t.parent != null && t.parent.name == "Masthead" && t.name.StartsWith("Star"))) UnityEngine.Object.DestroyImmediate(t.gameObject);
        var scaler = root.GetComponentInParent<CanvasScaler>();
        scaler.referenceResolution = new Vector2(1600, 900); scaler.matchWidthOrHeight = .5f;
        Paint(root, Cream, false);
        var ide = Find("IDEPanel"); var log = Find("LogPanel"); var story = Find("ScenePanel");
        Place(ide, root, 0, .29f, .585f, 1, 14, 8, -8, -66);
        Place(log, root, 0, 0, .585f, .29f, 14, 16, -8, -8);
        Place(story, root, .585f, 0, 1, 1, 8, 16, -14, -66);
        Paint(ide, Cream); Paint(log, Blue); Paint(story, Peach);
        Header(ide, "MEU CODIGO", "Python", Lavender, PixelIcon.Kind.Document);
        Header(log, "O QUE ACONTECEU", "", Hex("B9D8F5"), PixelIcon.Kind.Terminal);
        Header(story, "NOSSA HISTORIA", "", Hex("FFC5B8"), PixelIcon.Kind.Book);
        var masthead = Box("Masthead", root, Cream);
        Place(masthead, root, 0, 1, 1, 1, 0, -55, 0, 0); Paint(masthead, Cream, false);
        Icon("BrandStar", masthead, PixelIcon.Kind.Star, .15f, .5f, -35, -19, 3, 19);
        var brand=Label("Brand", masthead, "CodeStory", 28, FontStyles.Normal, .15f, 0, .33f, 1, 18, 0, 0, 0); brand.font=pixelFont;
        var chapter = Label("Chapter", masthead, "CAPITULO 01 : PRIMEIRAS PALAVRAS", 28, FontStyles.Bold, .34f, 0, .87f, 1, 14, 0, -10, 0);
        Plain("BrandDivider", masthead, Hex("AAA4C9"), .33f, .2f, .33f, .8f, 0, 0, 2, 0);
        Plain("HeaderRule", masthead, Ink, 0, 0, 1, 0, 0, 0, 0, 3);
        Icon("CloudLeft",masthead,PixelIcon.Kind.Cloud,0,0,0,3,100,49);
        Icon("CloudRight",masthead,PixelIcon.Kind.Cloud,1,0,-105,3,-5,49);
        var input = Find("CodeInput").GetComponent<TMP_InputField>();
        Place(input.transform, ide, 0, 0, 1, 1, 10, 105, -10, -55);
        Paint(input.transform, Hex("FFFDF5")); input.GetComponent<PixelFrame>().style=PixelFrame.Style.Paper;
        input.GetComponent<Image>().raycastTarget = true;
        input.text = "# Vamos dizer nossa primeira palavra!\n";
        input.richText = false; input.lineType = TMP_InputField.LineType.MultiLineNewline;
        input.fontAsset = codeFont; input.pointSize = 24; input.customCaretColor = true; input.caretColor = Ink;
        input.selectionColor = Hex("CDC1EF"); input.onFocusSelectAll = false;
        input.textComponent.richText = false; input.textComponent.color = Ink;
        input.textComponent.fontSize = 24; input.textComponent.enableAutoSizing = false;
        input.textComponent.alignment = TextAlignmentOptions.TopLeft;
        input.textComponent.margin = Vector4.zero; input.textComponent.lineSpacing=5;
        input.textComponent.fontStyle = FontStyles.Normal;
        Plain("GutterBackground",input.transform,Hex("F5F2E9"),0,0,0,1,3,3,68,-3).SetAsFirstSibling();
        Plain("GutterRule",input.transform,Hex("E0DDD3"),0,0,0,1,68,3,70,-3);
        Place(input.textViewport, input.transform, 0, 0, 1, 1, 88, 16, -20, -18);
        Place(input.textComponent.transform, input.textViewport, 0, 0, 1, 1, 0, 0, 0, 0);
        var numberViewport=GetOrCreate("LineNumberViewport",input.transform,typeof(RectMask2D)).transform;
        Place(numberViewport,input.transform,0,0,0,1,12,16,57,-18);
        var numbers=Label("LineNumbers",numberViewport,"",24,FontStyles.Normal,0,0,1,1,0,0,0,0);
        numbers.alignment=TextAlignmentOptions.TopRight; numbers.color=Hex("8A91AD");
        var gutter=input.GetComponent<CodeLineNumbers>()??input.gameObject.AddComponent<CodeLineNumbers>();
        Assign(gutter,"input",input);Assign(gutter,"numbers",numbers);
        var placeholder = input.placeholder as TMP_Text;
        TextStyle(placeholder,24); placeholder.text = "Escreva seu codigo Python aqui..."; placeholder.color = Hex("77758E");
        Place(placeholder.transform, input.textViewport, 0, 0, 1, 1, 0, 0, 0, 0);
        var execute = Find("ExecuteButton").GetComponent<Button>();
        Place(execute.transform, ide, 0, 0, .31f, 0, 26, 24, -2, 84);
        StyleButton(execute, Mint, "Executar"); ButtonIcon(execute,PixelIcon.Kind.Play);
        var clear=NewButton("ClearButton",ide,"Limpar",PixelIcon.Kind.Eraser,.34f,.52f);
        var hint=NewButton("HintButton",ide,"Dica",PixelIcon.Kind.Bulb,.54f,.72f);
        var controller=ide.GetComponent<IDEController>();
        Assign(controller,"executeButton",execute);Assign(controller,"clearButton",clear);Assign(controller,"hintButton",hint);
        Assign(controller,"phaseManager",UnityEngine.Object.FindAnyObjectByType<PhaseManager>());
        Assign(controller,"logController",log.GetComponent<LogController>());
        UnityEditor.Events.UnityEventTools.RemovePersistentListener(clear.onClick,controller.LimparCodigo);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(clear.onClick,controller.LimparCodigo);
        UnityEditor.Events.UnityEventTools.RemovePersistentListener(hint.onClick,controller.MostrarDica);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(hint.onClick,controller.MostrarDica);
        Icon("ToolbarToys",ide,PixelIcon.Kind.Blocks,1,0,-88,20,-24,88);
        Icon("ToolbarCloudArt",ide,PixelIcon.Kind.Cloud,1,0,-170,18,-100,52);
        var highlighter = input.GetComponent<PythonSyntaxHighlight>() ?? input.gameObject.AddComponent<PythonSyntaxHighlight>();
        var codeDisplay = GetOrCreate("SyntaxOverlay", input.textComponent.transform, typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
        Place(codeDisplay.transform, input.textComponent.transform, 0, 0, 1, 1, 0, 0, 0, 0);
        TextStyle(codeDisplay, 24); codeDisplay.lineSpacing=5; codeDisplay.richText = true;
        Assign(highlighter, "input", input); Assign(highlighter, "display", codeDisplay);
        highlighter.Refresh();gutter.Refresh();
        var logText = Find("LogText").GetComponent<TMP_Text>();
        logText.text = "> Tudo pronto!\nEscreva sua primeira palavra e clique em Executar.";
        ScrollText(log, logText, "LogScroll", 30, 18, -24, -73, 23);
        Assign(log.GetComponent<LogController>(), "scroll", log.Find("LogScroll").GetComponent<ScrollRect>());
        var artViewport=GetOrCreate("StoryArtViewport",story,typeof(RectMask2D)).transform;
        Place(artViewport,story,0,.448f,1,1,10,0,-10,-55);
        var art = Find("StoryIllustration").GetComponent<Image>();
        Place(art.transform,artViewport,0,0,1,1,0,0,0,0);
        var sprite = ImportArt("Assets/Art/Story/nursery-approved.png");
        art.sprite = sprite; art.color = Color.white; art.preserveAspect = false; art.raycastTarget = false;
        var aspect=art.GetComponent<AspectRatioFitter>();if(aspect!=null)UnityEngine.Object.DestroyImmediate(aspect);
        Place(art.transform,artViewport,0,0,1,1,0,0,0,0);
        if(art.GetComponent<StoryArtwork>()==null)art.gameObject.AddComponent<StoryArtwork>();
        var bubble=Box("BabySpeechBubble",artViewport,Cream);
        Place(bubble,artViewport,.12f,.67f,.88f,.67f,0,0,0,78);
        Card(bubble,Hex("9396CC"));
        bubble.GetComponent<PixelFrame>().style=PixelFrame.Style.SpeechBubble;
        var oldTail=bubble.Find("Tail");if(oldTail!=null)UnityEngine.Object.DestroyImmediate(oldTail.gameObject);
        var bubbleText=Label("BabySpeechText",bubble,"",24,FontStyles.Normal,0,0,1,1,18,12,-18,-12);
        bubbleText.alignment=TextAlignmentOptions.Center; bubbleText.richText=false;
        bubbleText.enableAutoSizing=true;bubbleText.fontSizeMin=16;bubbleText.fontSizeMax=24;
        bubbleText.overflowMode=TextOverflowModes.Ellipsis;
        Assign(story.GetComponent<SceneController>(),"babySpeechBubble",bubble.gameObject);
        Assign(story.GetComponent<SceneController>(),"babySpeechText",bubbleText);
        bubble.gameObject.SetActive(false);
        var narrator = Box("NarratorCard", story, Hex("E6D7FC"));
        Place(narrator, story, 0, .29f, 1, .448f, 10, 4, -10, 0);Card(narrator,Hex("B59BDF"));
        Icon("NarratorIcon",narrator,PixelIcon.Kind.Book,0,1,24,-70,74,-20);
        Label("NarratorLabel", narrator, "NARRADOR", 23, FontStyles.Bold, 0, 1, 1, 1, 94, -38, -20, -12).color=Hex("67429A");
        var speech = Find("SpeechText").GetComponent<TMP_Text>();
        speech.text = "Toda grande historia comeca com uma pequena palavra.";
        Place(speech.transform, narrator, 0, 0, 1, 1, 94, 12, -24, -45); TextStyle(speech, 20);
        var objective = Box("ObjectiveCard", story, Cream);
        Place(objective, story, 0, 0, 1, .29f, 10, 65, -10, -5);Card(objective,Hex("DDA35A"));
        Icon("ObjectiveIcon",objective,PixelIcon.Kind.Star,0,1,22,-75,76,-20);
        Label("ObjectiveLabel", objective, "SEU OBJETIVO", 23, FontStyles.Bold, 0, 1, 1, 1, 94, -38, -20, -12).color=Hex("976124");
        var objectiveText = Find("ObjectiveText").GetComponent<TMP_Text>();
        objectiveText.text = "Use print() para fazer o bebe dizer sua primeira palavra.";
        ScrollText(objective, objectiveText, "ObjectiveScroll", 94, 60, -20, -45, 20);
        var exampleBox=Plain("ExampleBox",objective,Hex("FFF0D6"),0,0,1,0,94,16,-20,52);
        var example=Label("Example",exampleBox,"Experimente: <color=#7943CA>print</color>(<color=#B04337>\"Ola, mundo!\"</color>)",18,FontStyles.Normal,0,0,1,1,10,2,-10,-2);
        example.richText=true;
        var progress = Label("Progress", story, "1 / 2", 24, FontStyles.Normal, .33f, 0, .5f, 0, 0, 18, 0, 50);
        Icon("FooterCloudLeft",story,PixelIcon.Kind.Cloud,0,0,26,15,90,46);
        Icon("ProgressStar",story,PixelIcon.Kind.Star,.53f,0,0,19,30,49);
        var next = Find("NextPhaseButton").GetComponent<Button>();
        Place(next.transform, story, .61f, 0, 1, 0, 0, 16, -18, 53);
        StyleButton(next, Mint, "Proxima fase"); next.GetComponentInChildren<TMP_Text>().fontSize=18;next.gameObject.SetActive(false);
        var ending = Find("EndingPanel");
        Place(ending, story, 0, .29f, 1, .74f, 18, 8, -18, -8); Paint(ending, Cream);
        var endingTitle = Find("EndingTitle").GetComponent<TMP_Text>();
        Place(endingTitle.transform, ending, 0, .70f, 1, 1, 24, 4, -24, -20); TextStyle(endingTitle, 26); endingTitle.fontStyle = FontStyles.Bold;
        var endingDescription = Find("EndingDescription").GetComponent<TMP_Text>();
        Place(endingDescription.transform, ending, 0, .25f, 1, .70f, 24, 4, -24, -8); TextStyle(endingDescription, 21);
        var close = GetOrCreate("BackToStory", ending, typeof(Image), typeof(Button)).GetComponent<Button>();
        Place(close.transform, ending, 0, 0, 1, .23f, 24, 18, -24, -4);
        if (close.GetComponentInChildren<TMP_Text>() == null) Label("Text", close.transform, "", 22, FontStyles.Normal, 0, 0, 1, 1, 8, 2, -8, -2);
        StyleButton(close, Lavender, "Voltar a historia");
        var endingController = UnityEngine.Object.FindAnyObjectByType<EndingController>();
        UnityEditor.Events.UnityEventTools.RemovePersistentListener(close.onClick, endingController.OcultarFinal);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(close.onClick, endingController.OcultarFinal);
        ending.gameObject.SetActive(false); Find("CompletionPanel").gameObject.SetActive(false);
        var presentation = root.GetComponent<StoryPresentation>() ?? root.gameObject.AddComponent<StoryPresentation>();
        Assign(presentation,"example",example); Assign(presentation, "chapter", chapter); Assign(presentation, "progress", progress);
        Assign(presentation, "storyBackground", story.GetComponent<Image>()); Assign(presentation, "illustration", art); Assign(presentation, "nursery", sprite);
        Assign(presentation, "childhood", ImportArt("Assets/Art/Story/childhood-apples-approved.png"));
        Assign(UnityEngine.Object.FindAnyObjectByType<GameManager>(), "presentation", presentation);
        EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
        Debug.Log("CodeStory: layout atualizado e salvo em MVP.");
    }

    static Transform Find(string name) => root.GetComponentsInChildren<Transform>(true).Single(t => t.name == name);
    static Color Hex(string s) { ColorUtility.TryParseHtmlString("#" + s, out var c); return c; }
    static GameObject GetOrCreate(string name, Transform parent, params Type[] components)
    {
        var t = parent.Find(name);
        if (t != null) return t.gameObject;
        var g = new GameObject(name, new[] { typeof(RectTransform) }.Concat(components).ToArray()); g.layer = 5; g.transform.SetParent(parent, false); return g;
    }
    static void Place(Transform t, Transform parent, float x0, float y0, float x1, float y1, float l, float b, float r, float top)
    {
        t.SetParent(parent, false); var rt = (RectTransform)t; rt.localScale = Vector3.one;
        rt.anchorMin = new Vector2(x0, y0); rt.anchorMax = new Vector2(x1, y1); rt.pivot = new Vector2(.5f, .5f);
        rt.offsetMin = new Vector2(l, b); rt.offsetMax = new Vector2(r, top);
    }
    static Transform Box(string name, Transform parent, Color color)
    {
        var t = GetOrCreate(name, parent, typeof(Image)).transform; Paint(t, color); return t;
    }
    static void Paint(Transform t, Color color, bool border = true)
    {
        var i=t.GetComponent<Image>();i.sprite=null;i.color=color;i.raycastTarget=false;
        foreach(var effect in t.GetComponents<Shadow>())UnityEngine.Object.DestroyImmediate(effect);
        var frame=t.GetComponent<PixelFrame>();
        if(!border){if(frame!=null)UnityEngine.Object.DestroyImmediate(frame);return;}
        if(frame==null)frame=t.gameObject.AddComponent<PixelFrame>();
        frame.style=PixelFrame.Style.Panel; frame.ink=Ink;frame.accent=Hex("9396CC");i.SetVerticesDirty();
    }
    static void Card(Transform t,Color accent)
    {
        var f=t.GetComponent<PixelFrame>();f.style=PixelFrame.Style.Card;f.ink=accent;f.accent=Color.Lerp(accent,Color.white,.35f);
    }
    static Transform Plain(string name,Transform parent,Color color,float x0,float y0,float x1,float y1,float l,float b,float r,float top)
    {
        var t=GetOrCreate(name,parent,typeof(Image)).transform;Paint(t,color,false);Place(t,parent,x0,y0,x1,y1,l,b,r,top);return t;
    }
    static void Icon(string name,Transform parent,PixelIcon.Kind kind,float x,float y,float l,float b,float r,float top)
    {
        var icon=GetOrCreate(name,parent,typeof(CanvasRenderer),typeof(PixelIcon)).GetComponent<PixelIcon>();
        if(icon.GetComponent<CanvasRenderer>()==null)icon.gameObject.AddComponent<CanvasRenderer>();icon.kind=kind;icon.raycastTarget=false;
        Place(icon.transform,parent,x,y,x,y,l,b,r,top);icon.SetVerticesDirty();
    }
    static void ButtonIcon(Button button,PixelIcon.Kind kind)
    {
        Icon("ActionIcon",button.transform,kind,0,.5f,18,-16,50,16);
        var text=button.GetComponentInChildren<TMP_Text>();Place(text.transform,button.transform,0,0,1,1,57,4,-10,-4);
    }
    static Button NewButton(string name,Transform parent,string title,PixelIcon.Kind kind,float left,float right)
    {
        var button=GetOrCreate(name,parent,typeof(Image),typeof(Button)).GetComponent<Button>();
        if(button.GetComponentInChildren<TMP_Text>()==null)Label("Label",button.transform,title,22,FontStyles.Normal,0,0,1,1,0,0,0,0);
        Place(button.transform,parent,left,0,right,0,0,24,0,84);StyleButton(button,Hex("F2EEFC"),title);ButtonIcon(button,kind);return button;
    }
    static void TextStyle(TMP_Text t, float size)
    {
        t.font = codeFont; t.fontSize = size; t.enableAutoSizing = false; t.color = Ink;
        t.alignment = TextAlignmentOptions.TopLeft; t.textWrappingMode = TextWrappingModes.Normal;
        t.overflowMode = TextOverflowModes.Overflow; t.richText = false; t.raycastTarget = false; t.margin = Vector4.zero;
    }
    static TMP_Text Label(string name, Transform parent, string text, float size, FontStyles style, float x0, float y0, float x1, float y1, float l, float b, float r, float top)
    {
        var t = GetOrCreate(name, parent, typeof(TextMeshProUGUI)).GetComponent<TMP_Text>();
        t.text = text; TextStyle(t, size); t.fontStyle = style; t.alignment = TextAlignmentOptions.MidlineLeft;
        if (style == FontStyles.Bold) { t.font = font; t.fontStyle = FontStyles.Bold; }
        Place(t.transform, parent, x0, y0, x1, y1, l, b, r, top); return t;
    }
    static void Header(Transform parent, string title, string tag, Color color,PixelIcon.Kind icon)
    {
        var bar=Box("PanelHeader",parent,color);Place(bar,parent,0,1,1,1,8,-55,-8,-8);
        bar.GetComponent<PixelFrame>().style=PixelFrame.Style.Header;
        Icon("HeaderIcon",bar,icon,0,.5f,18,-17,52,17);
        Label("Title",bar,title,34,FontStyles.Bold,0,0,.80f,1,70,0,0,0);
        var label=Label("Tag",bar,tag,23,FontStyles.Normal,.80f,0,1,1,0,0,-18,0);label.alignment=TextAlignmentOptions.MidlineRight;
        if(tag.Length>0)Icon("PythonIcon",bar,PixelIcon.Kind.Python,1,.5f,-160,-19,-122,19);
    }
    static void StyleButton(Button button, Color color, string text)
    {
        Paint(button.transform, color); button.GetComponent<PixelFrame>().style=PixelFrame.Style.Button; button.targetGraphic = button.GetComponent<Image>(); button.targetGraphic.raycastTarget = true;
        var colors = ColorBlock.defaultColorBlock; colors.highlightedColor = new Color(.9f, .95f, 1); colors.pressedColor = new Color(.75f, .83f, .95f); button.colors = colors;
        var label = button.GetComponentInChildren<TMP_Text>(); TextStyle(label, 24); label.font = codeFont; label.text = text; label.fontStyle = FontStyles.Bold; label.alignment = TextAlignmentOptions.Center;
        Place(label.transform, button.transform, 0, 0, 1, 1, 10, 3, -10, -3);
    }
    static void ScrollText(Transform parent, TMP_Text text, string name, float l, float b, float r, float top, float size)
    {
        var viewport = GetOrCreate(name, parent, typeof(Image), typeof(RectMask2D), typeof(ScrollRect)).transform;
        Place(viewport, parent, 0, 0, 1, 1, l, b, r, top); viewport.GetComponent<Image>().color = new Color(1, 1, 1, .001f);
        Place(text.transform, viewport, 0, 1, 1, 1, 0, 0, -12, 0); ((RectTransform)text.transform).pivot = new Vector2(.5f, 1);
        TextStyle(text, size);
        var fit = text.GetComponent<ContentSizeFitter>() ?? text.gameObject.AddComponent<ContentSizeFitter>(); fit.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        var scroll = viewport.GetComponent<ScrollRect>(); scroll.viewport = (RectTransform)viewport; scroll.content = (RectTransform)text.transform;
        scroll.horizontal = false; scroll.vertical = true; scroll.movementType = ScrollRect.MovementType.Clamped; scroll.scrollSensitivity = 28;
    }
    static Sprite ImportArt(string path)
    {
        if (!File.Exists(path)) throw new FileNotFoundException("Cenário ausente", path);
        AssetDatabase.ImportAsset(path);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path); importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single; importer.filterMode = FilterMode.Point; importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.Uncompressed; importer.maxTextureSize = 2048; importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
    static TMP_FontAsset ImportFont(string name)
    {
        var path = "Assets/Art/Fonts/" + name + " SDF.asset";
        var asset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
        if (asset != null) return asset;
        AssetDatabase.ImportAsset("Assets/Art/Fonts/" + name + ".ttf");
        asset = TMP_FontAsset.CreateFontAsset(AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/Fonts/" + name + ".ttf"));
        asset.name = name + " SDF";
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.AddObjectToAsset(asset.material, asset);
        foreach (var texture in asset.atlasTextures) AssetDatabase.AddObjectToAsset(texture, asset);
        asset.TryAddCharacters("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789áàâãéêíóôõúçÁÀÂÃÉÊÍÓÔÕÚÇ /:;.,!?()#_+-='");
        EditorUtility.SetDirty(asset); AssetDatabase.SaveAssets(); return asset;
    }
    static void Assign(UnityEngine.Object target, string field, UnityEngine.Object value)
    {
        var so = new SerializedObject(target); so.FindProperty(field).objectReferenceValue = value; so.ApplyModifiedPropertiesWithoutUndo();
    }
}
