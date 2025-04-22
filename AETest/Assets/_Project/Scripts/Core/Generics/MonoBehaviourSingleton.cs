using UnityEngine;

namespace AE._Project.Scripts.Core.Generics
{
    /// <summary>
    ///     Class for MonoBehaviours that should be accessed from other classes
    /// </summary>
    public abstract class MonoBehaviourSingleton<T> : InGameMonoBehaviour
        where T : class
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                var type = typeof(T);
                var obj = FindObjectsByType(type, FindObjectsSortMode.None);

                switch (obj.Length)
                {
                    case <= 0:
                        return null;
                    case > 1:
                        Debug.LogWarning($"Found duplicate of singleton {type.Name}!");
                        Destroy(obj[1]);
                        break;
                }

                return obj[0] as T;

            }
        }


        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this as T)
            {
                Destroy(gameObject);
            }
        }

        protected override void CleanUp()
        {
            if (_instance == this as T)
            {
                _instance = null;
            }

            base.CleanUp();
        }
    }
}