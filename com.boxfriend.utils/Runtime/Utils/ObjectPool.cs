using System;
using System.Collections.Generic;

namespace Boxfriend.Utils
{
	/// <summary>
	///	Pools objects of type T, will create new objects as necessary
	/// </summary>
    public class ObjectPool<T> where T : class
    {
        private readonly Stack<T> _stack = new ();
        private readonly Func<T> _objectCreator;
        private readonly Action<T> _returnObjectToPool, _getObjectFromPool, _destroyObject;
        private readonly int _maxSize;

        /// <summary>
        /// Number of objects currently in the pool.
        /// </summary>
        public int Count => _stack.Count;

        /// <param name="createObject">Creates and returns an object of the specified type.</param>
        /// <param name="getObjectFromPool">Action called on object when pulled from the pool or created.</param>
        /// <param name="returnObjectToPool">Action called on object when returned to pool.</param>
        /// <param name="onDestroyObject">Action called on object when it is to be destroyed. Can be null</param>
        /// <param name="defaultSize">Number of objects to immediately add to the pool</param>
        /// <param name="maxSize">Maximum number of objects in the pool</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public ObjectPool (Func<T> createObject, Action<T> getObjectFromPool, Action<T> returnObjectToPool, Action<T> onDestroyObject = null, int defaultSize = 10, int maxSize = 100)
        {
            if (maxSize < defaultSize)
                throw new ArgumentOutOfRangeException(nameof(maxSize), "maxSize must be greater than or equal to defaultSize");

            if (defaultSize < 0)
                throw new ArgumentOutOfRangeException(nameof(defaultSize), "defaultSize must be greater than or equal to 0");
            
            _returnObjectToPool = returnObjectToPool ?? throw new ArgumentNullException(nameof(returnObjectToPool));
            _getObjectFromPool = getObjectFromPool ?? throw new ArgumentNullException(nameof(getObjectFromPool));
            _objectCreator = createObject ?? throw new ArgumentNullException(nameof(createObject));
            _destroyObject = onDestroyObject;

            _maxSize = maxSize;

            for (var i = 0; i < defaultSize; i++)
            {
                ToPool(_objectCreator());
            }
        }

        /// <summary>
        /// Gets an object from the pool or creates a new one if the pool is empty. Calls <see langword="Action"/> <see cref="_getObjectFromPool"/> on the object
        /// </summary>
        public T FromPool ()
        {
            var poolObject = _stack.Count > 0 ? _stack.Pop() : _objectCreator();
            _getObjectFromPool(poolObject);
            return poolObject;
        }

        /// <summary>
        /// Adds an item to the pool and calls <see langword="Action"/> <see cref="_returnObjectToPool"/> on it
        /// </summary>
        /// <param name="item">Item to be added</param>
        public void ToPool (T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            _returnObjectToPool(item);

            if(_stack.Count >= _maxSize)
            {
                _destroyObject?.Invoke(item);
                return;
            }

            _stack.Push(item);
        }

        /// <summary>
        /// Removes all items from the pool, calling <see langword="Action"/> <see cref="_destroyObject"/> on it if not null.
        /// </summary>
        public void EmptyPool()
        {
            if(_destroyObject is null)
            {
                _stack.Clear();
                return;
            }

            while(_stack.Count > 0)
            {
                var obj = _stack.Pop();
                _destroyObject(obj);
            }
        }
    }
}
