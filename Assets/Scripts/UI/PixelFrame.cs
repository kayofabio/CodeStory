using UnityEngine;
using UnityEngine.UI;

// Resolution-independent pixel frame, built as UI geometry rather than stretched artwork.
[ExecuteAlways]
public class PixelFrame : BaseMeshEffect
{
    public enum Style { Panel, Card, Button, Header, Paper, SpeechBubble }
    public Style style;
    public Color ink = new Color(.13f, .16f, .39f, 1);
    public Color accent = new Color(.53f, .56f, .83f, 1);
    public override void ModifyMesh(VertexHelper mesh)
    {
        if (!IsActive()) return;
        mesh.Clear();
        var r = graphic.rectTransform.rect;
        var fill = graphic.color;
        if (style == Style.Header)
        {
            Quad(mesh, r.xMin, r.yMin, r.width, r.height, Color.Lerp(fill, Color.white, .16f), fill);
            Quad(mesh, r.xMin, r.yMin, r.width, 2, accent, accent); return;
        }
        if (style == Style.Paper)
        {
            Shape(mesh, r, 0, 3, accent, accent);
            Shape(mesh, r, 2, 2, fill, fill); return;
        }
        // One continuous outline. No offset shadow or overlaid corner ornaments:
        // those stacked dark pixels looked like broken seams at smaller Game-view scales.
        var outline = Color.Lerp(ink, accent, .55f);
        Shape(mesh, r, 0, 8, outline, outline);
        Shape(mesh, r, 2, 6, accent, accent);
        Shape(mesh, r, 4, 4, Color.Lerp(fill, Color.white, .85f), Color.Lerp(fill, Color.white, .85f));
        Shape(mesh, r, 6, 2, Color.Lerp(fill, Color.white, .10f), fill);
        if (style == Style.SpeechBubble)
        {
            float x=r.center.x,y=r.yMin;
            Quad(mesh,x-10,y-4,20,8,outline,outline);
            Quad(mesh,x-8,y-8,16,4,outline,outline);
            Quad(mesh,x-4,y-12,8,4,outline,outline);
            Quad(mesh,x-8,y-2,16,9,fill,fill);
            Quad(mesh,x-6,y-6,12,4,fill,fill);
            Quad(mesh,x-2,y-10,4,4,fill,fill);
        }
    }
    static void Shape(VertexHelper m, Rect r, float inset, float cut, Color top, Color bottom)
    {
        float x=r.x+inset,y=r.y+inset,w=r.width-inset*2,h=r.height-inset*2;
        if(w<=0||h<=0)return;
        cut=Mathf.Min(cut,Mathf.Min(w,h)/3);
        Quad(m,x+cut,y,w-2*cut,h,top,bottom);
        Quad(m,x+cut/2,y+cut/2,cut/2,h-cut,top,bottom);
        Quad(m,x+w-cut,y+cut/2,cut/2,h-cut,top,bottom);
        Quad(m,x,y+cut,cut/2,h-2*cut,top,bottom);
        Quad(m,x+w-cut/2,y+cut,cut/2,h-2*cut,top,bottom);
    }
    static void Quad(VertexHelper m,float x,float y,float w,float h,Color top,Color bottom)
    {
        if(w<=0||h<=0)return;
        int n=m.currentVertCount;
        m.AddVert(new Vector3(x,y),bottom,Vector2.zero); m.AddVert(new Vector3(x,y+h),top,Vector2.zero);
        m.AddVert(new Vector3(x+w,y+h),top,Vector2.zero);m.AddVert(new Vector3(x+w,y),bottom,Vector2.zero);
        m.AddTriangle(n,n+1,n+2);m.AddTriangle(n,n+2,n+3);
    }
}
