using System.Collections.Generic;
using UnityEngine;

namespace ColliderEditor.Models
{
    sealed class AutoColliderModel : ColliderContainerModelBase<AutoCollider>, IModel
    {
        readonly List<ColliderModel> _ownedColliders = new List<ColliderModel>();

        protected override bool OwnsColliders
        {
            get { return true; }
        }

        public string Type
        {
            get { return "Auto Collider"; }
        }

        public AutoCollider AutoCollider
        {
            get { return component; }
        }

        public AutoColliderModel(AutoCollider autoCollider, ColliderPreviewConfig config)
            : base(autoCollider, $"[au] {NameHelper.Simplify(autoCollider.name)}")
        {
            if(component.hardCollider != null)
            {
                _ownedColliders.Add(ColliderModel.CreateTyped(autoCollider.hardCollider, config));
            }

            if(component.jointCollider != null)
            {
                _ownedColliders.Add(ColliderModel.CreateTyped(component.jointCollider, config));
            }
        }

        public override IEnumerable<ColliderModel> GetColliders()
        {
            return _ownedColliders;
        }

        public IEnumerable<Rigidbody> GetRigidbodies()
        {
            if(component.jointRB != null)
            {
                yield return component.jointRB;
            }

            if(component.kinematicRB != null)
            {
                yield return component.kinematicRB;
            }
        }
    }
}
