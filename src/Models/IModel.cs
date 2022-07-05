using System.Collections.Generic;
using SimpleJSON;

public interface IModel
{
    string Type { get; }
    List<Group> Groups { get; }
    string Id { get; }
    string Label { get; }
    bool Shown { get; set; }
    bool Highlighted { get; set; }
    bool Selected { get; set; }
    bool Modified { get; }
    IModel MirrorModel { get; }
    bool SyncWithMirror { get; set; }

    void UpdatePreviewsFromConfig();
    void SyncPreviews();
    void DestroyPreviews();
    void ResetToInitial();
    bool SyncOverrides();

    void LoadJson(JSONClass jc);
    void AppendJson(JSONClass jc);
}
