using System.Collections.Generic;
using System.Linq;
using Xunit.Sdk;

namespace Xunit
{
    public partial class Assert
    {
        public static void Contains<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            GuardArgumentNotNull(nameof(expected), expected);
            GuardArgumentNotNull(nameof(actual), actual);

            var expectedList = expected.ToList();
            var actualList = actual.ToList();
            var intersect = expectedList.Intersect(actualList).ToList();

            // TODO: Improve exception message
            if (intersect.Count != expectedList.Count) throw new ContainsException(expectedList, actualList);
        }

        public static void DoesNotContain<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            GuardArgumentNotNull(nameof(expected), expected);

            var expectedSet = new HashSet<T>(expected);

            DoesNotContain(actual, item => expectedSet.Contains(item));
        }

        public static void ContainsInOrder<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            GuardArgumentNotNull(nameof(expected), expected);
            GuardArgumentNotNull(nameof(actual), actual);

            var expectedList = expected.ToList();
            var actualList = actual.ToList();

            if (expectedList.Count == 0)
                return;

            if (expectedList.Count > actualList.Count)
                throw new ContainsException(expectedList, actualList);

            var pos = 0;
            foreach (var e in actualList)
            {
                if (expectedList[pos].Equals(e))
                {
                    pos++;
                }
                else if (expectedList.Skip(pos).Contains(e))
                {
                    // Not in the correct order
                    throw new ContainsException(expectedList, actualList);
                }

                if (pos == expectedList.Count)
                    break;
            }

            if (pos != expectedList.Count)
            {
                // Not all items was in the list
                throw new ContainsException(expectedList, actualList);
            }
        }
    }
}
