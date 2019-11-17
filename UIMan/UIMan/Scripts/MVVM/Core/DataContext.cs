using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UnuGames.MVVM
{
    /// <summary>
    /// Data context.
    /// </summary>
    public class DataContext : MonoBehaviour
    {
        #region DataContext Factory

        private readonly static List<DataContext> contextsList = new List<DataContext>();

        public static void NotifyObjectChange(object modelInstance)
        {
            for (var i = 0; i < contextsList.Count; i++)
            {
                DataContext context = contextsList[i];
                if (context.model != null && context.model is ObservableModel)
                {
                    PropertyInfo propertyInfo = context.viewModel.IsBindingTo(modelInstance);
                    if (propertyInfo != null)
                    {
                        context.viewModel.NotifyModelChange(modelInstance);
                    }
                }
            }
        }

        #endregion DataContext Factory

        #region Instance

        public ContextType type;
        public ViewModelBehaviour viewModel;
        public object model;

        public string propertyName;
        private PropertyInfo propertyInfo;

        public PropertyInfo PropertyInfo
        {
            get
            {
                return this.propertyInfo;
            }
        }

        public void Clear()
        {
            this.viewModel = null;
            this.propertyName = null;
            this.propertyInfo = null;
        }

        private void Awake()
        {
            if (!contextsList.Contains(this))
                contextsList.Add(this);

            Init();
            RegisterBindingMessage(false);
        }

        // Subscript for property change event
        public void Init()
        {
            GetPropertyInfo();
            if (this.propertyInfo != null)
            {
                this.model = this.propertyInfo.GetValue(this.viewModel, null);
                if (this.model == null && this.type == ContextType.PROPERTY)
                    this.model = ReflectUtils.GetCachedTypeInstance(this.propertyInfo.PropertyType);
                if (this.model != null)
                {
                    this.viewModel.SubcriptObjectAction(this.model);
                    this.viewModel.SubscribeAction(this.propertyName, this.viewModel.NotifyModelChange);
                }
            }
        }

        // Register binding message for child binders
        private void RegisterBindingMessage(bool forceReinit = false)
        {
            BinderBase[] binders = GetComponentsInChildren<BinderBase>(true);
            for (var i = 0; i < binders.Length; i++)
            {
                BinderBase binder = binders[i];
                if (binder.dataContext == this)
                {
                    binder.Initialize(forceReinit);
                }
            }
        }

        public PropertyInfo GetPropertyInfo()
        {
#if !UNITY_EDITOR
			if(propertyInfo == null)
				propertyInfo = viewModel.GetCachedType ().GetProperty (propertyName);
#else
            this.propertyInfo = this.viewModel.GetCachedType().GetProperty(this.propertyName);
#endif
            return this.propertyInfo;
        }
    }

    #endregion Instance
}