using UnityEngine;
using System.Collections;

public class GameMono : MonoBehaviour {
    public delegate void loopFun();
    public static loopFun LoopEvent;
    public static GameMono instance;
	// Use this for initialization
	void Start () {
        instance = this;
	}
    void Update() {
        if(LoopEvent != null) {
            LoopEvent();
        }
    }

}
