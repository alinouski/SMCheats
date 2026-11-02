using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SMCheats
{
    internal static class CheatsTicker
    {
        private static readonly List<ICheatsUpdateTickable> UpdateTickables = new();
        private static readonly List<ICheatsLateUpdateTickable> LateUpdateTickables = new();
        private static readonly List<ICheatsFixedUpdateTickable> FixedUpdateTickables = new();

        private static CheatsTickerRunner _runner;

        public static void Add(object target)
        {
            if (target == null)
            {
                return;
            }

            if (target is ICheatsUpdateTickable updateTickable && !UpdateTickables.Contains(updateTickable))
            {
                UpdateTickables.Add(updateTickable);
            }

            if (target is ICheatsLateUpdateTickable lateUpdateTickable && !LateUpdateTickables.Contains(lateUpdateTickable))
            {
                LateUpdateTickables.Add(lateUpdateTickable);
            }

            if (target is ICheatsFixedUpdateTickable fixedUpdateTickable && !FixedUpdateTickables.Contains(fixedUpdateTickable))
            {
                FixedUpdateTickables.Add(fixedUpdateTickable);
            }

            if (HasTickables())
            {
                ValidateRunner();
            }
        }

        public static void Remove(object target)
        {
            if (target == null)
            {
                return;
            }

            if (target is ICheatsUpdateTickable updateTickable)
            {
                UpdateTickables.Remove(updateTickable);
            }

            if (target is ICheatsLateUpdateTickable lateUpdateTickable)
            {
                LateUpdateTickables.Remove(lateUpdateTickable);
            }

            if (target is ICheatsFixedUpdateTickable fixedUpdateTickable)
            {
                FixedUpdateTickables.Remove(fixedUpdateTickable);
            }

            if (!HasTickables())
            {
                ResetState();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetState()
        {
            UpdateTickables.Clear();
            LateUpdateTickables.Clear();
            FixedUpdateTickables.Clear();

            if (_runner != null)
            {
                Object.Destroy(_runner.gameObject);
                _runner = null;
            }
        }

        private static void ValidateRunner()
        {
            if (_runner != null)
            {
                return;
            }

            var runnerObject = new GameObject("CheatsTicker");
            runnerObject.hideFlags = HideFlags.HideInHierarchy;
            Object.DontDestroyOnLoad(runnerObject);
            _runner = runnerObject.AddComponent<CheatsTickerRunner>();
        }

        private static bool HasTickables()
        {
            return UpdateTickables.Count > 0 || LateUpdateTickables.Count > 0 || FixedUpdateTickables.Count > 0;
        }

        private static void TickUpdate()
        {
            for (var i = UpdateTickables.Count - 1; i >= 0; i--)
            {
                try
                {
                    UpdateTickables[i].Tick();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        private static void TickLateUpdate()
        {
            for (var i = LateUpdateTickables.Count - 1; i >= 0; i--)
            {
                try
                {
                    LateUpdateTickables[i].LateTick();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        private static void TickFixedUpdate()
        {
            for (var i = FixedUpdateTickables.Count - 1; i >= 0; i--)
            {
                try
                {
                    FixedUpdateTickables[i].FixedTick();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        private sealed class CheatsTickerRunner : MonoBehaviour
        {
            private void Update()
            {
                TickUpdate();
            }

            private void LateUpdate()
            {
                TickLateUpdate();
            }

            private void FixedUpdate()
            {
                TickFixedUpdate();
            }
        }
    }
}
