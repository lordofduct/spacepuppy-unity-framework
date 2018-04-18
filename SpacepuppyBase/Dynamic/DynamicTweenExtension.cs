using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.spacepuppy.Tween;

namespace com.spacepuppy.Dynamic
{

    public static class DynamicTweenExtension
    {

        public static TweenHash TweenToToken(this TweenHash hash, object token, com.spacepuppy.Tween.Ease ease, float dur)
        {
            if (hash == null || token == null) return hash;
            
            object value;
            if(token is IStateToken)
            {
                var st = token as IStateToken;
                foreach(var sname in st.GetMemberNames(false))
                {
                    value = st.GetValue(sname);
                    if (value == null) continue;

                    switch (VariantReference.GetVariantType(value.GetType()))
                    {
                        case VariantType.String:
                        case VariantType.Boolean:
                        case VariantType.Integer:
                        case VariantType.Float:
                        case VariantType.Double:
                        case VariantType.Vector2:
                        case VariantType.Vector3:
                        case VariantType.Vector4:
                        case VariantType.Quaternion:
                        case VariantType.Color:
                        case VariantType.Rect:
                            hash.To(sname, ease, value, dur);
                            break;
                    }
                }
            }
            else
            {
                foreach(var sname in DynamicUtil.GetMemberNames(token, false, System.Reflection.MemberTypes.Field | System.Reflection.MemberTypes.Property))
                {
                    value = DynamicUtil.GetValue(token, sname);
                    if (value == null) continue;

                    switch (VariantReference.GetVariantType(value.GetType()))
                    {
                        case VariantType.String:
                        case VariantType.Boolean:
                        case VariantType.Integer:
                        case VariantType.Float:
                        case VariantType.Double:
                        case VariantType.Vector2:
                        case VariantType.Vector3:
                        case VariantType.Vector4:
                        case VariantType.Quaternion:
                        case VariantType.Color:
                        case VariantType.Rect:
                            hash.To(sname, ease, value, dur);
                            break;
                    }
                }
            }

            return hash;
        }

    }

}
