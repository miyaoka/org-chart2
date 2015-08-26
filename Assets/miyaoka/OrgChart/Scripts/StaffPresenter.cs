using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

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

        node.parentDiff
            .Subscribe(diff => {
                if(diff.HasValue && diff.Value < 0)
                {
                    currentLevelText.color = new Color (1, .2f, .2f);
                }else{
                    currentLevelText.color = new Color (.2f, .2f, .2f);
                }
            })
            .AddTo(this);

        var hueInit = 1f / 6f;

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

                //世代別色分け
                s.age
                    .Subscribe (age => {
                        avatarImage.color =  Util.HSVToRGB((float)(age / 10) * .2f + hueInit, .7f, .7f);
                    })
                    .AddTo (staffResources);

                /*
                s.stdScore
                //0-4
                    .Select(ss => Mathf.Min(8f, Mathf.Max(0, Mathf.Round(ss * 10f) - 4f)))
                    .Subscribe (ss => {
                        avatarImage.color =  Util.HSVToRGB( 1f - ss  * .2f, .7f, .7f);

                    })
                    .AddTo (staffResources);
                */
                

                s.age
                  .Subscribe (age => {
                    if(age < 40)
                    {
                      ageText.color =  Util.HSVToRGB(.3f, 1, (1f - (float)age / 40) * .6f );
                    } else{
                      ageText.color = new Color(1,0,0);
                    }
                  })
                  .AddTo (staffResources);
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
                

                */
                s.moral
                    .Subscribe(m => {
                        moralTrans.anchorMin = new Vector2(0, m * 1f + .0f);
                    })
                    .AddTo(staffResources);

                //レベル値に応じてスケール表示
                s.baseLevel
                    .Select(b => .4f + Mathf.Max(b-4, 0) * .05f)
                    .Subscribe(b => {
                        avatarImage.transform.DOScale(new Vector3(b, b, 1), .3f).SetEase(Ease.OutBack, 20f);
                        shadowImage.transform.DOScale(new Vector3(b, b, 1), .3f).SetEase(Ease.OutBack, 20f);
//                        avatarImage.transform.localScale = shadowImage.transform.localScale = new Vector3(b, b, 1);
                    })
                    .AddTo(staffResources);
                /*

                node.hasChild
                    .Select(b => b ? 1f : .5f)
                    .Subscribe(sc => {
                        avatarImage.transform.DOScale(new Vector3(sc, sc, 1), .3f).SetEase(Ease.OutBack, 20f);
                        shadowImage.transform.DOScale(new Vector3(sc, sc, 1), .3f).SetEase(Ease.OutBack, 20f);
                        //                        avatarImage.transform.localScale = shadowImage.transform.localScale = new Vector3(b, b, 1);
                    })
                    .AddTo(staffResources);
                */



                //今回と直前の値のペア
                /*
                s.baseLevel
                    .Buffer(2, 1)
                    .Select(l => new Vector2(l[1], l[0]))
                    .Subscribe(l => Debug.Log(l))
                    .AddTo(staffResources);
                */

                avatarImage.transform.localScale = shadowImage.transform.localScale = new Vector3(.4f, .4f, 1);
            })
            .AddTo (this);



    }
    void OnDestroy()
    {
        staffResources.Dispose ();
    }
}
