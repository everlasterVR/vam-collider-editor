// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
using ColliderEditor.Extensions;

namespace ColliderEditor
{
    public class ColliderPreviewConfig
    {
        public const float EXPONENTIAL_SCALE_MIDDLE = 0.1f;

        public const bool DEFAULT_PREVIEWS_ENABLED = false;
        public const bool DEFAULT_X_RAY_PREVIEWS = true;
        public const float DEFAULT_PREVIEWS_OPACITY = 0.5f;
        public const float DEFAULT_SELECTED_PREVIEW_OPACITY = 1.0f;
        public const float DEFAULT_RELATIVE_X_RAY_OPACITY = 0.5f;
        public const bool DEFAULT_HIGHLIGHT_MIRROR = false;

        public bool PreviewsEnabled { get; set; } = DEFAULT_PREVIEWS_ENABLED;
        public bool XRayPreviews { get; set; } = DEFAULT_X_RAY_PREVIEWS;
        public bool HighlightMirror { get; set; } = DEFAULT_HIGHLIGHT_MIRROR;
        public float PreviewsOpacity { get; set; } = DEFAULT_PREVIEWS_OPACITY.ExponentialScale(EXPONENTIAL_SCALE_MIDDLE, 1f);
        public float SelectedPreviewsOpacity { get; set; } = DEFAULT_SELECTED_PREVIEW_OPACITY.ExponentialScale(EXPONENTIAL_SCALE_MIDDLE, 1f);
        public float RelativeXRayOpacity { get; set; } = DEFAULT_RELATIVE_X_RAY_OPACITY;
    }
}
