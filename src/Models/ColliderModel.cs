using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ColliderEditor.Models
{
    public abstract class ColliderModel<T> : ColliderModel where T : Collider
    {
        new protected T Collider { get; }

        protected ColliderModel(T collider, ColliderPreviewConfig config)
            : base(collider, config)
        {
            Collider = collider;
        }

        protected override GameObject[] CreatePreviewPrimitives()
        {
            var previewPrimitives = DoCreatePreview();

            foreach(var primitive in previewPrimitives)
            {
                primitive.GetComponent<Renderer>().material = MaterialHelper.GetNextMaterial(Id.GetHashCode());
                foreach(var c in primitive.GetComponents<Collider>())
                {
                    c.enabled = false;
                    Object.Destroy(c);
                }

                primitive.transform.SetParent(Collider.transform, false);
            }

            return previewPrimitives;
        }
    }

    public abstract class ColliderModel : ModelBase<Collider>, IModel
    {
        readonly ColliderPreviewConfig _config;

        public string Type
        {
            get { return "Collider"; }
        }

        public Collider Collider { get; }
        protected GameObject[] Preview { get; private set; }
        protected GameObject[] XRayPreview { get; private set; }
        public bool Shown { get; set; }

        public virtual void UpdatePreviewsFromConfig()
        {
            if(_config.PreviewsEnabled && Shown)
            {
                if(Preview == null)
                {
                    Preview = CreatePreviewPrimitives();
                }

                foreach(var primitive in Preview)
                {
                    var previewRenderer = primitive.GetComponent<Renderer>();
                    var material = previewRenderer.material;
                    var color = material.color;
                    color.a = Highlighted ? _config.SelectedPreviewsOpacity : _config.PreviewsOpacity;
                    material.color = color;

                    if(material.shader.name != "Standard")
                    {
                        material.shader = Shader.Find("Standard");
                        material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
                        material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.SetInt("_ZWrite", 0);
                        material.DisableKeyword("_ALPHATEST_ON");
                        material.EnableKeyword("_ALPHABLEND_ON");
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = 3000;
                        previewRenderer.material = material;
                    }
                }

                UpdateXRayPreviewFromConfig();

                SyncPreviews();
                RefreshHighlightedPreview();
                RefreshHighlightedXRayPreview();
            }
            else
            {
                DestroyPreviews();
            }
        }

        void UpdateXRayPreviewFromConfig()
        {
            if(_config.XRayPreviews)
            {
                if(XRayPreview == null)
                {
                    XRayPreview = CreatePreviewPrimitives();
                }

                foreach(var primitive in XRayPreview)
                {
                    var previewRenderer = primitive.GetComponent<Renderer>();
                    var material = previewRenderer.material;
                    var color = material.color;
                    color.a = Highlighted
                        ? _config.RelativeXRayOpacity * _config.SelectedPreviewsOpacity
                        : _config.RelativeXRayOpacity * _config.PreviewsOpacity;
                    material.color = color;

                    if(material.shader.name != "Battlehub/RTGizmos/Handles")
                    {
                        material.shader = Shader.Find("Battlehub/RTGizmos/Handles");
                        material.SetFloat("_Offset", 1f);
                        material.SetFloat("_MinAlpha", 1f);
                        previewRenderer.material = material;
                    }
                }
            }
            else
            {
                DestroyXRayPreview();
            }
        }

        protected ColliderModel(Collider collider, ColliderPreviewConfig config)
            : base(collider, CreateLabel(collider))
        {
            Collider = collider;
            _config = config;
        }

        static string CreateLabel(Collider collider)
        {
            string parent = collider.attachedRigidbody != null ? collider.attachedRigidbody.name : collider.transform.parent.name;
            string label = parent == collider.name
                ? NameHelper.Simplify(collider.name)
                : $"{NameHelper.Simplify(parent)}/{NameHelper.Simplify(collider.name)}";
            return $"[co] {label}";
        }

        public static ColliderModel CreateTyped(Collider collider, ColliderPreviewConfig config)
        {
            ColliderModel typed;

            // ReSharper disable once MergeCastWithTypeCheck
            if(collider is SphereCollider)
            {
                typed = new SphereColliderModel((SphereCollider) collider, config);
            }
            else if(collider is BoxCollider)
            {
                typed = new BoxColliderModel((BoxCollider) collider, config);
            }
            else if(collider is CapsuleCollider)
            {
                typed = new CapsuleColliderModel((CapsuleCollider) collider, config);
            }
            else
            {
                throw new InvalidOperationException("Unsupported collider type");
            }

            return typed;
        }

        public virtual void DestroyPreviews()
        {
            DestroyPreview();
            DestroyXRayPreview();
        }

        void DestroyPreview()
        {
            if(Preview == null)
            {
                return;
            }

            foreach(var primitive in Preview)
            {
                Object.Destroy(primitive);
            }

            Preview = null;
        }

        void DestroyXRayPreview()
        {
            if(XRayPreview == null)
            {
                return;
            }

            foreach(var primitive in XRayPreview)
            {
                Object.Destroy(primitive);
            }

            XRayPreview = null;
        }

        protected abstract GameObject[] CreatePreviewPrimitives();

        protected abstract GameObject[] DoCreatePreview();

        public new void SetHighlighted(bool value)
        {
            if(Highlighted == value)
            {
                return;
            }

            Highlighted = value;
            RefreshHighlightedPreview();
            RefreshHighlightedXRayPreview();
        }

        void RefreshHighlightedPreview()
        {
            if(Preview == null)
            {
                return;
            }

            foreach(var primitive in Preview)
            {
                var previewRenderer = primitive.GetComponent<Renderer>();
                var color = previewRenderer.material.color;
                color.a = Highlighted ? _config.SelectedPreviewsOpacity : _config.PreviewsOpacity;
                previewRenderer.material.color = color;
            }
        }

        void RefreshHighlightedXRayPreview()
        {
            if(XRayPreview == null)
            {
                return;
            }

            foreach(var primitive in XRayPreview)
            {
                var previewRenderer = primitive.GetComponent<Renderer>();
                var color = previewRenderer.material.color;
                float alpha = Highlighted ? _config.SelectedPreviewsOpacity : _config.PreviewsOpacity;
                color.a = _config.RelativeXRayOpacity * alpha;
                previewRenderer.material.color = color;
            }
        }

        public override string ToString()
        {
            return Id;
        }
    }
}
