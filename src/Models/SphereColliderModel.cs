using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SphereColliderModel : ColliderModel<SphereCollider>
{
    public SphereColliderModel(SphereCollider collider, ColliderPreviewConfig config)
        : base(collider, config)
    {
    }

    protected override List<GameObject> DoCreatePreview() => new List<GameObject>
    {
        GameObject.CreatePrimitive(PrimitiveType.Sphere)
    };

    public override void SyncPreviews()
    {
        SyncPreview(Preview?.First());
        SyncPreview(XRayPreview?.First());
    }

    private void SyncPreview(GameObject preview)
    {
        if (preview == null) return;

        preview.transform.localScale = Vector3.one * (Collider.radius * 2);
        preview.transform.localPosition = Collider.center;
    }
}
