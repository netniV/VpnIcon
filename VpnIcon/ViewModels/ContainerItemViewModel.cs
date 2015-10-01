using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VpnIcon.ViewModels
{
    public class ContainerItemViewModel<ChildType> : MenuItemViewModel, IEnumerable<ChildType>, IList<ChildType>, ICollection<ChildType> where ChildType : MenuItemViewModel
    {
        public ContainerItemViewModel() : this(null)
        {
        }

        public ContainerItemViewModel(MenuItemViewModel parentViewModel) : base(parentViewModel)
        {

        }

        public ContainerItemViewModel(MenuItemViewModel parentViewModel, string name, List<ChildType> connections)
        {
            ParentViewModel = parentViewModel;
            Header = name;
            AddRange(connections);
        }

        private static object mChildItemsLock = new object();

        private List<ChildType> mChildItems = new List<ChildType>();

        public long LongCount
        {
            get
            {
                lock (mChildItemsLock)
                {
                    return mChildItems.LongCount();
                }
            }
        }

        public IEnumerator<ChildType> GetEnumerator() { return this.Clone().GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }

        public void Add(ChildType item)
        {
            if (Equals(default(ChildType), item))
            {
                return;
            }
            lock (mChildItemsLock)
            {
                mChildItems.Add(item);
                ChildMenuItems.Add(item);
            }
        }

        public Boolean TryAdd(ChildType item)
        {
            try
            {
                if (Equals(default(ChildType), item))
                {
                    return false;
                }
                Add(item);
                return true;
            }
            catch (NullReferenceException) { }
            catch (ObjectDisposedException) { }
            catch (ArgumentNullException) { }
            catch (ArgumentOutOfRangeException) { }
            catch (ArgumentException) { }
            return false;
        }

        public void Clear()
        {
            lock (mChildItemsLock)
            {
                ChildMenuItems.RemoveAll(x => mChildItems.Contains(x));
                mChildItems.Clear();
            }
        }

        public bool Contains(ChildType item)
        {
            lock (mChildItemsLock)
            {
                return mChildItems.Contains(item);
            }
        }

        public void CopyTo(ChildType[] array, int arrayIndex)
        {
            lock (mChildItemsLock)
            {
                mChildItems.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(ChildType item)
        {
            lock (mChildItemsLock)
            {
                if (Equals(default(ChildType), item))
                    return false;

                if (mChildItems.Contains(item))
                {
                    if (ChildMenuItems.Contains(item))
                        ChildMenuItems.Remove(item);

                }

                return mChildItems.Remove(item);
            }
        }

        public int Count
        {
            get
            {
                lock (mChildItemsLock)
                {
                    return mChildItems.Count;
                }
            }
        }

        public bool IsReadOnly { get { return false; } }

        public int IndexOf(ChildType item)
        {
            lock (mChildItemsLock)
            {
                return mChildItems.IndexOf(item);
            }
        }

        public void Insert(int index, ChildType item)
        {
            lock (mChildItemsLock)
            {
                mChildItems.Insert(index, item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (mChildItemsLock)
            {
                var item = mChildItems[index];

                if (ChildMenuItems.Contains(item))
                    ChildMenuItems.Remove(item);

                mChildItems.RemoveAt(index);
            }
        }

        public ChildType this[int index]
        {
            get
            {
                lock (mChildItemsLock)
                {
                    return mChildItems[index];
                }
            }
            set
            {
                lock (mChildItemsLock)
                {
                    if (value == null)
                        throw new ArgumentNullException($"{typeof(ChildType).Name}#{index}");

                    var oldValue = mChildItems[index];
                    if (ChildMenuItems.Contains(oldValue))
                        ChildMenuItems.Remove(oldValue);

                    mChildItems[index] = value;

                    ChildMenuItems.Add(value);
                }
            }
        }

        /// <summary>
        ///     Add in an enumerable of items.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="asParallel"></param>
        public void AddRange(IEnumerable<ChildType> collection, Boolean asParallel = true)
        {
            if (collection == null)
            {
                return;
            }
            lock (mChildItemsLock)
            {
                var items = asParallel ? collection.AsParallel().Where(arg => !Equals(default(ChildType), arg))
                                              : collection.Where(arg => !Equals(default(ChildType), arg));
                mChildItems.AddRange(items);
                ChildMenuItems.AddRange(items);
            }
        }

        /// <summary>
        ///     Remove in an enumerable of items.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="asParallel"></param>
        public void RemoveRange(IEnumerable<ChildType> collection, Boolean asParallel = true)
        {
            if (collection == null)
            {
                return;
            }
            lock (mChildItemsLock)
            {
                var items = asParallel ? collection.AsParallel().Where(arg => !Equals(default(ChildType), arg))
                                              : collection.Where(arg => !Equals(default(ChildType), arg));
                mChildItems.RemoveAll(x => items.Contains(x));
                ChildMenuItems.RemoveAll(x => items.Contains(x));
            }
        }


        public Task AddAsync(ChildType item)
        {
            return Task.Factory.StartNew(() => { this.TryAdd(item); });
        }

        /// <summary>
        ///     Add in an enumerable of items.
        /// </summary>
        /// <param name="collection"></param>
        //public Task AddRangeAsync(IEnumerable<ChildType> collection)
        //{
        //    if (collection == null)
        //    {
        //        throw new ArgumentNullException("collection");
        //    }

        //    var produce = new TransformBlock<ChildType, ChildType>(item => item, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = Environment.ProcessorCount });

        //    var consume = new ActionBlock<ChildType>(action: async obj => await this.AddAsync(obj), dataflowBlockOptions: new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = Environment.ProcessorCount });
        //    produce.LinkTo(consume);

        //    return Task.Factory.StartNew(async () =>
        //    {
        //        collection.AsParallel().ForAll(item => produce.SendAsync(item));
        //        produce.Complete();
        //        await consume.Completion;
        //    });
        //}

        /// <summary>
        ///     Returns a new copy of all items in the <see cref="List{T}" />.
        /// </summary>
        /// <returns></returns>
        public List<ChildType> Clone(Boolean asParallel = true)
        {
            lock (mChildItemsLock)
            {
                return asParallel
                               ? new List<ChildType>(mChildItems.AsParallel())
                               : new List<ChildType>(mChildItems);
            }
        }

        /// <summary>
        ///     Perform the <paramref name="action" /> on each item in the list.
        /// </summary>
        /// <param name="action">
        ///     <paramref name="action" /> to perform on each item.
        /// </param>
        /// <param name="performActionOnClones">
        ///     If true, the <paramref name="action" /> will be performed on a <see cref="Clone" /> of the items.
        /// </param>
        /// <param name="asParallel">
        ///     Use the <see cref="ParallelQuery{TSource}" /> method.
        /// </param>
        /// <param name="inParallel">
        ///     Use the
        ///     <see
        ///         cref="Parallel.ForEach{TSource}(System.Collections.Generic.IEnumerable{TSource},System.Action{TSource})" />
        ///     method.
        /// </param>
        public void ForEach(Action<ChildType> action, Boolean performActionOnClones = true, Boolean asParallel = true, Boolean inParallel = false)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            var wrapper = new Action<ChildType>(obj =>
            {
                try
                {
                    action(obj);
                }
                catch (ArgumentNullException)
                {
                    //if a null gets into the list then swallow an ArgumentNullException so we can continue adding
                }
            });
            if (performActionOnClones)
            {
                var clones = this.Clone(asParallel: asParallel);
                if (asParallel)
                {
                    clones.AsParallel().ForAll(wrapper);
                }
                else if (inParallel)
                {
                    Parallel.ForEach(clones, wrapper);
                }
                else
                {
                    clones.ForEach(wrapper);
                }
            }
            else
            {
                lock (mChildItemsLock)
                {
                    if (asParallel)
                    {
                        mChildItems.AsParallel().ForAll(wrapper);
                    }
                    else if (inParallel)
                    {
                        Parallel.ForEach(mChildItems, wrapper);
                    }
                    else
                    {
                        mChildItems.ForEach(wrapper);
                    }
                }
            }
        }

        /// <summary>
        ///     Perform the <paramref name="action" /> on each item in the list.
        /// </summary>
        /// <param name="action">
        ///     <paramref name="action" /> to perform on each item.
        /// </param>
        /// <param name="performActionOnClones">
        ///     If true, the <paramref name="action" /> will be performed on a <see cref="Clone" /> of the items.
        /// </param>
        /// <param name="asParallel">
        ///     Use the <see cref="ParallelQuery{TSource}" /> method.
        /// </param>
        /// <param name="inParallel">
        ///     Use the
        ///     <see
        ///         cref="Parallel.ForEach{TSource}(System.Collections.Generic.IEnumerable{TSource},System.Action{TSource})" />
        ///     method.
        /// </param>
        public void ForAll(Action<ChildType> action, Boolean performActionOnClones = true, Boolean asParallel = true, Boolean inParallel = false)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            var wrapper = new Action<ChildType>(obj =>
            {
                try
                {
                    action(obj);
                }
                catch (ArgumentNullException)
                {
                    //if a null gets into the list then swallow an ArgumentNullException so we can continue adding
                }
            });
            if (performActionOnClones)
            {
                var clones = this.Clone(asParallel: asParallel);
                if (asParallel)
                {
                    clones.AsParallel().ForAll(wrapper);
                }
                else if (inParallel)
                {
                    Parallel.ForEach(clones, wrapper);
                }
                else
                {
                    clones.ForEach(wrapper);
                }
            }
            else
            {
                lock (mChildItemsLock)
                {
                    if (asParallel)
                    {
                        mChildItems.AsParallel().ForAll(wrapper);
                    }
                    else if (inParallel)
                    {
                        Parallel.ForEach(mChildItems, wrapper);
                    }
                    else
                    {
                        mChildItems.ForEach(wrapper);
                    }
                }
            }
        }
    }

}