using UnityEngine;
using UnityEngine.UI;

// Object-fit: cover for a sprite. UV cropping keeps the original proportions at every canvas size.
[RequireComponent(typeof(Image))]
public class StoryArtwork : BaseMeshEffect
{
    public override void ModifyMesh(VertexHelper mesh)
    {
        if (!IsActive()) return;
        var image=GetComponent<Image>(); if(image.sprite==null)return;
        var rect=image.rectTransform.rect;if(rect.height<=0)return;
        float source=image.sprite.rect.width/image.sprite.rect.height;
        float target=rect.width/rect.height;
        float width=target<source?target/source:1;
        float height=target>source?source/target:1;
        for(int i=0;i<mesh.currentVertCount;i++)
        {
            var vertex=UIVertex.simpleVert;mesh.PopulateUIVertex(ref vertex,i);
            vertex.uv0.x=.5f+(vertex.uv0.x-.5f)*width;
            vertex.uv0.y=.5f+(vertex.uv0.y-.5f)*height;
            mesh.SetUIVertex(vertex,i);
        }
    }
}
