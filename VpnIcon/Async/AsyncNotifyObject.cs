using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace Letter.Despatch.Async
{
    public abstract class AsyncNotifyObject : INotifyPropertyChanged, IDisposable
    {
        public AsyncNotifyObject()
        {
            this.Dispatcher = Dispatcher.CurrentDispatcher;
        }

        public AsyncNotifyObject(Dispatcher current)
        {
            this.Dispatcher = current;
        }

        protected Dispatcher Dispatcher { get; set; }

        private static Dictionary<string, List<string>> _DependencyMap = new Dictionary<string, List<string>>();

        protected void AddPropertyMap(string sourceProperty, params string[] dependentProperties)
        {
            if (!_DependencyMap.ContainsKey(sourceProperty))
                _DependencyMap.Add(sourceProperty, new List<string>());

            var source = _DependencyMap[sourceProperty].Union(dependentProperties).ToList();
            _DependencyMap[sourceProperty] = source;
            for (int index = 0; index < source.Count; index++)
            {
                string dependentProperty = source[index];
                List<string> dependent = null;
                if (_DependencyMap.ContainsKey(dependentProperty))
                    dependent = _DependencyMap[dependentProperty];

                if (dependent == null || !dependent.Contains(sourceProperty))
                    AddPropertyMap(dependentProperty, sourceProperty);
            }
        }

        #region · INotify events ·
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;
        #endregion · INotify events ·

        #region · INotify methods ·
        protected virtual void OnPropertyChanging(string propertyName)
        {
            if (Dispatcher.CheckAccess())
            {
                //if (Debugger.IsAttached)
                //    Debug.Assert(PropertyChanging != null, "PropertyChanging is null for " + propertyName + "!"); 
                if (PropertyChanging != null)
                    PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
            else
                Dispatcher.BeginInvoke(new Action(() => { OnPropertyChanging(propertyName); }));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (Dispatcher.CheckAccess())
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                    if (_DependencyMap.ContainsKey(propertyName))
                    {
                        foreach (string p in _DependencyMap[propertyName])
                        {
                            PropertyChanged(this, new PropertyChangedEventArgs(p));
                        }
                    }

                    CommandManager.InvalidateRequerySuggested();
                }
            }
            else
                Dispatcher.BeginInvoke(new Action(() => { OnPropertyChanged(propertyName); }));
        }
        #endregion · INotify methods ·


        #region · IDisposable methods ·
        private bool disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;


            if (disposing)
            {
                //TODO: Dispose of Managed resources here
            }

            // TODO: Dispose of unmanaged resources here
            disposed = true;
        }
        #endregion

    }
}
