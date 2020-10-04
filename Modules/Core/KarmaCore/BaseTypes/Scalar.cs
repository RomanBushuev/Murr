using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KarmaCore.Exceptions;

namespace KarmaCore.BaseTypes
{
    public abstract class Scalar<T>
    {
        private SortedDictionary<DateTime, T> _store = new SortedDictionary<DateTime, T>();

        public Scalar()
        {

        }

        public Scalar(IDictionary<DateTime, T> store)
        {
            _store = new SortedDictionary<DateTime, T>(store);
        }

        public virtual void Add(DateTime dateTime, T value)
        {
            _store[dateTime] = value;
        }

        public virtual T Get(DateTime dateTime)
        {
            if (!(_store?.Count > 0))
                throw new KarmaCoreException("Последовательность равна 0");

            if (_store.First().Key > dateTime)
            {
                var message = string.Format("Первое валидное значение датириуется {0}", _store.First().Key.ToShortDateString());
                throw new KarmaCoreException(message);
            }

            //проверяем последующие элементы
            for (int i = 0; i < _store.Count - 1; ++i)
            {
                var previousDate = _store.ElementAt(i).Key;
                var currentDate = _store.ElementAt(i + 1).Key;

                if (previousDate <= dateTime && currentDate > dateTime)
                {
                    T value = _store[previousDate];
                    return value;
                }
            }

            T lastValue = _store.Last().Value;
            return lastValue;
        }

        public virtual bool HasValue(DateTime dateTime)
        {
            if (!(_store?.Count > 0))
                return false;

            //проверяем первый элемент
            if (_store.First().Key > dateTime)
                return false;

            //проверяем последующие элементы
            for (int i = 0; i < _store.Count - 1; ++i)
            {
                var previousDate = _store.ElementAt(i).Key;
                var currentDate = _store.ElementAt(i + 1).Key;

                if (previousDate <= dateTime && currentDate > dateTime)
                {
                    return true;
                }
            }

            return true;
        }
    }
}
