using System;
using System.Runtime.CompilerServices;
using Client.Libs.UI.Internal;
using UnityEngine;

namespace Client.Libs.UI.Screens
{
    internal class UIScreenEntry : INotifyCompletion
    {
        public Type ScreenType { get; }
        internal UIScreenInfo ScreenInfo { get; }

        private Action _closeListeners;

        internal UIScreenEntry(UIScreenInfo screenInfo)
        {
            ScreenInfo = screenInfo;
            ScreenType = screenInfo.ScreenPresenter.GetType();
        }

        public void Close()
        {
            try
            {
                _closeListeners?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                _closeListeners = null;
            }

            IsCompleted = true;
        }

        public void OnCompleted(Action continuation) =>
            _closeListeners += continuation;

        public bool IsCompleted { get; private set; }

        public UIScreenEntry GetAwaiter() =>
            this;

        public void GetResult()
        {
        }
    }
}