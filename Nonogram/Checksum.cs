using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nonogram
{
    public class Checksum: IChecksum
    {
        private int _sum;
        public Checksum()
        {
            _checksum = new List<int>();
        }
        private List<int> _checksum;
        public int this[int pos]
        {
            get { return _checksum[pos]; }
            set
            {
                _sum += value - _checksum[pos];
                if (value == 0)
                {
                    _checksum.RemoveAt(pos);
                }
                else
                {
                    _checksum[pos] = value; 
                }
            }
        }
        /// <summary>
        /// Sum of all values of this checksum
        /// </summary>
        public int Sum
        {
            get { return _sum; }
        }
        public void Add(int value)
        {
            _checksum.Add(value);
        }

        public void Insert(int index, int value)
        {
            _checksum.Insert(index, value);
        }
        public int Count
        {
            get { return _checksum.Count; }
        }
        public void Clear()
        {
            _checksum.Clear();
        }

        public bool Equals(IChecksum other)
        {
            if(Count != other.Count)
            {
                return false;
            }
            for(int i = 0; i < Count; i++)
            {
                if(other[i] != this[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Remove all zero values from checksum
        /// </summary>
        public void Clean()
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i] == 0)
                {
                    _checksum.RemoveAt(i);
                }
            }
        }
        public IEnumerator<int> GetEnumerator()
        {
            return _checksum.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
    public enum Orientation { Vertical, Horizontal }

}
