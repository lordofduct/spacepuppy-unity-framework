using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    [CreateAssetMenu(fileName = "RNGAsset", menuName = "Spacepuppy/RNGAsset")]
    public class RNGAsset : ScriptableObject, IRandom
    {

        public enum RNGAlgorithms
        {
            Standard = 0,
            Microsoft = 1,
            LCG = 2,
            PCG = 3
        }

        #region Fields

        [SerializeField]
        private RNGAlgorithms _algorithm;

        [SerializeField]
        [DisableIf("AlgorithmIsSeedable", DisableIfNot = true)]
        private int _seed = -1;

        [System.NonSerialized]
        private IRandom _rnd;
        [System.NonSerialized]
        private RNGAlgorithms _configuredAlgorithm;

        #endregion

        #region Properties

        public RNGAlgorithms Algorithm
        {
            get { return _algorithm; }
            set { _algorithm = value; }
        }

        public int Seed
        {
            get { return _seed; }
            set { _seed = value; }
        }

        public bool AlgorithmIsSeedable
        {
            get { return _algorithm != RNGAlgorithms.Standard; }
        }

        public float Value
        {
            get
            {
                this.ValidateRNG();
                return _rnd.Next();
            }
        }

        #endregion

        #region IRandom Interface

        public float Next()
        {
            this.ValidateRNG();
            return _rnd.Next();
        }

        public double NextDouble()
        {
            this.ValidateRNG();
            return _rnd.NextDouble();
        }

        public int Next(int size)
        {
            this.ValidateRNG();
            return _rnd.Next(size);
        }

        public int Next(int low, int high)
        {
            this.ValidateRNG();
            return _rnd.Next(low, high);
        }

        #endregion

        #region RNG Methods

        public void Reset()
        {
            _rnd = null;
            _configuredAlgorithm = RNGAlgorithms.Standard;
        }

        public void Reset(int seed)
        {
            _rnd = null;
            _configuredAlgorithm = RNGAlgorithms.Standard;
            _seed = seed;
        }

        public int RangeInt(int min, int max)
        {
            this.ValidateRNG();
            return _rnd.Range(max, min);
        }

        public float Range(float min, float max)
        {
            this.ValidateRNG();
            return _rnd.Range(max, min);
        }

        public float Angle()
        {
            this.ValidateRNG();
            return _rnd.Angle();
        }

        public float Radian()
        {
            this.ValidateRNG();
            return _rnd.Radian();
        }

        public int Pop()
        {
            this.ValidateRNG();
            return _rnd.Pop();
        }

        public int Sign()
        {
            this.ValidateRNG();
            return _rnd.Sign();
        }

        public bool Bool()
        {
            this.ValidateRNG();
            return _rnd.Bool();
        }

        public int Shift()
        {
            this.ValidateRNG();
            return _rnd.Shift();
        }

        public Vector3 OnUnitSphere()
        {
            this.ValidateRNG();
            return _rnd.OnUnitSphere();
        }

        public Vector2 OnUnitCircle()
        {
            this.ValidateRNG();
            return _rnd.OnUnitCircle();
        }

        public Vector3 InsideUnitSphere()
        {
            this.ValidateRNG();
            return _rnd.InsideUnitSphere();
        }

        public Vector2 InsideUnitCircle()
        {
            this.ValidateRNG();
            return _rnd.InsideUnitCircle();
        }

        public Vector3 AroundAxis(Vector3 axis)
        {
            this.ValidateRNG();
            return _rnd.AroundAxis(axis);
        }

        public Quaternion Rotation()
        {
            this.ValidateRNG();
            return _rnd.Rotation();
        }

        private void ValidateRNG()
        {
            if (_rnd == null || _configuredAlgorithm != _algorithm)
            {
                _configuredAlgorithm = _algorithm;
                switch (_algorithm)
                {
                    case RNGAlgorithms.Standard:
                        _rnd = RandomUtil.Standard;
                        break;
                    case RNGAlgorithms.Microsoft:
                        _rnd = new RandomUtil.MicrosoftRNG(_seed);
                        break;
                    case RNGAlgorithms.LCG:
                        _rnd = RandomUtil.LinearCongruentialRNG.CreateMMIXKnuth(_seed);
                        break;
                    case RNGAlgorithms.PCG:
                        _rnd = new RandomUtil.SimplePCG(_seed);
                        break;
                    default:
                        _rnd = RandomUtil.CreateDeterministicRNG(_seed);
                        break;
                }
            }
        }

        #endregion

    }

}
