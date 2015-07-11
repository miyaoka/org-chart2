using UnityEngine;
using System.Collections;
using UniRx;
using UnityEngine.UI;

public class YearRollPresenter : MonoBehaviour {

  public Text yearText;

	// Use this for initialization
	void Start () {
        /*
    GameController.Instance.year
    .Select(y => Util.AddOrdinal(y) + " year")
      .Subscribe(y => {
        yearText.text = y;
        play();
      })
    .AddTo(this);
*/
    }


  public void play(){
    Vector2 pDelta = this.transform.parent.GetComponent<RectTransform> ().sizeDelta;
    float pH = pDelta.y;
    var pos = this.transform.localPosition;
    pos.y = -pH;
    this.transform.localPosition = pos;

    LeanTween.cancel (gameObject);
    LeanTween.moveLocalY (this.gameObject, 0, 1f).setEase (LeanTweenType.easeOutQuint).setOnComplete( () => {
      LeanTween.moveLocalY (this.gameObject, pH, 1f).setEase (LeanTweenType.easeInQuint);
    });
  }
	
	// Update is called once per frame
	void Update () {
	
	}
}
