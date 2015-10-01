using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Threading;

namespace Letter.Despatch.Async
{
    public class AsyncList<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private List<T> internalList;

        public bool AllowAsync { get; set; }

        public AsyncList()
        {
            AllowAsync = true;
            internalList = new List<T>();
            Dispatcher = Dispatcher.CurrentDispatcher;
        }

        public AsyncList(int capacity)
        {
            internalList = new List<T>(capacity);
        }

        public AsyncList(IEnumerable<T> collection)
        {
            internalList = new List<T>(collection);
        }

        public Dispatcher Dispatcher { get; set; }

        #region IList<T> Members

        public int IndexOf(T item)
        {
            return internalList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            internalList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            internalList.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                return internalList[index];
            }
            set
            {
                var oldItem = internalList[index];
                if (!EqualityComparer<T>.Default.Equals(oldItem, value))
                {
                    internalList[index] = value;
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldItem, index));
                }
            }
        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {
            if (AllowAsync || this.Dispatcher.CheckAccess())
            {
                internalList.Add(item);
                int index = IndexOf(item);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
                OnPropertyChanged("Count");
            }
            else
            {
                Dispatcher.BeginInvoke(new Action(() => { Add(item); }));
            }
        }

        public void AddRange(IEnumerable<T> items)
        {
            if (AllowAsync || this.Dispatcher.CheckAccess())
            {
                int index = Count;
                //Debug.WriteLine("[{0}] Before {1} Adding {2}, Count {3}", this.GetHashCode(), items.GetHashCode(), items.Count(), this.Count);
                internalList.AddRange(items);
                //Debug.WriteLine("[{0}] After {1} Adding {2}, Count {3}", this.GetHashCode(), items.GetHashCode(), items.Count(), this.Count);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items.ToList(), index));
                OnPropertyChanged("Count");
            }
            else
            {
                Dispatcher.BeginInvoke(new Action(() => { AddRange(items); }));
            }
        }

        public void Clear()
        {
            if (AllowAsync || this.Dispatcher.CheckAccess())
            {
                internalList.Clear();
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                OnPropertyChanged("Count");
            }
            else
            {
                Dispatcher.BeginInvoke(new Action(() => { Clear(); }));
            }
        }

        public bool Contains(T item)
        {
            return internalList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            internalList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return internalList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            if (AllowAsync || Dispatcher.CheckAccess())
            {
                int index = internalList.IndexOf(item);
                bool success = internalList.Remove(item);
                if (success)
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
                    OnPropertyChanged("Count");
                }
                return success;
            }
            else
            {
                return (bool)Dispatcher.Invoke(new Func<bool>(() => { return Remove(item); }));
            }
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return internalList.GetEnumerator();
        }

        #endregion

        #region " INotifyCollectionChanged "
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!AllowAsync || Dispatcher.CheckAccess())
            {
                NotifyCollectionChangedEventHandler handlers = this.CollectionChanged;
                if (handlers != null)
                {
                    foreach (NotifyCollectionChangedEventHandler handler in handlers.GetInvocationList())
                    {
                        if (handler.Target is CollectionView)
                            ((CollectionView)handler.Target).Refresh();
                        else
                            handler(this, e);
                    }
                }
            }
            else
            {
                // Raises the CollectionChanged event on the creator thread
                Dispatcher.Invoke(new Action(() => { OnCollectionChanged(e); }));
            }
        }

        #endregion

        #region " INotifyPropertyChanged "
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanging(string propertyName)
        {
            if (Dispatcher.CheckAccess())
            {
                if (PropertyChanging != null)
                    PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
            else
            {
                // Raises the PropertyChanged event on the creator thread
                // Raises the CollectionChanged event on the creator thread
                Dispatcher.BeginInvoke(new Action(() => { OnPropertyChanging(propertyName); }));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (Dispatcher.CheckAccess())
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
            else
            {
                // Raises the CollectionChanged event on the creator thread
                Dispatcher.BeginInvoke(new Action(() => { OnPropertyChanged(propertyName); }));
            }
        }

        #endregion
    }
}
