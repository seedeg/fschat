using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FsChat.Utilities.Common
{
    public static class NumberUtils
    {
        public static IEnumerable<int> FindMissing(this List<int> list)
        {
            // Sorting the list
            list.Sort();

            // First number of the list
            var firstNumber = list.First();

            // Last number of the list
            var lastNumber = list.Last();

            // Range that contains all numbers in the interval
            // [ firstNumber, lastNumber ]
            var range = Enumerable.Range(firstNumber, lastNumber - firstNumber);

            // Getting the set difference
            var missingNumbers = range.Except(list);

            return missingNumbers;
        }
    }
}
