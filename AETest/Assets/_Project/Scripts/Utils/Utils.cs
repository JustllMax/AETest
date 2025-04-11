using UnityEngine;

    public static class Utils
    {
        /// <summary>
        /// Creates a donut shaped ring, then sends raycast 
        /// </summary>
        /// <param name="center">Center of the ring</param>
        /// <param name="minRadius">Inner radius</param>
        /// <param name="maxRadius">Outer radius</param>
        /// <returns>Hit position and rotation towards center</returns>
        public static (Vector3 position, Vector3 rotation) GetRandomGroundPositionAndRotation(Transform center, float minRadius, float maxRadius)
        {
            LayerMask groundLayer = LayerMask.GetMask("Environment");
            
            const int maxAttempts = 5;
            
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {

                // Pick a random point in a ring (between minRadius and maxRadius)
                float distance = UnityEngine.Random.Range(minRadius, maxRadius);

                Vector2 randomCircle = UnityEngine.Random.insideUnitCircle.normalized * distance;

                // Start position 2 units above
                Vector3 samplePos = center.position + new Vector3(randomCircle.x, 2f, randomCircle.y);

                // Send raycast downwards, trying to hit ground
                if (Physics.Raycast(samplePos, Vector3.down, out RaycastHit hit, 10f, groundLayer))
                {
                    Vector3 direction = center.position - hit.point;
                    direction.y = 0;

                    Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);
                    Vector3 eulerAngles = lookRotation.eulerAngles;
                    return (hit.point, eulerAngles);
                }
            }
            
            // If none of the attempts hit, return fallback
            return (center.position, Vector3.zero);
        }
    }

