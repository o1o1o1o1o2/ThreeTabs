using System;

namespace Client.Libs.UI.Types
{
    /// <summary>
    ///     UI screen configuration flags
    /// </summary>
    [Flags]
    public enum ScreenStyle
    {
        None = 0,

        PauseInput = 1 << 0,

        PauseGame = 1 << 1,

        FullScreen = 1 << 2,

        AlwaysOnTop = 1 << 3,

        System = 1 << 4,

        UnloadOnClose = 1 << 5,

        Default = PauseGame | PauseInput,
        DefaultFullscreen = PauseGame | PauseInput | FullScreen
    }
}