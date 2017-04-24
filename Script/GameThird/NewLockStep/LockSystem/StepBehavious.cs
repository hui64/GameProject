using UnityEngine;
using System.Collections;
namespace LockStep {
   //类似MonoBehavious
    public class StepBehavious : StepComponent {
        public GameObject model;  //表现的gameObject 不是逻辑的gameObject(逻辑的unity.gameObject私有化的)
        public void ComponentUpdate() {
            for(int i = 0;i < transform.Components.Count;i++) {
                transform.Components[i].componentEvent.update();
            }
            for(int i = 0;i < transform.Components.Count;i++) {
                transform.Components[i].ContinueLoop();
            }
            this.componentEvent.update();
            this.ContinueLoop();
        }
        public new void Destroy(){
            DestroyActorComponent();
            StepManager.Instance.RemoveActor(this);
        }
        public new void Destroy(StepBehavious actor){
            DestroyActorComponent();
            StepManager.Instance.RemoveActor(actor);
        }
        private void DestroyActorComponent() {
            transform.RemoveAllComponent();
        }
    }
}
