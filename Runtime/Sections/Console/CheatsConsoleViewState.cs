using System;
using UnityEngine;

namespace SMCheats
{
    internal sealed class CheatsConsoleViewState
    {
        public bool ShowLogs { get; private set; } = true;
        public bool ShowWarnings { get; private set; } = true;
        public bool ShowErrors { get; private set; } = true;
        public bool ShowSearch { get; private set; }
        public bool AutoScroll { get; private set; } = true;
        public bool PinnedEntriesVisible { get; private set; } = true;
        public string SearchText { get; private set; } = string.Empty;
        public int SelectedEntryId { get; private set; } = -1;
        public int FilterRevision { get; private set; }

        public bool ToggleFilter(CheatsConsoleFilterType filterType)
        {
            switch (filterType)
            {
                case CheatsConsoleFilterType.Warnings:
                    ShowWarnings = !ShowWarnings;
                    break;
                case CheatsConsoleFilterType.Errors:
                    ShowErrors = !ShowErrors;
                    break;
                default:
                    ShowLogs = !ShowLogs;
                    break;
            }

            FilterRevision++;
            return true;
        }

        public void ToggleSearch()
        {
            ShowSearch = !ShowSearch;
            if (!ShowSearch)
            {
                SetSearchText(string.Empty);
            }
        }

        public bool SetSearchText(string value)
        {
            value ??= string.Empty;
            if (string.Equals(SearchText, value, StringComparison.Ordinal))
            {
                return false;
            }

            SearchText = value;
            FilterRevision++;
            return true;
        }

        public void ToggleAutoScroll()
        {
            AutoScroll = !AutoScroll;
        }

        public void TogglePinnedEntriesVisible()
        {
            PinnedEntriesVisible = !PinnedEntriesVisible;
        }

        public void ResetPinnedEntriesVisible()
        {
            PinnedEntriesVisible = true;
        }

        public void ToggleSelectedEntry(int id)
        {
            SelectedEntryId = SelectedEntryId == id ? -1 : id;
        }

        public void ClearSelection()
        {
            SelectedEntryId = -1;
        }

        public bool IsVisible(CheatsConsoleEntry entry)
        {
            if (entry == null || !IsTypeVisible(entry.LogType))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                return true;
            }

            return Contains(entry.Message, SearchText) ||
                   Contains(entry.StackTrace, SearchText) ||
                   Contains(entry.SceneName, SearchText) ||
                   ContainsLogType(entry.LogType, SearchText);
        }

        public bool IsTypeVisible(LogType type)
        {
            switch (type)
            {
                case LogType.Warning:
                    return ShowWarnings;
                case LogType.Assert:
                case LogType.Error:
                case LogType.Exception:
                    return ShowErrors;
                default:
                    return ShowLogs;
            }
        }

        private static bool ContainsLogType(LogType type, string value)
        {
            switch (type)
            {
                case LogType.Warning:
                    return Contains(nameof(LogType.Warning), value);
                case LogType.Assert:
                    return Contains(nameof(LogType.Assert), value);
                case LogType.Error:
                    return Contains(nameof(LogType.Error), value);
                case LogType.Exception:
                    return Contains(nameof(LogType.Exception), value);
                default:
                    return Contains(nameof(LogType.Log), value);
            }
        }

        private static bool Contains(string source, string value)
        {
            return !string.IsNullOrEmpty(source) &&
                   source.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
