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

        public static void NotifyObjectChange(object modelInstance, string propertyName, object value)
        {
            for (var i = 0; i < contextsList.Count; i++)
            {
                DataContext context = contextsList[i];

                if (context.model != null && context.model is ObservableModel)
                {
                    PropertyInfo propertyInfo = context.viewModel.IsBindingTo(modelInstance);

                    if (propertyInfo != null && propertyInfo.Name.Equals(propertyName))
                    {
                        context.viewModel.NotifyModelChangedValue(value);
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

        public PropertyInfo PropertyInfo { get; private set; }

        public void Clear()
        {
            this.viewModel = null;
            this.propertyName = null;
            this.PropertyInfo = null;
        }

        private void Awake()
        {
            if (!contextsList.Contains(this))
                contextsList.Add(this);

            Initialize();
            RegisterBindingMessage(false);
        }

        // Subscript for property change event
        public void Initialize()
        {
            GetPropertyInfo();

            if (this.PropertyInfo != null)
            {
                this.model = this.PropertyInfo.GetValue(this.viewModel, null);

                if (this.model == null && this.type == ContextType.Property)
                    this.model = UIManReflection.GetCachedTypeInstance(this.PropertyInfo.PropertyType);

                if (this.model != null)
                {
                    this.viewModel.CacheNotifiableMembers(this.model);
                    this.viewModel.SubscribeAction(this.propertyName, this.viewModel.NotifyModelPropertyChange);
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
#if UNITY_EDITOR
            this.PropertyInfo = this.viewModel.GetCachedType().GetProperty(this.propertyName);
#else
            if (this.PropertyInfo == null)
				this.PropertyInfo = this.viewModel.GetCachedType().GetProperty(this.propertyName);
#endif
            return this.PropertyInfo;
        }
    }

    #endregion Instance
}