namespace AE.Core.Generics
{
 
    /// <summary>
    /// Class for Singleton MonoBehaviours that aren't supposed to be destroyed on load
    /// </summary>
    public abstract class PersistentMonoSingleton<T> : MonoBehaviourSingleton<T>
        where T : class
    {
        protected override void Awake()
        {
            base.Awake();
            if(Instance == this as T) DontDestroyOnLoad(gameObject);
        }
    }
}