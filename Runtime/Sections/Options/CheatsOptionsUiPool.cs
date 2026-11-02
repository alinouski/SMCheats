using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SMCheats
{
    internal sealed class CheatsOptionsUiPool
    {
        private readonly ICheatsUiElementFactory _uiFactory;
        private readonly CheatsOptionsControlBinder _controlBinder;
        private readonly string _categoryResourcePath;
        private readonly Dictionary<string, CheatsOptionsCategoryView> _categories = new(StringComparer.Ordinal);
        private readonly Dictionary<string, CheatsOptionRowBinding> _mainRows = new(StringComparer.Ordinal);
        private readonly Dictionary<string, CheatsOptionRowBinding> _pinnedRows = new(StringComparer.Ordinal);
        private readonly HashSet<string> _activeCategories = new(StringComparer.Ordinal);
        private readonly HashSet<string> _activeMainRows = new(StringComparer.Ordinal);
        private readonly HashSet<string> _activePinnedRows = new(StringComparer.Ordinal);
        private Text _emptyText;

        public CheatsOptionsUiPool(
            ICheatsUiElementFactory uiFactory,
            CheatsOptionsControlBinder controlBinder,
            string categoryResourcePath)
        {
            _uiFactory = uiFactory;
            _controlBinder = controlBinder;
            _categoryResourcePath = categoryResourcePath;
        }

        public void BeginRefresh()
        {
            _activeCategories.Clear();
            _activeMainRows.Clear();
            _activePinnedRows.Clear();
        }

        public CheatsOptionsCategoryView GetCategory(string category, Transform parent)
        {
            var key = category ?? string.Empty;
            _activeCategories.Add(key);

            if (!_categories.TryGetValue(key, out var view))
            {
                view = _uiFactory.InstantiatePrefabView<CheatsOptionsCategoryView>(_categoryResourcePath, parent);
                _categories[key] = view;
            }

            Activate(view.transform, parent);
            return view;
        }

        public CheatsOptionRowBinding GetOptionRow(CheatOptionDefinition option, bool pinnedContext, Transform parent)
        {
            var key = CheatsTabUiUtility.GetOptionKey(option);
            var rows = pinnedContext ? _pinnedRows : _mainRows;
            var activeRows = pinnedContext ? _activePinnedRows : _activeMainRows;
            activeRows.Add(key);

            var kind = _controlBinder.ResolveKind(option);
            if (rows.TryGetValue(key, out var row) && row.Kind == kind)
            {
                row.Activate(parent);
                return row;
            }

            if (row != null)
            {
                Object.Destroy(row.Root);
            }

            row = _controlBinder.CreateOptionRow(option, kind, parent);
            rows[key] = row;
            row.Activate(parent);
            return row;
        }

        public void SetEmptyVisible(Transform parent, bool visible)
        {
            if (visible && _emptyText == null)
            {
                _emptyText = _uiFactory.CreateText("Empty", parent, "No cheat options registered.", 16, TextAnchor.MiddleLeft);
            }

            if (_emptyText == null)
            {
                return;
            }

            Activate(_emptyText.transform, parent);
            _emptyText.gameObject.SetActive(visible);
        }

        public void EndRefresh()
        {
            DeactivateUnusedCategories();
            DeactivateUnusedRows(_mainRows, _activeMainRows);
            DeactivateUnusedRows(_pinnedRows, _activePinnedRows);
        }

        public void Clear()
        {
            DestroyRows(_mainRows);
            DestroyRows(_pinnedRows);
            DestroyViews(_categories);

            if (_emptyText != null)
            {
                Object.Destroy(_emptyText.gameObject);
            }

            _emptyText = null;
            _activeCategories.Clear();
            _activeMainRows.Clear();
            _activePinnedRows.Clear();
        }

        private void DeactivateUnusedCategories()
        {
            foreach (var pair in _categories)
            {
                pair.Value.gameObject.SetActive(_activeCategories.Contains(pair.Key));
            }
        }

        private static void DeactivateUnusedRows(
            Dictionary<string, CheatsOptionRowBinding> rows,
            HashSet<string> activeRows)
        {
            foreach (var pair in rows)
            {
                if (!activeRows.Contains(pair.Key))
                {
                    pair.Value.Deactivate();
                }
            }
        }

        private static void DestroyViews(Dictionary<string, CheatsOptionsCategoryView> views)
        {
            foreach (var pair in views)
            {
                Object.Destroy(pair.Value.gameObject);
            }

            views.Clear();
        }

        private static void DestroyRows(Dictionary<string, CheatsOptionRowBinding> rows)
        {
            foreach (var pair in rows)
            {
                Object.Destroy(pair.Value.Root);
            }

            rows.Clear();
        }

        private static void Activate(Transform viewTransform, Transform parent)
        {
            if (parent != null && viewTransform.parent != parent)
            {
                viewTransform.SetParent(parent, false);
            }

            viewTransform.SetAsLastSibling();
            viewTransform.gameObject.SetActive(true);
        }
    }
}
