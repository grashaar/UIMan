using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

namespace UnuGames.MVVM
{
    /// <summary>
    /// ViewModel behavior (part of MVVM pattern).
    /// </summary>
    public class ViewModelBehaviour : MonoBehaviour, IObservable
    {
        private readonly Dictionary<string, Action<object>> actions = new Dictionary<string, Action<object>>();
        private readonly Dictionary<string, PropertyInfo> propertyCache = new Dictionary<string, PropertyInfo>();
        private readonly List<MemberInfo> notifiableMembers = new List<MemberInfo>();

        /// <summary>
        /// Invoke when any property has been changed.
        /// </summary>
        public event Action<ViewModelBehaviour> PropertyChanged;

        private RectTransform rectTransform;

        public RectTransform RectTransform
        {
            get
            {
                if (this.rectTransform == null)
                    this.rectTransform = GetComponent<RectTransform>();
                return this.rectTransform;
            }
        }

        public Transform Transform
        {
            get
            {
                return this.transform;
            }
        }

        /// <summary>
        /// Notify the property which has change to all binder that has been subcribed with property name and value.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public virtual void NotifyPropertyChanged(string propertyName, object value)
        {
            if (!this.actions.TryGetValue(propertyName, out Action<object> actions))
                return;

            try
            {
                actions?.Invoke(value);
            }
            catch (Exception e)
            {
                UnuLogger.LogException(e, this);
            }
        }

        /// <summary>
        /// Raise the change event automatically without name and value,
        /// only use this function in property getter
        /// </summary>
        public void OnPropertyChanged()
        {
            var propertyName = GetCaller();
            if (this.propertyCache.TryGetValue(propertyName, out PropertyInfo property))
            {
                var newValue = property.GetValue(this, null);
                NotifyPropertyChanged(propertyName, newValue);
            }
        }

        /// <summary>
        /// Raise the change event automatically,
        /// only use this function in property getter
        /// </summary>
        public void OnPropertyChanged(string propertyName, object value)
        {
            this.PropertyChanged?.Invoke(this);
            NotifyPropertyChanged(propertyName, value);
        }

        /// <summary>
        /// Get the caller of current function
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private static string GetCaller(int level = 2)
        {
            var sf = new StackFrame(level);
            return sf.GetMethod().Name;
        }

        /// <summary>
        /// Subcribe action to notify on property changed
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="updateAction"></param>
        public void SubscribeAction(string propertyName, Action<object> updateAction)
        {
            var propertyKey = "set_" + propertyName;
            if (this.actions.ContainsKey(propertyKey))
            {
                this.actions[propertyKey] += updateAction;
            }
            else
            {
                this.actions.Add(propertyKey, updateAction);
                this.propertyCache.Add(propertyKey, this.GetCachedType().GetProperty(propertyName));
            }
        }

        /// <summary>
        /// Unsubcribe action from notify on property changed
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="updateAction"></param>
        public void UnsubscribeAction(string propertyName, Action<object> updateAction)
        {
            var propertyKey = "set_" + propertyName;
            if (this.actions.ContainsKey(propertyKey))
            {
                this.actions[propertyKey] -= updateAction;
            }
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        /// <param name="value">Value.</param>
        public void SetValue(string propertyName, object value)
        {
            if (this.propertyCache.TryGetValue("set_" + propertyName, out PropertyInfo property))
            {
                property.SetValue(this, value, null);
            }
        }

        /// <summary>
        /// Notifies the model change.
        /// </summary>
        /// <param name="obj">Object.</param>
        public void NotifyModelChange(object obj)
        {
            if (this.notifiableMembers == null)
                return;

            for (var i = 0; i < this.notifiableMembers.Count; i++)
            {
                object value;
                if (this.notifiableMembers[i] is FieldInfo)
                {
                    FieldInfo field = this.notifiableMembers[i].ToField();
                    value = field.GetValue(obj);
                }
                else
                {
                    PropertyInfo property = this.notifiableMembers[i].ToProperty();
                    value = property.GetValue(obj, null);
                }

                NotifyPropertyChanged("set_" + this.notifiableMembers[i].Name, value);
            }
        }

        /// <summary>
        /// Notifies the model change with the changed value.
        /// </summary>
        /// <param name="value">Changed value.</param>
        public void NotifyModelChangedValue(object value)
        {
            if (this.notifiableMembers == null)
                return;

            for (var i = 0; i < this.notifiableMembers.Count; i++)
            {
                NotifyPropertyChanged("set_" + this.notifiableMembers[i].Name, value);
            }
        }

        /// <summary>
        /// Subcripts the object action.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="onChange">On change.</param>
        public void SubcriptObjectAction(object obj)
        {
            MemberInfo[] members = obj.GetCachedType().GetMembers();
            for (var i = 0; i < members.Length; i++)
            {
                if (members[i] is FieldInfo || members[i] is PropertyInfo)
                {
                    this.notifiableMembers.Add(members[i]);
                }
            }
        }

        /// <summary>
        /// Subcripts the object action.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="onChange">On change.</param>
        public void SubcriptObjectAction(PropertyInfo property)
        {
            MemberInfo[] members = property.GetType().GetMembers();
            for (var i = 0; i < members.Length; i++)
            {
                if (members[i] is FieldInfo || members[i] is PropertyInfo)
                {
                    this.notifiableMembers.Add(members[i]);
                }
            }
        }

        /// <summary>
        /// Determines whether this instance is binding to the specified modelInstance.
        /// </summary>
        /// <returns><c>true</c> if this instance is binding to the specified modelInstance; otherwise, <c>false</c>.</returns>
        /// <param name="modelInstance">Model instance.</param>
        public PropertyInfo IsBindingTo(object modelInstance)
        {
            foreach (KeyValuePair<string, PropertyInfo> property in this.propertyCache)
            {
                if (property.Value != null)
                {
                    var propertyVal = property.Value.GetValue(this, null);
                    if (propertyVal != null && propertyVal.Equals(modelInstance))
                    {
                        return property.Value;
                    }
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Binding type.
    /// </summary>
    public enum ContextType
    {
        [Description("None")]
        None = 0,

        [Description("Mono Behaviour")]
        MonoBehaviour,

        [Description("Type Instance")]
        Property
    }
}