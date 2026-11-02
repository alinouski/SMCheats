using System.Collections.Generic;

namespace SMCheats
{
    internal sealed class CheatsConsoleEntriesModel
    {
        private readonly List<CheatsConsoleEntry> _entriesSnapshot = new();
        private readonly List<CheatsConsoleEntry> _filteredEntries = new();
        private readonly Dictionary<int, int> _filteredEntryIndices = new();
        private int _lastDataRevision = -1;
        private int _lastFilterRevision = -1;

        public IReadOnlyList<CheatsConsoleEntry> FilteredEntries => _filteredEntries;

        public void Clear()
        {
            _entriesSnapshot.Clear();
            _filteredEntries.Clear();
            _filteredEntryIndices.Clear();
            _lastDataRevision = -1;
            _lastFilterRevision = -1;
        }

        public bool Refresh(CheatsConsoleService consoleService, CheatsConsoleViewState state, bool force)
        {
            if (consoleService == null || state == null)
            {
                return false;
            }

            var currentDataRevision = consoleService.Revision;
            var filterChanged = _lastFilterRevision != state.FilterRevision;
            if (!force && !filterChanged && _lastDataRevision == currentDataRevision)
            {
                return false;
            }

            if (ShouldRebuild(consoleService, force, filterChanged, currentDataRevision))
            {
                _lastDataRevision = currentDataRevision;
                _lastFilterRevision = state.FilterRevision;
                consoleService.CopyEntriesTo(_entriesSnapshot);
                RebuildFilteredEntries(_entriesSnapshot, state);
                return true;
            }

            _lastDataRevision = currentDataRevision;
            _lastFilterRevision = state.FilterRevision;
            ApplyLastEntryChange(consoleService, state);
            return true;
        }

        public CheatsConsoleEntry Find(int id)
        {
            return _filteredEntryIndices.TryGetValue(id, out var index) &&
                   index >= 0 &&
                   index < _filteredEntries.Count
                ? _filteredEntries[index]
                : null;
        }

        private bool ShouldRebuild(
            CheatsConsoleService consoleService,
            bool force,
            bool filterChanged,
            int currentDataRevision)
        {
            return force ||
                   filterChanged ||
                   _lastDataRevision < 0 ||
                   currentDataRevision < _lastDataRevision ||
                   currentDataRevision - _lastDataRevision != 1 ||
                   consoleService.LastChangeRequiresFullRefresh;
        }

        private void ApplyLastEntryChange(CheatsConsoleService consoleService, CheatsConsoleViewState state)
        {
            if (!consoleService.TryGetEntry(consoleService.LastChangedEntryId, out var entry))
            {
                consoleService.CopyEntriesTo(_entriesSnapshot);
                RebuildFilteredEntries(_entriesSnapshot, state);
                return;
            }

            var isVisible = state.IsVisible(entry);
            if (_filteredEntryIndices.TryGetValue(entry.Id, out var existingIndex))
            {
                if (!isVisible)
                {
                    RemoveFilteredEntryAt(existingIndex);
                    if (state.SelectedEntryId == entry.Id)
                    {
                        state.ClearSelection();
                    }
                }

                return;
            }

            if (!isVisible)
            {
                return;
            }

            _filteredEntryIndices[entry.Id] = _filteredEntries.Count;
            _filteredEntries.Add(entry);
        }

        private void RebuildFilteredEntries(IReadOnlyList<CheatsConsoleEntry> entries, CheatsConsoleViewState state)
        {
            _filteredEntries.Clear();
            _filteredEntryIndices.Clear();

            for (var i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                if (!state.IsVisible(entry))
                {
                    continue;
                }

                _filteredEntryIndices[entry.Id] = _filteredEntries.Count;
                _filteredEntries.Add(entry);
            }

            if (state.SelectedEntryId >= 0 && Find(state.SelectedEntryId) == null)
            {
                state.ClearSelection();
            }
        }

        private void RemoveFilteredEntryAt(int index)
        {
            if (index < 0 || index >= _filteredEntries.Count)
            {
                return;
            }

            _filteredEntryIndices.Remove(_filteredEntries[index].Id);
            _filteredEntries.RemoveAt(index);
            for (var i = index; i < _filteredEntries.Count; i++)
            {
                _filteredEntryIndices[_filteredEntries[i].Id] = i;
            }
        }
    }
}
