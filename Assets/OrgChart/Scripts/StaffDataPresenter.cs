using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class StaffDataPresenter : MonoBehaviour {
  //view
  [SerializeField] Text currentLevelText;
  [SerializeField] Text baseLevelText;
  [SerializeField] GameObject diffLevelUI;
  [SerializeField] Text ageText;
  [SerializeField] RectTransform healthUI;
  [SerializeField] Text nameText;
  [SerializeField] RectTransform avatarUI;
  [SerializeField] GameObject costUI;
  [SerializeField] Text costText;



  //model
  private StaffNodePresenter node;
  private Image relation;
  private Image diffBg;
  private Text diffText;
  CompositeDisposable eventResources = new CompositeDisposable();


  CompositeDisposable staffResources = new CompositeDisposable();

  void Start(){
    node = GetComponentInParent<StaffNodePresenter> ();
    relation = GetComponent<Image> ();
    diffBg = diffLevelUI.GetComponent<Image> ();
    diffText = diffLevelUI.GetComponentInChildren<Text> ();


    node.tier
      .CombineLatest(node.childCount, (t, c) => (0 < c) ? Mathf.Min(t, 2) : t)
      .Subscribe(t => {
        if(node.isHired.Value){
        }
        else{
        }
      })
      .AddTo(eventResources);

    node.currentLevel
      .SubscribeToText(currentLevelText)
      .AddTo(eventResources);
    node.parentDiff
      .Subscribe(diff => {
        if(diff.HasValue)
        {
          if (diff.Value < 0) {
            relation.color = new Color (1, 0, 0);
          } else if (diff.Value < 2) {
            relation.color = new Color (1, 1, Mathf.Pow(diff.Value/2f, .8f));
          } else {
            relation.color = new Color (1, 1, 1);
          }
        }else{
          relation.color = new Color (1, 1, 1);
        }
      })
      .AddTo(eventResources);

    node.staffModel
      .Where (s => s != null)
      .Subscribe (s => {

        staffResources.Clear();

        node.isHired
          .Subscribe(h => costUI.SetActive(!h))
          .AddTo(staffResources);

        s.recruitCost
          .SubscribeToText(costText)
          .AddTo(staffResources);


        s.baseLevel
          .CombineLatest(node.hasChild, (l, r) => r ? "/" + l : "" )
          .SubscribeToText(baseLevelText)
          .AddTo(staffResources);


        s.baseLevel
          .CombineLatest(s.age, (skill,age) => age == 0 ? .5f : Mathf.Min(1, (float)skill/age/.8f))
          .Subscribe (rate => {
            currentLevelText.color = Util.HSVToRGB(rate * 100f/360f, .9f, .7f);
          })
          .AddTo(staffResources);

        s.age
          .Select(age => age + 20)
          .SubscribeToText(ageText, age => "(" + age.ToString() + ")" )
          .AddTo(staffResources);

                /*
        s.age
          .Subscribe (age => {
            if(age < GameController.retirementAge)
            {
              ageText.color =  Util.HSVToRGB(.3f, 1, (1f - (float)age / GameController.retirementAge) * .6f );
            } else{
              ageText.color = new Color(1,0,0);
            }
          })
          .AddTo (staffResources);
          */

        s.name
          .SubscribeToText (nameText)
          .AddTo (staffResources);

        s.gender
          .Subscribe (g => {
            nameText.color = ((g == 0) ? new Color(1f, .8f, .8f) : new Color(.9f, .9f, 1f));
          })
          .AddTo (staffResources);

        s.health = 
          s.damage
            .CombineLatest (node.currentLevel, (l, r) => r- l)
            .ToReactiveProperty();

        s.health
          .CombineLatest (node.currentLevel, (l, r) => Mathf.Max(0, r == 0 ? 0 : l / r ))
          .Subscribe (w => healthUI.anchorMax = new Vector2(w, 1))
          .AddTo (staffResources);




    })
      .AddTo (this);



  }
  void OnDestroy()
  {
    eventResources.Dispose ();
    staffResources.Dispose ();
  }
}
