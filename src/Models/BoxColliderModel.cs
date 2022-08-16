using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoxColliderModel : ColliderModel<BoxCollider>
{
    public BoxColliderModel(BoxCollider collider, ColliderPreviewConfig config)
        : base(collider, config)
    {
    }

    protected override List<GameObject> DoCreatePreview() => new List<GameObject>
    {
        GameObject.CreatePrimitive(PrimitiveType.Cube)
    };

    public override void SyncPreviews()
    {
        SyncPreview(Preview?.First());
        SyncPreview(XRayPreview?.First());
    }

    private void SyncPreview(GameObject preview)
    {
        if (preview == null) return;

        preview.transform.localScale = Collider.size;
        preview.transform.localPosition = Collider.center;
    }
}
