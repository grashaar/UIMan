using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace UnuGames.MVVM
{
    public class ObservableModel : IObservable
    {
        private readonly Dictionary<string, Action<object>> actions = new Dictionary<string, Action<object>>();
        private readonly Dictionary<string, PropertyInfo> propertyCache = new Dictionary<string, PropertyInfo>();

        /// <summary>
        /// Invoke when any property has been changed.
        /// </summary>
        public event Action<ObservableModel> PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnuGames.ObservableModel"/> class.
        /// </summary>
        public static T New<T>() where T : ObservableModel
        {
            return (T)new ObservableModel();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnuGames.ObservableModel"/> class.
        /// </summary>
        /// <param name="instance">Instance.</param>
        public static T New<T>(T instance) where T : ObservableModel, new()
        {
            var other = new T();

            if (instance != null)
            {
                foreach (var kv in instance.actions)
                {
                    other.actions.Add(kv.Key, kv.Value);
                }

                foreach (var kv in instance.propertyCache)
                {
                    other.propertyCache.Add(kv.Key, other.GetType().GetProperty(kv.Value.Name));
                }
            }

            return other;
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
                UnuLogger.LogException(e);
            }
        }

        /// <summary>
        /// Raise the change event automatically without name and value,
        /// only use this function in property getter
        /// </summary>
        public void OnPropertyChanged()
        {
            DataContext.NotifyObjectChange(this);
        }

        /// <summary>
        /// Raise the change event automatically,
        /// only use this function in property getter
        /// </summary>
        public void OnPropertyChanged(string propertyName, object value)
        {
            this.PropertyChanged?.Invoke(this);

            if (this.actions.TryGetValue($"set_{propertyName}", out Action<object> actions))
            {
                try
                {
                    actions?.Invoke(value);
                }
                catch (Exception e)
                {
                    UnuLogger.LogException(e);
                }
            }

            DataContext.NotifyObjectChange(this, propertyName, value);
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
            if (this.propertyCache.TryGetValue(propertyName, out PropertyInfo property))
            {
                property.SetValue(this, value, null);
            }
        }
    }
}