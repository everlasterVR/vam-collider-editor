using System.Collections.Generic;
using System.Linq;

public class AutoColliderGroupModel : ColliderContainerModelBase<AutoColliderGroup>, IModel
{
    private readonly List<AutoColliderModel> _autoColliders;

    protected override bool OwnsColliders => true;

    public string Type => "Auto Collider Group";
    public AutoColliderGroup AutoColliderGroup => Component;

    public AutoColliderGroupModel(AutoColliderGroup autoColliderGroup, List<AutoColliderModel> autoColliders)
        : base(autoColliderGroup, $"[ag] {NameHelper.Simplify(autoColliderGroup.name)}")
    {
        _autoColliders = autoColliders;
    }

    public override IEnumerable<ColliderModel> GetColliders() => _autoColliders.SelectMany(ac => ac.GetColliders());
}
