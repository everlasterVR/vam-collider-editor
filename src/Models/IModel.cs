using SimpleJSON;

public interface IModel
{
    string Type { get; }
    Group Group { get; }
    string Id { get; }
    string Label { get; }
    string QualifiedName { get; }
    bool IsDuplicate { get; }
    bool Shown { get; set; }
    bool Selected { get; set; }
    bool Modified { get; }
    IModel Linked { get; }

    void UpdatePreviewFromConfig();
    void SyncPreview();
    void DestroyPreview();
    void ResetToInitial();
    bool SyncOverrides();

    void LoadJson(JSONClass jc);
    void AppendJson(JSONClass jc);
}
