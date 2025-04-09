namespace AE.Interfaces
{

    /// <summary>
    /// Represents class that requires additional setup on Start.
    /// </summary>
    public interface IWithSetUp
    {

        /// <summary>
        /// Used instead of method Start, called during start initialization pipeline
        /// </summary>
        public void SetUp();


        /// <summary>
        /// Cleans up data, called during OnDestroy
        /// </summary>
        public void TearDown();
    }
}