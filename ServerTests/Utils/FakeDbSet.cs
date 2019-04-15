using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace ServerTests.Utils
{
    public class FakeDbSet<T> : IDbSet<T> where T : class, new()
    {
        private readonly List<T> collection;

        public FakeDbSet(params T[] initialCollection)
            : this(initialCollection.AsEnumerable())
        {
        }

        public FakeDbSet(IEnumerable<T> initialCollection)
        {
            collection = initialCollection.ToList();
        }

        #region Callbacks
        public event Action<T> ItemAdded;
        public event Action<T, bool> ItemRemoved;
        public event Action ItemCreated;
        #endregion

        #region IEnumerable
        public IEnumerator<T> GetEnumerator() => collection.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        #region IQueryable
        public Type ElementType => typeof(T);
        public Expression Expression => collection.AsQueryable().Expression;
        public IQueryProvider Provider => collection.AsQueryable().Provider;
        #endregion

        #region IDbSet
        public T Find(params object[] keyValues)
        {
            throw new NotSupportedException();
        }

        public T Add(T entity)
        {
            collection.Add(entity);
            ItemAdded?.Invoke(entity);
            return entity;
        }

        public T Remove(T entity)
        {
            var willBeRemoved = collection.Contains(entity);

            if (!willBeRemoved)
            {
                ItemRemoved?.Invoke(entity, false);
            }
            else
            {
                collection.Remove(entity);
                ItemRemoved?.Invoke(entity, false);
            }
            return entity;
        }

        public T Attach(T entity)
        {
            throw new NotSupportedException();
        }

        public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
        {
            throw new NotSupportedException();
        }

        public ObservableCollection<T> Local => throw new NotSupportedException();

        public T Create()
        {
            var entity = new T();
            ItemCreated?.Invoke();
            return entity;
        }
        #endregion
    }
}
