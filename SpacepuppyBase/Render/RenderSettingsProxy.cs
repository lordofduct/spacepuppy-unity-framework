using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

using com.spacepuppy.Dynamic;

namespace com.spacepuppy.Render
{

    [CreateAssetMenu(fileName = "RenderSettingsProxy", menuName = "Services/RenderSettingsProxy")]
    public class RenderSettingsProxy : ScriptableObject, ITokenizable
    {

        #region Static Interface

        private static RenderSettingsProxy _default;
        public static RenderSettingsProxy Default
        {
            get
            {
                if (_default == null)
                {
                    _default = ScriptableObject.CreateInstance<RenderSettingsProxy>();
                }
                return _default;
            }
        }
        
        #endregion
        
        #region Properties

        public int DefaultReflectionResolution
        {
            get { return RenderSettings.defaultReflectionResolution; }
            set { RenderSettings.defaultReflectionResolution = value; }
        }
        public DefaultReflectionMode DefaultReflectionMode
        {
            get { return RenderSettings.defaultReflectionMode; }
            set { RenderSettings.defaultReflectionMode = value; }
        }
        public Light Sun
        {
            get { return RenderSettings.sun; }
            set { RenderSettings.sun = value; }
        }
        public Material Skybox
        {
            get { return RenderSettings.skybox; }
            set { RenderSettings.skybox = value; }
        }
        public float FlareFadeSpeed
        {
            get { return RenderSettings.flareFadeSpeed; }
            set { RenderSettings.flareFadeSpeed = value; }
        }
        public float FlareStrength
        {
            get { return RenderSettings.flareStrength; }
            set { RenderSettings.flareStrength = value; }
        }
        public float HaloStrength
        {
            get { return RenderSettings.haloStrength; }
            set { RenderSettings.haloStrength = value; }
        }
        public int ReflectionBounces
        {
            get { return RenderSettings.reflectionBounces; }
            set { RenderSettings.reflectionBounces = value; }
        }
        public float ReflectionIntensity
        {
            get { return RenderSettings.reflectionIntensity; }
            set { RenderSettings.reflectionIntensity = value; }
        }
        public Color SubtractiveShadowColor
        {
            get { return RenderSettings.subtractiveShadowColor; }
            set { RenderSettings.subtractiveShadowColor = value; }
        }
        public SphericalHarmonicsL2 AmbientProbe
        {
            get { return RenderSettings.ambientProbe; }
            set { RenderSettings.ambientProbe = value; }
        }
        public float AmbientIntensity
        {
            get { return RenderSettings.ambientIntensity; }
            set { RenderSettings.ambientIntensity = value; }
        }
        public Color AmbientLight
        {
            get { return RenderSettings.ambientLight; }
            set { RenderSettings.ambientLight = value; }
        }
        public Color AmbientGroundColor
        {
            get { return RenderSettings.ambientGroundColor; }
            set { RenderSettings.ambientGroundColor = value; }
        }
        public Color AmbientEquatorColor
        {
            get { return RenderSettings.ambientEquatorColor; }
            set { RenderSettings.ambientEquatorColor = value; }
        }
        public Color AmbientSkyColor
        {
            get { return RenderSettings.ambientSkyColor; }
            set { RenderSettings.ambientSkyColor = value; }
        }
        public AmbientMode AmbientMode
        {
            get { return RenderSettings.ambientMode; }
            set { RenderSettings.ambientMode = value; }
        }
        public float FogEndDistance
        {
            get { return RenderSettings.fogEndDistance; }
            set { RenderSettings.fogEndDistance = value; }
        }
        public float FogStartDistance
        {
            get { return RenderSettings.fogStartDistance; }
            set { RenderSettings.fogStartDistance = value; }
        }
        public float FogDensity
        {
            get { return RenderSettings.fogDensity; }
            set { RenderSettings.fogDensity = value; }
        }
        public Color FogColor
        {
            get { return RenderSettings.fogColor; }
            set { RenderSettings.fogColor = value; }
        }
        public FogMode FogMode
        {
            get { return RenderSettings.fogMode; }
            set { RenderSettings.fogMode = value; }
        }
        public bool Fog
        {
            get { return RenderSettings.fog; }
            set { RenderSettings.fog = value; }
        }
        public Cubemap CustomReflection
        {
            get { return RenderSettings.customReflection; }
            set { RenderSettings.customReflection = value; }
        }

