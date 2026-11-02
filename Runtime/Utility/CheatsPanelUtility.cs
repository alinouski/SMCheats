using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SMCheats
{
    public static class CheatsPanelUtility
    {
        public static bool IsPanelEnabled(CheatsBuiltInTabs builtInFlag)
        {
            if (builtInFlag == CheatsBuiltInTabs.None)
            {
                return true;
            }

            var settings = CheatsSettingsProxy.Settings;
            return settings == null || (settings.EnabledTabs & builtInFlag) == builtInFlag;
        }

        public static (GameObject, IDisposable) GetDestroyDisposable(this GameObject gameObject, IDisposable extraDispose = null)
        {
            var destroyDisposable = gameObject.CreateDestroyDisposable();
            
            if (extraDispose != null)
            {
                return (gameObject, new DisposableAction(() =>
                {
                    extraDispose.Dispose();
                    destroyDisposable.Dispose();
                }));
            }

            return (gameObject, destroyDisposable);
        }
        
        public static IDisposable CreateDestroyDisposable(this GameObject gameObject)
        {
            return new DisposableAction(() => { Object.Destroy(gameObject); });
        }
    }
}