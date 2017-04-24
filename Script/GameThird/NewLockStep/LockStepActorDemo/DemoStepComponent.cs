using UnityEngine;
using System.Collections;
namespace LockStep {
    public class DemoStepComponent: StepComponent {
        void Awake() {
            Debug.Log("componentStart");
        }
        void Update() {
            Debug.Log("componentUpdate");
            Destroy();
        }
        void OnDestroy() {
            Debug.Log("ComponentOndestory");
        }
    }
}
