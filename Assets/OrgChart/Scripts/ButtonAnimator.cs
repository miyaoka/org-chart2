using UnityEngine;
using System.Collections;
using UniRx;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ButtonAnimator : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, ISelectHandler {
  #region IPointerClickHandler implementation

  public void OnPointerClick (PointerEventData eventData)
  {
//    GameSounds.submit.Play ();
    onSubmit ();
  }

  #endregion

  #region IPointerDownHandler implementation

  public void OnPointerDown (PointerEventData eventData)
  {
    onPress ();
  }

  #endregion

  #region IPointerEnterHandler implementation

  public void OnPointerEnter (PointerEventData eventData)
  {
    if (eventData.eligibleForClick) {
      onPress ();
    }
  }

  #endregion

  #region IPointerExitHandler implementation

  public void OnPointerExit (PointerEventData eventData)
  {
    onExit ();
  }

  #endregion

  #region IPointerUpHandler implementation

  public void OnPointerUp (PointerEventData eventData)
  {
  }

  #endregion

  #region ISelectHandler implementation

  public void OnSelect (BaseEventData eventData)
  {
  }

  #endregion
  [SerializeField] GameObject animUI;

  private float enterAnimTime = .1f;
  private float exitAnimTime = .02f;
  private float submitAnimTime = .2f;
  private float pushScale = .85f;
  private Vector3 origScale;


  void onPress(){
    LeanTween.cancel (animUI);
    LeanTween.scale (animUI, origScale * pushScale, enterAnimTime).setEase (LeanTweenType.easeOutQuint);
  }
  void onExit(){
    LeanTween.cancel (animUI);
    LeanTween.scale (animUI, origScale, exitAnimTime).setEase (LeanTweenType.easeOutQuint);
  }
  void onSubmit(){
    LeanTween.cancel (animUI);
    LeanTween.scale (animUI, origScale, submitAnimTime).setEase (LeanTweenType.easeOutBack);
  }
	// Use this for initialization
	void Start () {
    origScale = animUI.transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
