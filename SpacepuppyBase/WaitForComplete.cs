using UnityEngine;
using System.Collections;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    /// <summary>
    /// A composite yield instruction that waits for multiple instructions to all complete before continuing.
    /// </summary>
    public class WaitForAllComplete : RadicalYieldInstruction
    {

        private MonoBehaviour _handle;
        private System.Collections.Generic.List<object> _instructions;
        private int _waitCount;

        public WaitForAllComplete(MonoBehaviour handle, params object[] instructions)
        {
            _handle = handle;
            _instructions = new System.Collections.Generic.List<object>(instructions);
        }

        public void Add(object instruction)
        {
            _instructions.Add(instruction);
        }

        protected override bool Tick(out object yieldObject)
        {
            if (this.IsComplete)
            {
                yieldObject = null;
                return false;
            }

            object current;
            for (int i = 0; i < _instructions.Count; i++)
            {
                current = _instructions[i];
                if (current == null)
                {
                    _instructions.RemoveAt(i);
                    i--;
                }
                else if (current is YieldInstruction || current is WWW)
                {
                    _instructions.RemoveAt(i);
                    i--;
                    _handle.StartCoroutine(this.WaitForStandard(current));
                }
                else if (current is RadicalCoroutine)
                {
                    if ((current as RadicalCoroutine).Complete)
                    {
                        _instructions.RemoveAt(i);
                        i--;
                    }
                }
                else if (current is IRadicalYieldInstruction)
                {
                    object sub;
                    if ((current as IRadicalYieldInstruction).Tick(out sub))
                    {
                        if (sub != null)
                        {
                            _instructions[i] = _handle.StartRadicalCoroutine(this.WaitForRadical(current as IRadicalYieldInstruction));
                        }
                    }
                    else
                    {
                        _instructions.RemoveAt(i);
                        i--;
                    }
                }
                else if (current is IEnumerable)
                {
                    var e = (current as IEnumerable).GetEnumerator();
                    _instructions[i] = e;
                    if (e.MoveNext())
                    {
                        if (e.Current != null) _instructions.Add(e.Current);
                    }
                    else
                    {
                        _instructions.RemoveAt(i);
                        i--;
                    }
                }
                else if (current is IEnumerator)
                {
                    var e = current as IEnumerator;
                    if (e.MoveNext())
                    {
                        if (e.Current != null) _instructions.Add(e.Current);
                    }
                    else
                    {
                        _instructions.RemoveAt(i);
                        i--;
                    }
                }
                else
                {
                    _instructions[i] = null;
                }
            }

            if (_instructions.Count == 0 && _waitCount <= 0)
            {
                this.SetSignal();
                yieldObject = null;
                return false;
            }
            else
            {
                yieldObject = null;
                return true;
            }
        }

        private IEnumerator WaitForStandard(object inst)
        {
            _waitCount++;
            yield return inst;
            _waitCount--;
        }

        private IEnumerator WaitForRadical(IRadicalYieldInstruction inst)
        {
            object yieldObject;
            while (inst.Tick(out yieldObject))
            {
                yield return yieldObject;
            }
        }

    }

    /// <summary>
    /// A composite yield instruction that waits for any one of multiple instruction to complete before continuing.
    /// </summary>
    public class WaitForAnyComplete : RadicalYieldInstruction
    {

        private MonoBehaviour _handle;
        private System.Collections.Generic.List<object> _instructions;
        private System.Collections.Generic.List<object> _waitingRoutines = new System.Collections.Generic.List<object>();
        private bool _signalNextTime;

        public WaitForAnyComplete(MonoBehaviour handle, params object[] instructions)
        {
            _handle = handle;
            _instructions = new System.Collections.Generic.List<object>(instructions);
        }

        protected override void SetSignal()
        {
            object obj;
            for (int i = 0; i < _waitingRoutines.Count; i++)
            {
                obj = _waitingRoutines[i];
                if (obj is Coroutine)
                    _handle.StopCoroutine(obj as Coroutine);
                else if (obj is RadicalCoroutine)
                    (obj as RadicalCoroutine).Cancel();
            }
            _waitingRoutines.Clear();
            base.SetSignal();
        }

        protected override bool Tick(out object yieldObject)
        {
            yieldObject = null;
            if (this.IsComplete) return false;
            if (_signalNextTime)
            {
                this.SetSignal();
                return false;
            }

            object current;
            for (int i = 0; i < _instructions.Count; i++)
            {
                current = _instructions[i];
                if (current == null)
                {
                    _signalNextTime = true;
                }
                else if (current is YieldInstruction || current is WWW)
                {
                    _instructions.RemoveAt(i);
                    i--;
                    _waitingRoutines.Add(_handle.StartCoroutine(this.WaitForStandard(current)));
                }
                else if (current is RadicalCoroutine)
                {
                    if ((current as RadicalCoroutine).Complete)
                    {
                        this.SetSignal();
                        return false;
                    }
                }
                else if (current is IRadicalYieldInstruction)
                {
                    object sub;
                    if ((current as IRadicalYieldInstruction).Tick(out sub))
                    {
                        if (sub != null)
                        {
                            _instructions.RemoveAt(i);
                            i--;
                            _waitingRoutines.Add(_handle.StartRadicalCoroutine(this.WaitForRadical(current)));
                        }
                    }
                    else
                    {
                        this.SetSignal();
                        return false;
                    }
                }
                else if (current is IEnumerator || current is IEnumerable)
                {
                    _instructions.RemoveAt(i);
                    i--;
                    _waitingRoutines.Add(_handle.StartRadicalCoroutine(this.WaitForRadical(current)));
                }
                else
                {
                    _signalNextTime = true;
                }
            }

            return true;
        }

        private IEnumerator WaitForStandard(object inst)
        {
            yield return inst;
            this.SetSignal();
        }

        private IEnumerator WaitForRadicalYield(IRadicalYieldInstruction inst)
        {
            object yieldObject;
            while (inst.Tick(out yieldObject))
            {
                yield return yieldObject;
            }
        }

        private IEnumerator WaitForRadical(object inst)
        {
            yield return inst;
            this.SetSignal();
        }

    }

}
