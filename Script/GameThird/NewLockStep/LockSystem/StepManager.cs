using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
namespace LockStep{
   
    public class StepManager {

        private static StepManager _instance;
        private StepManager() { }
        public static StepManager Instance {
            get {
                if(_instance == null)
                    _instance = new StepManager();
                return _instance;
            }
        }
        private Dictionary<int, StepBehavious> Actors = new Dictionary<int, StepBehavious>();
        private int ListFag = 0;
        //添加actor
        public T AddActor<T>(Transform _tran, GameObject drawModel)where T: StepBehavious, new (){
            T t = new T();
            t.fag = ListFag;
            t.transform = new StepTransform(_tran);
            t.gameObject = new StepGameObject();
            t.gameObject.transform = t.transform;
            t.model = drawModel;
            t.SetComponentEvent();
            Actors.Add(ListFag, t);
            ListFag++;
#if CHECKSTEP
            LockStepCheck.getIntance().Check(Actors.Count);
            LockStepCheck.getIntance().Check(t.GetType().Name);
#endif
            return t;
        }
        public bool RemoveActor(StepBehavious actor) {
            return RemoveActor(actor.fag);
        }
        //移除actor
        public bool RemoveActor(int fag){
            if(Actors.ContainsKey(fag)) {
                Debug.Log("onDestory");
                Actors[fag].componentEvent.onDestroy();
                Actors.Remove(fag);
#if CHECKSTEP
                LockStepCheck.getIntance().Check(fag);
                LockStepCheck.getIntance().Check(Actors.Count);
#endif
                return true;
            }
            return false;
        }
        public void LoopLockStep(float time) {
            //Debug.Log("loop");
            for(int i = 0;i < Actors.Count;i++) {
                if(Actors[i].gameObject.activeSelf) {
                    Actors[i].ComponentUpdate();
                }
            }   
        }
	}
}
