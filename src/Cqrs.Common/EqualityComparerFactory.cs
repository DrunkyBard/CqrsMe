using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Cqrs.Common
{
    public static class EqualityComparerFactory
    {
        private class GeneratedEqualityComparer<T> : IEqualityComparer<T>
        {
            private readonly Func<T, T, bool> _equalsFunc;
            private readonly Func<T, int> _getHashCodeFunc;

            internal GeneratedEqualityComparer(Func<T, T, bool> equalsFunc, Func<T, int> getHashCodeFunc)
            {
                _equalsFunc = equalsFunc;
                _getHashCodeFunc = getHashCodeFunc;
            }

            public bool Equals(T x, T y)
            {
                return _equalsFunc(x, y);
            }

            public int GetHashCode(T obj)
            {
                return _getHashCodeFunc(obj);
            }
        }

        public static IEqualityComparer<T> Create<T>(Func<T, T, bool> equalsFunc, Func<T, int> getHashCodeFunc)
        {
            Contract.Requires(equalsFunc != null, "EqualsFunc should not be null");
            Contract.Requires(getHashCodeFunc != null, "GetHashCodeFunc should not be null");

            return new GeneratedEqualityComparer<T>(equalsFunc, getHashCodeFunc);
        }
    }
}
