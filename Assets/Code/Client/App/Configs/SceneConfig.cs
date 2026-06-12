using UnityEngine;

namespace Client.App.Configs
{
    [CreateAssetMenu(menuName = "Three Tabs/Scene Config", fileName = "SceneConfig")]
    internal sealed class SceneConfig : ScriptableObject, ISceneConfig
    {
        [field: SerializeField] public string MainSceneName { get; private set; } = "Main";
        [field: SerializeField] public string MenuSceneName { get; private set; } = "Menu";
    }
}