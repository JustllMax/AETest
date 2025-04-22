namespace AE._Project.Scripts.Interfaces
{
    /// <summary>
    ///     Interface for classes that require listeners
    /// </summary>
    public interface IAttachListeners
    {
        /// <summary>
        ///     Attaches listeners
        /// </summary>
        public void AttachListeners();

        /// <summary>
        ///     Detaches listeners
        /// </summary>
        public void DetachListeners();
    }
}