using System.Linq;
using UnityEngine;

namespace ColliderEditor.Models
{
    sealed class BoxColliderModel : ColliderModel<BoxCollider>
    {
        public BoxColliderModel(BoxCollider collider, ColliderPreviewConfig config)
            : base(collider, config)
        {
        }

        protected override GameObject[] DoCreatePreview()
        {
            return new[]
            {
                GameObject.CreatePrimitive(PrimitiveType.Cube),
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

            preview.transform.localScale = Collider.size;
            preview.transform.localPosition = Collider.center;
        }
    }
}
