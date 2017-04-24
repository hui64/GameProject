using UnityEngine;
using System.Collections;
namespace LockStep {
    public class DemoStepBehavious:StepBehavious {
        void Awake(){
            transform.AddComponent<DemoStepComponent>();
            Debug.Log("ActorMonoDemo :Awake");
            StartContinue("Move");
        }
        IEnumerator Move() {
            while(true) {
                yield return new WaitForSeconds(1.0f);
                Debug.Log("Move");
            }
        }
        void OnEnable() {

        }
        void OnDisable() {

        }
        void Update(){
            //Debug.Log("......");
            //Destroy();
        }
        void OnDestroy(){
            Debug.Log("ActorMonoDemo :OnDestory");
        }
    }
}
