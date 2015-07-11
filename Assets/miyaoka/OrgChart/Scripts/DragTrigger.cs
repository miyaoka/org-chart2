using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

[DisallowMultipleComponent]
public class DragTrigger : ObservableTriggerBase, IBeginDragHandler, IDragHandler, IEndDragHandler {

    Subject<PointerEventData> _onBeginDrag = new Subject<PointerEventData> ();
    Subject<PointerEventData> _onDrag = new Subject<PointerEventData>();
    Subject<PointerEventData> _onEndDrag = new Subject<PointerEventData>();

    public IObservable<PointerEventData> OnBeginDragAsObservable()
    {
        return _onBeginDrag.Publish().RefCount();
    }
    public IObservable<PointerEventData> OnDragAsObservable()
    {
        return _onDrag.Publish().RefCount();
    }
    public IObservable<PointerEventData> OnEndDragAsObservable()
    {
        return _onEndDrag.Publish().RefCount();
    }

    #region IBeginDragHandler implementation

    public void OnBeginDrag (PointerEventData eventData)
    {
        _onBeginDrag.OnNext (eventData);
    }

    #endregion

    #region IDragHandler implementation

    public void OnDrag (PointerEventData eventData)
    {
        _onDrag.OnNext (eventData);
    }

    #endregion

    #region IEndDragHandler implementation

    public void OnEndDrag (PointerEventData eventData)
    {
        _onEndDrag.OnNext (eventData);
    }

    #endregion

    protected override void RaiseOnCompletedOnDestroy()
    {
        _onBeginDrag.OnCompleted();
        _onDrag.OnCompleted();
        _onDrag.OnCompleted();
    }
}