using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.spacepuppy.Tween;

namespace com.spacepuppy.Dynamic
{

    public static class DynamicTweenExtension
    {

        public static TweenHash TweenWithToken(this TweenHash hash, TweenHash.AnimMode mode, com.spacepuppy.Tween.Ease ease, float dur, object valueToken, object valueAltToken)
        {
            if (hash == null || valueToken == null) return hash;

            object value;
            if (valueToken is IStateToken)
            {
                var st = valueToken as IStateToken;
                foreach (var sname in st.GetMemberNames(false))
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
                            hash.ByAnimMode(mode, sname, ease, dur, value, DynamicUtil.GetValue(valueAltToken, sname));
                            break;
                    }
                }
            }
            else
            {
                foreach (var sname in DynamicUtil.GetMemberNames(valueToken, false, System.Reflection.MemberTypes.Field | System.Reflection.MemberTypes.Property))
                {
                    value = DynamicUtil.GetValue(valueToken, sname);
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
                            hash.ByAnimMode(mode, sname, ease, dur, value, DynamicUtil.GetValue(valueAltToken, sname));
                            break;
                    }
                }
            }

            return hash;
        }

        public static TweenHash TweenToToken(this TweenHash hash, com.spacepuppy.Tween.Ease ease, float dur, object token)
        {
            if (hash == null || token == null) return hash;

            object value;
            if (token is IStateToken)
            {
                var st = token as IStateToken;
                foreach (var sname in st.GetMemberNames(false))
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
                            hash.To(sname, ease, dur, value);
                            break;
                    }
                }
            }
            else
            {
                foreach (var sname in DynamicUtil.GetMemberNames(token, false, System.Reflection.MemberTypes.Field | System.Reflection.MemberTypes.Property))
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
                            hash.To(sname, ease, dur, value);
                            break;
                    }
                }
            }

            return hash;
        }

    }

}
