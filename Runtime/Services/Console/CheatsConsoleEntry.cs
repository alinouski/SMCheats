using System;
using UnityEngine;

namespace SMCheats
{
    public sealed class CheatsConsoleEntry
    {
        public CheatsConsoleEntry(
            int id,
            LogType logType,
            string message,
            string stackTrace,
            DateTime time,
            int frameCount,
            string sceneName)
        {
            Id = id;
            LogType = logType;
            Message = message;
            StackTrace = stackTrace;
            Time = time;
            FrameCount = frameCount;
            SceneName = sceneName;
            Count = 1;
        }

        public int Id { get; }
        public LogType LogType { get; }
        public string Message { get; }
        public string StackTrace { get; }
        public DateTime Time { get; private set; }
        public int FrameCount { get; private set; }
        public string SceneName { get; private set; }
        public int Count { get; private set; }

        public void Increment(DateTime time, int frameCount, string sceneName)
        {
            Count++;
            Time = time;
            FrameCount = frameCount;
            SceneName = sceneName;
        }
    }
}