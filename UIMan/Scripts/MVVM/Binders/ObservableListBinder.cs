using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace UnuGames.MVVM
{
    [RequireComponent(typeof(ScrollRect))]
    [DisallowMultipleComponent]
    public class ObservableListBinder : BinderBase
    {
        private readonly struct Cell
        {
            public readonly int row;
            public readonly int column;

            public Cell(int row, int column)
            {
                this.row = row;
                this.column = column;
            }

            public override string ToString()
            {
                return string.Format("({0},{1})", this.column, this.row);
            }

            public static bool operator !=(in Cell c1, in Cell c2)
            {
                return (c1.column != c2.column || c1.row != c2.row);
            }

            public static bool operator ==(in Cell c1, in Cell c2)
            {
                return (c1.column == c2.column && c1.row == c2.row);
            }

            public override bool Equals(object obj)
            {
                if (obj is Cell other)
                    return this == other;

                return false;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hash = (int)2166136261;
                    hash = (hash * 16777619) ^ this.column;
                    hash = (hash * 16777619) ^ this.row;
                    return hash;
                }
            }
        }

        private const int BOUND_BUFFERS = 3;

        public GameObject contentPrefab;
        public RectTransform contentRect;
        public RectTransform viewPort;

        [Space]
        public float contentWidth;
        public float contentHeight;
        public Vector2 contentSpacing;
        public Vector2 padding;
        public Vector2 grouping;

        [Space]
        public bool usePoolRectToHide;
        public RectTransform poolRect;

        [HideInInspector]
        public BindingField observableList = new BindingField("Data Source");

        private readonly static Vector3 _hidePosition = new Vector3(999999999, 999999999, 0);

        private readonly Queue<IModule> modulesPool = new Queue<IModule>();
        private readonly List<IModule> listModules = new List<IModule>();
        private readonly Dictionary<Cell, IModule> activeCells = new Dictionary<Cell, IModule>();
        private readonly Dictionary<Cell, object> dataDict = new Dictionary<Cell, object>();
        private readonly List<object> orgDataList = new List<object>();

        [HideInInspector]
        public Rect viewPortRect;
        private ScrollRect scrollRect;
        private int poolSize = 10;
        private IObservaleCollection dataList;
        private MemberInfo sourceMember;

#if UNITY_EDITOR

        private void Update()
        {
            if (this.viewPort != null)
                this.viewPortRect = this.viewPort.rect;
        }

#endif

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.scrollRect = GetComponent<ScrollRect>();

            if (this.grouping == Vector2.zero)
                this.grouping = Vector2.one;

            RectTransform contentItemRect = this.contentPrefab.GetComponent<RectTransform>();

            if (this.contentWidth == 0)
                this.contentWidth = contentItemRect.sizeDelta.x;

            if (this.contentHeight == 0)
                this.contentHeight = contentItemRect.sizeDelta.y;

            if (this.contentRect == null)
                this.contentRect = this.scrollRect.content;

            if (this.viewPort == null)
                this.viewPort = this.scrollRect.viewport;

            InitPool();

            this.sourceMember = this.dataContext.viewModel.GetMemberInfo(this.observableList.member);

            switch (this.sourceMember)
            {
                case FieldInfo fieldInfo:
                    this.dataList = (IObservaleCollection)fieldInfo.GetValue(this.dataContext.viewModel);
                    break;

                case PropertyInfo sourceProperty:
                    this.dataList = (IObservaleCollection)sourceProperty.GetValue(this.dataContext.viewModel, null);
                    break;
            }

            if (this.dataList != null)
            {
                this.dataList.OnAddObject += HandleOnAdd;
                this.dataList.OnRemoveObject += HandleOnRemove;
                this.dataList.OnRemoveAt += HandleOnRemoveAt;
                this.dataList.OnInsertObject += HandleOnInsert;
                this.dataList.OnClearObjects += HandleOnClear;
                this.dataList.OnChangeObject += HandleOnChange;
            }

            this.scrollRect.onValueChanged.AddListener(OnScroll);

            if (!string.IsNullOrEmpty(this.contentPrefab.scene.name))
                this.contentPrefab.SetActive(false);
        }

        #region Pooling

        private void InitPool()
        {
            if (this.scrollRect.horizontal)
            {
                var columnWidth = this.contentWidth + this.contentSpacing.x;
                var column = Mathf.RoundToInt(Mathf.Abs(this.viewPortRect.width) / columnWidth);
                this.poolSize = (column + 2 * BOUND_BUFFERS) * (int)this.grouping.y;
            }
            else
            {
                var rowHeight = this.contentHeight + this.contentSpacing.y;
                var row = Mathf.RoundToInt(Mathf.Abs(this.viewPortRect.height) / rowHeight);
                this.poolSize = (row + 2 * BOUND_BUFFERS) * (int)this.grouping.x;
            }

            for (var i = 0; i < this.poolSize; i++)
            {
                var position = this.usePoolRectToHide ? Vector3.zero : _hidePosition;

                var obj = Instantiate(this.contentPrefab, position, Quaternion.identity) as GameObject;
                ViewModelBehaviour vm = obj.GetComponent<ViewModelBehaviour>();
                vm.RectTransform.SetParent(this.contentRect, true);
                vm.RectTransform.localScale = Vector3.one;

                PoolModule((IModule)vm);
            }
        }

        private IModule GetModuleFromPool()
        {
            if (this.modulesPool.Count <= 0)
                return null;

            var module = this.modulesPool.Dequeue();

            if (this.usePoolRectToHide)
                module.ViewModel.RectTransform.SetParent(this.contentRect);

            return module;
        }

        private void PoolModule(IModule module)
        {
            if (this.usePoolRectToHide)
                module.ViewModel.RectTransform.SetParent(this.poolRect);
            else
                module.ViewModel.transform.position = _hidePosition;

            this.modulesPool.Enqueue(module);
        }

        #endregion Pooling

        private void OnScroll(Vector2 velocity)
        {
            Vector2 rectBounds = GetScrollRectBounds();

            // Check for hidden cell and push to pool
            var releaseCells = new List<Cell>();

            foreach (var cell in this.activeCells)
            {
                if (!IsVisible(cell.Key, rectBounds))
                {
                    PoolModule(cell.Value);
                    this.listModules.Remove(cell.Value);
                    releaseCells.Add(cell.Key);
                }
            }

            // Remove hidden cell from active list
            for (var i = 0; i < releaseCells.Count; i++)
            {
                this.activeCells.Remove(releaseCells[i]);
            }

            // Check and get cell from pool to fill blank cell
            var colRange = new Vector2(rectBounds.x, rectBounds.y);
            var rowRange = new Vector2(0, this.grouping.y);

            if (this.scrollRect.vertical)
            {
                colRange = new Vector2(0, this.grouping.x);
                rowRange = new Vector2(rectBounds.x, rectBounds.y);
            }

            for (var col = (int)colRange.x; col < colRange.y; col++)
            {
                for (var row = (int)rowRange.x; row < rowRange.y; row++)
                {
                    if (col < 0 || row < 0)
                        continue;

                    var cell = new Cell(row, col);

                    if (!this.activeCells.ContainsKey(cell) && IsVisible(cell, rectBounds) && this.dataDict.ContainsKey(cell))
                    {
                        IModule module = GetModuleFromPool();
                        if (module != null)
                        {
                            module.ViewModel.RectTransform.anchoredPosition = GetPositionByCell(cell);
                            if (this.activeCells.ContainsKey(cell))
                                this.activeCells[cell] = module;
                            else
                                this.activeCells.Add(cell, module);
                            this.listModules.Add(module);
                            module.OriginalData = this.dataDict[cell];
                        }
                    }
                }
            }
        }

        private void RecalculateBounds()
        {
            var pageCount = Mathf.CeilToInt(this.orgDataList.Count / (this.grouping.x * this.grouping.y));

            if (this.scrollRect.horizontal)
            {
                this.contentRect.sizeDelta = new Vector2(
                    this.contentWidth * pageCount * this.grouping.x + this.contentSpacing.x * pageCount * this.grouping.x,
                    this.contentHeight * this.grouping.y + this.contentSpacing.y * this.grouping.y
                );
            }
            else if (this.scrollRect.vertical)
            {
                this.contentRect.sizeDelta = new Vector2(
                    this.contentWidth * this.grouping.x + this.contentSpacing.x * this.grouping.x,
                    this.contentHeight * pageCount * this.grouping.y + this.contentSpacing.y * pageCount * this.grouping.y
                );
            }
        }

        private Vector2 GetScrollRectBounds()
        {
            if (this.scrollRect.horizontal)
            {
                var rectPos = this.contentRect.anchoredPosition.x;
                var columnWidth = this.contentWidth + this.contentSpacing.x;
                var minColumn = Mathf.FloorToInt(Mathf.Abs(rectPos) / columnWidth);
                var maxColumn = minColumn + Mathf.RoundToInt(this.viewPortRect.width / columnWidth);

                return new Vector2(minColumn - BOUND_BUFFERS, maxColumn + BOUND_BUFFERS);
            }
            else
            {
                var rectPos = this.contentRect.anchoredPosition.y;
                var rowHeight = this.contentHeight + this.contentSpacing.y;
                var minRow = Mathf.FloorToInt(Mathf.Abs(rectPos) / rowHeight);
                var maxRow = minRow + Mathf.RoundToInt(this.viewPortRect.height / rowHeight);

                return new Vector2(minRow - BOUND_BUFFERS, maxRow + BOUND_BUFFERS);
            }
        }

        private Cell GetCellByIndex(int targetIndex)
        {
            Cell cell = default;

            var index = 0;
            var pageCount = Mathf.CeilToInt(this.orgDataList.Count / (this.grouping.x * this.grouping.y));

            for (var page = 0; page < pageCount; page++)
            {
                for (var row = 0; row < this.grouping.y; row++)
                {
                    for (var col = 0; col < this.grouping.x; col++)
                    {
                        if (index == targetIndex)
                        {
                            if (this.scrollRect.horizontal)
                            {
                                cell = new Cell(row, page * (int)this.grouping.x + col);
                            }
                            else if (this.scrollRect.vertical)
                            {
                                cell = new Cell(page * (int)this.grouping.y + row, col);
                            }

                            return cell;
                        }

                        index++;
                    }
                }
            }

            return cell;
        }

        private Vector2 GetPositionByCell(in Cell cell)
        {
            Vector2 position = Vector2.zero;
            position.x = this.contentWidth * cell.column + cell.column * this.contentSpacing.x + this.padding.x;
            position.y = -this.contentHeight * cell.row - cell.row * this.contentSpacing.y - this.padding.y;

            return position;
        }

        private bool IsVisible(in Cell cell, Vector2? rectBounds = null)
        {
            if (!rectBounds.HasValue)
                rectBounds = GetScrollRectBounds();

            if (this.scrollRect.horizontal)
            {
                return cell.column > rectBounds.Value.x && cell.column < rectBounds.Value.y;
            }
            else
            {
                return cell.row > rectBounds.Value.x && cell.row < rectBounds.Value.y;
            }
        }

        private void RefreshDataDict(int index)
        {
            for (var i = index; i < this.orgDataList.Count; i++)
            {
                var cell = GetCellByIndex(i);

                if (this.dataDict.ContainsKey(cell))
                    this.dataDict[cell] = this.orgDataList[i];
                else
                    this.dataDict.Add(cell, this.orgDataList[i]);

                if (this.activeCells.ContainsKey(cell))
                    this.activeCells[cell].OriginalData = this.orgDataList[i];
            }
        }

        private void HandleOnInsert(int index, object obj)
        {
            this.orgDataList.Insert(index, obj);
            RecalculateBounds();
            RefreshDataDict(index);
            OnScroll(Vector2.zero);
        }

        private void HandleOnClear()
        {
            foreach (var cell in this.activeCells)
            {
                PoolModule(cell.Value);
            }

            this.listModules.Clear();
            this.dataDict.Clear();
            this.activeCells.Clear();
            this.orgDataList.Clear();
        }

        private void HandleOnRemove(object obj)
        {
            var indexToRemove = 0;

            for (var i = 0; i < this.listModules.Count; i++)
            {
                if (this.listModules[i].OriginalData == obj)
                {
                    indexToRemove = i;
                    break;
                }
            }

            if (indexToRemove >= 0)
            {
                HandleOnRemoveAt(indexToRemove);
            }
        }

        private void HandleOnRemoveAt(int index)
        {
            var cell = GetCellByIndex(this.orgDataList.Count - 1);

            if (this.activeCells.ContainsKey(cell))
            {
                PoolModule((IModule)this.activeCells[cell].ViewModel);

                if (!this.usePoolRectToHide)
                    this.activeCells[cell].ViewModel.RectTransform.anchoredPosition = _hidePosition;
            }

            this.listModules.RemoveAt(index);
            this.dataDict.Remove(cell);
            this.orgDataList.RemoveAt(index);

            RecalculateBounds();
            RefreshDataDict(index);
            OnScroll(Vector2.zero);
        }

        private void HandleOnAdd(object obj)
        {
            this.orgDataList.Add(obj);
            RecalculateBounds();

            var cell = GetCellByIndex(this.orgDataList.Count - 1);

            if (IsVisible(cell))
            {
                IModule module = GetModuleFromPool();

                if (module != null)
                {
                    this.listModules.Add(module);
                    module.OriginalData = obj;
                    module.ViewModel.RectTransform.anchoredPosition = GetPositionByCell(cell);

                    if (this.activeCells.ContainsKey(cell))
                        this.activeCells[cell] = module;
                    else
                        this.activeCells.Add(cell, module);
                }
            }

            this.dataDict.Add(cell, obj);
        }

        private void HandleOnChange(int index, object obj)
        {
            this.listModules[index].OriginalData = obj;
            this.orgDataList[index] = obj;

            var cell = GetCellByIndex(index);
            this.dataDict[cell] = obj;
        }

        public bool IsActive(GameObject item)
        {
            IModule module = item.GetComponent<IModule>();

            if (module != null)
            {
                foreach (var cell in this.activeCells)
                {
                    if (cell.Value == module)
                        return true;
                }
            }

            return false;
        }

        public ViewModelBehaviour GetItem(int index)
        {
            Cell cell = GetCellByIndex(index);

            if (this.activeCells.ContainsKey(cell))
                return this.activeCells[cell].ViewModel;

            return null;
        }

        public ViewModelBehaviour GetItem(int row, int column)
        {
            var cell = new Cell(row, column);

            if (this.activeCells.ContainsKey(cell))
                return this.activeCells[cell].ViewModel;

            return null;
        }
    }
}