using UnityEngine;
using System.Collections;
namespace LockStep {
    public class Demo : MonoBehaviour {

        void Start() {
            Debug.Log("DemoStart");
            GameObject drawModel = Resources.Load("locktest") as GameObject;
            StepManager.Instance.AddActor<DemoStepBehavious>(this.transform,drawModel);
        }
        void FixedUpdate() {
            StepManager.Instance.LoopLockStep(0.02f);
        }
    }
}