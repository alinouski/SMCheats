using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public sealed class CheatsConsoleEntriesView : MonoBehaviour
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _entriesRoot;
        [SerializeField] private CheatsConsoleEntryView _entryTemplate;

        private readonly List<CheatsConsoleEntryView> _entryPool = new();
        private Action<int> _selectEntry;
        private ScrollRect _cachedScrollRect;

        public void Initialize()
        {
            _entryTemplate.gameObject.SetActive(false);
        }

        public void ClearPool()
        {
            _entryPool.Clear();
        }

        public void Render(
            IReadOnlyList<CheatsConsoleEntry> entries,
            int maxRenderedEntries,
            int selectedEntryId,
            bool autoScroll,
            Action<int> selectEntry)
        {
            _selectEntry = selectEntry;
            var entriesCount = entries?.Count ?? 0;
            var renderedCount = Mathf.Min(entriesCount, Mathf.Max(0, maxRenderedEntries));
            var startIndex = autoScroll ? Mathf.Max(0, entriesCount - renderedCount) : 0;

            EnsureEntryPool(renderedCount);
            for (var i = 0; i < _entryPool.Count; i++)
            {
                var view = _entryPool[i];
                var isActive = i < renderedCount;
                view.gameObject.SetActive(isActive);
                if (!isActive)
                {
                    continue;
                }

                BindEntryView(view, entries[startIndex + i], selectedEntryId);
            }

            if (autoScroll)
            {
                ScrollToBottom();
            }
        }

        private void EnsureEntryPool(int count)
        {
            while (_entryPool.Count < count)
            {
                var entryView = Object.Instantiate(_entryTemplate, _entriesRoot, false);
                entryView.name = _entryTemplate.name;
                _entryPool.Add(entryView);
            }
        }

        private void BindEntryView(
            CheatsConsoleEntryView view,
            CheatsConsoleEntry entry,
            int selectedEntryId)
        {
            var isSelected = entry.Id == selectedEntryId;
            var backgroundColor = CheatsSettingsProxy.UiSettings.GetConsoleEntryBackgroundColor(entry.LogType, isSelected);

            view.Initialize(HandleEntrySelected);
            view.Bind(entry, backgroundColor);
        }

        private void ScrollToBottom()
        {
            var scrollRect = GetScrollRect();
            scrollRect.StopMovement();
            scrollRect.verticalNormalizedPosition = 0f;
            scrollRect.horizontalNormalizedPosition = 0f;
        }

        private ScrollRect GetScrollRect()
        {
            if (_scrollRect != null)
            {
                return _scrollRect;
            }

            if (_cachedScrollRect == null)
            {
                _cachedScrollRect = _entriesRoot.GetComponentInParent<ScrollRect>();
            }

            return _cachedScrollRect;
        }

        private void HandleEntrySelected(int entryId)
        {
            _selectEntry?.Invoke(entryId);
        }
    }
}
