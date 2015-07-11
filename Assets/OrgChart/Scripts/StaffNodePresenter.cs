using UnityEngine;
using System.Collections;
using UniRx;
using UnityEngine.EventSystems;
using System.Reflection;
using UnityEngine.UI;
using UniRx.Triggers;

public class StaffNodePresenter : NodePresenter {

  //view
//  [SerializeField] GameObject contentUI;
  [SerializeField] public GameObject staffUI;
  [SerializeField] GameObject emptyUI;
  [SerializeField] GameObject familyLineUI;
  [SerializeField] CanvasGroup panelCG;

  [SerializeField] Text childCountText;
  [SerializeField] Text levelCountText;
  private UILineRenderer familyLine;
  private const float familyLineHeight = 24.0F;

  //model


  IObservable<Vector2> parentDelta;
  IObservable<Vector2> thisDelta;

  /*

  public StaffData staffData{
    get {
      var sd = new StaffData();        
      foreach(FieldInfo fi in sd.GetType().GetFields()){
        object reactiveProp = this.GetType ().GetField (fi.Name).GetValue (this);
        sd.GetType().GetField(fi.Name).SetValue(
          sd, 
          reactiveProp.GetType ().GetProperty ("Value").GetValue(reactiveProp, null)
        );
      }
      return sd;
    }
    set {
      foreach(FieldInfo fi in value.GetType().GetFields()){
        object reactiveProp = this.GetType ().GetField (fi.Name).GetValue (this);
        reactiveProp.GetType ().GetProperty ("Value").SetValue(
          reactiveProp,
          value.GetType ().GetField (fi.Name).GetValue (value),
          null
        );
      }
    }
  }
*/



	void Start () {
//    var gc = GameController.Instance;
    familyLine = familyLineUI.GetComponent<UILineRenderer> ();

        /*
    gc.draggingNode
      .Select (d => d != null)
      .Subscribe (d => panelCG.blocksRaycasts = d ? false : true)
      .AddTo (this);
*/


    var parentPos = 
      transform.parent.transform.ObserveEveryValueChanged (t => t.position);
    var familyPos = 
      familyLineUI.transform.ObserveEveryValueChanged (t => t.position);

    isHired
      .Where(h => h)
      .CombineLatest(parentPos, (l, r) => r)
      .CombineLatest (familyPos, (l, r) => l - r)
      .Skip(1)
      .Subscribe (p => drawFamilyLine (new Vector2 (0, 0), new Vector2 (p.x, p.y)))
      .AddTo (this);

    isHired
      .CombineLatest (isRoot, (l, r) => l && !r)
      .Subscribe (familyLineUI.SetActive)
      .AddTo (this);







    isEmpty
    //      .CombineLatest(isDragging, (l, r) => l || r)
      .Subscribe (e => {
        staffUI.SetActive(!e);
        emptyUI.SetActive(e);
      })
      .AddTo(this);






    childCount
      .CombineLatest(childCountTotal, (l,r) => l + "/" + r)
      .SubscribeToText (childCountText)
      .AddTo (this);

    currentLevelTotal
      .SubscribeToText (levelCountText)
      .AddTo (this);


	}

  void drawFamilyLine(Vector2 start, Vector2 end){
    var centerY = (start.y + end.y) * .4f;
    familyLine.Points = new Vector2[] { 
      new Vector2(start.x, start.y), 
      new Vector2(start.x, centerY),
      new Vector2(end.x, centerY),
      new Vector2(end.x, end.y)
    };
    familyLine.SetVerticesDirty();    
  }

}
