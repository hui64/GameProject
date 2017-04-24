using UnityEngine;
using System.Collections;
namespace LockStep{
    public class StepGameObject: Object {
        //public GameObject model;
        public StepTransform transform;
        public bool _isActive = true;
        public bool activeSelf {
            get {
                return _isActive;
            }
        }
        public void SetActive(bool isActive) {
            _isActive = isActive;
        }
        public T AddComponent<T>() where T:StepComponent,new() {
            return transform.AddComponent<T>();
        }
        public T GetComponent<T>() where T:StepComponent {
            return transform.GetComponent<T>();
        }
        public bool RemoveComponent<T>() where T:StepComponent {
            return transform.RemoveComponent<T>();
        }
        public bool RemoveComponent(StepComponent component) {
            return transform.RemoveComponent(component);
        }
        public void RemoveAllComponent() {
            transform.RemoveAllComponent();
        }
    }
}
