using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace com.spacepuppy.Scenario
{
    public class t_OnEventTrigger : SPComponent, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IBeginDragHandler, IInitializePotentialDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IScrollHandler, IUpdateSelectedHandler, ISelectHandler, IDeselectHandler, IMoveHandler, ISubmitHandler, ICancelHandler
    {

        #region Fields

        [SerializeField()]
        private List<Entry> _triggers;

        #endregion

        #region Methods

        private void Execute(EventTriggerType id, BaseEventData eventData)
        {
            if (_triggers == null)
                return;
            int index = 0;
            for (int count = _triggers.Count; index < count; ++index)
            {
                t_OnEventTrigger.Entry entry = _triggers[index];
                if (entry.EventID == id)
                    entry.ActivateTrigger(eventData);
            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            this.Execute(EventTriggerType.PointerEnter, (BaseEventData)eventData);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            this.Execute(EventTriggerType.PointerExit, (BaseEventData)eventData);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            this.Execute(EventTriggerType.Drag, (BaseEventData)eventData);
        }

        public virtual void OnDrop(PointerEventData eventData)
        {
            this.Execute(EventTriggerType.Drop, (BaseEventData)eventData);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            this.Execute(EventTriggerType.PointerDown, (BaseEventData)eventData);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            this.Execute(EventTriggerType.PointerUp, (BaseEventData)eventData);
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            this.Execute(EventTriggerType.PointerClick, (BaseEventData)eventData);
        }

        public virtual void OnSelect(BaseEventData eventData)
        {
            this.Execute(EventTriggerType.Select, eventData);
        }

        public virtual void OnDeselect(BaseEventData eventData)
        {
            this.Execute(EventTriggerType.Deselect, eventData);
        }

        public virtual void OnScroll(PointerEventData eventData)
        {
            this.Execute(EventTriggerType.Scroll, (BaseEventData)eventData);
        }

        public virtual void OnMove(AxisEventData eventData)
        {
            this.Execute(EventTriggerType.Move, (BaseEventData)eventData);
        }

        public virtual void OnUpdateSelected(BaseEventData eventData)
        {
            this.Execute(EventTriggerType.UpdateSelected, eventData);
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            this.Execute(EventTriggerType.InitializePotentialDrag, (BaseEventData)eventData);
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            this.Execute(EventTriggerType.BeginDrag, (BaseEventData)eventData);
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            this.Execute(EventTriggerType.EndDrag, (BaseEventData)eventData);
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            this.Execute(EventTriggerType.Submit, eventData);
        }

        public virtual void OnCancel(BaseEventData eventData)
        {
            this.Execute(EventTriggerType.Cancel, eventData);
        }

        #endregion

        #region Special Types

        [System.Serializable()]
        public class Entry : Trigger
        {
            public EventTriggerType EventID = EventTriggerType.PointerClick;
        }

        #endregion

    }
}
