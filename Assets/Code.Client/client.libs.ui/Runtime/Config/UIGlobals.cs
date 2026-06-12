using Client.Libs.Extensions;
using Client.Libs.UI.Controls;
using UnityEngine;

namespace Client.Libs.UI.Config
{
    [CreateAssetMenu(menuName = "Three Tabs/UIGlobals", fileName = "UIGlobals", order = 0)]
    public class UIGlobals : ScriptableObject, IUIGlobals
    {
        public const string AssetName = "UIGlobals";

        [Header("Scale")]
        [SerializeField, Range(0, 1.0f),] private float _scale189 = 1.0f;
        [SerializeField, Range(0, 1.0f),] private float _scale169 = 0.85f;

        public void ApplyUIScale()
        {
            var aspect = GfxUtils.GetAspectRatioLerp(GfxUtils.AspectRatio.Aspect_16_9, GfxUtils.AspectRatio.Aspect_18_9);
            UIScaler.GlobalUIScale = Mathf.Lerp(_scale169, _scale189, aspect);
        }
    }
}