using UnityEngine;

public class CapsuleColliderModel : ColliderModel<CapsuleCollider>
{
    public CapsuleColliderModel(CapsuleCollider collider, ColliderPreviewConfig config)
        : base(collider, config)
    {
    }

    protected override GameObject DoCreatePreview() => GameObject.CreatePrimitive(PrimitiveType.Capsule);

    public override void SyncPreviews()
    {
        SyncPreview(Preview);
        SyncPreview(XRayPreview);
    }

    private void SyncPreview(GameObject preview)
    {
        if (preview == null) return;

        float size = Collider.radius * 2;
        float height = Collider.height / 2;
        preview.transform.localScale = new Vector3(size, height, size);
        if (Collider.direction == 0)
            preview.transform.localRotation = Quaternion.AngleAxis(90, Vector3.forward);
        else if (Collider.direction == 2)
            preview.transform.localRotation = Quaternion.AngleAxis(90, Vector3.right);
        preview.transform.localPosition = Collider.center;
    }
}
