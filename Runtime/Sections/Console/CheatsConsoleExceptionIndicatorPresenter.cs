using System;
using UnityEngine;

namespace SMCheats
{
    internal sealed class CheatsConsoleExceptionIndicatorPresenter : IDisposable
    {
        private readonly CheatsConsoleService _consoleService;
        private readonly CheatsConsoleExceptionIndicatorView _view;
        private readonly IDisposable _destroyDisposable;

        public CheatsConsoleExceptionIndicatorPresenter(
            ICheatsUiElementFactory uiFactory,
            Transform parent,
            CheatsConsoleService consoleService)
        {
            _consoleService = consoleService ?? throw new ArgumentNullException(nameof(consoleService));
            _view = uiFactory.InstantiatePrefabView<CheatsConsoleExceptionIndicatorView>(
                CheatsPrefabResourcePaths.ConsoleExceptionIndicator,
                parent);
            _destroyDisposable = _view.gameObject.CreateDestroyDisposable();
            _consoleService.ExceptionReceived += _view.Flash;
        }

        public void Dispose()
        {
            _consoleService.ExceptionReceived -= _view.Flash;
            _destroyDisposable.Dispose();
        }
    }
}
