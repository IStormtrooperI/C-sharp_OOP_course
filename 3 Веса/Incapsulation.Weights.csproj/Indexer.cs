using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incapsulation.Weights
{
    public class Indexer
    {
        private int length;
        private int start;
        private double[] array;
        public Indexer(double[] indexerArray, int start, int length)
        {
            CheckArgumentException(indexerArray.Length, start.ToString(), length.ToString());
            this.array = indexerArray;
            this.start = start;
            this.length = length;
        }

        public int Length
        {
            get
            {
                return this.length;
            }
        }
        public double this[int index]
        {
            get
            {
                CheckIndexOutOfRangeException(index);
                return array[start + index];
            }
            set
            {
                CheckIndexOutOfRangeException(index);
                array[start + index] = value;
            }
        }
        private void CheckIndexOutOfRangeException(int index)
        {
            if (index < 0 || index >= Length)
                throw new IndexOutOfRangeException("Index");
        }

        private static void CheckArgumentException(int arrayLength, string start = null, string length = null)
        {
            if (Int32.Parse(start) < 0)
            {
                throw new ArgumentException("Start");
            }
            if (Int32.Parse(length) < 0)
                throw new ArgumentException("Length");
            if (Int32.Parse(start) + Int32.Parse(length) > arrayLength)
                throw new ArgumentException("Length");
        }
    }
}
