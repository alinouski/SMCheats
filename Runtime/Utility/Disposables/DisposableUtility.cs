using System;
using System.Collections.Generic;

namespace SMCheats
{
    public static class DisposableUtility
    {
        public static void AddTo(this IDisposable disposable, List<IDisposable> disposables)
        {
            if (disposable == null || disposables == null)
            {
                return;
            }

            disposables.Add(disposable);
        }
        public static void AddTo(this List<IDisposable> disposable, List<IDisposable> disposables)
        {
            if (disposable == null || disposables == null)
            {
                return;
            }

            disposables.AddRange(disposable);
        }
        
        public static void DisposeAndClear(this List<IDisposable> disposables)
        {
            foreach (var disposable in disposables)
            {
                disposable?.Dispose();
            }

            disposables.Clear();
        }
    }
}
