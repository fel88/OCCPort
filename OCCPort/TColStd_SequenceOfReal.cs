using System;
using System.Collections.Generic;

namespace OCCPort
{
    public class TColStd_SequenceOfReal : List<double>
    {
        internal void Append(double keptT3d)
        {
            Add(keptT3d);
        }

        internal void ChangeValue(int i, double theParam)
        {
            this[i-1] = theParam;
        }

        public new double this[int key]
        {
            get => base[key - 1];
            set => base[key - 1] = value;
        }

        internal void InsertBefore(int i, double theParam)
        {
            Insert(i-1, theParam);
        }
    }
}