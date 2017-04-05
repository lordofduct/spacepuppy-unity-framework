using UnityEngine;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Anim
{

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

        #endregion

        #region Static Interface

        public static AnimSettings Default
        {
            get
            {
                return new AnimSettings()
                {
                    weight = 1f,
                    speed = 1f,
                    layer = 0,
                    wrapMode = WrapMode.Default,
                    blendMode = AnimationBlendMode.Blend
                };
            }
        }

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

        #endregion

    }
}
