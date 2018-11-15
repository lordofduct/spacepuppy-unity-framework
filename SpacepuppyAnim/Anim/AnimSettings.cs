using UnityEngine;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Anim
{

    [System.Flags]
    public enum AnimSettingsMask
    {
        Weight = 1,
        Speed = 2,
        Layer = 4,
        WrapMode = 8,
        BlendMode = 16,
        TimeSupplier = 32
    }

    [System.Serializable]
    public struct AnimSettings
    {

        #region Fields

        public float weight;
        public float speed;
        public int layer;
        public WrapMode wrapMode;
        public AnimationBlendMode blendMode;
        public SPTime timeSupplier;

        #endregion

        #region Methods

        public void Apply(SPAnim anim)
        {
            anim.Weight = this.weight;
            anim.Speed = this.speed;
            anim.Layer = this.layer;
            anim.WrapMode = this.wrapMode;
            anim.BlendMode = this.blendMode;
            anim.TimeSupplier = this.timeSupplier.TimeSupplier;
        }

        public void Apply(AnimationState anim)
        {
            anim.weight = this.weight;
            anim.speed = this.speed;
            anim.layer = this.layer;
            anim.wrapMode = this.wrapMode;
            anim.blendMode = this.blendMode;
            if(this.timeSupplier.IsCustom)
            {
                anim.speed = this.speed * SPTime.GetInverseScale(this.timeSupplier.TimeSupplier);
            }
        }
        
        public void Apply(ISPAnim anim)
        {
            if(anim is SPAnim)
            {
                this.Apply(anim as SPAnim);
                return;
            }
            //can't set weight
            anim.Speed = this.speed;
            anim.Layer = this.layer;
            anim.WrapMode = this.wrapMode;
            //can't set blend mode
            anim.TimeSupplier = this.timeSupplier.TimeSupplier;
        }

        public void Apply(SPAnim anim, AnimSettingsMask mask)
        {
            if ((mask & AnimSettingsMask.Weight) != 0) anim.Weight = this.weight;
            if ((mask & AnimSettingsMask.Speed) != 0) anim.Speed = this.speed;
            if ((mask & AnimSettingsMask.Layer) != 0) anim.Layer = this.layer;
            if ((mask & AnimSettingsMask.WrapMode) != 0) anim.WrapMode = this.wrapMode;
            if ((mask & AnimSettingsMask.BlendMode) != 0) anim.BlendMode = this.blendMode;
            if ((mask & AnimSettingsMask.TimeSupplier) != 0) anim.TimeSupplier = this.timeSupplier.TimeSupplier;
        }

        public void Apply(AnimationState anim, AnimSettingsMask mask)
        {
            if ((mask & AnimSettingsMask.Weight) != 0) anim.weight = this.weight;
            if ((mask & AnimSettingsMask.Speed) != 0) anim.speed = this.speed;
            if ((mask & AnimSettingsMask.Layer) != 0) anim.layer = this.layer;
            if ((mask & AnimSettingsMask.WrapMode) != 0) anim.wrapMode = this.wrapMode;
            if ((mask & AnimSettingsMask.BlendMode) != 0) anim.blendMode = this.blendMode;
            if ((mask & AnimSettingsMask.TimeSupplier) != 0 && this.timeSupplier.IsCustom)
            {
                anim.speed = this.speed * SPTime.GetInverseScale(this.timeSupplier.TimeSupplier);
            }
        }

        public void Apply(ISPAnim anim, AnimSettingsMask mask)
        {
            if (anim is SPAnim)
            {
                this.Apply(anim as SPAnim, mask);
                return;
            }
            //can't set weight
            if ((mask & AnimSettingsMask.Speed) != 0) anim.Speed = this.speed;
            if ((mask & AnimSettingsMask.Layer) != 0) anim.Layer = this.layer;
            if ((mask & AnimSettingsMask.WrapMode) != 0) anim.WrapMode = this.wrapMode;
            //can't set blend mode
            if ((mask & AnimSettingsMask.TimeSupplier) != 0) anim.TimeSupplier = this.timeSupplier.TimeSupplier;
        }

        #endregion

        #region Static Interface

        public static readonly AnimSettings Default = new AnimSettings()
        {
            weight = 1f,
            speed = 1f,
            layer = 0,
            wrapMode = WrapMode.Default,
            blendMode = AnimationBlendMode.Blend
        };

        public static AnimSettings From(SPAnim anim)
        {
            return new AnimSettings()
            {
                weight = anim.Weight,
                speed = anim.Speed,
                layer = anim.Layer,
                wrapMode = anim.WrapMode,
                blendMode = anim.BlendMode,
                timeSupplier = new SPTime(anim.TimeSupplier)
            };
        }

        public static AnimSettings From(AnimationState anim)
        {
            return new AnimSettings()
            {
                weight = anim.weight,
                speed = anim.speed,
                layer = anim.layer,
                wrapMode = anim.wrapMode,
                blendMode = anim.blendMode
            };
        }

        public static AnimSettings Intersect(AnimSettings settings, AnimSettings with, AnimSettingsMask mask)
        {
            if ((mask & AnimSettingsMask.Weight) != 0) settings.weight = with.weight;
            if ((mask & AnimSettingsMask.Speed) != 0) settings.speed = with.speed;
            if ((mask & AnimSettingsMask.Layer) != 0) settings.layer = with.layer;
            if ((mask & AnimSettingsMask.WrapMode) != 0) settings.wrapMode = with.wrapMode;
            if ((mask & AnimSettingsMask.BlendMode) != 0) settings.blendMode = with.blendMode;
            if ((mask & AnimSettingsMask.TimeSupplier) != 0) settings.timeSupplier = with.timeSupplier;
            return settings;
        }

        #endregion

    }
}
