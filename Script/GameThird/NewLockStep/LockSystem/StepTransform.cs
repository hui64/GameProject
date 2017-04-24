using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace LockStep {
    public class StepTransform : StepComponent {
        public StepTransform(Transform _tran) {
            _transform = _tran;
        }
        //========================================= StepVector3 =====================================
        private StepVector3 _position;
        private StepVector3 _localScan;
        private StepVector3 _localPosition;
        public Vector3 forward {
            get {
                return _transform.forward;
            }
            set {
                _transform.forward = value;
            }
        }
        public Vector3 right {
            get {
               return _transform.right;
            }
            set {
                _transform.right = value;
            }
        }
        /// <summary>
        /// 世界位置
        /// </summary>
        public Vector3 position {
            get {
                return _position.Vector3;
            }
            set {
                _position = new StepVector3(value);
                _transform.position = _position.Vector3;
#if CHECKSTEP
                LockStepCheck.getIntance().Check(_position);
#endif
            }
        }
        /// <summary>
        /// 自身位置
        /// </summary>
        public Vector3 localPosition {
            get {
                return _localPosition.Vector3;
            }
            set {
                _localPosition = new StepVector3(value);
                _transform.localPosition = _localPosition.Vector3;
#if CHECKSTEP
                LockStepCheck.getIntance().Check(_localPosition);
#endif
            }
        }
        /// <summary>
        /// 自身大小
        /// </summary>
        public Vector3 localScan {
            get {
                return _localScan.Vector3;
            }
            set {
                _localScan = new StepVector3(value);
                _transform.localScale = _localScan.Vector3;
#if CHECKSTEP
                LockStepCheck.getIntance().Check(_localScan);
#endif
            }
        }
        public void LookAt(StepTransform target) {
            this._transform.LookAt(target._transform);
        }

        //======================================= 组件 ActorComponent =======================================
        public List<StepComponent> Components = new List<StepComponent>();
        public T AddComponent<T>() where T:StepComponent,new() {
            T t = new T();
            t.transform = this;
            t.transform._transform = _transform;
            t.SetComponentEvent();
            Components.Add(t);
#if CHECKSTEP
            LockStepCheck.getIntance().Check(Components.Count);
            LockStepCheck.getIntance().Check(t.GetType().Name);
#endif
            return t;
        }
        public T GetComponent<T>() where T : StepComponent {
            for(int i = 0;i < Components.Count;i++) {
                if(Components[i] is T) {
                    return Components[i] as T;
                }
            }
            return null;
        }
        public bool RemoveComponent<T>() where T:StepComponent {
            for(int i = 0;i < Components.Count;i++) {
                if(Components[i] is T) {
#if CHECKSTEP
                    LockStepCheck.getIntance().Check(i);
                    LockStepCheck.getIntance().Check(Components[i].GetType().Name);
#endif
                    Components.Remove(Components[i]);
                    Components[i].componentEvent.onDestroy();

                    return true;
                }
            }
            return false;
        }
        public bool RemoveComponent(StepComponent component) {
            for(int i = 0;i < Components.Count;i++) {
                if(Components[i].Equals(component)) {
#if CHECKSTEP
                    LockStepCheck.getIntance().Check(i);
                    LockStepCheck.getIntance().Check(Components[i].GetType().Name);
#endif
                    Components[i].componentEvent.onDestroy();
                    Components.Remove(Components[i]);
                    return true;
                }
            }
            return false;
        }
        public void RemoveAllComponent() {
            for(int i = 0;i < Components.Count;i++) {
                Components[i].componentEvent.onDestroy();
            }
#if CHECKSTEP
            LockStepCheck.getIntance().Check(Components.Count);
#endif
            Components = null;
        }
    }
}