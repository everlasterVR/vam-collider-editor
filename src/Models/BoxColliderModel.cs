using UnityEngine;

public class BoxColliderModel : ColliderModel<BoxCollider>
{
    public BoxColliderModel(BoxCollider collider, ColliderPreviewConfig config)
        : base(collider, config)
    {
    }

    protected override GameObject DoCreatePreview() => GameObject.CreatePrimitive(PrimitiveType.Cube);

    public override void SyncPreviews()
    {
        SyncPreview(Preview);
        SyncPreview(XRayPreview);
    }

    private void SyncPreview(GameObject preview)
    {
        if (preview == null) return;

        preview.transform.localScale = Collider.size;
        preview.transform.localPosition = Collider.center;
    }
}
