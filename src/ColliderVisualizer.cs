using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

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
public class ColliderVisualizer : MonoBehaviour
{
    private const string _noSelectionLabel = "Select...";
    private const string _allLabel = "All";

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

    private List<IModel> _filteredSelectables;
    private IModel _selected, _selectedMirror;

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
        catch (Exception e)
        {
            SuperController.LogError($"{nameof(ColliderVisualizer)}: {e}");
        }
    }

    private void SetupStorables()
    {
        ShowPreviewsJSON = new JSONStorableBool("showPreviews", ColliderPreviewConfig.DefaultPreviewsEnabled, value =>
        {
            Config.PreviewsEnabled = value;
            SelectablesList.All.ForEach(model => model.UpdatePreviewsFromConfig());
        })
        {
            isStorable = false,
            isRestorable = false
        };

        XRayPreviewsJSON = new JSONStorableBool("xRayPreviews", ColliderPreviewConfig.DefaultXRayPreviews, value =>
        {
            Config.XRayPreviews = value;
            SelectablesList.All.ForEach(model => model.UpdatePreviewsFromConfig());
        });

        XRayPreviewsOffJSON = new JSONStorableBool("xRayPreviewsOff", !ColliderPreviewConfig.DefaultXRayPreviews, value =>
        {
            XRayPreviewsJSON.val = !value;
        });

        PreviewOpacityJSON = new JSONStorableFloat("previewOpacity", ColliderPreviewConfig.DefaultPreviewsOpacity, value =>
        {
            float alpha = value.ExponentialScale(ColliderPreviewConfig.ExponentialScaleMiddle, 1f);
            Config.PreviewsOpacity = alpha;
            SelectablesList.All.ForEach(model => model.UpdatePreviewsFromConfig());
        }, 0f, 1f);

        SelectedPreviewOpacityJSON = new JSONStorableFloat("selectedPreviewOpacity", ColliderPreviewConfig.DefaultSelectedPreviewOpacity, value =>
        {
            float alpha = value.ExponentialScale(ColliderPreviewConfig.ExponentialScaleMiddle, 1f);
            Config.SelectedPreviewsOpacity = alpha;
            if (_selected != null)
                _selected.UpdatePreviewsFromConfig();
            if (_selectedMirror != null)
                _selectedMirror.UpdatePreviewsFromConfig();
        }, 0f, 1f);

        RelativeXRayOpacityJSON = new JSONStorableFloat("relativeXRayOpacity", ColliderPreviewConfig.DefaultRelativeXRayOpacity, value =>
        {
            Config.RelativeXRayOpacity = value;
            if (_selected != null)
                _selected.UpdatePreviewsFromConfig();
            if (_selectedMirror != null)
                _selectedMirror.UpdatePreviewsFromConfig();
        }, 0f, 1f);

        HighlightMirrorJSON = new JSONStorableBool(
            "highlightMirror",
            ColliderPreviewConfig.DefaultHighlightMirror,
            value => Config.HighlightMirror = value
        );

        var groups = new List<string> { _noSelectionLabel };
        groups.AddRange(SelectablesList.Groups.Select(e => e.Name).Distinct());
        groups.Add(_allLabel);
        GroupsJSON = new JSONStorableStringChooser("Group", groups, groups[0], "Group")
        {
            setCallbackFunction = _ => UpdateFilter(),
            isStorable = false,
            isRestorable = false
        };

        var types = new List<string> { _noSelectionLabel };
        types.AddRange(SelectablesList.All.Select(e => e.Type).Distinct());
        types.Add(_allLabel);
        TypesJSON = new JSONStorableStringChooser("Type", types, types[0], "Type")
        {
            setCallbackFunction = _ => UpdateFilter(),
            isStorable = false,
            isRestorable = false
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
            isRestorable = false
        };

        SelectablesJSON.setCallbackFunction = id =>
        {
            IModel val;
            if (SelectablesList.ByUuid.TryGetValue(id, out val))
                SelectModel(val);
            else
                SelectModel(null);
        };
    }

    public void SelectModel(IModel val)
    {
        Deselect(ref _selected);
        Deselect(ref _selectedMirror);

        if (val == null)
        {
            SelectablesJSON.valNoCallback = "";
            return;
        }

        SelectablesJSON.valNoCallback = val.Id;
        SelectablesList.PrepareForUI();

        Select(ref _selected, val);
        if (Config.HighlightMirror && _selected.MirrorModel != null)
            Select(ref _selectedMirror, _selected.MirrorModel, false, true, false);
    }

    private void Deselect(ref IModel selected)
    {
        if (selected == null) return;
        selected.Selected = false;
        selected.Highlighted = false;
        selected.Shown = _filteredSelectables.Contains(selected);
        selected.UpdatePreviewsFromConfig();
        selected = null;
    }

    // ReSharper disable once RedundantAssignment
    private void Select(ref IModel selected, IModel val, bool shown = true, bool highlighted = true, bool showUI = true)
    {
        selected = val;
        selected.Shown = shown || _filteredSelectables.Contains(selected);
        selected.Highlighted = highlighted;
        selected.Selected = showUI;
        selected.UpdatePreviewsFromConfig();
    }

    private void UpdateFilter()
    {
        try
        {
            HideCurrentFilteredSelectables();

            var filtered = SelectablesList.All;

            if (GroupsJSON.val != _allLabel && GroupsJSON.val != _noSelectionLabel)
                filtered = filtered.Where(e => e.Groups.Any(group => group?.Name == GroupsJSON.val)).ToList();

            if (TypesJSON.val != _allLabel && TypesJSON.val != _noSelectionLabel)
                filtered = filtered.Where(e => e.Type == TypesJSON.val).ToList();

            _filteredSelectables = filtered;
            SelectablesJSON.choices = _filteredSelectables.Select(x => x.Id).ToList();
            SelectablesJSON.displayChoices = _filteredSelectables.Select(x => x.Label).ToList();

            SelectModel(_selected);
            foreach (var model in _filteredSelectables)
            {
                model.Shown = true;
                model.UpdatePreviewsFromConfig();
            }

            if (!SelectablesJSON.choices.Contains(SelectablesJSON.val) || string.IsNullOrEmpty(SelectablesJSON.val))
                SelectablesJSON.val = SelectablesJSON.choices.FirstOrDefault() ?? "";
        }
        catch (Exception e)
        {
            LogError(nameof(UpdateFilter), e.ToString());
        }
    }

    private void HideCurrentFilteredSelectables()
    {
        var previous = SelectablesJSON.choices.Where(x => SelectablesList.ByUuid.ContainsKey(x)).Select(x => SelectablesList.ByUuid[x]);
        foreach (var model in previous)
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

    #region Unity events

    public void OnEnable()
    {
        if (SelectablesList?.All == null || ShowPreviewsJSON == null) return;
        try
        {
            SelectablesList.All.ForEach(model => model.UpdatePreviewsFromConfig());
        }
        catch (Exception e)
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
        catch (Exception e)
        {
            LogError(nameof(OnDisable), e.ToString());
        }
    }

    #endregion

    public void SyncPreviews()
    {
        if(SelectablesList != null && ShowPreviewsJSON != null && ShowPreviewsJSON.val)
        {
            SelectablesList.All.ForEach(model => model.SyncPreviews());
        }
    }

    private static void LogError(string method, string message) => SuperController.LogError($"{nameof(ColliderVisualizer)}.{method}: {message}");
}
