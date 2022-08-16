using System.Collections.Generic;
using UnityEngine;

public class CapsuleColliderModel : ColliderModel<CapsuleCollider>
{
    public CapsuleColliderModel(CapsuleCollider collider, ColliderPreviewConfig config)
        : base(collider, config)
    {
    }

    protected override List<GameObject> DoCreatePreview() => new List<GameObject>
    {
        GameObject.CreatePrimitive(PrimitiveType.Sphere),
        GameObject.CreatePrimitive(PrimitiveType.Cylinder),
        GameObject.CreatePrimitive(PrimitiveType.Sphere)
    };

    public override void SyncPreviews()
    {
        SyncPreview(Preview);
        SyncPreview(XRayPreview);
    }

    private void SyncPreview(List<GameObject> preview)
    {
        if (preview == null) return;

        float radius = Collider.radius;
        float height = Collider.height;

        float d = radius * 2;
        float offset = height > d ? (height - d) / 2 : 0;

        /* top sphere */
        {
            var primitive = preview[0];
            primitive.transform.localScale = new Vector3(d, d, d);
            SetRotation(primitive, Collider.direction);

            primitive.transform.localPosition = Collider.center;
            primitive.transform.Translate(primitive.transform.up * offset, Space.World);
        }

        /* middle cylinder */
        {
            var primitive = preview[1];
            var previewRenderer = primitive.GetComponent<Renderer>();
            if(offset > 0)
            {
                previewRenderer.enabled = true;
                primitive.transform.localScale = new Vector3(d, offset, d);
                SetRotation(primitive, Collider.direction);
                primitive.transform.localPosition = Collider.center;
            }
            else
            {
                previewRenderer.enabled = false;
            }
        }

        /* bottom sphere */
        {
            var primitive = preview[2];
            var previewRenderer = primitive.GetComponent<Renderer>();
            if(offset > 0)
            {
                previewRenderer.enabled = true;
                primitive.transform.localScale = new Vector3(d, d, d);
                SetRotation(primitive, Collider.direction);

                primitive.transform.localPosition = Collider.center;
                primitive.transform.Translate(-primitive.transform.up * offset, Space.World);
            }
            else
            {
                previewRenderer.enabled = false;
            }
        }
    }

    private static void SetRotation(GameObject primitive, int direction)
    {
        if (direction == 0)
        {
            primitive.transform.localRotation = Quaternion.AngleAxis(90, Vector3.forward);
        }
        else if (direction == 2)
        {
            primitive.transform.localRotation = Quaternion.AngleAxis(90, Vector3.right);
        }
    }
}
