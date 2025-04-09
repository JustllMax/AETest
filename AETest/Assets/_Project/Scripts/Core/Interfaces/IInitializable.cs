
namespace AE.Core.Interfaces
{

    /// <summary>
    /// Interface for objects that are under the initialization pipeline
    /// </summary>
    public interface IInitializable
    {
        public void Initialize();

        public bool IsInitialized { get; }
    }
}