using ColliderEditor.Extensions;
using ColliderEditor.Models;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ColliderEditor
{
    /// <summary>
    /// Collider Visualizer
    /// Source: https://github.com/everlasterVR/vam-collider-editor/tree/visualizer
    /// Based on:
    ///
    /// Collider Editor
    /// By Acidbubbles, ProjectCanyon and via5
    /// Configures and customizes collisions (rigidbodies and colliders)
    /// Source: https://github.com/acidbubbles/vam-collider-editor
    /// </summary>
    sealed class ColliderVisualizer : MonoBehaviour
    {
        const string NO_SELECTION_LABEL = "Select...";
        const string ALL_LABEL = "All";

        // ReSharper disable UnusedAutoPropertyAccessor.Global MemberCanBePrivate.Global
        public JSONStorableBool ShowPreviewsJSON { get; set; }
        public JSONStorableBool XRayPreviewsJSON { get; set; }
        public JSONStorableBool XRayPreviewsOffJSON { get; set; }
        public JSONStorableFloat PreviewOpacityJSON { get; set; }
        public JSONStorableFloat SelectedPreviewOpacityJSON { get; set; }
        public JSONStorableFloat RelativeXRayOpacityJSON { get; set; }
        public JSONStorableBool HighlightMirrorJSON { get; set; }
        public JSONStorableStringChooser GroupsJSON { get; set; }
        public JSONStorableStringChooser TypesJSON { get; set; }
        public JSONStorableStringChooser SelectablesJSON { get; set; }
        public SelectablesList SelectablesList { get; private set; }

        public ColliderPreviewConfig Config { get; } = new ColliderPreviewConfig();
        // ReSharper restore UnusedAutoPropertyAccessor.Global MemberCanBePrivate.Global

        List<IModel> _filteredSelectables;
        IModel _selected, _selectedMirror;

        public void Init(
            MVRScript script,
            List<Group> customGroups = null
        )
        {
            try
            {
                SelectablesList = SelectablesList.Build(script.containingAtom, Config, customGroups);
                SetupStorables();
                UpdateFilter();
            }
            catch(Exception e)
            {
                SuperController.LogError($"{nameof(ColliderVisualizer)}: {e}");
            }
        }

        void SetupStorables()
        {
            ShowPreviewsJSON = new JSONStorableBool("showPreviews", ColliderPreviewConfig.DEFAULT_PREVIEWS_ENABLED, value =>
            {
                Config.PreviewsEnabled = value;
                SelectablesList.All.ForEach(model => model.UpdatePreviewsFromConfig());
            })
            {
                isStorable = false,
                isRestorable = false,
            };

            XRayPreviewsJSON = new JSONStorableBool("xRayPreviews", ColliderPreviewConfig.DEFAULT_X_RAY_PREVIEWS, value =>
            {
                Config.XRayPreviews = value;
                SelectablesList.All.ForEach(model => model.UpdatePreviewsFromConfig());
            });

            XRayPreviewsOffJSON = new JSONStorableBool("xRayPreviewsOff", !ColliderPreviewConfig.DEFAULT_X_RAY_PREVIEWS,
                value => { XRayPreviewsJSON.val = !value; });

            PreviewOpacityJSON = new JSONStorableFloat("previewOpacity", ColliderPreviewConfig.DEFAULT_PREVIEWS_OPACITY, value =>
            {
                float alpha = value.ExponentialScale(ColliderPreviewConfig.EXPONENTIAL_SCALE_MIDDLE, 1f);
                Config.PreviewsOpacity = alpha;
                SelectablesList.All.ForEach(model => model.UpdatePreviewsFromConfig());
            }, 0f, 1f);

            SelectedPreviewOpacityJSON = new JSONStorableFloat("selectedPreviewOpacity", ColliderPreviewConfig.DEFAULT_SELECTED_PREVIEW_OPACITY,
                value =>
                {
                    float alpha = value.ExponentialScale(ColliderPreviewConfig.EXPONENTIAL_SCALE_MIDDLE, 1f);
                    Config.SelectedPreviewsOpacity = alpha;
                    if(_selected != null)
                    {
                        _selected.UpdatePreviewsFromConfig();
                    }

                    if(_selectedMirror != null)
                    {
                        _selectedMirror.UpdatePreviewsFromConfig();
                    }
                }, 0f, 1f);

            RelativeXRayOpacityJSON = new JSONStorableFloat("relativeXRayOpacity", ColliderPreviewConfig.DEFAULT_RELATIVE_X_RAY_OPACITY, value =>
            {
                Config.RelativeXRayOpacity = value;
                if(_selected != null)
                {
                    _selected.UpdatePreviewsFromConfig();
                }

                if(_selectedMirror != null)
                {
                    _selectedMirror.UpdatePreviewsFromConfig();
                }
            }, 0f, 1f);

            HighlightMirrorJSON = new JSONStorableBool(
                "highlightMirror",
                ColliderPreviewConfig.DEFAULT_HIGHLIGHT_MIRROR,
                value => Config.HighlightMirror = value
            );

            var groups = new List<string> { NO_SELECTION_LABEL };
            groups.AddRange(SelectablesList.Groups.Select(e => e.Name).Distinct());
            groups.Add(ALL_LABEL);
            GroupsJSON = new JSONStorableStringChooser("Group", groups, groups[0], "Group")
            {
                setCallbackFunction = _ => UpdateFilter(),
                isStorable = false,
                isRestorable = false,
            };

            var types = new List<string> { NO_SELECTION_LABEL };
            types.AddRange(SelectablesList.All.Select(e => e.Type).Distinct());
            types.Add(ALL_LABEL);
            TypesJSON = new JSONStorableStringChooser("Type", types, types[0], "Type")
            {
                setCallbackFunction = _ => UpdateFilter(),
                isStorable = false,
                isRestorable = false,
            };

            SelectablesJSON = new JSONStorableStringChooser(
                "Select",
                new List<string>(),
                new List<string>(),
                "",
                "Select"
            )
            {
                isStorable = false,
                isRestorable = false,
            };

            SelectablesJSON.setCallbackFunction = id =>
            {
                IModel val;
                if(SelectablesList.ByUuid.TryGetValue(id, out val))
                {
                    SelectModel(val);
                }
                else
                {
                    SelectModel(null);
                }
            };
        }

        public void SelectModel(IModel val)
        {
            Deselect(ref _selected);
            Deselect(ref _selectedMirror);

            if(val == null)
            {
                SelectablesJSON.valNoCallback = "";
                return;
            }

            SelectablesJSON.valNoCallback = val.Id;
            SelectablesList.PrepareForUI();

            Select(ref _selected, val);
            if(Config.HighlightMirror && _selected.MirrorModel != null)
            {
                Select(ref _selectedMirror, _selected.MirrorModel, false, true, false);
            }
        }

        void Deselect(ref IModel selected)
        {
            if(selected == null)
            {
                return;
            }

            selected.Selected = false;
            selected.Highlighted = false;
            selected.Shown = _filteredSelectables.Contains(selected);
            selected.UpdatePreviewsFromConfig();
            selected = null;
        }

        // ReSharper disable once RedundantAssignment
        void Select(ref IModel selected, IModel val, bool shown = true, bool highlighted = true, bool showUI = true)
        {
            selected = val;
            selected.Shown = shown || _filteredSelectables.Contains(selected);
            selected.Highlighted = highlighted;
            selected.Selected = showUI;
            selected.UpdatePreviewsFromConfig();
        }

        void UpdateFilter()
        {
            try
            {
                HideCurrentFilteredSelectables();

                var filtered = SelectablesList.All;

                if(GroupsJSON.val != ALL_LABEL && GroupsJSON.val != NO_SELECTION_LABEL)
                {
                    filtered = filtered.Where(e => e.Groups.Any(group => group?.Name == GroupsJSON.val)).ToList();
                }

                if(TypesJSON.val != ALL_LABEL && TypesJSON.val != NO_SELECTION_LABEL)
                {
                    filtered = filtered.Where(e => e.Type == TypesJSON.val).ToList();
                }

                _filteredSelectables = filtered;
                SelectablesJSON.choices = _filteredSelectables.Select(x => x.Id).ToList();
                SelectablesJSON.displayChoices = _filteredSelectables.Select(x => x.Label).ToList();

                // SelectModel(_selected);
                foreach(var model in _filteredSelectables)
                {
                    model.Shown = true;
                    model.UpdatePreviewsFromConfig();
                }

                // if (!SelectablesJSON.choices.Contains(SelectablesJSON.val) || string.IsNullOrEmpty(SelectablesJSON.val))
                //     SelectablesJSON.val = SelectablesJSON.choices.FirstOrDefault() ?? "";
            }
            catch(Exception e)
            {
                LogError(nameof(UpdateFilter), e.ToString());
            }
        }

        void HideCurrentFilteredSelectables()
        {
            var previous = SelectablesJSON.choices.Where(x => SelectablesList.ByUuid.ContainsKey(x)).Select(x => SelectablesList.ByUuid[x]);
            foreach(var model in previous)
            {
                model.Shown = false;
                model.UpdatePreviewsFromConfig();
            }
        }

        public void DestroyAllPreviews()
        {
            if(SelectablesList?.All != null)
            {
                SelectablesList.All.ForEach(model => model.DestroyPreviews());
            }
        }

        public void OnEnable()
        {
            if(SelectablesList?.All == null || ShowPreviewsJSON == null)
            {
                return;
            }

            try
            {
                SelectablesList.All.ForEach(model => model.UpdatePreviewsFromConfig());
            }
            catch(Exception e)
            {
                LogError(nameof(OnEnable), e.ToString());
            }
        }

        public void OnDisable()
        {
            try
            {
                DestroyAllPreviews();
            }
            catch(Exception e)
            {
                LogError(nameof(OnDisable), e.ToString());
            }
        }

        void OnDestroy()
        {
            MaterialHelper.Destroy();
        }

        public void SyncPreviews()
        {
            if(SelectablesList != null && ShowPreviewsJSON != null && ShowPreviewsJSON.val)
            {
                SelectablesList.All.ForEach(model => model.SyncPreviews());
            }
        }

        static void LogError(string method, string message)
        {
            SuperController.LogError($"{nameof(ColliderVisualizer)}.{method}: {message}");
        }
    }
}
