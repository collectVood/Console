using System.Collections.Generic;

namespace Console
{
    public class Pool<T>
    {
        private static Pool<T> _currentPool = new Pool<T>();

        private bool _initialized;
        private Queue<T> _pool = new Queue<T>();

        /// <summary>
        /// Create new pool and initialize it
        /// </summary>
        static Pool()
        {
            lock (_currentPool)
            {
                var pool = _currentPool;
                if (pool._initialized)
                    return;

                pool.Initialize();
            }
        }

        /// <summary>
        /// Initialize pool with 10 default values of T
        /// </summary>
        private void Initialize()
        {
            for (var i = 0; i < 10; i++)
            {
                Add();
            }

            _initialized = true;
        }

        /// <summary>
        /// Get T from pool
        /// </summary>
        /// <returns>T from pool</returns>
        public static T Get()
        {
            lock (_currentPool)
            {
                var pool = _currentPool;

                while (true)
                {
                    if (pool._pool.Count == 0)
                        pool.Initialize();

                    if (pool._initialized)
                        return pool._pool.Dequeue();

                    pool.Initialize();
                }
            }
        }

        /// <summary>
        /// Add an item to pool
        /// </summary>
        private static void Add()
        {
            lock (_currentPool)
            {
                var pool = _currentPool;
                pool._pool.Enqueue(default);
            }
        }
    }

    public class PoolNew<T> where T : new()
    {
        private static PoolNew<T> _currentPool = new PoolNew<T>();

        private bool _initialized;
        private Queue<T> _pool = new Queue<T>();

        /// <summary>
        /// Create new pool and initialize it
        /// </summary>
        static PoolNew()
        {
            lock (_currentPool)
            {
                var pool = _currentPool;
                if (pool._initialized)
                    return;

                pool.Initialize();
            }
        }

        /// <summary>
        /// Initialize pool with 10 default values of T
        /// </summary>
        private void Initialize()
        {
            for (var i = 0; i < 10; i++)
            {
                Add();
            }

            _initialized = true;
        }

        /// <summary>
        /// Get T from pool
        /// </summary>
        /// <returns>T from pool</returns>
        public static T Get()
        {
            lock (_currentPool)
            {
                var pool = _currentPool;

                while (true)
                {
                    if (pool._pool.Count == 0)
                        pool.Initialize();

                    if (pool._initialized)
                        return pool._pool.Dequeue();

                    pool.Initialize();
                }
            }
        }

        /// <summary>
        /// Add an item to pool
        /// </summary>
        private static void Add()
        {
            lock (_currentPool)
            {
                var pool = _currentPool;
                pool._pool.Enqueue(new T());
            }
        }
    }
}