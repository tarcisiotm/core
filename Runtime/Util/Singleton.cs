using UnityEngine;

namespace TG.Core
{
    /// <summary>
    /// Singleton implementation
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        protected static T instance = null;

        public static T I => instance;

        protected virtual void OnEnable()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this as T;
        }

        protected virtual void OnDisable()
        {
            if (instance == this) { instance = null; }
        }
    }
}