        #endregion


        #region ITokenizable Interface

        object ITokenizable.CreateStateToken()
        {
            return this.CreateStateToken();
        }

        public Token CreateStateToken()
        {
            var obj = new Token();
            obj.DefaultReflectionResolution = this.DefaultReflectionResolution;
            obj.DefaultReflectionMode = this.DefaultReflectionMode;
            obj.Sun = this.Sun;
            obj.Skybox = this.Skybox;
            obj.FlareFadeSpeed = this.FlareFadeSpeed;
            obj.FlareStrength = this.FlareStrength;
            obj.HaloStrength = this.HaloStrength;
            obj.ReflectionBounces = this.ReflectionBounces;
            obj.ReflectionIntensity = this.ReflectionIntensity;
            obj.SubtractiveShadowColor = this.SubtractiveShadowColor;
            obj.AmbientProbe = this.AmbientProbe;
            obj.AmbientIntensity = this.AmbientIntensity;
            obj.AmbientLight = this.AmbientLight;
            obj.AmbientGroundColor = this.AmbientGroundColor;
            obj.AmbientEquatorColor = this.AmbientEquatorColor;
            obj.AmbientSkyColor = this.AmbientSkyColor;
            obj.AmbientMode = this.AmbientMode;
            obj.FogEndDistance = this.FogEndDistance;
            obj.FogStartDistance = this.FogStartDistance;
            obj.FogDensity = this.FogDensity;
            obj.FogColor = this.FogColor;
            obj.FogMode = this.FogMode;
            obj.Fog = this.Fog;
            obj.CustomReflection = this.CustomReflection;

            return obj;
        }

        public void RestoreFromStateToken(object token)
        {
            if(token is Token)
            {
                var obj = token as Token;
                this.DefaultReflectionResolution = obj.DefaultReflectionResolution;
                this.DefaultReflectionMode = obj.DefaultReflectionMode;
                this.Sun = obj.Sun;
                this.Skybox = obj.Skybox;
                this.FlareFadeSpeed = obj.FlareFadeSpeed;
                this.FlareStrength = obj.FlareStrength;
                this.HaloStrength = obj.HaloStrength;
                this.ReflectionBounces = obj.ReflectionBounces;
                this.ReflectionIntensity = obj.ReflectionIntensity;
                this.SubtractiveShadowColor = obj.SubtractiveShadowColor;
                this.AmbientProbe = obj.AmbientProbe;
                this.AmbientIntensity = obj.AmbientIntensity;
                this.AmbientLight = obj.AmbientLight;
                this.AmbientGroundColor = obj.AmbientGroundColor;
                this.AmbientEquatorColor = obj.AmbientEquatorColor;
                this.AmbientSkyColor = obj.AmbientSkyColor;
                this.AmbientMode = obj.AmbientMode;
                this.FogEndDistance = obj.FogEndDistance;
                this.FogStartDistance = obj.FogStartDistance;
                this.FogDensity = obj.FogDensity;
                this.FogColor = obj.FogColor;
                this.FogMode = obj.FogMode;
                this.Fog = obj.Fog;
                this.CustomReflection = obj.CustomReflection;
            }
            else
            {
                Dynamic.DynamicUtil.CopyState(this, token);
            }
        }

        #endregion

        #region Special Types

        [System.Serializable]
        public class Token
        {

            public int DefaultReflectionResolution;
            public DefaultReflectionMode DefaultReflectionMode;
            [System.NonSerialized]
            public Light Sun;
            [System.NonSerialized]
            public Material Skybox;
            public float FlareFadeSpeed;
            public float FlareStrength;
            public float HaloStrength;
            public int ReflectionBounces;
            public float ReflectionIntensity;
            public Color SubtractiveShadowColor;
            public SphericalHarmonicsL2 AmbientProbe;
            public float AmbientIntensity;
            public Color AmbientLight;
            public Color AmbientGroundColor;
            public Color AmbientEquatorColor;
            public Color AmbientSkyColor;
            public AmbientMode AmbientMode;
            public float FogEndDistance;
            public float FogStartDistance;
            public float FogDensity;
            public Color FogColor;
            public FogMode FogMode;
            public bool Fog;
            [System.NonSerialized]
            public Cubemap CustomReflection;

        }

        #endregion

    }

}
