using System.Collections.Generic;
using UnityEngine;

public class RigidbodyModel : ColliderContainerModelBase<Rigidbody>, IModel
{
    protected override bool OwnsColliders => false;

    public string Type => "Rigidbody";
    public List<ColliderModel> Colliders { get; } = new List<ColliderModel>();
    public Rigidbody Rigidbody => Component;

    public RigidbodyModel(Rigidbody rigidbody) : base(rigidbody, $"[rb] {NameHelper.Simplify(rigidbody.name)}")
    {
    }

    public override IEnumerable<ColliderModel> GetColliders() => Colliders;
}
