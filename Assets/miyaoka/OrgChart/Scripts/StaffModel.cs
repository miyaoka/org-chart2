using UnityEngine;
using System.Collections;
using UniRx;

public class StaffModel {
    public ReactiveProperty<string> name = new ReactiveProperty<string> ();
    public ReactiveProperty<int> gender = new ReactiveProperty<int> ();
    public ReactiveProperty<int> baseLevel =  new ReactiveProperty<int> ();  
    public ReactiveProperty<int> lastLevel =  new ReactiveProperty<int> ();  
    public ReactiveProperty<int> age = new ReactiveProperty<int> ();
    public ReactiveProperty<float> health =  new ReactiveProperty<float> (); 
    public ReactiveProperty<float> damage =  new ReactiveProperty<float> (); 
    public ReactiveProperty<float> attackInterval = new ReactiveProperty<float>();
    public ReactiveProperty<float> attackTimer = new ReactiveProperty<float>();

    public ReactiveProperty<Color> skinColor = new ReactiveProperty<Color>();
    public ReactiveProperty<Color> hairColor = new ReactiveProperty<Color>();
    public ReactiveProperty<Color> clothColor = new ReactiveProperty<Color>();

    public ReactiveProperty<float> recruitCost = new ReactiveProperty<float>();
    public ReactiveProperty<float> stdScore = new ReactiveProperty<float>();
    public ReactiveProperty<float> hue = new ReactiveProperty<float>();
    public ReactiveProperty<float> moral = new ReactiveProperty<float>();

}
