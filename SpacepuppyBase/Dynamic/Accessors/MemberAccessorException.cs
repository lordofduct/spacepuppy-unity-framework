//
// MemberAccessorException.cs
//
// Author: James Nies
// Licensed under The Code Project Open License (CPOL): http://www.codeproject.com/info/cpol10.aspx

using System;

namespace com.spacepuppy.Dynamic.Accessors
{
    /// <summary>
    /// PropertyAccessorException class.
    /// </summary>
    public class MemberAccessorException : Exception
    {
        internal MemberAccessorException(string message)
            : base(message)
        {
        }
    }
}