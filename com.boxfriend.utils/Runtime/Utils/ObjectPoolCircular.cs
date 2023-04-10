using System;
using System.Collections.Generic;
using System.Linq;

namespace Boxfriend.Utils
{
	/// <summary>
	///	Pools a specific number of objects, will reuse oldest active objects when all objects are in use.
	/// </summary>
    public class ObjectPoolCircular<T> where T : class
    {

		private Queue<T> _activeQueue, _inactiveQueue;
        private readonly Func<T> _objectCreator;
        private readonly Action<T> _returnObjectToPool, _getObjectFromPool, _destroyObject;
		private readonly int _size;

        /// <summary>
        /// Total number of objects in the pool.
        /// </summary>
        public int Count => _size;
		
		/// <summary>
		///	Total number of currently active pooled objects
		/// </summary>
		public int ActiveCount => _activeQueue.Count;
		
		/// <summary>
		/// Total number of currently inactive pooled objects
		/// </summary>
		public int InactiveCount => _inactiveQueue.Count;

        /// <param name="createObject">Creates and returns an object of the specified type.</param>
        /// <param name="getObjectFromPool">Action called on object when pulled from the pool or created.</param>
        /// <param name="returnObjectToPool">Action called on object when returned to pool.</param>
        /// <param name="onDestroyObject">Action called on object when it is to be destroyed. Can be null</param>
        /// <param name="size">Total number of objects in the pool</param>
        /// <exception cref="ArgumentOutOfRangeException">Size must be greater than zero</exception>
        /// <exception cref="ArgumentNullException"></exception>
        public ObjectPoolCircular (Func<T> createObject, Action<T> getObjectFromPool, Action<T> returnObjectToPool, Action<T> onDestroyObject = null, int size = 100)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException(nameof(size), "size must be greater than zero");

            
            _returnObjectToPool = returnObjectToPool ?? throw new ArgumentNullException(nameof(returnObjectToPool));
            _getObjectFromPool = getObjectFromPool ?? throw new ArgumentNullException(nameof(getObjectFromPool));
            _objectCreator = createObject ?? throw new ArgumentNullException(nameof(createObject));
            _destroyObject = onDestroyObject;
			
			_size = size;
			_inactiveQueue = new(size);
			_activeQueue = new(size);
            for (var i = 0; i < size; i++)
            {
				var obj = _objectCreator();
				_returnObjectToPool(obj);
                _inactiveQueue.Enqueue(obj);
            }
        }

        /// <summary>
        /// Gets an object from the pool or reuses the oldest active object if all pooled objects are in use. Calls <see langword="Action"/> <see cref="_getObjectFromPool"/> on the object
		/// Will call <see langword="Action"/> <see cref="_returnObjectToPool"/> if reusing an active object.
        /// </summary>
        public T FromPool ()
        {
			if(_inactiveQueue.Count + _activeQueue.Count == 0)
				throw new InvalidOperationException("Object pool has been cleared, there is nothing left to get");
			
			
			T poolObject;
			if(_inactiveQueue.Count == 0)
			{
				poolObject = _activeQueue.Dequeue();
				_returnObjectToPool(poolObject);
			}
			else
			{
				poolObject = _inactiveQueue.Dequeue();
			}
			
            _getObjectFromPool(poolObject);
            return poolObject;
        }

        /// <summary>
        /// Adds an item to the pool and calls <see langword="Action"/> <see cref="_returnObjectToPool"/> on it
		/// Generates garbage if item is not the oldest object pulled from the pool
        /// </summary>
        /// <param name="item">Item to be added</param>
        public void ToPool (T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

			if(_activeQueue.Peek() == item)
				_activeQueue.Dequeue();
			else
				_activeQueue = new Queue<T>(_activeQueue.Where(x => x != item));
			
            _returnObjectToPool(item);
			_inactiveQueue.Enqueue(item);
        }


		/// <summary>
        /// Removes all items from the pool, calling <see langword="Action"/> <see cref="_destroyObject"/> on it if not null.
		/// Does not call <see langword="Action"/> <see cref="_returnObjectToPool"/>.
        /// </summary>
        public void EmptyPool()
        {
            if(_destroyObject is null)
            {
                _activeQueue.Clear();
				_inactiveQueue.Clear();
                return;
            }

            while(_activeQueue.Count > 0)
            {
                var obj = _activeQueue.Dequeue();
                _destroyObject(obj);
            }
			
			while(_inactiveQueue.Count > 0)
            {
                var obj = _inactiveQueue.Dequeue();
                _destroyObject(obj);
            }
        }
    }
}
