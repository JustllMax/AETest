namespace AE._Project.Scripts.Core.Interfaces
{
    /// <summary>
    ///     Interface for objects that are under the initialization pipeline
    /// </summary>
    public interface IInitializable
    {
        public bool IsInitialized { get; }
        public void Initialize();
    }
}