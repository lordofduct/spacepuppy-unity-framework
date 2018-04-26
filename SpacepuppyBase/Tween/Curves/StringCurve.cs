using System;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween.Curves
{

    [CustomMemberCurve(typeof(string))]
    public class StringCurve : MemberCurve, ISupportRedirectToMemberCurve
    {

        #region Fields

        private string _start;
        private string _end;
        private StringTweenStyle _style;

        #endregion

        #region CONSTRUCTOR

        protected StringCurve()
        {
            _start = string.Empty;
            _end = string.Empty;
        }

        public StringCurve(string propName, float dur, string start, string end, StringTweenStyle style)
            : base(propName, dur)
        {
            _start = start;
            _end = end;
            _style = style;
        }

        public StringCurve(string propName, Ease ease, float dur, string start, string end, StringTweenStyle style)
            : base(propName, ease, dur)
        {
            _start = start;
            _end = end;
            _style = style;
        }

        protected override void ReflectiveInit(System.Type memberType, object start, object end, object option)
        {
            _start = Convert.ToString(start) ?? string.Empty;
            _end = Convert.ToString(end) ?? string.Empty;
            _style = ConvertUtil.ToEnum<StringTweenStyle>(option, StringTweenStyle.Default);
        }

        void ISupportRedirectToMemberCurve.ConfigureAsRedirectTo(System.Type memberType, float totalDur, object current, object start, object end, object option)
        {
            _style = ConvertUtil.ToEnum<StringTweenStyle>(option, StringTweenStyle.Default);

            var c = Convert.ToString(current) ?? string.Empty;
            var s = Convert.ToString(start) ?? string.Empty;
            var e = Convert.ToString(end) ?? string.Empty;
            _start = c;
            _end = e;

            int tl = e.Length - s.Length;
            int l = c.Length - s.Length;
            if (tl == 0)
                this.Duration = 0f;
            else
                this.Duration = totalDur * (1f - (float)l / (float)tl);
        }

        #endregion

        #region Properties

        public string Start
        {
            get { return _start; }
            set
            {
                _start = value ?? string.Empty;
            }
        }

        public string End
        {
            get { return _end; }
            set
            {
                _end = value ?? string.Empty;
            }
        }

        public StringTweenStyle Style
        {
            get { return _style; }
            set
            {
                _style = value;
            }
        }

        #endregion

        #region MemberCurve Interface

        protected override object GetValueAt(float delta, float t)
        {
            t = (this.Duration == 0) ? 1f : this.Ease(t, 0f, 1f, this.Duration);
            if (float.IsNaN(t)) throw new System.ArgumentException("t must be a real number.", "t");

            if (!_style.HasFlag(StringTweenStyle.Jumble))
            {
                if(!_style.HasFlag(StringTweenStyle.RightToLeft))
                {
                    //left to right, none jumble
                    if(_start.Length == 0)
                    {
                        return _end.Substring(0, MathUtil.Interpolate(0, _end.Length, MathUtil.Clamp01(t)));
                    }
                    else if(_end.Length == 0)
                    {
                        int ipos = MathUtil.Interpolate(0, _start.Length, MathUtil.Clamp01(t));
                        if (ipos == 0)
                            return _start;
                        else if (ipos == _start.Length)
                            return string.Empty;
                        else
                            return new string(' ', ipos) + _start.Substring(ipos);
                    }
                    else
                    {
                        int len = Math.Max(_start.Length, _end.Length);
                        int ipos = MathUtil.Interpolate(0, len, MathUtil.Clamp01(t));
                        if (ipos == 0)
                            return _start;
                        else if (ipos == len)
                            return _end;
                        else if (ipos < _start.Length)
                        {
                            var builder = StringUtil.GetTempStringBuilder();
                            if (ipos < _end.Length)
                            {
                                builder.Append(_end.Substring(0, ipos));
                            }
                            else
                            {
                                builder.Append(_end);
                                int diff = ipos - _end.Length;
                                if (diff > 0) builder.Append(new string(' ', diff));
                            }

                            builder.Append(_start.Substring(ipos));
                            
                            return StringUtil.Release(builder);
                        }
                        else
                        {
                            return _end.Substring(0, ipos);
                        }
                    }
                }
                else
                {
                    //right to left, none jumble
                    if(_end.Length == 0)
                    {
                        return _start.Substring(0, MathUtil.Interpolate(_start.Length, 0, MathUtil.Clamp01(t)));
                    }
                    else if(_start.Length == 0)
                    {
                        int ipos = MathUtil.Interpolate(_end.Length, 0, MathUtil.Clamp01(t));
                        if (ipos == 0)
                            return _end;
                        if (ipos == _end.Length)
                            return string.Empty;
                        else
                            return new string(' ', ipos) + _end.Substring(ipos);
                    }
                    else
                    {
                        int len = Math.Max(_start.Length, _end.Length);
                        int ipos = MathUtil.Interpolate(len, 0, MathUtil.Clamp01(t));
                        if (ipos == 0)
                            return _end;
                        else if (ipos == len)
                            return _start;
                        else if (ipos < _end.Length)
                        {
                            var builder = StringUtil.GetTempStringBuilder();
                            if(ipos < _start.Length)
                            {
                                builder.Append(_start.Substring(0, ipos));
                            }
                            else
                            {
                                builder.Append(_start);
                                int diff = ipos - _start.Length;
                                if (diff > 0) builder.Append(new string(' ', diff));
                            }

                            builder.Append(_end.Substring(ipos));
                            
                            return StringUtil.Release(builder);
                        }
                        else
                        {
                            return _start.Substring(0, ipos);
                        }
                    }
                }
            }
            else
            {
                if (!_style.HasFlag(StringTweenStyle.RightToLeft))
                {
                    //left to right, jumble

                    int len = Math.Max(_start.Length, _end.Length);
                    float pos = (float)len * MathUtil.Clamp01(t);
                    float dt = MathUtil.Shear(pos);
                    int posLow = (int)Math.Floor(pos);
                    int posHigh = (int)Math.Ceiling(pos);
                    if(posHigh == 0)
                    {
                        return _start;
                    }
                    else if(posHigh == len)
                    {
                        return _end;
                    }
                    else if(posHigh < _start.Length)
                    {
                        var builder = StringUtil.GetTempStringBuilder();

                        if(posLow < _end.Length)
                        {
                            builder.Append(_end.Substring(0, posLow));
                        }
                        else
                        {
                            builder.Append(_end);
                            int diff = posLow - _end.Length;
                            if (diff > 0) builder.Append(new string(' ', diff));
                        }

                        if(posHigh < _end.Length)
                        {
                            builder.Append((char)MathUtil.Clamp(MathUtil.Interpolate((int)_start[posHigh], (int)_end[posHigh], dt), 255, 32));
                        }
                        else
                        {
                            builder.Append((char)MathUtil.Clamp(MathUtil.Interpolate((int)_start[posHigh], 32, dt), 255, 32));
                        }

                        if(posHigh + 1 < _start.Length) builder.Append(_start.Substring(posHigh + 1));
                        
                        return StringUtil.Release(builder);

                    }
                    else
                    {
                        if(posHigh < _end.Length)
                        {
                            return _end.Substring(0, posLow) + (char)MathUtil.Clamp(MathUtil.Interpolate(32, (int)_end[posHigh], dt), 255, 32);
                        }
                        else
                        {
                            return _end.Substring(0, posLow);
                        }
                    }
                }
                else
                {
                    //right to left, jumble

                    int len = Math.Max(_start.Length, _end.Length);
                    float pos = (float)len * (1f - MathUtil.Clamp01(t));
                    float dt = MathUtil.Shear(pos);
                    int posLow = (int)Math.Floor(pos);
                    int posHigh = (int)Math.Ceiling(pos);
                    if(posHigh == len)
                    {
                        return _start;
                    }
                    else if(posHigh == 0)
                    {
                        return _end;
                    }
                    else if(posHigh < _end.Length)
                    {
                        var builder = StringUtil.GetTempStringBuilder();
                        if (posLow < _start.Length)
                        {
                            builder.Append(_start.Substring(0, posLow));
                        }
                        else
                        {
                            builder.Append(_start);
                            int diff = posLow - _start.Length;
                            if (diff > 0) builder.Append(new string(' ', diff));
                        }

                        if (posHigh < _end.Length)
                        {
                            builder.Append((char)MathUtil.Clamp(MathUtil.Interpolate((int)_start[posHigh], (int)_end[posHigh], dt), 255, 32));
                        }
                        else
                        {
                            builder.Append((char)MathUtil.Clamp(MathUtil.Interpolate((int)_start[posHigh], 32, dt), 255, 32));
                        }

                        if (posHigh + 1 < _end.Length) builder.Append(_end.Substring(posHigh + 1));
                        
                        return StringUtil.Release(builder);
                    }
                    else
                    {
                        if(posHigh < _start.Length)
                        {
                            return _start.Substring(0, posLow) + (char)MathUtil.Clamp(MathUtil.Interpolate((int)_start[posHigh], 32, dt), 255, 32);
                        }
                        else
                        {
                            return _start;
                        }
                    }
                }
            }
        }


        #endregion

        #region Methods


        /// <summary>
        /// Return the duration that should pass for the transition between 2 strings to occur at some constant speed in characters per second.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="charsPerSecond"></param>
        /// <returns></returns>
        public static float CalculateDuration(string start, string end, float charsPerSecond)
        {
            if (charsPerSecond <= 0f) throw new System.ArgumentException("speed must be a positive value.", "charsPerSecond");
            return (float)Math.Max((start != null) ? start.Length : 0, (end != null) ? end.Length : 0) / charsPerSecond;
        }

        #endregion

    }
}
