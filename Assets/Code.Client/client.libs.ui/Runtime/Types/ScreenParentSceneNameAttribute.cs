using System;

namespace Client.Libs.UI.Types
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ScreenParentSceneNameAttribute : Attribute
    {
        public readonly string SceneName;

        public ScreenParentSceneNameAttribute(string sceneName)
        {
            SceneName = sceneName;
        }
    }
}