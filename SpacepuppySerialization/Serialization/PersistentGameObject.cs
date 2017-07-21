using UnityEngine;
using System.Collections.Generic;

using System.Runtime.Serialization;

using com.spacepuppy.Geom;
using com.spacepuppy.Project;
using com.spacepuppy.Utils;
using System;

namespace com.spacepuppy.Serialization
{
    
    public class PersistentGameObject : MonoBehaviour, IPersistantAsset
    {

        #region Fields

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("PrefabId")]
        private string _assetId;

        #endregion

        #region Properties

        public string AssetId
        {
            get { return _assetId; }
            set { _assetId = value; }
        }

        #endregion

        #region IPersistantUnityObject Interface

        string IPersistantAsset.AssetId { get { return _assetId; } }

        void IPersistantUnityObject.OnSerialize(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("pos", this.transform.position);
            info.AddValue("rot", this.transform.rotation);
            info.AddValue("scale", this.transform.localScale);
            
            var arr = this.GetComponentsInChildren<IPersistantUnityObject>();
            if(arr.Length > 0)
            {
                var data = new ChildObjectData();
                int cnt = 0;

                for (int i = 0; i < arr.Length; i++)
                {
                    if (object.ReferenceEquals(this, arr[i])) continue;

                    data.Path = GameObjectUtil.GetPathNameRelativeTo((arr[i] as Component).transform, this.transform);
                    data.ComponentType = arr[i].GetType();
                    data.Pobj = arr[i];
                    info.AddValue(cnt.ToString(), data, typeof(ChildObjectData));
                    cnt++;
                }
                info.AddValue("count", cnt);
            }
        }

        void IPersistantUnityObject.OnDeserialize(SerializationInfo info, StreamingContext context, IAssetBundle assetBundle)
        {
            this.transform.position = (Vector3)info.GetValue("pos", typeof(Vector3));
            this.transform.rotation = (Quaternion)info.GetValue("rot", typeof(Quaternion));
            this.transform.localScale = (Vector3)info.GetValue("scale", typeof(Vector3));

            int cnt = info.GetInt32("count");
            for(int i = 0; i < cnt; i++)
            {
                ChildObjectData data = (ChildObjectData)info.GetValue(i.ToString(), typeof(ChildObjectData));
                if (data != null && data.ComponentType != null)
                {
                    IPersistantUnityObject pobj = ComponentUtil.GetComponentFromSource(data.ComponentType, (data.Path != null) ? this.transform.Find(data.Path) : this.transform) as IPersistantUnityObject;
                    if (pobj != null)
                    {
                        pobj.OnDeserialize(data.DeserializeInfo, data.DeserializeContext, assetBundle);
                    }
                }
            }
        }

        #endregion

        #region Special Types

        [System.Serializable()]
        private class ChildObjectData : ISerializable
        {

            [System.NonSerialized()]
            public string Path;
            [System.NonSerialized()]
            public System.Type ComponentType;

            [System.NonSerialized()]
            public IPersistantUnityObject Pobj;
            [System.NonSerialized()]
            public SerializationInfo DeserializeInfo;
            [System.NonSerialized()]
            public StreamingContext DeserializeContext;

            public ChildObjectData()
            {
            }


            public ChildObjectData(SerializationInfo info, StreamingContext context)
            {
                this.DeserializeInfo = info;
                this.DeserializeContext = context;
                this.Path = info.GetString("sp_p");
                this.ComponentType = info.GetValue("sp_t", typeof(System.Type)) as System.Type;
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("sp_p", this.Path);
                info.AddValue("sp_t", this.ComponentType, typeof(System.Type));
                if (Pobj != null) Pobj.OnSerialize(info, context);
            }
            
        }

        #endregion

    }
}
