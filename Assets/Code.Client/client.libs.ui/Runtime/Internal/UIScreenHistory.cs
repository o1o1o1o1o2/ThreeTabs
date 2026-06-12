using System;
using System.Collections.Generic;
using System.Linq;
using Client.Libs.UI.Screens;

namespace Client.Libs.UI.Internal
{
    internal sealed class UIScreenHistoryStack
    {
        private readonly List<UIScreenEntry> _entries = new(8);

        public int Count => _entries.Count;

        public UIScreenEntry LastOrDefault(Type screenType) =>
            _entries.LastOrDefault(x => x.ScreenType == screenType);

        public UIScreenEntry Last() =>
            _entries[^1];

        public void Add(UIScreenEntry entry) =>
            _entries.Add(entry ?? throw new ArgumentNullException(nameof(entry)));

        public bool Remove(UIScreenEntry entry) =>
            _entries.Remove(entry);

        public void UpdateVisualOrder()
        {
            for (var i = 0; i < _entries.Count; i++)
                _entries[i].ScreenInfo.VisualOrder = i;
        }

        public override string ToString() =>
            $"[stack={string.Join(", ", _entries.Select(x => x.ScreenType.Name))}]";
    }
}
