using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

using com.spacepuppy;
using com.spacepuppy.Render;

namespace com.spacepuppyeditor.Render
{

    [CustomEditor(typeof(RenderSettingsProxy))]
    public class RenderSettingsProxyInpsector : SPEditor
    {
        
        protected override void OnSPInspectorGUI()
        {
            base.OnSPInspectorGUI();

            var obj = this.serializedObject.targetObject as RenderSettingsProxy;
            if(obj != null)
            {
                EditorGUILayout.HelpBox("This object is a proxy for the RenderSettings to be used as a tween/set target at runtime. The values below are just a reflection of the current render setting's values for reference purposes. You shouldn't really modify these and instead use Window->Lighting instead.", MessageType.Warning);

                if(Application.isPlaying)
                {
                    obj.Skybox = EditorGUILayout.ObjectField("Skybox", obj.Skybox, typeof(Material), false) as Material;
                    obj.Sun = EditorGUILayout.ObjectField("Sun", obj.Sun, typeof(Light), false) as Light;

                    EditorGUILayout.Space();

                    obj.Fog = EditorGUILayout.Toggle("Fog", obj.Fog);
                    obj.FogMode = (FogMode)EditorGUILayout.EnumPopup("FogMode", obj.FogMode);
                    obj.FogColor = EditorGUILayout.ColorField("FogColor", obj.FogColor);
                    obj.FogStartDistance = EditorGUILayout.FloatField("FogStartDistance", obj.FogStartDistance);
                    obj.FogDensity = EditorGUILayout.FloatField("FogDensity", obj.FogDensity);
                    obj.FogEndDistance = EditorGUILayout.FloatField("FogEndDistance", obj.FogEndDistance);

                    EditorGUILayout.Space();

                    obj.AmbientMode = (AmbientMode)EditorGUILayout.EnumPopup("AmbientMode", obj.AmbientMode);
                    obj.AmbientIntensity = EditorGUILayout.FloatField("AmbientIntensity", obj.AmbientIntensity);
                    obj.AmbientLight = EditorGUILayout.ColorField("AmbientLight", obj.AmbientLight);
                    obj.AmbientGroundColor = EditorGUILayout.ColorField("AmbientGroundColor", obj.AmbientGroundColor);
                    obj.AmbientEquatorColor = EditorGUILayout.ColorField("AmbientEquatorColor", obj.AmbientEquatorColor);
                    obj.AmbientSkyColor = EditorGUILayout.ColorField("AmbientSkyColor", obj.AmbientSkyColor);

                    EditorGUILayout.Space();

                    obj.DefaultReflectionMode = (DefaultReflectionMode)EditorGUILayout.EnumPopup("DefaultReflectionMode", obj.DefaultReflectionMode);
                    obj.DefaultReflectionResolution = EditorGUILayout.IntField("DefaultReflectionResolution", obj.DefaultReflectionResolution);
                    obj.ReflectionIntensity = EditorGUILayout.FloatField("ReflectionIntensity", obj.ReflectionIntensity);
                    obj.ReflectionBounces = EditorGUILayout.IntField("ReflectionBounces", obj.ReflectionBounces);
                    obj.CustomReflection = EditorGUILayout.ObjectField("CustomReflection", obj.CustomReflection, typeof(Cubemap), false) as Cubemap;

                    EditorGUILayout.Space();

                    obj.FlareFadeSpeed = EditorGUILayout.FloatField("FlareFadeSpeed", obj.FlareFadeSpeed);
                    obj.FlareStrength = EditorGUILayout.FloatField("FlareStrength", obj.FlareStrength);
                    obj.HaloStrength = EditorGUILayout.FloatField("HaloStrength", obj.HaloStrength);
                    obj.SubtractiveShadowColor = EditorGUILayout.ColorField("SubtractiveShadowColor", obj.SubtractiveShadowColor);
                    //obj.AmbientProbe = EditorGUILayout...("AmbientProbe", obj.AmbientProbe);
                }
                else
                {
                    EditorGUILayout.ObjectField("Skybox", obj.Skybox, typeof(Material), false);
                    EditorGUILayout.ObjectField("Sun", obj.Sun, typeof(Light), false);

                    EditorGUILayout.Space();

                    EditorGUILayout.Toggle("Fog", obj.Fog);
                    EditorGUILayout.EnumPopup("FogMode", obj.FogMode);
                    EditorGUILayout.ColorField("FogColor", obj.FogColor);
                    EditorGUILayout.FloatField("FogStartDistance", obj.FogStartDistance);
                    EditorGUILayout.FloatField("FogDensity", obj.FogDensity);
                    EditorGUILayout.FloatField("FogEndDistance", obj.FogEndDistance);

                    EditorGUILayout.Space();

                    EditorGUILayout.EnumPopup("AmbientMode", obj.AmbientMode);
                    EditorGUILayout.FloatField("AmbientIntensity", obj.AmbientIntensity);
                    EditorGUILayout.ColorField("AmbientLight", obj.AmbientLight);
                    EditorGUILayout.ColorField("AmbientGroundColor", obj.AmbientGroundColor);
                    EditorGUILayout.ColorField("AmbientEquatorColor", obj.AmbientEquatorColor);
                    EditorGUILayout.ColorField("AmbientSkyColor", obj.AmbientSkyColor);

                    EditorGUILayout.Space();

                    EditorGUILayout.EnumPopup("DefaultReflectionMode", obj.DefaultReflectionMode);
                    EditorGUILayout.IntField("DefaultReflectionResolution", obj.DefaultReflectionResolution);
                    EditorGUILayout.FloatField("ReflectionIntensity", obj.ReflectionIntensity);
                    EditorGUILayout.IntField("ReflectionBounces", obj.ReflectionBounces);
                    EditorGUILayout.ObjectField("CustomReflection", obj.CustomReflection, typeof(Cubemap), false);

                    EditorGUILayout.Space();

                    EditorGUILayout.FloatField("FlareFadeSpeed", obj.FlareFadeSpeed);
                    EditorGUILayout.FloatField("FlareStrength", obj.FlareStrength);
                    EditorGUILayout.FloatField("HaloStrength", obj.HaloStrength);
                    EditorGUILayout.ColorField("SubtractiveShadowColor", obj.SubtractiveShadowColor);
                    //EditorGUILayout...("AmbientProbe", obj.AmbientProbe);
                }
            }
        }

    }

}
