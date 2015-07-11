using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class StaffPresenter : MonoBehaviour {
    //view
    [SerializeField] Text currentLevelText;
    [SerializeField] Text baseLevelText;
//    [SerializeField] GameObject diffLevelUI;
    [SerializeField] Text ageText;
//    [SerializeField] RectTransform healthUI;
    [SerializeField] Text titleText;
    [SerializeField] Text nameText;
//    [SerializeField] RectTransform avatarUI;
//    [SerializeField] GameObject costUI;
//    [SerializeField] Text costText;

    [SerializeField] Image avatarImage;
    [SerializeField] Image shadowImage;
    [SerializeField] RectTransform moralTrans;



    //model
    CompositeDisposable staffResources = new CompositeDisposable();

    void Start(){
        var node = GetComponentInParent<NodePresenter> ();
//        var relation = GetComponent<Image> ();
        var relation = avatarImage;

//        diffBg = diffLevelUI.GetComponent<Image> ();
//        diffText = diffLevelUI.GetComponentInChildren<Text> ();


        /*
        node.tier
            .CombineLatest(node.childCount, (t, c) => (0 < c) ? Mathf.Min(t, 2) : t)
            .Subscribe(t => {
                if(node.isHired.Value){
                }
                else{
                }
            })
            .AddTo(this);
            */

//        string[] tierNames = " 代表取締役 部長 課長 係長 社員".Split (' ');
        string[] tierNames = " CEO Director Manager Staff".Split (' ');

        node.tier
            .Select(t => tierNames[Mathf.Min(t, tierNames.Length - 1)])
            .SubscribeToText (titleText)
            .AddTo (this);

        node.currentLevel
            .SubscribeToText(currentLevelText)
            .AddTo(this);

        /*
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
            .AddTo(this);
            */

        node.staffModel
            .Subscribe (s => {

                staffResources.Clear();
                if(s == null){
                    return;
                }

                s.baseLevel
                    .CombineLatest(node.hasChild, (l, r) => r ? "/" + l : "" )
                    .SubscribeToText(baseLevelText)
                    .AddTo(staffResources);

                /*
                s.baseLevel
                    .CombineLatest(s.age, (skill,age) => age == 0 ? .5f : Mathf.Min(1, (float)skill/age/.8f))
                    .Subscribe (rate => {
                        currentLevelText.color = Util.HSVToRGB(rate * 100f/360f, .9f, .7f);
                    })
                    .AddTo(staffResources);
*/

                /*
                s.baseLevel
                    .Select(l => Mathf.Max(0, 1 - l / 15f))
                    .Subscribe (rate => {
//                        currentLevelText.color = Util.HSVToRGB(0, 0, rate * .5f);
                    })
                    .AddTo(staffResources);

                s.stdScore
                    .Select(ss => "FFFFEEDDDCCBBAASSSS".Substring((int)Mathf.Floor(ss * 20),1) + " " + ss.ToString("P1"))
                    .SubscribeToText(nameText)
                    .AddTo(staffResources);
                */
                s.age
                    .Select(age => age + 20)
                    .SubscribeToText(ageText, age => "(" + age.ToString() + ")" )
                    .AddTo(staffResources);

                s.age
                    .Subscribe (age => {
                        if(5 <= node.currentLevel.Value )
                        {
                            //(1f - (float)age / 40) * .5f + 
                            relation.color =  Util.HSVToRGB(s.hue.Value, .4f, .8f);
                        } else{
                            relation.color = Util.HSVToRGB(0, 0, .75f);
                        }
                    })
                    .AddTo (staffResources);

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
                /*
                s.name
                    .SubscribeToText (nameText)
                    .AddTo (staffResources);
*/

                /*
                s.gender
                    .Subscribe (g => {
//                        nameText.color = ((g == 0) ? new Color(1f, .8f, .8f) : new Color(.9f, .9f, 1f));
                    })
                    .AddTo (staffResources);
                

                s.moral
                    .Subscribe(m => {
                        moralTrans.anchorMin = new Vector2(0, m * .8f + .2f);
                    })
                    .AddTo(staffResources);
                */


                s.baseLevel
                    .Select(b => .4f + Mathf.Max(b-4, 0) * .05f)
                    .Subscribe(b => {
                        avatarImage.transform.localScale = shadowImage.transform.localScale = new Vector3(b, b, 1);
                    })
                    .AddTo(staffResources);
            })
            .AddTo (this);



    }
    void OnDestroy()
    {
        staffResources.Dispose ();
    }
}
