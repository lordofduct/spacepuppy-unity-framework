using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy
{
    public interface ITimeSupplier
    {

        float Total { get; }
        float Delta { get; }

    }

    public interface IScalableTimeSupplier : ITimeSupplier
    {

        float Scale { get; }
        bool Paused { get; set; }


        IEnumerable<string> ScaleIds { get; }

        void SetScale(string id, float scale);
        float GetScale(string id);
        bool RemoveScale(string id);
        bool HasScale(string id);

    }

}
