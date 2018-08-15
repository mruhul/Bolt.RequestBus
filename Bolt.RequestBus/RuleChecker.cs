using System;
using System.Collections.Generic;

namespace Bolt.RequestBus
{
    public class RuleChecker<T>
    {
        private readonly T _value;

        private List<IError> _errors = new List<IError>();

        public RuleChecker(T value)
        {
            _value = value;
        }

        public RuleChecker<T> Should(Func<T,bool> fetch, string propertyName, string message, string code)
        {
            if (!fetch.Invoke(_value))
            {
                _errors.Add(Error.Create(propertyName, message, code));
            }

            return this;
        }

        public RuleChecker<T> Should(Func<T, bool> fetch, string propertyName, string message)
        {
            return Should(fetch, propertyName, message, string.Empty);
        }

        public RuleChecker<T> Should(Func<T, bool> fetch, string message)
        {
            return Should(fetch, string.Empty, message, string.Empty);
        }

        public IEnumerable<IError> Done()
        {
            return _errors;
        }
    }

    public static class RuleChecker
    {
        public static RuleChecker<T> For<T>(T value)
        {
            return new RuleChecker<T>(value);
        }
    }
}
