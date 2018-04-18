using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Dynamic;
using com.spacepuppy.Tween;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Waypoints
{

    /// <summary>
    /// Used to translate a target Vector3 property of any object with name 'propName' along a IWaypointPath.
    /// </summary>
    public class WaypointPathTweenCurve : MemberCurve
    {

        #region Fields

        private IWaypointPath _path;
        private float _redact;

        #endregion

        #region CONSTRUCTOR

        protected WaypointPathTweenCurve()
        {
            //REQUIRED - all MemberCurves need a zero-param constructor
        }

        public WaypointPathTweenCurve(string propName, float dur, IWaypointPath path)
            : base(propName, dur)
        {
            _path = path;
        }

        public WaypointPathTweenCurve(string propName, Ease ease, float dur, IWaypointPath path)
            : base(propName, ease, dur)
        {
            _path = path;
        }

        #endregion

        #region Properties

        public IWaypointPath Path
        {
            get { return _path; }
            set { _path = value; }
        }

        public float ArcLengthRedaction
        {
            get { return _redact; }
            set { _redact = value; }
        }

        #endregion

        #region MemberCurve Interface

        protected override void ReflectiveInit(object start, object end, object option)
        {
            //do nothing
        }

        protected override object GetValueAt(float dt, float t)
        {
            if (_path == null) return Vector3.zero;

            t = this.Ease(t, 0f, 1f, this.Duration);
            if (_redact != 0f)
            {
                float rt = _redact / _path.GetArcLength();
                t -= rt;
            }
            return _path.GetPositionAt(t);
        }

        #endregion

    }


    /// <summary>
    /// Used to update a target across a WaypointPathComponent. This will update the Transform position and rotation, as well as attempt any modifier 
    /// property updates based on modifiers attached to the waypoint.
    /// </summary>
    public class AdvancedWaypointPathTweenCurve : TweenCurve
    {
        
        public enum TranslationOptions
        {
            None = 0,
            Position = 1,
            LocalPosition = 2
        }

        public enum RotationOptions
        {
            None = 0,
            Rotation = 1,
            LocalRotation = 2,
            Heading = 3,
            LocalHeading = 4
        }

        #region Fields

        private Ease _ease;
        private float _dur;
        private float _delay;

        private WaypointPathComponent _path;

        private TranslationOptions _updateTranslation;
        private RotationOptions _updateRotation;
        private Dictionary<System.Type, IStateModifier[]> _modifierTable;

        #endregion

        #region CONSTRUCTOR

        public AdvancedWaypointPathTweenCurve(float dur, WaypointPathComponent path)
            : base()
        {
            _ease = EaseMethods.LinearEaseNone;
            _dur = dur;
            _delay = 0f;
            _path = path;
        }

        public AdvancedWaypointPathTweenCurve(Ease ease, float dur, WaypointPathComponent path)
            : base()
        {
            _ease = ease ?? EaseMethods.LinearEaseNone;
            _dur = dur;
            _path = path;
            _delay = 0f;
        }

        public AdvancedWaypointPathTweenCurve(float dur, float delay, WaypointPathComponent path)
            : base()
        {
            _ease = EaseMethods.LinearEaseNone;
            _dur = dur;
            _path = path;
            _delay = delay;
        }

        public AdvancedWaypointPathTweenCurve(Ease ease, float dur, float delay, WaypointPathComponent path)
            : base()
        {
            _ease = ease ?? EaseMethods.LinearEaseNone;
            _dur = dur;
            _path = path;
            _delay = delay;
        }

        #endregion

        #region Properties

        public float Duration
        {
            get { return _dur; }
            set { _dur = value; }
        }

        public float Delay
        {
            get { return _delay; }
            set { _delay = value; }
        }

        public Ease Ease
        {
            get { return _ease; }
            set { _ease = value ?? EaseMethods.LinearEaseNone; }
        }

        public WaypointPathComponent Path
        {
            get { return _path; }
        }


        public TranslationOptions UpdateTranslation
        {
            get { return _updateTranslation; }
            set { _updateTranslation = value; }
        }

        public RotationOptions UpdateRotation
        {
            get { return _updateRotation; }
            set { _updateRotation = value; }
        }

        public bool HasModifiers
        {
            get { return _modifierTable != null && _modifierTable.Count > 0; }
        }
        
        #endregion

        #region Methods

        public void AddNodeModifierType<T>() where T : IStateModifier
        {
            if (_modifierTable == null)
                _modifierTable = new Dictionary<System.Type, IStateModifier[]>();

            var path = _path.Path;
            int cnt = path.Count;
            IStateModifier[] arr = new IStateModifier[cnt];
            for (int i = 0; i < cnt; i++)
            {
                var p = GameObjectUtil.GetGameObjectFromSource(path.ControlPoint(i));
                if (p != null) arr[i] = p.GetComponent<T>() as IStateModifier;
            }

            _modifierTable[typeof(T)] = arr;
        }

        public void AddNodeModifierType(System.Type tp)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");
            if (!TypeUtil.IsType(tp, typeof(IStateModifier))) return; // throw new System.ArgumentException("Type must implement IModifier.", "tp");

            if (_modifierTable == null)
                _modifierTable = new Dictionary<System.Type, IStateModifier[]>();

            var path = _path.Path;
            int cnt = path.Count;
            IStateModifier[] arr = new IStateModifier[cnt];
            for(int i = 0; i < cnt; i++)
            {
                var p = GameObjectUtil.GetGameObjectFromSource(path.ControlPoint(i));
                if(p != null) arr[i] = p.GetComponent(tp) as IStateModifier;
            }

            _modifierTable[tp] = arr;
        }

        public bool RemoveNodeModifierType<T>() where T : IStateModifier
        {
            return _modifierTable.Remove(typeof(T));
        }

        public bool RemoveNodeModifierType(System.Type tp)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");
            if (!TypeUtil.IsType(tp, typeof(IStateModifier))) return false; // throw new System.ArgumentException("Type must implement IStateModifier.", "tp");
            return _modifierTable.Remove(tp);
        }

        public bool ContainsNodeModifier<T>() where T : IStateModifier
        {
            return _modifierTable.ContainsKey(typeof(T));
        }

        public bool ContainsNodeModifier(System.Type tp)
        {
            return _modifierTable.ContainsKey(tp);
        }

        public void SetToTarget(object targ, float t)
        {
            this.LerpToTarget(targ, t, float.NaN);
        }

        public void LerpToTarget(object targ, float t, float lerpT)
        {
            if (_dur == 0f)
                t = 1f;
            else
                t = t / _dur;

            if (_updateTranslation > TranslationOptions.None || _updateRotation >= RotationOptions.Heading)
            {
                var trans = GameObjectUtil.GetTransformFromSource(targ);
                if (trans != null)
                {
                    if (_updateRotation == RotationOptions.Heading)
                    {
                        var wp = _path.Path.GetWaypointAt(t);
                        this.SetPosition(trans, wp.Position, float.NaN);
                        this.SetRotation(trans, Quaternion.LookRotation(wp.Heading), lerpT);
                    }
                    else if (_updateTranslation > TranslationOptions.None)
                    {
                        this.SetPosition(trans, _path.Path.GetPositionAt(t), lerpT);
                    }
                }
            }

            bool useModifiers = (_modifierTable != null && _modifierTable.Count > 0);
            if (useModifiers || _updateRotation == RotationOptions.Rotation || _updateRotation == RotationOptions.LocalRotation)
            {
                var data = _path.Path.GetRelativePositionData(t);

                var cnt = _path.Path.Count;
                int i = data.Index;
                int j = (_path.Path.IsClosed) ? (i + 1) % cnt : i + 1;

                if (_updateRotation == RotationOptions.Rotation || _updateRotation == RotationOptions.LocalRotation)
                {
                    var trans = GameObjectUtil.GetTransformFromSource(targ);
                    if (trans != null)
                    {
                        var a = (i >= 0 && i < cnt) ? ComponentUtil.GetComponentFromSource<Transform>(_path.Path.ControlPoint(i)) : null;
                        var b = (j >= 0 && j < cnt) ? ComponentUtil.GetComponentFromSource<Transform>(_path.Path.ControlPoint(j)) : null;

                        if (a != null)
                        {
                            bool useRelative = _path.TransformRelativeTo != null;
                            var r = (useRelative) ? a.GetRelativeRotation(_path.TransformRelativeTo) : a.rotation;
                            if (b != null)
                            {
                                var rb = (useRelative) ? b.GetRelativeRotation(_path.TransformRelativeTo) : b.rotation;
                                r = Quaternion.LerpUnclamped(r, rb, data.TPrime);
                            }
                            this.SetRotation(trans, r, lerpT);
                        }
                    }
                }

                if (useModifiers)
                {
                    var e = _modifierTable.GetEnumerator();
                    while (e.MoveNext())
                    {
                        var len = e.Current.Value.Length;
                        var ma = (i >= 0 && i < len) ? e.Current.Value[i] : null;
                        var mb = (j >= 0 && j < len) ? e.Current.Value[j] : null;

                        if(float.IsNaN(lerpT))
                        {
                            if (ma != null)
                            {
                                if (mb != null)
                                {
                                    using (var state = StateToken.GetTempToken())
                                    {
                                        ma.CopyTo(state);
                                        mb.LerpTo(state, data.TPrime);
                                        ma.ModifyWith(targ, state);
                                    }
                                }
                                else
                                {
                                    ma.Modify(targ);
                                }
                            }
                            else if (mb != null)
                            {
                                mb.Modify(targ);
                            }
                        }
                        else
                        {
                            using (var curState = StateToken.GetTempToken())
                            using (var state = StateToken.GetTempToken())
                            {
                                IStateModifier m = null;
                                if(ma != null)
                                {
                                    m = ma;
                                    if(mb != null)
                                    {
                                        ma.CopyTo(state);
                                        mb.LerpTo(state, data.TPrime);
                                    }
                                    else
                                    {
                                        ma.CopyTo(state);
                                    }
                                }
                                else if(mb != null)
                                {
                                    m = mb;
                                    mb.CopyTo(state);
                                }

                                if(m != null)
                                {
                                    state.CopyTo(curState);
                                    curState.SyncFrom(targ);
                                    curState.LerpTo(state, lerpT);
                                    m.ModifyWith(state, curState);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SetPosition(Transform trans, Vector3 pos, float lerpT)
        {
            if(!float.IsNaN(lerpT))
            {
                switch (_updateTranslation)
                {
                    case TranslationOptions.Position:
                        trans.position = Vector3.LerpUnclamped(trans.position, pos, lerpT);
                        break;
                    case TranslationOptions.LocalPosition:
                        trans.localPosition = Vector3.LerpUnclamped(trans.localPosition, pos, lerpT);
                        break;
                }
            }
            else
            {
                switch (_updateTranslation)
                {
                    case TranslationOptions.Position:
                        trans.position = pos;
                        break;
                    case TranslationOptions.LocalPosition:
                        trans.localPosition = pos;
                        break;
                }
            }
        }

        private void SetRotation(Transform trans, Quaternion rot, float lerpT)
        {
            if (!float.IsNaN(lerpT))
            {
                switch (_updateRotation)
                {
                    case RotationOptions.Rotation:
                    case RotationOptions.Heading:
                        trans.rotation = Quaternion.LerpUnclamped(trans.rotation, rot, lerpT);
                        break;
                    case RotationOptions.LocalRotation:
                    case RotationOptions.LocalHeading:
                        trans.localRotation = Quaternion.LerpUnclamped(trans.localRotation, rot, lerpT);
                        break;
                }
            }
            else
            {
                switch (_updateRotation)
                {
                    case RotationOptions.Rotation:
                    case RotationOptions.Heading:
                        trans.rotation = rot;
                        break;
                    case RotationOptions.LocalRotation:
                    case RotationOptions.LocalHeading:
                        trans.localRotation = rot;
                        break;
                }
            }
        }

        #endregion

        #region TweenCurve Interface

        public override float TotalTime
        {
            get
            {
                return _dur + _delay;
            }
        }

        public override void Update(object targ, float dt, float t)
        {
            if (_path == null || targ == null) return;

            t -= _delay;
            t = this.Ease(t, 0f, 1f, this.Duration) * this.Duration;

            this.SetToTarget(targ, t);
        }

        #endregion
        
    }

}
