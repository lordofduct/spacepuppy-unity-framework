using UnityEngine;
using System.Collections;

namespace com.spacepuppy.Hooks
{

    [AddComponentMenu("SpacePuppy/Hooks/UpdateEventHooks")]
    public class UpdateEventHooks : com.spacepuppy.SPComponent
    {

        public event System.EventHandler UpdateHook;
        public event System.EventHandler FixedUpdateHook;
        public event System.EventHandler LateUpdateHook;

        // Update is called once per frame
        void Update()
        {
            if (this.UpdateHook != null) this.UpdateHook(this.gameObject, System.EventArgs.Empty);
        }

        void FixedUpdate()
        {
            if (this.FixedUpdateHook != null) this.FixedUpdateHook(this.gameObject, System.EventArgs.Empty);
        }

        void LateUpdate()
        {
            if (this.LateUpdateHook != null) this.LateUpdateHook(this.gameObject, System.EventArgs.Empty);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            UpdateHook = null;
            FixedUpdateHook = null;
            LateUpdateHook = null;
        }


        public void ScheduleNextUpdate(System.Action callback)
        {

        }

        public void ScheduleNextFixedUpdate(System.Action callback)
        {

        }

        public void ScheduleNextLateUpdate(System.Action callback)
        {

        }

    }

}