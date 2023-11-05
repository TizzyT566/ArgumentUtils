using System;
using System.Collections.Generic;

namespace ArgumentUtils;

public static class ArgumentParser
{
    private static ArgNode? _head = null;

    static ArgumentParser()
    {
        SetArgs(Environment.GetCommandLineArgs());
    }

    public static void SetArgs(string[] args)
    {
        _head = null;
        ArgNode? _ptr = null;

        void Add(string arg)
        {
            ArgNode newNode = new(arg);
            if (_head is null)
            {
                _head = newNode;
            }
            else
            {
                newNode._prev = _ptr;
                _ptr!._next = newNode;
            }
            _ptr = newNode;
        }

        foreach (string arg in args)
            Add(arg);
    }

    public static Option Arg(string option, bool required = false)
    {
        ArgNode? _ptr = _head;
        while (_ptr is not null)
        {
            if (_ptr._arg == option)
            {
                return new(_ptr, option, required);
            }
            _ptr = _ptr._next;
        }

        if (required)
        {
            throw new ArgumentException($"Required option {option} not found.");
        }

        return new(null, option, required);
    }

    public sealed class FailedParsingArgumentException(string option, int index, Type type) : Exception($"Failed to parse argument {index} for option {option}, of type {type}.");

    internal sealed class ArgNode(string arg)
    {
        internal readonly string _arg = arg;
        internal ArgNode? _prev, _next;
    }

    public sealed class Option
    {
        private readonly string _option;
        private readonly bool _required;
        private ArgNode? _start, _ptr;
        private int _index;

        internal Option(ArgNode? start, string option, bool required)
        {
            _start = start;
            _ptr = start;
            _option = option;
            _required = required;
        }

        public Option Map<TParse, TResult>(IDictionary<TParse, TResult> dict, out TResult value, IFormatProvider? formatProvider = null) where TParse : IParsable<TParse>
        {
            _index++;
            if (_ptr is null || _ptr._next is null)
            {
                if (_required)
                    throw new FailedParsingArgumentException(_option, _index, typeof(TParse));
                _start = null;
                value = default!;
                return this;
            }
            _ptr = _ptr._next;
            if (!TParse.TryParse(_ptr._arg, formatProvider, out TParse? t) || !dict.TryGetValue(t, out value!))
            {
                if (_required)
                    throw new FailedParsingArgumentException(_option, _index, typeof(TParse));
                _start = null;
                value = default!;
                return this;
            }
            return this;
        }

        public Option Map<TParse, TResult>(IDictionary<TParse, TResult> dict, out TResult value, TResult defaultValue, IFormatProvider? formatProvider = null) where TParse : IParsable<TParse>
        {
            _index++;
            if (_ptr is null || _ptr._next is null)
            {
                value = defaultValue;
                _start = null;
                return this;
            }
            _ptr = _ptr._next;
            if (!TParse.TryParse(_ptr._arg, formatProvider, out TParse? t) || !dict.TryGetValue(t, out value!))
            {
                value = defaultValue;
                _start = null;
            }
            return this;
        }

        public Option Param<T>(out T value, string? name = null, string? description = null, IFormatProvider? formatProvider = null) where T : IParsable<T>
        {
            _index++;
            if (_ptr is null || _ptr._next is null)
            {
                if (_required)
                    throw new FailedParsingArgumentException(_option, _index, typeof(T));
                value = default!;
                return this;
            }
            _ptr = _ptr._next;
            if (!T.TryParse(_ptr._arg, formatProvider, out value!))
            {
                if (_required)
                    throw new FailedParsingArgumentException(_option, _index, typeof(T));
                _start = null;
                value = default!;
                return this;
            }
            return this;
        }

        public Option Param<T>(out T value, T defaultValue, string? name = null, string? description = null, IFormatProvider? formatProvider = null) where T : IParsable<T>
        {
            _index++;
            if (_ptr is null || _ptr._next is null)
            {
                value = defaultValue;
                _start = null;
                return this;
            }
            _ptr = _ptr._next;
            if (!T.TryParse(_ptr._arg, formatProvider, out value!))
            {
                value = defaultValue;
                _start = null;
            }
            return this;
        }

        public Option Param(out string value)
        {
            _index++;
            if (_ptr is null || _ptr._next is null)
            {
                if (_required)
                    throw new FailedParsingArgumentException(_option, _index, typeof(string));
                _start = null;
                value = default!;
                return this;
            }
            _ptr = _ptr._next;
            value = _ptr._arg;
            return this;
        }

        public Option Param(out string value, string defaultValue)
        {
            _index++;
            if (_ptr is null || _ptr._next is null)
            {
                value = defaultValue;
                _start = null;
                return this;
            }
            _ptr = _ptr._next;
            value = _ptr._arg;
            return this;
        }

        internal bool Parse()
        {
            if (_start is not null)
            {
                if (_ptr is null || _ptr._next is null)
                {
                    if (_start._prev is not null)
                    {
                        _start._prev._next = null;
                    }
                    else
                    {
                        _head = null;
                    }
                }
                else
                {
                    if (_start._prev is not null)
                    {
                        _start._prev._next = _ptr._next;
                        _ptr._next._prev = _start._prev;
                    }
                    else
                    {
                        _head = _ptr._next;
                        _head._prev = null;
                    }
                }
                return true;
            }
            return false;
        }

        public static implicit operator bool(Option option) => option.Parse();
    }
}
