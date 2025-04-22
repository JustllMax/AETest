using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace AE._Project.Scripts.Extensions
{
    public static class CollectionsExtenstions
    {
        public static bool IsNullOrEmpty<T>(this IList<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        public static void AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> range)
        {
            foreach (var obj in range)
                hashSet.Add(obj);
        }

        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            if (collection == null || collection.Count == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Adds element only if it doesn't already exist in the collection
        /// </summary>
        public static bool AddUnique<T>(this ICollection<T> collection, T element)
        {
            if (!collection.Contains(element))
            {
                collection.Add(element);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Removes element only if it exists in the collection
        /// </summary>
        public static bool RemoveElement<T>(this ICollection<T> collection, T element)
        {
            if (collection.Contains(element))
            {
                collection.Remove(element);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Returns random element from collection and removes it
        /// </summary>
        public static T PopRandomElement<T>(this IList<T> collection)
        {
            if (IsNullOrEmpty(collection))
            {
                return default;
            }

            // Get random index
            var index = Random.Range(0, collection.Count);

            // Get element
            var element = collection[index];

            // Remove element
            collection.Remove(element);

            // Return element
            return element;
        }

        /// <summary>
        ///     Returns random element from the list
        /// </summary>
        public static T GetRandomElement<T>(this IList collection)
        {
            if (collection == null || collection.Count == 0)
            {
                return default;
            }

            var index = Random.Range(0, collection.Count);

            return collection[index] is T output ? output : default;
        }

        /// <summary>
        ///     Returns random element from the collection
        /// </summary>
        public static T GetRandomElement<T>(this T[] collection)
        {
            if (collection == null || collection.Length == 0)
            {
                return default;
            }

            var index = Random.Range(0, collection.Length);

            return collection[index];
        }


        /// <summary>
        ///     Returns random index from the list
        /// </summary>
        public static int GetRandomElementIndex<T>(this IList<T> collection)
        {
            if (collection == null || collection.Count == 0)
            {
                return -1;
            }

            return Random.Range(0, collection.Count);
        }

        /// <summary>
        ///     Returns random index from the collection
        /// </summary>
        public static int GetRandomElementIndex<T>(this T[] collection)
        {
            if (collection == null || collection.Length == 0)
            {
                return -1;
            }

            return Random.Range(0, collection.Length);
        }
    }
}