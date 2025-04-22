using UnityEngine;

namespace AE._Project.Scripts.Utils
{
    public static class Utils
    {
        public static int InteractableLayerMask = LayerMask.NameToLayer("Interactable");
        public static int PickupableLayerMask = LayerMask.NameToLayer("Pickupable");
        public static int IgnoreRaycastMask = LayerMask.NameToLayer("Ignore Raycast");
        public static int HoldableLayerMask = LayerMask.NameToLayer("Holding");

        /// <summary>
        ///     Creates a donut shaped ring, then sends raycast
        /// </summary>
        /// <param name="center">Center of the ring</param>
        /// <param name="minRadius">Inner radius</param>
        /// <param name="maxRadius">Outer radius</param>
        /// <returns>Hit position and rotation towards center</returns>
        public static (Vector3 position, Vector3 rotation) GetRandomGroundPositionAndRotation(Transform center,
            float minRadius, float maxRadius)
        {
            LayerMask groundLayer = LayerMask.GetMask("Environment");

            const int maxAttempts = 5;

            for (var attempt = 0; attempt < maxAttempts; attempt++)
            {
                // Pick a random point in a ring (between minRadius and maxRadius)
                var distance = Random.Range(minRadius, maxRadius);

                var randomCircle = Random.insideUnitCircle.normalized * distance;

                // Start position 2 units above
                var samplePos = center.position + new Vector3(randomCircle.x, 2f, randomCircle.y);

                // Send raycast downwards, trying to hit ground
                if (Physics.Raycast(samplePos, Vector3.down, out var hit, 10f, groundLayer))
                {
                    var direction = center.position - hit.point;
                    direction.y = 0;

                    var lookRotation = Quaternion.LookRotation(direction.normalized);
                    var eulerAngles = lookRotation.eulerAngles;
                    return (hit.point, eulerAngles);
                }
            }

            // If none of the attempts hit, return fallback
            return (center.position, Vector3.zero);
        }
    }
}