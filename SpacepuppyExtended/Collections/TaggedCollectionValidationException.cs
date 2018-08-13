using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Collections
{
    public class TaggedCollectionValidationException : System.Exception
    {
        private const string MSG = "Tag and Value failed to match in validation.";

        public TaggedCollectionValidationException()
            : base(MSG)
        {

        }

        public TaggedCollectionValidationException(System.Exception innerException)
            : base(MSG, innerException)
        {

        }

    }
}
