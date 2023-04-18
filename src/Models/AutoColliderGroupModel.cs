using System.Collections.Generic;
using System.Linq;

namespace ColliderEditor.Models
{
    public class AutoColliderGroupModel : ColliderContainerModelBase<AutoColliderGroup>, IModel
    {
        readonly List<AutoColliderModel> _autoColliders;

        protected override bool OwnsColliders
        {
            get { return true; }
        }

        public string Type
        {
            get { return "Auto Collider Group"; }
        }

        public AutoColliderGroup AutoColliderGroup
        {
            get { return component; }
        }

        public AutoColliderGroupModel(AutoColliderGroup autoColliderGroup, List<AutoColliderModel> autoColliders)
            : base(autoColliderGroup, $"[ag] {NameHelper.Simplify(autoColliderGroup.name)}")
        {
            _autoColliders = autoColliders;
        }

        public override IEnumerable<ColliderModel> GetColliders()
        {
            return _autoColliders.SelectMany(ac => ac.GetColliders());
        }
    }
}
