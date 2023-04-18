using ColliderEditor.Extensions;
using ColliderEditor.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ColliderEditor
{
    sealed class SelectablesList
    {
        public static SelectablesList Build(
            Atom containingAtom,
            ColliderPreviewConfig config,
            List<Group> customGroups
        )
        {
            List<Group> groups;
            if(customGroups != null)
            {
                groups = customGroups;
            }
            else if(containingAtom.type == "Person")
            {
                groups = new List<Group>
                {
                    new Group("Head / Ears",
                        @"^((AutoCollider(Female)?)?AutoColliders)?(s?[Hh]ead|lowerJaw|[Tt]ongue|neck|s?Face|_?Collider(Lip|Ear|Nose))"),
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
                    new Group("Physics mesh joints", @"^PhysicsMeshJoint.+$"),
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
                .ForEach(model =>
                {
                    var matching = groups.Where(g => g.Test(model.AutoCollider.name)).ToList();
                    model.Groups.AddRange(matching.Any() ? matching : other);
                })
                .ToList();

            var autoCollidersRigidBodies = new HashSet<Rigidbody>(autoColliders.SelectMany(x => x.GetRigidbodies()));
            var autoCollidersColliders = new HashSet<Collider>(autoColliders.SelectMany(x => x.GetColliders()).Select(x => x.Collider));
            var autoCollidersMap = autoColliders.ToDictionary(x => x.AutoCollider);

            // AutoColliderGroups

            var autoColliderGroupDuplicates = new HashSet<string>();
            var autoColliderGroups = containingAtom.GetComponentsInChildren<AutoColliderGroup>()
                .Select(autoColliderGroup =>
                {
                    var childAutoColliders = autoColliderGroup.GetAutoColliders()
                        .Where(acg => autoCollidersMap.ContainsKey(acg))
                        .Select(acg => autoCollidersMap[acg])
                        .ToList();
                    return new AutoColliderGroupModel(autoColliderGroup, childAutoColliders);
                })
                .Where(model => autoColliderGroupDuplicates.Add(model.Id))
                .ForEach(model =>
                {
                    var matching = groups.Where(g => g.Test(model.AutoColliderGroup.name)).ToList();
                    model.Groups.AddRange(matching.Any() ? matching : other);
                })
                .ToList();

            // Rigidbodies

            var rigidbodyDuplicates = new HashSet<string>();
            var controllerRigidbodies = new HashSet<Rigidbody>(containingAtom.freeControllers.SelectMany(fc => fc.GetComponents<Rigidbody>()));
            var rigidbodies = containingAtom.GetComponentsInChildren<Rigidbody>(true)
                .Where(rigidbody => !autoCollidersRigidBodies.Contains(rigidbody))
                .Where(rigidbody => !controllerRigidbodies.Contains(rigidbody))
                .Where(IsRigidbodyIncluded)
                .Select(rigidbody => new RigidbodyModel(rigidbody))
                .Where(model => rigidbodyDuplicates.Add(model.Id))
                .ForEach(model =>
                {
                    var matching = groups.Where(g => g.Test(model.Rigidbody.name)).ToList();
                    model.Groups.AddRange(matching.Any() ? matching : other);
                })
                .ToList();
            var rigidbodiesDict = rigidbodies.ToDictionary(x => x.Id);

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

            // Attach colliders to rigidbodies

            foreach(var colliderModel in colliders)
            {
                if(colliderModel.Collider.attachedRigidbody != null)
                {
                    RigidbodyModel rigidbodyModel;
                    if(rigidbodiesDict.TryGetValue(colliderModel.Collider.attachedRigidbody.Uuid(), out rigidbodyModel))
                    {
                        rigidbodyModel.Colliders.Add(colliderModel);
                        colliderModel.Groups = rigidbodyModel.Groups;
                    }
                    else
                    {
                        SuperController.LogError(
                            $"Could not find a matching rigidbody for collider '{colliderModel.Id}', rigidbody '{colliderModel.Collider.attachedRigidbody.Uuid()}'.");
                        var matching = groups.Where(g => g.Test(colliderModel.Collider.name)).ToList();
                        colliderModel.Groups.AddRange(matching.Any() ? matching : other);
                    }
                }
                else
                {
                    var matching = groups.Where(g => g.Test(colliderModel.Collider.name)).ToList();
                    colliderModel.Groups.AddRange(matching.Any() ? matching : other);
                }
            }

            return new SelectablesList(
                groups.Concat(other).ToList(),
                colliders,
                autoColliders,
                autoColliderGroups
                // rigidbodies
            );
        }

        static bool IsColliderIncluded(Collider collider)
        {
            if(collider.name == "control")
            {
                return false;
            }

            if(collider.name == "object")
            {
                return false;
            }

            if(collider.name.Contains("Tool"))
            {
                return false;
            }

            if(collider.name.EndsWith("Control"))
            {
                return false;
            }

            if(collider.name.EndsWith("Link"))
            {
                return false;
            }

            if(collider.name.EndsWith("Trigger"))
            {
                return false;
            }

            if(collider.name.EndsWith("UI"))
            {
                return false;
            }

            if(collider.name.Contains("Ponytail"))
            {
                return false;
            }

            if(collider.name.EndsWith("Joint"))
            {
                return false;
            }

            return !(collider is MeshCollider);
        }

        static bool IsRigidbodyIncluded(Rigidbody rigidbody)
        {
            if(rigidbody == null)
            {
                return false;
            }

            if(rigidbody.isKinematic)
            {
                return false;
            }

            if(rigidbody.name == "control")
            {
                return false;
            }

            if(rigidbody.name == "object")
            {
                return false;
            }

            if(rigidbody.name.EndsWith("Control"))
            {
                return false;
            }

            if(rigidbody.name.StartsWith("hairTool"))
            {
                return false;
            }

            if(rigidbody.name.EndsWith("Trigger"))
            {
                return false;
            }

            if(rigidbody.name.EndsWith("UI"))
            {
                return false;
            }

            return !rigidbody.name.Contains("Ponytail");
        }

        public List<Group> Groups { get; }
        readonly List<ColliderModel> _colliders;
        readonly List<AutoColliderModel> _autoColliders;
        readonly List<AutoColliderGroupModel> _autoColliderGroups;
        readonly MirrorRegexReplace[] _mirrorRegexes;
        public List<IModel> All { get; }
        public Dictionary<string, IModel> ByUuid { get; }
        bool _readyForUI;

        SelectablesList(List<Group> groups, List<ColliderModel> colliders, List<AutoColliderModel> autoColliders,
            List<AutoColliderGroupModel> autoColliderGroups
        )
        {
            Groups = groups;
            _colliders = colliders;
            _autoColliders = autoColliders;
            _autoColliderGroups = autoColliderGroups;
            _mirrorRegexes = Mirrors.GetMirrorRegexes();

            // ReSharper disable RedundantEnumerableCastCall
            All = colliders.Cast<IModel>()
                .Concat(autoColliderGroups.Cast<IModel>())
                .Concat(autoColliders.Cast<IModel>())
                .OrderBy(a => a.Label)
                .ToList();

            ByUuid = All.ToDictionary(x => x.Id, x => x);
        }

        public void PrepareForUI()
        {
            if(_readyForUI)
            {
                return;
            }

            _readyForUI = true;
            MatchMirror<AutoColliderModel, AutoCollider>(_autoColliders);
            MatchMirror<AutoColliderGroupModel, AutoColliderGroup>(_autoColliderGroups);
            MatchMirror<ColliderModel, Collider>(_colliders);
        }

        void MatchMirror<TModel, TComponent>(List<TModel> items)
            where TModel : ModelBase<TComponent>
            where TComponent : Component
        {
            var map = items.ToDictionary(i => i.Id, i => i);
            var skip = new HashSet<TModel>();
            foreach(var left in items)
            {
                if(skip.Contains(left))
                {
                    continue;
                }

                string rightId = FindMirror(left.Id);
                if(rightId == null)
                {
                    continue;
                }

                TModel right;
                if(!map.TryGetValue(rightId, out right))
                {
                    if(left.Id.Contains("Shin"))
                    {
                        continue;
                    }
                }

                left.Mirror = right;
                if(right != null)
                {
                    right.Mirror = left;
                }

                skip.Add(right);
            }
        }

        string FindMirror(string name)
        {
            bool matched = false;
            string m = name;

            foreach(var r in _mirrorRegexes)
            {
                if(!r.regex.IsMatch(m))
                {
                    continue;
                }

                m = r.regex.Replace(m, r.replacement);
                matched = true;
            }

            return matched ? m : null;
        }
    }
}
