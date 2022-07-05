using System.Collections.Generic;

public interface IModel
{
    string Type { get; }
    List<Group> Groups { get; }
    string Id { get; }
    string Label { get; }
    bool Shown { get; set; }
    bool Highlighted { get; set; }
    bool Selected { get; set; }
    IModel MirrorModel { get; }

    void UpdatePreviewsFromConfig();
    void SyncPreviews();
    void DestroyPreviews();
}
