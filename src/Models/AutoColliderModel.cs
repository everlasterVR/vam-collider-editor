using System.Collections.Generic;

public class AutoColliderModel : ColliderContainerModelBase<AutoCollider>, IModel
{
    private readonly List<ColliderModel> _ownedColliders = new List<ColliderModel>();

    protected override bool OwnsColliders => true;

    public string Type => "Auto Collider";
    public AutoCollider AutoCollider => Component;

    public AutoColliderModel(AutoCollider autoCollider, ColliderPreviewConfig config)
        : base(autoCollider, $"[au] {NameHelper.Simplify(autoCollider.name)}")
    {
        if (Component.hardCollider != null) _ownedColliders.Add(ColliderModel.CreateTyped(autoCollider.hardCollider, config));
        if (Component.jointCollider != null) _ownedColliders.Add(ColliderModel.CreateTyped(Component.jointCollider, config));
    }

    public override IEnumerable<ColliderModel> GetColliders() => _ownedColliders;
}
