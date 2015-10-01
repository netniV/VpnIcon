using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;

namespace Letter.Despatch.Async
{

    public class AsyncObservableCollection<T> : ObservableCollection<T>
    {
        private SynchronizationContext _synchronizationContext = SynchronizationContext.Current;

        public AsyncObservableCollection()
        {
        }

        public AsyncObservableCollection(IEnumerable<T> list)
            : base(list)
        {
        }

        private object rangeLock = new object();
        public void InsertRange(IEnumerable<T> items)
        {
            using (this.BlockReentrancy())
            {
                int start = -1;

                lock (rangeLock)
                {
                    start = Items.Count;
                    foreach (var item in items) Items.Add(item);
                }

                //var type = NotifyCollectionChangedAction.Add;
                //var colChanged = new NotifyCollectionChangedEventArgs(type, items.ToList(), start);
                var type = NotifyCollectionChangedAction.Reset;
                var colChanged = new NotifyCollectionChangedEventArgs(type);
                var countChanged = new PropertyChangedEventArgs("Count");

                OnBasePropertyChanged(countChanged);
                OnCollectionChanged(colChanged);
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (SynchronizationContext.Current == _synchronizationContext)
            {
                // Execute the CollectionChanged event on the current thread
                RaiseCollectionChanged(e);
            }
            else
            {
                // Raises the CollectionChanged event on the creator thread
                _synchronizationContext.Send(RaiseCollectionChanged, e);
            }
        }

        private void RaiseCollectionChanged(object param)
        {
            // We are in the creator thread, call the base implementation directly
            base.OnCollectionChanged((NotifyCollectionChangedEventArgs)param);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (SynchronizationContext.Current == _synchronizationContext)
            {
                // Execute the PropertyChanged event on the current thread
                OnBasePropertyChanged(e);
            }
            else
            {
                // Raises the PropertyChanged event on the creator thread
                _synchronizationContext.Send(OnBasePropertyChanged, e);
            }
        }

        private void OnBasePropertyChanged(object param)
        {
            // We are in the creator thread, call the base implementation directly
            base.OnPropertyChanged((PropertyChangedEventArgs)param);
        }
    }
}
