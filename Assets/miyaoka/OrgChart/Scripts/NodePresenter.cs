using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class NodePresenter : MonoBehaviour {

    //view
    [SerializeField] public RectTransform childNodes;
    [SerializeField] public GameObject panelUI;
    [SerializeField] public GameObject panelContentUI;
    [SerializeField] public GameObject staffPanelUI;
    [SerializeField] public GameObject emptyPanelUI;
    [SerializeField] public GameObject dropAreaUI;
    [SerializeField] Image parentLine;
    [SerializeField] Image childrenLineH;
    [SerializeField] Image childrenLineV;

    [SerializeField] Text wageText;
    [SerializeField] Text wageTotalText;
    [SerializeField] Text salesText;
    [SerializeField] Text salesTotalText;
    [SerializeField] Text profitText;


    //model
    public ReactiveProperty<int> childCount { get; private set; }
    public ReactiveProperty<int> childCountTotal = new ReactiveProperty<int>();
    public ReactiveProperty<int> currentLevel = new ReactiveProperty<int>();
    public ReactiveProperty<int> currentLevelTotal = new ReactiveProperty<int>();
    public ReactiveProperty<int> manCount = new ReactiveProperty<int>();
    public ReactiveProperty<int> manCountTotal = new ReactiveProperty<int>();
    public ReactiveProperty<float> wage = new ReactiveProperty<float>();
    public ReactiveProperty<float> wageTotal = new ReactiveProperty<float>();
    public ReactiveProperty<float> sales = new ReactiveProperty<float>();
    public ReactiveProperty<float> salesTotal = new ReactiveProperty<float>();
    public ReactiveProperty<float> profit = new ReactiveProperty<float>();

    public ReadOnlyReactiveProperty<bool> hasChild { get; private set; }

    public ReactiveProperty<int> tier = new ReactiveProperty<int> (0);
    public ReactiveProperty<bool> isHired = new ReactiveProperty<bool> ();
    public ReactiveProperty<bool> isRoot = new ReactiveProperty<bool> ();

    public ReactiveProperty<bool> isAssigned = new ReactiveProperty<bool> (true);
    public ReadOnlyReactiveProperty<bool> isEmpty { get; private set; }


    public ReactiveProperty<NodePresenter> parentNode = new ReactiveProperty<NodePresenter>();
    public ReactiveProperty<int?> parentDiff = new ReactiveProperty<int?>();

    public ReactiveProperty<StaffModel> staffModel = new ReactiveProperty<StaffModel> ();

    CompositeDisposable childResources = new CompositeDisposable();
    CompositeDisposable staffResources = new CompositeDisposable();


    void Awake(){


        //difine props
        childCount = childNodes
            .ObserveEveryValueChanged(t => t.childCount)
            .ToReactiveProperty ();

        hasChild = childCount
            .Select (c => 0 < c)
            .ToReadOnlyReactiveProperty ();


        //if have parent node
        parentNode
            .Where (pn => pn != null)
            .Subscribe (pn => {
                parentDiff = pn.currentLevel
                    .CombineLatest(currentLevel, (l, r) => (int?)l - r)
                    .CombineLatest(pn.isEmpty, (l, r) => r ? null : l)
                    .ToReactiveProperty ();
            })
            .AddTo(this);

        parentNode
            .Select (p => p != null)
            .Subscribe (p => parentLine.gameObject.SetActive (p))
            .AddTo (this);

        childCount
            .Select (c => 0 < c)
            .Subscribe (c => childrenLineV.gameObject.SetActive (c))
            .AddTo (this);

        childCount
            .Select (c => 1 < c)
            .Subscribe (c => childrenLineH.gameObject.SetActive (c))
            .AddTo (this);

        childCount
            .Subscribe (_ => watchChildProps ())
            .AddTo (this);



        sales.SubscribeToText (salesText, x => Mathf.Floor (x).ToString ());
        salesTotal.SubscribeToText (salesTotalText, x => Mathf.Floor (x).ToString ());
        wage.SubscribeToText (wageText, x => Mathf.Floor (x).ToString ());
        wageTotal.SubscribeToText (wageTotalText, x => Mathf.Floor (x).ToString ());
        profit.SubscribeToText (profitText, x => Mathf.Floor (x).ToString ());

        salesTotal
            .CombineLatest (wageTotal, (l, r) => l - r)
            .Subscribe (p => profit.Value = p)
            .AddTo (this);

        //if have staff
        staffModel
            .Subscribe (s => {

                staffResources.Clear();
                if(s == null){
                    isAssigned.Value = false;
                    return;
                }
                isAssigned.Value = true;

                s.baseLevel
                    .CombineLatest (childCount, (l, r) =>  Mathf.Max(1, l - r))
                    .Subscribe(b => currentLevel.Value = b)
                    .AddTo(staffResources);

                //給料
                s.baseLevel
                    .Select (c => c * 10f + 10f)
                    .CombineLatest(s.age, (l, r) => l + r * .5f)
                    .Subscribe(c => wage.Value = c)
                    .AddTo(staffResources);

                //sales
                currentLevel
                    .Select(l =>  Mathf.Log10 (l * .25f + .75f) * 400f)
//                    .Select(l =>  Mathf.Log10 (l) * 200f)
//                    .CombineLatest (childCount, (l, r) => r > 0 ? l / r : l)
//                    .CombineLatest(s.age, (l, r) => l * ( Mathf.Log10( Mathf.Max(1f, (1f - r / 40f) * 9f + 1f) ) * .5f + .5f))
                    //            .CombineLatest (hasChild, (l, r) => r ? l * .5f : l)
                    //            .Select (l => Mathf.Log10 (l) * 200f)
                    .Subscribe (_ => sales.Value = _)
                    .AddTo(staffResources);


            })
            .AddTo (this);



        /*

        manCount =
            isEmpty
                .Select (a => a ? 0 : 1)
                .ToReactiveProperty ();

        //subscribe
        childCount
            .Subscribe (_ => watchChildSum ())
            .AddTo (this);

        */

        //        watchChildSum ();



    }
    void Start(){
        var gm = GameManager.Instance;
        var drag = staffPanelUI.GetComponent<DragTrigger> ();
        var drop = dropAreaUI.GetComponent<DropTrigger> ();
        var panelOutLine = panelContentUI.GetComponent<Outline> ();

        var panelCG = panelContentUI.GetComponent<CanvasGroup>();
        var staffPanelCG = staffPanelUI.GetComponent<CanvasGroup> ();
        var dropCG = dropAreaUI.GetComponent<CanvasGroup> ();

        var isDragged = gm.draggingNode
            .Select (n => n == this)
            .ToReactiveProperty ();


        isEmpty = isAssigned
            .CombineLatest (isDragged, (l, r) => !l || r)
            .ToReadOnlyReactiveProperty ();


        //show or hide staffpanel
        isEmpty
            .Subscribe (e => {
                staffPanelCG.alpha = e ? 0 : 1;
                staffPanelCG.blocksRaycasts = !e;
                emptyPanelUI.SetActive(e);
            })
            .AddTo (this);

        //enable drop if other node is dragging
        gm.draggingNode
            .Select (n => n && n != this)
            .Subscribe (d => {
                panelCG.blocksRaycasts = !d;
                dropCG.blocksRaycasts = d;
            })
            .AddTo (this);

        //destory if no content
        isDragged
            .CombineLatest (isAssigned, (l, r) => l || r)
            .CombineLatest (hasChild, (l, r) => l || r)
            .CombineLatest (isRoot, (l, r) => l || r)
            .Where(exist => !exist)
            .Subscribe (_ => Destroy (gameObject))
            .AddTo (this);


        //drag trigger
        drag.OnBeginDragAsObservable ()
            .Subscribe (_ => {
                //create cursor for drag
                var cursor = gm.createNode();
                cursor.staffModel.Value = staffModel.Value;
                var cursorCG = cursor.GetComponent<CanvasGroup>();
                cursorCG.blocksRaycasts = false;
                cursorCG.alpha = .7f;
                cursor.tier.Value = tier.Value;

                //set drag node
                gm.draggingNode.Value = this;

                drag.OnDragAsObservable ()
                    .Subscribe (d => {
                        (cursor.transform as RectTransform).position = Input.mousePosition;
                    })
                    .AddTo(cursor);

                drag.OnEndDragAsObservable()
                    .Subscribe(e => {
                        //remove cursor
                        Destroy (cursor.gameObject);

                        //clear drag node
                        gm.draggingNode.Value = null;
                    })
                    .AddTo(cursor);
            })
            .AddTo (this);

        //drop trigger
        drop.OnDropAsObservable ()
            .Subscribe (e => {
                var dragNode = e.pointerDrag.GetComponentInParent<NodePresenter>();

                //create child or copy value
                if(isAssigned.Value)
                {
                    var child = gm.createNode(childNodes);
                    child.staffModel.Value = dragNode.staffModel.Value;
                    child.parentNode.Value = this;
                    child.tier.Value = tier.Value + 1;
                }else{
                    staffModel.Value = dragNode.staffModel.Value;
                }

                //clear pointer value
                dragNode.staffModel.Value = null;
                gm.draggingNode.Value = null;

                //highlight
                panelContentUI.transform.localScale = Vector3.one;
                panelOutLine.effectColor = Color.black;
            })
            .AddTo (this);

        drop.OnPointerEnterAsObservable ()
            .Subscribe (_ => {
                panelContentUI.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
                panelOutLine.effectColor = Color.red;
            })
            .AddTo (this);

        drop.OnPointerExitAsObservable ()
            .Subscribe (_ => {
                panelContentUI.transform.localScale = Vector3.one;
                panelOutLine.effectColor = Color.black;
            })
            .AddTo (this);    

    }
    void watchChildProps(){
        childResources.Clear ();

        var lvList = new List<ReactiveProperty<int>> {currentLevel};
        var ccList = new List<ReactiveProperty<int>> {childCount};
        var mcList = new List<ReactiveProperty<int>> {manCount};
        var wageList = new List<ReactiveProperty<float>> {wage};
        var salesList = new List<ReactiveProperty<float>> {sales};

        foreach (Transform child in childNodes) {
            var node = child.GetComponent<NodePresenter> ();
            lvList.Add (node.currentLevelTotal);
            ccList.Add (node.childCountTotal);
            mcList.Add (node.manCountTotal);
            wageList.Add (node.wageTotal);
            salesList.Add (node.salesTotal);
        }


        //draw child line
        if (1 < childCount.Value) {
            var firstC = childNodes.GetChild (0);
            var lastC = childNodes.GetChild (childCount.Value - 1);

            var w = childNodes
                .ObserveEveryValueChanged (t => t.sizeDelta)
                .Select (d => d.x / 2)
                .ToReactiveProperty();
            var left = (firstC.transform as RectTransform)
                .ObserveEveryValueChanged (t => t.sizeDelta)
                .Select (d => d.x / 2)
                .ToReactiveProperty ();
            var right = (lastC.transform as RectTransform)
                .ObserveEveryValueChanged (t => t.sizeDelta)
                .Select (d => d.x / 2)
                .ToReactiveProperty ();

            left
                .CombineLatest(right, (l, r) => new Vector2(l, r))
                .CombineLatest(w, (l, r) => new Vector2(-r + l.x, r - l.y))
                .Subscribe (v => {
                    childrenLineH.rectTransform.offsetMin = new Vector2(v.x, childrenLineH.rectTransform.offsetMin.y);
                    childrenLineH.rectTransform.offsetMax = new Vector2(v.y, childrenLineH.rectTransform.offsetMax.y);
                })
                .AddTo(childResources);
        }

        Observable
            .CombineLatest (lvList.ToArray ())
            .Select (list => list.Sum())
            .Subscribe (v => currentLevelTotal.Value = v)
            .AddTo (childResources);

        Observable
            .CombineLatest (ccList.ToArray ())
            .Select (list => list.Sum())
            .Subscribe (v => childCountTotal.Value = v)
            .AddTo (childResources);

        Observable
            .CombineLatest (mcList.ToArray ())
            .Select (list => list.Sum ())
            .Subscribe (v => manCountTotal.Value = v)
            .AddTo (childResources);    

        Observable
            .CombineLatest (wageList.ToArray ())
            .Select (list => list.Sum ())
            .Subscribe (v => wageTotal.Value = v)
            .AddTo (childResources);    
        
        Observable
            .CombineLatest (salesList.ToArray ())
            .Select (list => list.Sum ())
            .Subscribe (v => salesTotal.Value = v)
            .AddTo (childResources);    
    
    }

    void OnDestroy()
    {
        childResources.Dispose ();
        staffResources.Dispose ();
    }

}
