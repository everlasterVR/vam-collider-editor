using System.Collections.Generic;
using UnityEngine;

namespace ColliderEditor.Models
{
    public abstract class ColliderContainerModelBase<T> : ModelBase<T> where T : Component
    {
        protected abstract bool OwnsColliders { get; }

        public bool Shown
        {
            set
            {
                if(OwnsColliders)
                {
                    foreach(var c in GetColliders())
                    {
                        c.Shown = value;
                    }
                }
            }
        }

        protected ColliderContainerModelBase(T component, string label)
            : base(component, label)
        {
        }

        protected override void SetHighlighted(bool value)
        {
            foreach(var collider in GetColliders())
            {
                collider.SetHighlighted(value);
            }
        }

        public override void SyncPreviews()
        {
            if(!OwnsColliders)
            {
                return;
            }

            foreach(var colliderModel in GetColliders())
            {
                colliderModel.SyncPreviews();
            }
        }

        public virtual void UpdatePreviewsFromConfig()
        {
            if(!OwnsColliders)
            {
                return;
            }

            foreach(var colliderModel in GetColliders())
            {
                colliderModel.UpdatePreviewsFromConfig();
            }
        }

        public void DestroyPreviews()
        {
            foreach(var colliderModel in GetColliders())
            {
                colliderModel.DestroyPreviews();
            }
        }

        public abstract IEnumerable<ColliderModel> GetColliders();
    }
}
