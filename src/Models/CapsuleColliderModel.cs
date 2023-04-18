using UnityEngine;

namespace ColliderEditor.Models
{
    public class CapsuleColliderModel : ColliderModel<CapsuleCollider>
    {
        public CapsuleColliderModel(CapsuleCollider collider, ColliderPreviewConfig config)
            : base(collider, config)
        {
        }

        protected override GameObject[] DoCreatePreview()
        {
            return new[]
            {
                GameObject.CreatePrimitive(PrimitiveType.Sphere),
                GameObject.CreatePrimitive(PrimitiveType.Cylinder),
                GameObject.CreatePrimitive(PrimitiveType.Sphere),
            };
        }

        public override void SyncPreviews()
        {
            SyncPreview(Preview);
            SyncPreview(XRayPreview);
        }

        void SyncPreview(GameObject[] preview)
        {
            if(preview == null)
            {
                return;
            }

            float radius = Collider.radius;
            float height = Collider.height;

            float diameter = radius * 2;
            float offset = height > diameter ? (height - diameter) / 2 : 0;

            /* top sphere */
            {
                var primitive = preview[0];
                primitive.transform.localScale = new Vector3(diameter, diameter, diameter);
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
                    primitive.transform.localScale = new Vector3(diameter, offset, diameter);
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
                    primitive.transform.localScale = new Vector3(diameter, diameter, diameter);
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

        static void SetRotation(GameObject primitive, int direction)
        {
            if(direction == 0)
            {
                primitive.transform.localRotation = Quaternion.AngleAxis(90, Vector3.forward);
            }
            else if(direction == 2)
            {
                primitive.transform.localRotation = Quaternion.AngleAxis(90, Vector3.right);
            }
        }
    }
}
