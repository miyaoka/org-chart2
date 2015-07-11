using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

[DisallowMultipleComponent]
public class DropTrigger : ObservableTriggerBase, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

    readonly Subject<PointerEventData> _onDrop = new Subject<PointerEventData>();
    readonly Subject<PointerEventData> _onPointerEnter = new Subject<PointerEventData>();
    readonly Subject<PointerEventData> _onPointerExit = new Subject<PointerEventData>();

    #region IPointerExitHandler implementation

    void IPointerExitHandler.OnPointerExit (PointerEventData eventData)
    {
        _onPointerExit.OnNext (eventData);
    }

    #endregion

    #region IPointerEnterHandler implementation
    void IPointerEnterHandler.OnPointerEnter (PointerEventData eventData)
    {
        _onPointerEnter.OnNext (eventData);
    }
    #endregion

    #region IDropHandler implementation
    void UnityEngine.EventSystems.IDropHandler.OnDrop (UnityEngine.EventSystems.PointerEventData eventData)
    {
        _onDrop.OnNext (eventData);
    }
    #endregion


    public IObservable<PointerEventData> OnDropAsObservable()
    {
        return _onDrop.Publish().RefCount();
    }
    public IObservable<PointerEventData> OnPointerEnterAsObservable()
    {
        return _onPointerEnter.Publish().RefCount();
    }
    public IObservable<PointerEventData> OnPointerExitAsObservable()
    {
        return _onPointerExit.Publish().RefCount();
    }

    protected override void RaiseOnCompletedOnDestroy()
    {
        _onDrop.OnCompleted();
        _onPointerEnter.OnCompleted ();
        _onPointerExit.OnCompleted ();
    }
}
