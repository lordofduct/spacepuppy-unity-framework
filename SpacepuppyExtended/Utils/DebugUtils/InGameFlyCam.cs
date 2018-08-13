using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Utils.DebugUtils
{
    public class InGameFlyCam : SPComponent
    {

        public Transform Camera;

        public float CameraSensitivity = 5.0f;
        public float StartSpeed = 5.0f;
        public float TopSpeed = 25.0f;
        public float AccelerationRate = 1.0f;

        private Vector3 _lastMousePos;
        private bool _activelyMoving;
        private float _activelyMovingStartTime;

        protected override void Awake()
        {
            base.Awake();

            if (Camera == null) this.Camera = this.transform;
        }

        protected override void Start()
        {
            base.Start();

            _lastMousePos = Input.mousePosition;
        }

        void Update()
        {
            if (this.Camera != null)
            {
                //rotate
                if (Input.GetMouseButton(1))
                {
                    var dp = Input.mousePosition - _lastMousePos;
                    var e = new Vector3(-dp.y, dp.x, 0f);
                    //this.Camera.transform.rotation *= Quaternion.Euler(e * 3.0f * Speed * Time.deltaTime);

                    this.Camera.eulerAngles += e * this.CameraSensitivity * Time.deltaTime;
                }

                //move
                var mv = Vector3.zero;
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                    mv += Vector3.forward;

                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                    mv += Vector3.back;

                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                    mv += Vector3.right;

                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                    mv += Vector3.left;

                if (Input.GetKey(KeyCode.E))
                    mv += Vector3.up;

                if (Input.GetKey(KeyCode.Q))
                    mv += Vector3.down;

                if (mv != Vector3.zero)
                {
                    if (!_activelyMoving)
                    {
                        _activelyMoving = true;
                        _activelyMovingStartTime = Time.unscaledTime;
                    }

                    var spd = this.StartSpeed;
                    spd += Mathf.Pow(Time.unscaledTime - _activelyMovingStartTime, 2.0f) * this.AccelerationRate;
                    spd = Mathf.Min(spd, this.TopSpeed);
                    this.Camera.position += this.Camera.rotation * mv * spd * Time.deltaTime;
                }
                else
                {
                    _activelyMoving = false;
                }
            }


            _lastMousePos = Input.mousePosition;
        }

    }
}
