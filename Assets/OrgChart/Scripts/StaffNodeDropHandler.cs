using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;

public class StaffNodeDropHandler : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler{
  protected StaffNodePresenter staffNode;
  [SerializeField] protected Outline outline;
  [SerializeField] protected GameObject animUI;

  protected float enterAnimTime = .2f;
  protected float exitAnimTime = .02f;
  protected float enlarge = 1.3f;
  protected Vector3 origScale;

  protected void Awake(){
    staffNode = GetComponentInParent<StaffNodePresenter> ();
    origScale = animUI.transform.localScale;
  }
  protected StaffNodePresenter getPointerStaffNode(PointerEventData eventData){
    return eventData.pointerDrag ? eventData.pointerDrag.GetComponentInParent<StaffNodePresenter> () : null;
  }


	#region IDropHandler implementation
	public void OnDrop (PointerEventData eventData)
	{
    StaffNodePresenter pointerNode = getPointerStaffNode (eventData);
    if (!pointerNode || pointerNode == staffNode) {
      return;
    }
//    GameController.Instance.moveStaffNode (pointerNode, staffNode);

	}
	#endregion

	#region IPointerEnterHandler implementation

  public virtual void OnPointerEnter (PointerEventData eventData)
	{
    if (staffNode.isEmpty.Value || getPointerStaffNode(eventData) ) {
      LeanTween.cancel (animUI);
      LeanTween.scale (animUI, origScale * enlarge, enterAnimTime).setEase (LeanTweenType.easeOutBack);
      outline.enabled = true;
		}
	}

	#endregion
  public void OnPointerExit (PointerEventData eventData)
  {
    LeanTween.cancel (animUI);
    LeanTween.scale (animUI, origScale, exitAnimTime).setEase (LeanTweenType.easeOutQuint);
    outline.enabled = false;
  }
}
