using Client.Libs.UI.Internal;

namespace Client.Libs.UI.Screens
{
    internal class UIScreenEntryWithResult : UIScreenEntry
    {
        private readonly object _result;

        internal UIScreenEntryWithResult(UIScreenInfo screenInstance, IUiScreenWithResult screenWithStateResult) : base(screenInstance) =>
            _result = screenWithStateResult.GetResult();

        public new UIScreenEntryWithResult GetAwaiter() =>
            this;

        public new object GetResult() =>
            _result;
    }
}