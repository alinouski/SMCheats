using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SMCheats
{
    public sealed class CheatsConsoleService : IDisposable
    {
        private const int MaxMessageChars = 4096;
        private const int MaxStackTraceChars = 12000;

        private readonly object _sync = new();
        private readonly LinkedList<CheatsConsoleEntry> _entries;
        private readonly Dictionary<LogKey, CheatsConsoleEntry> _entriesByKey;
        private readonly Dictionary<int, CheatsConsoleEntry> _entriesById;
        private readonly int _maximumEntries;
        private bool _isListening;
        private int _nextEntryId;

        public CheatsConsoleService(int maximumEntries)
        {
            _maximumEntries = Mathf.Max(50, maximumEntries);
            _entries = new LinkedList<CheatsConsoleEntry>();
            _entriesByKey = new Dictionary<LogKey, CheatsConsoleEntry>(_maximumEntries);
            _entriesById = new Dictionary<int, CheatsConsoleEntry>(_maximumEntries);
        }

        public int ErrorCount { get; private set; }
        public int ExceptionCount { get; private set; }
        public int ErrorAndExceptionCount => ErrorCount + ExceptionCount;
        public int WarningCount { get; private set; }
        public int LogCount { get; private set; }
        public int ExceptionRevision { get; private set; }
        public int Revision { get; private set; }
        public int LastChangedEntryId { get; private set; } = -1;
        public bool LastChangeRequiresFullRefresh { get; private set; } = true;
        public event Action ExceptionReceived;

        public void StartListening()
        {
            if (_isListening)
            {
                return;
            }

            Application.logMessageReceived += HandleLogMessageReceived;
            _isListening = true;
        }

        public void StopListening()
        {
            if (!_isListening)
            {
                return;
            }

            Application.logMessageReceived -= HandleLogMessageReceived;
            _isListening = false;
        }

        public void CopyEntriesTo(List<CheatsConsoleEntry> target)
        {
            if (target == null)
            {
                return;
            }

            lock (_sync)
            {
                target.Clear();
                if (target.Capacity < _entries.Count)
                {
                    target.Capacity = _entries.Count;
                }

                foreach (var entry in _entries)
                {
                    target.Add(entry);
                }
            }
        }

        public bool TryGetEntry(int id, out CheatsConsoleEntry entry)
        {
            lock (_sync)
            {
                return _entriesById.TryGetValue(id, out entry);
            }
        }

        public void Clear()
        {
            lock (_sync)
            {
                _entries.Clear();
                _entriesByKey.Clear();
                _entriesById.Clear();
                ErrorCount = 0;
                ExceptionCount = 0;
                WarningCount = 0;
                LogCount = 0;
                ExceptionRevision = 0;
                LastChangedEntryId = -1;
                LastChangeRequiresFullRefresh = true;
                Revision++;
            }
        }

        public void Dispose()
        {
            StopListening();
        }

        private void HandleLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            var message = Trim(condition, MaxMessageChars);
            var safeStackTrace = Trim(stackTrace, MaxStackTraceChars);
            var key = new LogKey(type, message, safeStackTrace);
            var scene = SceneManager.GetActiveScene();
            var sceneName = string.IsNullOrEmpty(scene.name) ? scene.buildIndex.ToString() : scene.name;
            var time = DateTime.Now;
            var frameCount = Time.frameCount;
            var isException = type == LogType.Exception;

            lock (_sync)
            {
                AddCounter(type, 1);
                if (isException)
                {
                    ExceptionRevision++;
                }

                if (_entriesByKey.TryGetValue(key, out var existingEntry))
                {
                    existingEntry.Increment(time, frameCount, sceneName);
                    LastChangedEntryId = existingEntry.Id;
                    LastChangeRequiresFullRefresh = false;
                    Revision++;
                }
                else
                {
                    var requiresFullRefresh = false;
                    if (_entries.Count >= _maximumEntries)
                    {
                        var removedEntry = _entries.First.Value;
                        AddCounter(removedEntry.LogType, -removedEntry.Count);
                        _entriesByKey.Remove(new LogKey(removedEntry.LogType, removedEntry.Message, removedEntry.StackTrace));
                        _entriesById.Remove(removedEntry.Id);
                        _entries.RemoveFirst();
                        requiresFullRefresh = true;
                    }

                    var entry = new CheatsConsoleEntry(_nextEntryId++, type, message, safeStackTrace, time, frameCount, sceneName);
                    _entries.AddLast(entry);
                    _entriesByKey.Add(key, entry);
                    _entriesById.Add(entry.Id, entry);
                    LastChangedEntryId = entry.Id;
                    LastChangeRequiresFullRefresh = requiresFullRefresh;
                    Revision++;
                }
            }

            if (isException)
            {
                ExceptionReceived?.Invoke();
            }
        }

        private void AddCounter(LogType type, int amount)
        {
            switch (type)
            {
                case LogType.Assert:
                case LogType.Error:
                    ErrorCount += amount;
                    break;
                case LogType.Exception:
                    ExceptionCount += amount;
                    break;
                case LogType.Warning:
                    WarningCount += amount;
                    break;
                default:
                    LogCount += amount;
                    break;
            }
        }

        private static string Trim(string value, int maximumLength)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maximumLength)
            {
                return value ?? string.Empty;
            }

            return value.Substring(0, maximumLength);
        }

        private readonly struct LogKey : IEquatable<LogKey>
        {
            private readonly LogType _type;
            private readonly string _message;
            private readonly string _stackTrace;

            public LogKey(LogType type, string message, string stackTrace)
            {
                _type = type;
                _message = message ?? string.Empty;
                _stackTrace = stackTrace ?? string.Empty;
            }

            public bool Equals(LogKey other)
            {
                return _type == other._type &&
                       string.Equals(_message, other._message, StringComparison.Ordinal) &&
                       string.Equals(_stackTrace, other._stackTrace, StringComparison.Ordinal);
            }

            public override bool Equals(object obj)
            {
                return obj is LogKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (int)_type;
                    hashCode = (hashCode * 397) ^ StringComparer.Ordinal.GetHashCode(_message);
                    hashCode = (hashCode * 397) ^ StringComparer.Ordinal.GetHashCode(_stackTrace);
                    return hashCode;
                }
            }
        }
    }
}
