using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditablesList
{
    public static EditablesList Build(
        Atom containingAtom,
        ColliderPreviewConfig config,
        List<Group> customGroups
    )
    {
        List<Group> groups;
        if (customGroups != null)
        {
            groups = customGroups;
        }
        else if (containingAtom.type == "Person")
        {
            groups = new List<Group>
            {
                new Group("Head / Ears", @"^((AutoCollider(Female)?)?AutoColliders)?(s?[Hh]ead|lowerJaw|[Tt]ongue|neck|s?Face|_?Collider(Lip|Ear|Nose))"),
                new Group("Left arm", @"^l(Shldr|ForeArm)"),
                new Group("Left hand", @"^l(Index|Mid|Ring|Pinky|Thumb|Carpal|Hand)[0-9]?$"),
                new Group("Right arm", @"^r(Shldr|ForeArm)"),
                new Group("Right hand", @"^r(Index|Mid|Ring|Pinky|Thumb|Carpal|Hand)[0-9]?$"),
                new Group("Chest", @"^(chest|(AutoCollider)?FemaleAutoColliderschest|MaleAutoColliderschest)"),
                new Group("Breasts", @"(Pectoral|Nipple)"),
                new Group("Left breast", @"l(Pectoral|Nipple)"),
                new Group("Right breast", @"r(Pectoral|Nipple)"),
                new Group("Abdomen / Belly / Back", @"^((AutoCollider)?FemaleAutoColliders)?abdomen"),
                new Group("Hip / Pelvis", @"^((Female)?AutoColliders?|MaleAutoColliders)?(hip|pelvis)"),
                new Group("Glutes", @"^((AutoCollider)?FemaleAutoColliders)?[LR]Glute"),
                new Group("Left glute", @"^((AutoCollider)?FemaleAutoColliders)?LGlute"),
                new Group("Right glute", @"^((AutoCollider)?FemaleAutoColliders)?RGlute"),
                new Group("Anus", @"^_JointA[rl]"),
                new Group("Vagina", @"^_Joint(Gr|Gl|B)"),
                new Group("Penis", @"^((AutoCollider)?Gen[1-3])|Testes"),
                new Group("Left leg", @"^((AutoCollider)?(FemaleAutoColliders)?)?l(Thigh|Shin)"),
                new Group("Left foot", @"^l(Foot|Toe|BigToe|SmallToe)"),
                new Group("Right leg", @"^((AutoCollider)?(FemaleAutoColliders)?)?r(Thigh|Shin)"),
                new Group("Right foot", @"^r(Foot|Toe|BigToe|SmallToe)"),
                new Group("Physics mesh joints", @"^PhysicsMeshJoint.+$")
            };
        }
        else
        {
            groups = new List<Group>
            {
                new Group("All", @"^.+$"),
            };
        }
        var other = new List<Group> { new Group("Other", "") };

        // AutoColliders

        var autoColliderDuplicates = new HashSet<string>();
        var autoColliders = containingAtom.GetComponentsInChildren<AutoCollider>()
            .Select(autoCollider => new AutoColliderModel(autoCollider, config))
            .Where(model => autoColliderDuplicates.Add(model.Id))
            .ForEach(model => {
                var matching = groups.Where(g => g.Test(model.AutoCollider.name)).ToList();
                model.Groups.AddRange(matching.Any() ? matching : other);
            })
            .ToList();

        var autoCollidersColliders = new HashSet<Collider>(autoColliders.SelectMany(x => x.GetColliders()).Select(x => x.Collider));
        var autoCollidersMap = autoColliders.ToDictionary(x => x.AutoCollider);

        // AutoColliderGroups

        var autoColliderGroupDuplicates = new HashSet<string>();
        var autoColliderGroups = containingAtom.GetComponentsInChildren<AutoColliderGroup>()
            .Select(autoColliderGroup =>
            {
                var childAutoColliders = autoColliderGroup.GetAutoColliders().Where(acg => autoCollidersMap.ContainsKey(acg)).Select(acg => autoCollidersMap[acg]).ToList();
                return new AutoColliderGroupModel(autoColliderGroup, childAutoColliders);
            })
            .Where(model => autoColliderGroupDuplicates.Add(model.Id))
            .ForEach(model => {
                var matching = groups.Where(g => g.Test(model.AutoColliderGroup.name)).ToList();
                model.Groups.AddRange(matching.Any() ? matching : other);
            })
            .ToList();

        // Colliders

        var colliderDuplicates = new HashSet<string>();
        var colliders = containingAtom.GetComponentsInChildren<Collider>(true)
            .Where(collider => collider.gameObject.activeInHierarchy)
            .Where(collider => !autoCollidersColliders.Contains(collider))
            .Where(collider => IsRigidbodyIncluded(collider.attachedRigidbody))
            .Where(IsColliderIncluded)
            .Select(collider => ColliderModel.CreateTyped(collider, config))
            .Where(model => colliderDuplicates.Add(model.Id))
            .ToList();

        // All Editables
        return new EditablesList(
            groups.Concat(other).ToList(),
            colliders,
            autoColliders,
            autoColliderGroups
        );
    }

    private static bool IsColliderIncluded(Collider collider)
    {
        if (collider.name == "control") return false;
        if (collider.name == "object") return false;
        if (collider.name.Contains("Tool")) return false;
        if (collider.name.EndsWith("Control")) return false;
        if (collider.name.EndsWith("Link")) return false;
        if (collider.name.EndsWith("Trigger")) return false;
        if (collider.name.EndsWith("UI")) return false;
        if (collider.name.Contains("Ponytail")) return false;
        if (collider.name.EndsWith("Joint")) return false;
        if (collider is MeshCollider) return false;
        return true;
    }

    private static bool IsRigidbodyIncluded(Rigidbody rigidbody)
    {
        if (rigidbody == null) return false;
        if (rigidbody.isKinematic) return false;
        if (rigidbody.name == "control") return false;
        if (rigidbody.name == "object") return false;
        if (rigidbody.name.EndsWith("Control")) return false;
        if (rigidbody.name.StartsWith("hairTool")) return false;
        if (rigidbody.name.EndsWith("Trigger")) return false;
        if (rigidbody.name.EndsWith("UI")) return false;
        if (rigidbody.name.Contains("Ponytail")) return false;
        return true;
    }

    public List<Group> Groups { get; }
    public readonly List<ColliderModel> Colliders;
    public readonly List<AutoColliderModel> AutoColliders;
    public readonly List<AutoColliderGroupModel> AutoColliderGroups;
    public List<IModel> All { get; }
    public Dictionary<string, IModel> ByUuid { get; }
    private bool _readyForUI;

    private EditablesList(List<Group> groups, List<ColliderModel> colliders, List<AutoColliderModel> autoColliders, List<AutoColliderGroupModel> autoColliderGroups)
    {
        Groups = groups;
        Colliders = colliders;
        AutoColliders = autoColliders;
        AutoColliderGroups = autoColliderGroups;

        All = colliders.Cast<IModel>()
            .Concat(autoColliderGroups.Cast<IModel>())
            .Concat(autoColliders.Cast<IModel>())
            .OrderBy(a => a.Label)
            .ToList();

        ByUuid = All.ToDictionary(x => x.Id, x => x);
    }

    public void PrepareForUI()
    {
        if (_readyForUI) return;
        _readyForUI = true;
        MatchMirror<AutoColliderModel, AutoCollider>(AutoColliders);
        MatchMirror<AutoColliderGroupModel, AutoColliderGroup>(AutoColliderGroups);
        MatchMirror<ColliderModel, Collider>(Colliders);
    }

    private static void MatchMirror<TModel, TComponent>(List<TModel> items)
        where TModel : ModelBase<TComponent>
        where TComponent : Component
    {
        var map = items.ToDictionary(i => i.Id, i => i);
        var skip = new HashSet<TModel>();
        foreach (var left in items)
        {
            if (skip.Contains(left))
            {
                continue;
            }
            var rightId = Mirrors.Find(left.Id);
            if (rightId == null)
            {
                continue;
            }
            TModel right;
            if (!map.TryGetValue(rightId, out right))
            {
                if (left.Id.Contains("Shin")) continue;
            }
            left.Mirror = right;
            right.Mirror = left;
            skip.Add(right);
        }
    }
}
