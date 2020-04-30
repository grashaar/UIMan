using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UnuGames.MVVM
{
    [DisallowMultipleComponent]
    public class SimpleObservableListBinder : BinderBase
    {
        [HideInInspector]
        public BindingField observableList = new BindingField("Data Source");

        public GameObject contentPrefab;
        public RectTransform contentRect;

        [Space]
        public bool usePoolRectToHide;
        public RectTransform poolRect;

        private MemberInfo sourceMember;
        private IObservaleCollection dataList;

        private readonly Queue<IModule> modulesPool = new Queue<IModule>();
        private readonly List<IModule> modules = new List<IModule>();

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.sourceMember = this.dataContext.viewModel.GetMemberInfo(this.observableList.member);

            switch (this.sourceMember)
            {
                case FieldInfo sourceField:
                    this.dataList = (IObservaleCollection)sourceField.GetValue(this.dataContext.viewModel);
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

            if (!string.IsNullOrEmpty(this.contentPrefab.scene.name))
                this.contentPrefab.SetActive(false);
        }

        private IModule GetModuleFromPool()
        {
            if (this.modulesPool.Count > 0)
            {
                var module = this.modulesPool.Dequeue();

                if (this.usePoolRectToHide)
                    module.ViewModel.RectTransform.SetParent(this.contentRect);
                else
                    module.ViewModel.gameObject.SetActive(true);

                return module;
            }

            var obj = Instantiate(this.contentPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            var vm = obj.GetComponent<ViewModelBehaviour>();
            vm.RectTransform.SetParent(this.contentRect, true);
            vm.RectTransform.localScale = Vector3.one;

            return vm as IModule;
        }

        private void PoolModule(IModule module)
        {
            if (this.usePoolRectToHide)
                module.ViewModel.RectTransform.SetParent(this.poolRect);
            else
                module.ViewModel.gameObject.SetActive(false);

            this.modulesPool.Enqueue(module);
        }

        private void HandleOnInsert(int index, object obj)
        {
            var module = GetModuleFromPool();

            if (module == null)
                return;

            this.modules.Insert(index, module);
            module.OriginalData = obj;
        }

        private void HandleOnClear()
        {
            for (var i = 0; i < this.modules.Count; i++)
            {
                PoolModule(this.modules[i]);
            }

            this.modules.Clear();
        }

        private void HandleOnRemove(object obj)
        {
            var indexToRemove = -1;

            for (var i = 0; i < this.modules.Count; i++)
            {
                if (this.modules[i].OriginalData == obj)
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
            var module = this.modules[index];
            this.modules.RemoveAt(index);
            PoolModule(module);
        }

        private void HandleOnAdd(object obj)
        {
            var module = GetModuleFromPool();

            if (module == null)
                return;

            this.modules.Add(module);
            module.OriginalData = obj;
        }

        private void HandleOnChange(int index, object obj)
        {
            this.modules[index].OriginalData = obj;
        }
    }
}