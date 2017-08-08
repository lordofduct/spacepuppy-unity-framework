using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

using com.spacepuppy.Dynamic;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Audio
{
    public class GlobalAmbientAudioSourceProxy : MonoBehaviour, IDynamic, IProxy
    {
        

        public AudioSource Target
        {
            get
            {
                var manager = Services.Get<IAudioManager>();
                return manager != null ? manager.BackgroundAmbientAudioSource : null;
            }
        }

        #region IDynamic Interface

        object IDynamic.this[string sMemberName]
        {
            get
            {
                return DynamicUtil.GetValueDirect(this.Target, sMemberName);
            }
            set
            {
                DynamicUtil.SetValueDirect(this.Target, sMemberName, value);
            }
        }

        MemberInfo IDynamic.GetMember(string sMemberName, bool includeNonPublic)
        {
            return DynamicUtil.GetMemberFromType(typeof(AudioSource), sMemberName, includeNonPublic);
        }

        IEnumerable<MemberInfo> IDynamic.GetMembers(bool includeNonPublic)
        {
            return DynamicUtil.GetMembersFromType(typeof(AudioSource), includeNonPublic);
        }

        object IDynamic.GetValue(string sMemberName, params object[] args)
        {
            return DynamicUtil.GetValue(this.Target, sMemberName, args);
        }

        bool IDynamic.HasMember(string sMemberName, bool includeNonPublic)
        {
            return DynamicUtil.TypeHasMember(typeof(AudioSource), sMemberName, includeNonPublic);
        }

        object IDynamic.InvokeMethod(string sMemberName, params object[] args)
        {
            return DynamicUtil.InvokeMethodDirect(this.Target, sMemberName, args);
        }

        bool IDynamic.SetValue(string sMemberName, object value, params object[] index)
        {
            return DynamicUtil.SetValueDirect(this.Target, sMemberName, value, index);
        }

        bool IDynamic.TryGetValue(string sMemberName, out object result, params object[] args)
        {
            return DynamicUtil.TryGetValueDirect(this.Target, sMemberName, out result, args);
        }

        #endregion

        #region IProxy Interface

        object IProxy.GetTarget()
        {
            return this.Target;
        }

        object IProxy.GetTarget(object arg)
        {
            return this.Target;
        }

        System.Type IProxy.GetTargetType()
        {
            return typeof(AudioSource);
        }

        #endregion

    }
}
