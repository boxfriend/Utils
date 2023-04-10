using System;
namespace Boxfriend.Utils
{
	public abstract class Singleton<T> where T : class, new()
	{
        public static T Instance => _instance;
		private static T _instance;

        private static T InitializeSingleton (T obj = null) => _instance = obj ?? new T();

        public Singleton():this(null){}
        public Singleton (T obj) => InitializeSingleton(obj);
    }
}
