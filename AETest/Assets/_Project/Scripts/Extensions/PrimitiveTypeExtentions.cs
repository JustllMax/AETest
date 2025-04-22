namespace AE._Project.Scripts.Extensions
{
    public static class PrimitiveTypeExtentions
    {
        /// <summary>
        ///     Remaps value between given ranges
        /// </summary>
        public static float Remap(this float value, float inMin, float inMax, float outMin, float outMax)
        {
            return (value - inMin) / (inMax - inMin) * (outMax - outMin) + outMin;
        }
    }
}