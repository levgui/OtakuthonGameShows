using System.ComponentModel;
using System.Linq.Expressions;

namespace Classes.Common
{
    //Example
    //public string Title
    //{
    //    get { return GetValue(() => Title); }
    //    set { SetValue(() => this.Title, value); }
    //}

    public abstract class BindableBase : INotifyPropertyChanged
    {
        private readonly Dictionary<string, object> values = new Dictionary<string, object>();

        protected T GetValue<T>(Expression<Func<T>> propertySelector)
        {
            string propertyName = GetPropertyName(propertySelector);

            return GetValue<T>(propertyName);
        }

        protected T GetValue<T>(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Invalid property name", propertyName);
            }

            object value;

            if (!values.TryGetValue(propertyName, out value))
            {
                value = default(T);

                values.Add(propertyName, value);
            }

            return (T)value;
        }

        protected bool SetValue<T>(Expression<Func<T>> propertySelector, T value)
        {
            string propertyName = GetPropertyName(propertySelector);

            return SetValue<T>(propertyName, value);
        }

        protected bool SetValue<T>(string propertyName, T value)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Invalid property name", propertyName);
            }

            if (!values.TryGetValue(propertyName, out var oldValue))
            {
                if (oldValue == null || value == null || !oldValue.Equals(value))
                {
                    values[propertyName] = value;
                    this.NotifyPropertyChanged(propertyName);
                    return true;
                }
            }
            else
            {
                values[propertyName] = value;
                this.NotifyPropertyChanged(propertyName);
                return false;
            }

            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;

            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        protected void NotifyPropertyChanged<T>(Expression<Func<T>> propertySelector)
        {
            var propertyChanged = PropertyChanged;

            if (propertyChanged != null)
            {
                string propertyName = GetPropertyName(propertySelector);

                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        private string GetPropertyName(LambdaExpression expression)
        {
            var memberExpression = expression.Body as MemberExpression;

            if (memberExpression == null)
            {
                throw new InvalidOperationException();
            }

            return memberExpression.Member.Name;
        }

        private object GetValue(string propertyName)
        {
            if (!values.TryGetValue(propertyName, out object value))
            {
                var propertyDescriptor = TypeDescriptor.GetProperties(GetType()).Find(propertyName, false);

                if (propertyDescriptor == null)
                {
                    throw new ArgumentException("Invalid property name", propertyName);
                }

                value = propertyDescriptor.GetValue(this);
                values.Add(propertyName, value);

            }

            return value;

        }
    }
}
