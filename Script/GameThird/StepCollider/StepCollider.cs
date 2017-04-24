using UnityEngine;
using System.Collections;
# if  UNITY_EDITOR
using UnityEditor;


# endif
//可以扩展显示在Intpector(每个碰撞体都挂在一个GameObject上, 碰撞体的属性可以在上面显示)
public class StepCollider {
    public StepCollider() {

    }
    public StepCollider(Vector2 pos,int r,GameCamp camp) {
        this.pos = pos;
        this.r = r;
        this.camp = camp;
        cacheX = Mathf.CeilToInt(pos.x);
        cacheY = Mathf.CeilToInt(pos.x);
    }
    public StepCollider(Vector2 pos,int width,int height,GameCamp camp) {
        this.pos = pos;
        this.width = width;
        this.height = height;
        this.camp = camp;
        cacheX = Mathf.CeilToInt(pos.x);
        cacheY = Mathf.CeilToInt(pos.x);
    }
    public GameCamp camp = GameCamp.Player;
    public ushort id;
    public int cacheX;
    public int cacheY;
    public float r = 1;
    public float width = 1;
    public float height = 1;
    private Vector2 _pos;
    public int shapeType = 1;   //0代表圆形 1代表矩形 
    
    public Vector2 pos {
        get {
            return _pos;
        }
        set {
            _pos = value;
        }
    }
    public void EnterTrigger(StepCollider other) {

    }
    public void OnLeaveTrigger(StepCollider other) {
    
    }
    public void ExitTrigger(StepCollider other) {
    
    }
#if  UNITY_EDITOR
    //属性绘制
    public void DrawAttirbute() {
        EditorGUILayout.LabelField("pos :(" + _pos.x + "," + _pos.y +")");
        if(shapeType == 0) {
            EditorGUILayout.LabelField("r :" + r);
        }
        if(shapeType == 1) {
            EditorGUILayout.LabelField("width :" + width);
            EditorGUILayout.LabelField("height :" + height);
        }
    }
# endif
}
