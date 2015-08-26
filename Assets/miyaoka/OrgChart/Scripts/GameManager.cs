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
        var root = createNode(orgStaffContainer);
        root.isRoot.Value = true;
        root.tier.Value = 1;

        var count = 7;
        while (0 < count--) {
            createRecruit ();
        }
    }
    public NodePresenter createNode(Transform parent = null){
        var obj = Instantiate (nodePrefab);
        parent = parent ?? canvas.transform;
        obj.transform.SetParent (parent, false);
        var node = obj.GetComponent<NodePresenter> ();
        return node;
    }
    void createRecruit(){
        var n = createNode (recruitsContainer);
        var staff = new StaffModel();
        n.staffModel.Value = staff;

        var ss = (float)NormalDistributionConfidenceCalculator.NormInv ((double)Random.value, .5d, .1d);
        staff.stdScore.Value = ss;

        var age = Random.Range (0, 40);
        staff.age.Value = 0;
        staff.baseLevel.Value = Random.Range(1, 3);

        while (0 < age--) {
            addAge (staff);
        }
        staff.baseLevel.Value = (int)Mathf.Max(1, Mathf.Floor((float)staff.baseLevel.Value * .85f));
        staff.lastLevel.Value = -1;

        staff.name.Value = "";
        staff.hue.Value = Mathf.Floor (Random.value * 3) / 3 + (.2f > Random.value ? 1f/6f : 0);
        staff.moral.Value = Random.value * .6f + .3f;
    }
    void addAge(StaffModel sm){
        if (sm.age.Value < 40) {
            if (Random.value < .25f + (sm.stdScore.Value - .5f) * .5f) {
                sm.baseLevel.Value += 1;
            }
        } else {
            if (Random.value < .5) {
                sm.baseLevel.Value = (int)Mathf.Max(1, sm.baseLevel.Value - 1);
            }
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
            if (s.age.Value >= 45 && n.tier.Value > 1) {
                n.isAssigned.Value = false;
            }
        }

        foreach(Transform t in recruitsContainer)
        {
            Destroy (t.gameObject);
        }

        var count = 3;
        while (0 < count--) {
            createRecruit ();
        }
    }

}
