using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UnuGames.MVVM
{
    /// <summary>
    /// Base class for all binders.
    /// </summary>
    [Serializable]
    [ExecuteInEditMode]
    public abstract class BinderBase : MonoBehaviour
    {
        protected bool isInitialize = false;

        private Type type;

        public Type Type
        {
            get
            {
                if (this.type == null)
                    this.type = GetType();
                return this.type;
            }
        }

        [HideInInspector]
        public DataContext dataContext;

        /// <summary>
        /// Find the Data Context for this binder
        /// </summary>
        /// <returns></returns>
        public DataContext FindDataContext()
        {
            if (this.dataContext == null)
            {
                this.dataContext = GetComponent<DataContext>();
                if (this.dataContext == null)
                    this.dataContext = GetComponentInParent<DataContext>();
            }

            return this.dataContext;
        }

        /// <summary>
        /// Get the view model attached to current data context
        /// </summary>
        /// <returns></returns>
        public IObservable GetViewModel()
        {
            if (this.dataContext == null)
                FindDataContext();

            return this.dataContext.viewModel;
        }

        public string[] GetMembers(params MemberTypes[] memberTypes)
        {
            if (this.dataContext == null)
                FindDataContext();

            if (this.dataContext.type == ContextType.MonoBehaviour)
            {
                return this.dataContext.viewModel.GetAllMembers(memberTypes);
            }
            else if (this.dataContext.type == ContextType.Property)
            {
                return this.dataContext.GetPropertyInfo().GetAllMembers(memberTypes);
            }

            return new string[0];
        }

        public MemberInfo GetMemberInfo(string memberName, params MemberTypes[] memberTypes)
        {
            if (this.dataContext == null)
                FindDataContext();

            MemberInfo[] infos = null;
            if (this.dataContext.type == ContextType.MonoBehaviour)
            {
                infos = this.dataContext.viewModel.GetAllMembersInfo(memberTypes);
            }
            else if (this.dataContext.type == ContextType.Property)
            {
                infos = this.dataContext.GetPropertyInfo().GetAllMembersInfo(memberTypes);
            }

            for (var i = 0; i < infos.Length; i++)
            {
                if (infos[i].Name == memberName)
                    return infos[i];
            }

            return null;
        }

        /// <summary>
        /// All binder must implement this method to initialize the binder's instance.
        /// </summary>
        public abstract void Initialize(bool forceInit = false);

        /// <summary>
        /// All binder must implement this method to unsubsribe onchanged event when object is disable
        /// </summary>
        //public  abstract void OnDisable ();

        /// <summary>
        /// Subscribe the on changed event
        /// </summary>
        /// <param name="field">_binding info.</param>
        /// <param name="onChanged">On changed.</param>
        protected void SubscribeOnChangedEvent(BindingField field, Action<object> onChanged)
        {
            field.UpdateAction += onChanged;
            RegisterViewModel(field.member, onChanged);
        }

        /// <summary>
        /// Unsubscribe the on changed event
        /// </summary>
        /// <param name="field"></param>
        /// <param name="onChanged"></param>
        protected void UnsubscribeOnChangedEvent(BindingField field, Action<object> onChanged)
        {
            field.UpdateAction -= onChanged;
            UnregisterViewModel(field.member, onChanged);
        }

        /// <summary>
        /// Register the view model.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        private void RegisterViewModel(string propertyName, Action<object> updateAction)
        {
            if (this.dataContext != null && !string.IsNullOrEmpty(propertyName))
            {
                if (this.dataContext.model != null && this.dataContext.model is ObservableModel)
                {
                    ((ObservableModel)this.dataContext.model).SubscribeAction(propertyName, updateAction);
                    this.dataContext.viewModel.SubscribeAction(propertyName, updateAction);
                }
                else
                {
                    this.dataContext.viewModel.SubscribeAction(propertyName, updateAction);
                }
            }
        }

        /// <summary>
        /// Un register the view model.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        private void UnregisterViewModel(string propertyName, Action<object> updateAction)
        {
            if (this.dataContext != null && !string.IsNullOrEmpty(propertyName) && this.dataContext.viewModel != null)
            {
                this.dataContext.viewModel.UnsubscribeAction(propertyName, updateAction);
            }
        }

        protected BindingField[] fields;

        /// <summary>
        /// Get and cache all binding fields of this binder
        /// </summary>
        /// <returns></returns>
        public BindingField[] GetBindingFields()
        {
#if !UNITY_EDITOR
            if(fields != null)
                return  fields;
#endif

            var listField = new List<BindingField>();
            MemberInfo[] members = this.Type.GetMembers();
            for (var i = 0; i < members.Length; i++)
            {
                MemberInfo memberInfo = members[i];
                if (memberInfo.MemberType == MemberTypes.Field)
                {
                    var fieldInfo = memberInfo as FieldInfo;
                    if (fieldInfo.FieldType == typeof(BindingField))
                    {
                        listField.Add(fieldInfo.GetValue(this) as BindingField);
                    }
                }
            }

            this.fields = listField.ToArray();
            return this.fields;
        }

        protected bool CheckInitialize(bool forceInitialize)
        {
            if (!Application.isPlaying)
                return false;
            if (this.isInitialize)
                return forceInitialize;
            this.isInitialize = true;
            return true;
        }

        protected void SetValue(string memberName, object value)
        {
            if (!this.enabled)
                return;

            if (string.IsNullOrEmpty(memberName))
                return;

            GetViewModel().SetValue(memberName, value);
        }
    }
}