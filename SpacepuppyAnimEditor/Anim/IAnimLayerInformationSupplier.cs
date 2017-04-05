using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppyeditor.Anim
{

    public struct AnimLayerInformation
    {
        public string Name;
        public int Layer;

        public AnimLayerInformation(string name, int layer)
        {
            this.Name = name;
            this.Layer = layer;
        }
    }

    [System.AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    public class AnimLayerInformationSupplierConfigAttribute : System.Attribute
    {

        public readonly int Priority;

        public AnimLayerInformationSupplierConfigAttribute(int priority)
        {
            this.Priority = priority;
        }

    }

    public interface IAnimLayerInformationSupplier
    {

        AnimLayerInformation[] GetLayers();

    }

}
