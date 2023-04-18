using System.Linq;
using UnityEngine;

namespace ColliderEditor.Models
{
    public class SphereColliderModel : ColliderModel<SphereCollider>
    {
        public SphereColliderModel(SphereCollider collider, ColliderPreviewConfig config)
            : base(collider, config)
        {
        }

        protected override GameObject[] DoCreatePreview()
        {
            return new[]
            {
                GameObject.CreatePrimitive(PrimitiveType.Sphere),
            };
        }

        public override void SyncPreviews()
        {
            SyncPreview(Preview?.First());
            SyncPreview(XRayPreview?.First());
        }

        void SyncPreview(GameObject preview)
        {
            if(preview == null)
            {
                return;
            }

            preview.transform.localScale = Vector3.one * (Collider.radius * 2);
            preview.transform.localPosition = Collider.center;
        }
    }
}
