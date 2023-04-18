using System.Collections.Generic;
using UnityEngine;

namespace ColliderEditor.Models
{
    public class RigidbodyModel : ColliderContainerModelBase<Rigidbody>, IModel
    {
        protected override bool OwnsColliders
        {
            get { return false; }
        }

        public string Type
        {
            get { return "Rigidbody"; }
        }

        public List<ColliderModel> Colliders { get; } = new List<ColliderModel>();

        public Rigidbody Rigidbody
        {
            get { return component; }
        }

        public RigidbodyModel(Rigidbody rigidbody) : base(rigidbody, $"[rb] {NameHelper.Simplify(rigidbody.name)}")
        {
        }

        public override IEnumerable<ColliderModel> GetColliders()
        {
            return Colliders;
        }
    }
}
