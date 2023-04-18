using System.Collections.Generic;

namespace ColliderEditor.Models
{
    interface IModel
    {
        string Type { get; }
        List<Group> Groups { get; }
        string Id { get; }
        string Label { get; }
        bool Shown { set; }
        bool Highlighted { set; }
        bool Selected { set; }
        IModel MirrorModel { get; }

        void UpdatePreviewsFromConfig();
        void SyncPreviews();
        void DestroyPreviews();
    }
}
