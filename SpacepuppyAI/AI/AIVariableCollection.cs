using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.AI
{

    [System.Serializable()]
    public class AIVariableCollection : VariantCollection
    {
        
        public AIVariableCollection()
        {

        }

        
        public ComplexTarget GetAsComplexTarget(string key)
        {
            var v = this.GetVariant(key);
            if (v == null) return ComplexTarget.Null;

            switch(v.ValueType)
            {
                case VariantType.Object:
                    return ComplexTarget.FromObject(v.Value);
                case VariantType.Vector2:
                    return new ComplexTarget(v.Vector2Value);
                case VariantType.Vector3:
                case VariantType.Vector4:
                    return new ComplexTarget(v.Vector3Value);
                case VariantType.GameObject:
                case VariantType.Component:
                    return ComplexTarget.FromObject(v.Value);
                default:
                    return ComplexTarget.Null;
            }
        }

        public void SetAsComplexTarget(string key, ComplexTarget target)
        {
            switch(target.TargetType)
            {
                case ComplexTargetType.Null:
                    this.GetVariant(key, true).Value = null;
                    break;
                case ComplexTargetType.Aspect:
                    this.GetVariant(key, true).Value = target.TargetAspect;
                    break;
                case ComplexTargetType.Transform:
                    this.GetVariant(key, true).Value = target.Transform;
                    break;
                case ComplexTargetType.Vector2:
                    this.GetVariant(key, true).Value = target.Position2D;
                    break;
                case ComplexTargetType.Vector3:
                    this.GetVariant(key, true).Value = target.Position;
                    break;
            }
        }
        
    }
}
