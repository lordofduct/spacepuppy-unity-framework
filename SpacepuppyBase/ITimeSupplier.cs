using System.Collections.Generic;

namespace com.spacepuppy
{

    /// <summary>
    /// Represents an object that supplies the current game time. See com.spacepuppy.SPTime.
    /// </summary>
    public interface ITimeSupplier
    {

        string Id { get; }

        float Total { get; }
        float Delta { get; }
        float Scale { get; }

        double TotalPrecise { get; }

    }

    /// <summary>
    /// A ITimeSupplier that has a scale property (this gives things like Time.timeScale an object identity).  See com.spacepuppy.SPTime.
    /// </summary>
    public interface IScalableTimeSupplier : ITimeSupplier
    {

        event System.EventHandler TimeScaleChanged;

        bool Paused { get; set; }


        IEnumerable<string> ScaleIds { get; }

        void SetScale(string id, float scale);
        float GetScale(string id);
        bool RemoveScale(string id);
        bool HasScale(string id);

    }

    /// <summary>
    /// Implement this interface if you want your custom time supplier type to receive update from SPTime when registered.
    /// </summary>
    public interface ICustomTimeSupplier : ITimeSupplier
    {
        void Update(bool isFixed);
    }

}
