﻿using System;

namespace MvvmNano
{
    /// <summary>
    /// Is thrown whenever something bad happens internally of MvvmNano.
    /// </summary>
    public class MvvmNanoException : Exception
    {
        public MvvmNanoException(string message) : base(message)
        {
        }
    }
}

