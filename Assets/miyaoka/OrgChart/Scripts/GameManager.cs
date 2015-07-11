using UnityEngine;
using System.Collections;
using UniRx;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField] public RectTransform orgStaffContainer;
    [SerializeField] public RectTransform recruitsContainer;
    [SerializeField] public Canvas canvas;
    [SerializeField] public GameObject nodePrefab;
    [SerializeField] public GameObject ignore;


    public ReactiveProperty<NodePresenter> draggingNode = new ReactiveProperty<NodePresenter>();

    public void Awake ()
    {
        if (this != Instance) {
            Destroy (this);
            return;
        }
        DontDestroyOnLoad (this.gameObject);
    }

    // Use this for initialization
    void Start ()
    {
        Destroy (ignore);
        var root = createNode(null, orgStaffContainer);
        root.isRoot.Value = true;
        root.tier.Value = 1;

        var count = 7;
        while (0 < count--) {
            createRecruit ();
        }
    }
    public NodePresenter createNode(StaffModel staff = null, Transform parent = null){
        var obj = Instantiate (nodePrefab);
        parent = parent ?? canvas.transform;
        obj.transform.SetParent (parent, false);
        var node = obj.GetComponent<NodePresenter> ();
        node.staffModel.Value = staff;
        return node;
    }
    void createRecruit(){
        var staff = new StaffModel();
        var ss = (float)NormalDistributionConfidenceCalculator.NormInv ((double)Random.value, .5d, .1d);
        staff.stdScore.Value = ss;
        var age = Random.Range (0, 30);
        staff.age.Value = 0;
        staff.baseLevel.Value = 0;

        while (0 < age--) {
            addAge (staff);
        }
        staff.baseLevel.Value = (int)Mathf.Floor((float)staff.baseLevel.Value * .8f);
        staff.name.Value = "";
        staff.hue.Value = Mathf.Floor (Random.value * 3) / 3 + (.2f > Random.value ? 1f/6f : 0);
        staff.moral.Value = Random.value * .8f + .2f;
        createNode (staff, recruitsContainer);
    }
    void addAge(StaffModel sm){
        if (sm.age.Value < 40) {
            if (Random.value < .4){//sm.stdScore.Value) {
                sm.baseLevel.Value += 1;
            }
        } else {
            sm.baseLevel.Value -= 1;
        }
        sm.age.Value++;
    }
    public void nextYear(){

        NodePresenter[] nodes = orgStaffContainer.GetComponentsInChildren<NodePresenter> ();
        foreach (NodePresenter n in nodes) {
            
            var s = n.staffModel.Value;
            if (s == null) {
                continue;
            }
            addAge (s);
        }

        foreach(Transform t in recruitsContainer)
        {
            Destroy (t.gameObject);
        }

        var count = 5;
        while (0 < count--) {
            createRecruit ();
        }
    }

}
