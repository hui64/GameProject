using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LockStep {
    public class YieldInstruction {
        public virtual bool IsDone() {
            return true;
        }
    }
    public class WaitForSeconds:YieldInstruction {
        public float stepdelTime = 0.02f;
        public float _wait;
        public float _start = 0;
        public WaitForSeconds(float time) {
            _wait = time;
        }
        public override bool IsDone() {
            _start += stepdelTime;
            return _start >= _wait;
        }
    }
    public class ComponnetEvent {
        public StepComponent actor;
        public System.Reflection.MethodInfo Update;
        public System.Reflection.MethodInfo OnEnable;
        public System.Reflection.MethodInfo OnDisable;
        public System.Reflection.MethodInfo OnDestroy;
        public void update() {
            if(Update != null)
                Update.Invoke(actor, null);
        }
        public void onEnable() {
            if(OnEnable != null)
                OnEnable.Invoke(actor, null);
        }
        public void onDisable() {
            if(OnDisable != null)
                OnDisable.Invoke(actor,null);
        }
        public void onDestroy() {
            if(OnDestroy != null)
                OnDestroy.Invoke(actor,null);
        }
    }
    public class StepComponent: Object {
        //=========================外部调用==================
        public void Wait(float time,System.Action fun) {
            StartContinue(WaitTime(time,fun));
        }
        public void StartContinue(IEnumerator ie) {
            Coroutines.Add(ie);
        }
        public void StartContinue(string name) {
            System.Reflection.MethodInfo fun = this.GetType().GetMethod(name,System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            NameCoroutines.Add(name,fun.Invoke(this,null) as IEnumerator);
        }
        public void StopAllContinue() {
            Coroutines.Clear();
            NameCoroutines.Clear();
            removeCache.Clear();
            NameRemoveCache.Clear();
        }
        public void StopContinue(string name) {
            if(NameCoroutines.ContainsKey(name)) {
                NameCoroutines.Remove(name);
                if(NameRemoveCache.Contains(name))
                    NameRemoveCache.Remove(name);
            }
        }
        public virtual void Destroy() {
            transform.RemoveComponent(this);
        }
        public virtual void Destroy(StepComponent component) {
            transform.RemoveComponent(component);
        }
        public void print(object obj) {
            Debug.Log(obj);
        }
        //=========================外部调用end==================
        public int fag;
        public StepTransform transform;
        public StepGameObject gameObject;
        protected Transform _transform;
        public ComponnetEvent componentEvent;
        private bool _enable;
        public bool Enable {
            get {
                return _enable;
            }
            set {
                if(value)
                    componentEvent.onEnable();
                else
                    componentEvent.onDisable();
                _enable = value;
            }
        }
        internal void SetComponentEvent() {
            if(componentEvent != null)
                return;
            componentEvent = new ComponnetEvent();
            componentEvent.actor = this;
            System.Type type = this.GetType();
            System.Reflection.MethodInfo methodAwake = GetFunInfo(type,"Awake");
            if(methodAwake != null)
                methodAwake.Invoke(this,null);
            componentEvent.Update = GetFunInfo(type,"Update");
            componentEvent.OnEnable = GetFunInfo(type,"OnEnable");
            componentEvent.OnDisable = GetFunInfo(type,"OnDisable");
            componentEvent.OnDestroy = GetFunInfo(type,"OnDestroy");
        }
        private System.Reflection.MethodInfo GetFunInfo(System.Type type, string funName) {
            return type.GetMethod(funName,System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        }
        private List<IEnumerator> Coroutines = new List<IEnumerator>();
        private List<int> removeCache = new List<int>();
        private Dictionary<string,IEnumerator> NameCoroutines = new Dictionary<string,IEnumerator>();
        private List<string> NameRemoveCache = new List<string>();
        private IEnumerator WaitTime(float time,System.Action fun) {
            yield return new WaitForSeconds(time);
            fun();
        }
        internal void ContinueLoop() {
            //==name
            var keys = NameCoroutines.Keys;
            foreach(string key in keys) {
                if(NameCoroutines[key].Current == null) {
                    NameCoroutines[key].MoveNext();
                }
                if(NameCoroutines[key].Current is YieldInstruction)
                    if(!(NameCoroutines[key].Current as YieldInstruction).IsDone())
                        continue;
                if(!NameCoroutines[key].MoveNext()) {
                    NameRemoveCache.Add(key);
                }
            }
            for(int i = 0;i < NameRemoveCache.Count;i++) {
                NameCoroutines.Remove(NameRemoveCache[i]);
            }
            NameRemoveCache.Clear();
            //==fun
            for(int i = 0;i < Coroutines.Count;i++) {
                if(Coroutines[i].Current == null) {
                    Coroutines[i].MoveNext();
                }
                if(Coroutines[i].Current is YieldInstruction)
                    if(!(Coroutines[i].Current as YieldInstruction).IsDone())
                        continue;
                if(!Coroutines[i].MoveNext()) {
                    removeCache.Add(i);
                }
            }
            for(int i = 0;i < removeCache.Count;i++) {
                Coroutines.RemoveAt(removeCache[i]);
            }
            removeCache.Clear();
        }
    }
}