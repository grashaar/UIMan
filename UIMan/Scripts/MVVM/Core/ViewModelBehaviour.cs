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
        private readonly Dictionary<string, PropertyInfo> propertyCache = new Dictionary<string, PropertyInfo>();
        private readonly List<MemberInfo> notifiableMembers = new List<MemberInfo>();

        private readonly Dictionary<string, Action<object>> objectActions
            = new Dictionary<string, Action<object>>();

        private readonly Dictionary<string, Action<string, object>> stringObjectActions
            = new Dictionary<string, Action<string, object>>();

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
            if (!this.objectActions.TryGetValue(propertyName, out Action<object> actions))
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
        /// Notify the property which has change to all binder that has been subcribed with property name and value.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public virtual void NotifyModelPropertyChanged(string propertyName, object value)
        {
            if (!this.stringObjectActions.TryGetValue(propertyName, out Action<string, object> actions))
                return;

            try
            {
                actions?.Invoke(propertyName, value);
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
            NotifyPropertyChanged($"set_{propertyName}", value);
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

        private void CacheProperty(string propertyKey, string propertyName)
        {
            if (this.propertyCache.ContainsKey(propertyKey))
                return;

            this.propertyCache.Add(propertyKey, this.GetCachedType().GetProperty(propertyName));
        }

        /// <summary>
        /// Subcribe action to notify on property changed
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="updateAction"></param>
        public void SubscribeAction(string propertyName, Action<object> updateAction)
        {
            var propertyKey = "set_" + propertyName;

            if (this.objectActions.ContainsKey(propertyKey))
            {
                this.objectActions[propertyKey] += updateAction;
            }
            else
            {
                this.objectActions.Add(propertyKey, updateAction);
            }

            CacheProperty(propertyKey, propertyName);
        }

        /// <summary>
        /// Unsubcribe action from notify on property changed
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="updateAction"></param>
        public void UnsubscribeAction(string propertyName, Action<object> updateAction)
        {
            var propertyKey = "set_" + propertyName;

            if (this.objectActions.ContainsKey(propertyKey))
            {
                this.objectActions[propertyKey] -= updateAction;
            }
        }

        /// <summary>
        /// Subcribe action to notify on property changed
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="updateAction"></param>
        public void SubscribeAction(string propertyName, Action<string, object> updateAction)
        {
            var propertyKey = "set_" + propertyName;

            if (this.stringObjectActions.ContainsKey(propertyKey))
            {
                this.stringObjectActions[propertyKey] += updateAction;
            }
            else
            {
                    this.stringObjectActions.Add(propertyKey, updateAction);
            }

            CacheProperty(propertyKey, propertyName);
        }

        /// <summary>
        /// Unsubcribe action from notify on property changed
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="updateAction"></param>
        public void UnsubscribeAction(string propertyName, Action<string, object> updateAction)
        {
            var propertyKey = "set_" + propertyName;

            if (this.stringObjectActions.ContainsKey(propertyKey))
            {
                this.stringObjectActions[propertyKey] -= updateAction;
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
                switch (this.notifiableMembers[i])
                {
                    case FieldInfo field:
                    {
                        var value = field.GetValue(obj);
                        NotifyPropertyChanged("set_" + this.notifiableMembers[i].Name, value);
                        break;
                    }

                    case PropertyInfo property:
                    {
                        var value = property.GetValue(obj, null);
                        NotifyPropertyChanged("set_" + this.notifiableMembers[i].Name, value);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Notifies the model change.
        /// </summary>
        /// <param name="obj">Object.</param>
        public void NotifyModelPropertyChange(string propertyName, object obj)
        {
            if (this.notifiableMembers == null)
                return;

            var index = this.notifiableMembers.FindIndex(x => string.Equals(x.Name, propertyName));

            if (index < 0)
                return;

            var member = this.notifiableMembers[index];

            if (member is FieldInfo field)
            {
                var value = field.GetValue(obj);
                NotifyModelPropertyChanged("set_" + member.Name, value);
            }
            else if (member is PropertyInfo property)
            {
                var value = property.GetValue(obj, null);
                NotifyModelPropertyChanged("set_" + member.Name, value);
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
        /// Notifies the model change with the changed value.
        /// </summary>
        /// <param name="value">Changed value.</param>
        public void NotifyModelPropertyChangedValue(string propertyName, object value)
        {
            if (this.notifiableMembers == null)
                return;

            var index = this.notifiableMembers.FindIndex(x => string.Equals(x.Name, propertyName));

            if (index < 0)
                return;

            var member = this.notifiableMembers[index];
            NotifyModelPropertyChanged("set_" + member.Name, value);
        }

        /// <summary>
        /// Subcripts the object action.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="onChange">On change.</param>
        public void CacheNotifiableMembers(object obj)
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
        public void CacheNotifiableMembers(PropertyInfo property)
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