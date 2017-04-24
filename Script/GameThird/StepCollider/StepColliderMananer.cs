using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class StepColliderMananer : MonoBehaviour {
    public List<ushort> OnTrigger = new List<ushort>();
    public List<ushort> CacheTrigger = new List<ushort>();
    private int ArrayBeginX;
    private int ArrayBeginY;
    private int ArrayLenX;
    private int ArrayLenY;
    private int OwnBeginX;
    private int OwnBeginY;
    private int OwnEndX;
    private int OwnEndY;
    private List<ushort> Enter = new List<ushort>();
    private List<ushort> OnLeave = new List<ushort>();
    private List<ushort> Exit = new List<ushort>();
    private List<ushort> OnRect = new List<ushort>();
    private List<ushort> OnSphere = new List<ushort>();
    public static StepColliderMananer Instance;
    private ushort MapX = 128;
    private ushort MapY = 128;
    private byte MaxR = 2;
    public List<ushort>[,] MapDatas;
    public ushort StepIndex = 0;
    //暂时分两边阵营
    Dictionary<ushort,StepCollider> StepColliders = new Dictionary<ushort,StepCollider>();
    List<ushort> PlayerIndexs = new List<ushort>();
    List<ushort> EnemyIndexs = new List<ushort>();
    public Vector4 Rect;
    public Vector3 Sphere;
    private StepCollider Other;

    public void AddSphereCollider(Vector2 pos,int r,GameCamp camp) {
        StepCollider info = new StepCollider(pos,r,camp);
        StepColliders.Add(StepIndex,info);
        if(camp == GameCamp.Player)
            PlayerIndexs.Add(StepIndex++);
        if(camp == GameCamp.Enemy)
            EnemyIndexs.Add(StepIndex++);
    }
    public void AddCubeCollider(Vector2 pos,int width,int height,GameCamp camp) {
        StepCollider info = new StepCollider(pos,width,height,camp);
        StepColliders.Add(StepIndex,info);
        if(camp == GameCamp.Player)
            PlayerIndexs.Add(StepIndex++);
        if(camp == GameCamp.Enemy)
            EnemyIndexs.Add(StepIndex++);
    }
    public void RemoveCollider(StepCollider someOne) {
        RemoveCollider(someOne.id);
    }
    public void RemoveCollider(ushort id) {
        StepColliders.Remove(id);
        if(StepColliders[id].camp == GameCamp.Player) {
            PlayerIndexs.Remove(id);
        }
        else {
            EnemyIndexs.Remove(id);
        }
        MapDatas[StepColliders[id].cacheX,StepColliders[id].cacheY].Remove(id);
    }
    //获取矩形范围内的物体
    int x1, x2, y1, y2;
    public void GetGameObjectByRect(Vector4 rect) {
        OnRect.Clear();
        x1 = Mathf.CeilToInt(rect.x - rect.z) - 2;
        x2 = Mathf.CeilToInt(rect.x + rect.z) + 2;
        y1 = Mathf.CeilToInt(rect.y + rect.w) + 2;
        y2 = Mathf.CeilToInt(rect.y - rect.w) - 2;
        for(int i = x1;i <= x2;i++) {
            for(int j = y2;j <= y1;j++) {
                if(i < 0 || j < 0) continue;
                if(i >= MapX || j >= MapY) continue;
                if(MapDatas[i,j] == null) continue;
                for(int k = 0;k < MapDatas[i,j].Count;k++) {
                    Other = StepColliders[MapDatas[i,j][k]];
                    if(Other.shapeType == 0) {
                        if(ShapeTrigger.CheckSphereToCube(new Vector3(Other.pos.x,Other.pos.y,Other.r),rect))
                            OnRect.Add(MapDatas[i,j][k]);
                        continue;
                    }
                    if(ShapeTrigger.CheckCubeToCube(rect,new Vector4(Other.pos.x,Other.pos.y,Other.width,Other.height)))
                        OnRect.Add(MapDatas[i,j][k]);
                }
            }
        }
        //Debug.Log(OnRect.Count);
    }
    //获取圆形范围内的物体
    public void GetGameObjectSphere(Vector3 sphere) {
        OnSphere.Clear();
        x1 = Mathf.CeilToInt(sphere.x - sphere.z) - 2;
        x2 = Mathf.CeilToInt(sphere.x + sphere.z) + 2;
        y1 = Mathf.CeilToInt(sphere.y + sphere.z) + 2;
        y2 = Mathf.CeilToInt(sphere.y - sphere.z) - 2;
        for(int i = x1;i <= x2;i++) {
            for(int j = y2;j <= y1;j++) {
                if(i < 0 || j < 0) continue;
                if(i >= MapX || j >= MapY) continue;
                if(MapDatas[i,j] == null) continue;
                for(int k = 0;k < MapDatas[i,j].Count;k++) {
                    Other = StepColliders[MapDatas[i,j][k]];
                    if(Other.shapeType == 1) {
                        if(ShapeTrigger.CheckSphereToCube(new Vector3(sphere.x,sphere.y,sphere.z),new Vector4(Other.pos.x,Other.pos.y,Other.width,Other.height)))
                            OnSphere.Add(MapDatas[i,j][k]);
                        continue;
                    }
                    if(ShapeTrigger.CheckSphereToSphere(sphere,new Vector3(Other.pos.x,Other.pos.y,Other.r)))
                        OnSphere.Add(MapDatas[i,j][k]);
                }
            }
        }
    }
    void Awake() {
        Instance = this;
    }
	void Start () {
        Init();
	}
    private void Init() {
        InitStepCollider();
        InitByColliders();
    }
    private void InitStepCollider() {
        ushort index = 0;
        for(ushort i = 0;i < 500;i++) {
            StepCollider info = new StepCollider();
            info.id = index++;
            info.r = Random.Range(1.0f,2.0f);
            info.width = Random.Range(1.0f,2.0f);
            info.height = Random.Range(1.0f,2.0f);
            info.shapeType = Random.Range(0,2);
            if(i % 2 == 0) {
                info.camp = GameCamp.Player;
                PlayerIndexs.Add(info.id);
            }
            else {
                info.camp = GameCamp.Enemy;
                EnemyIndexs.Add(info.id);
            }
            info.pos = new Vector2(Random.Range(0,MapX),Random.Range(0,MapY));
            info.cacheX = Mathf.CeilToInt(info.pos.x);
            info.cacheY = Mathf.CeilToInt(info.pos.x);
            StepColliders.Add(info.id,info);
        }
    }
    private void InitByColliders() {
        MapDatas = new List<ushort>[MapX,MapY];
        for(ushort i = 0;i < StepColliders.Count;i++) {
            if(MapDatas[StepColliders[i].cacheX,StepColliders[i].cacheY] == null)
                MapDatas[StepColliders[i].cacheX,StepColliders[i].cacheY] = new List<ushort>();
            MapDatas[StepColliders[i].cacheX,StepColliders[i].cacheY].Add(StepColliders[i].id);
        }
    }
    void Update() {
        Enter.Clear();
        OnLeave.Clear();
        Exit.Clear();
        for(ushort i = 0;i < StepColliders.Count;i++) {
            StepColliders[i].pos *= 1 + 0.0001f * (i % 4);
            if(StepColliders[i].pos.x >= MapX - 1 || StepColliders[i].pos.y >= MapX - 1) {
                StepColliders[i].pos = new Vector2(Random.Range(0,MapX),Random.Range(0,MapY));
                //StepColliders[i].pos = Vector2.zero;
            }
            ChangeMapPos(StepColliders[i]);
        }
        CheckAll();
        GetGameObjectByRect(Rect);
        GetGameObjectSphere(Sphere);
    }
    //位置修改后重新定位在Mapdatas中的位置
    private void ChangeMapPos(StepCollider someOne) {
        MapDatas[someOne.cacheX,someOne.cacheY].Remove(someOne.id);
        someOne.cacheX = Mathf.CeilToInt(someOne.pos.x);
        someOne.cacheY = Mathf.CeilToInt(someOne.pos.y);
        if(MapDatas[someOne.cacheX,someOne.cacheY] == null)
            MapDatas[someOne.cacheX,someOne.cacheY] = new List<ushort>();
        MapDatas[someOne.cacheX,someOne.cacheY].Add(someOne.id);
    }
    private void CheckAll() {
        for(ushort i = 0;i < PlayerIndexs.Count;i++) {
            SomeOneMove(StepColliders[PlayerIndexs[i]]);
        }
    }
    int r;
    //移动后检测碰撞
    private void SomeOneMove(StepCollider someOne) {
        CacheTrigger.Clear();
        r = Mathf.FloorToInt(someOne.r);
        OwnBeginX = someOne.cacheX - r;
        OwnBeginY = someOne.cacheY - r;
        OwnEndX = someOne.cacheX + r;
        OwnEndY = someOne.cacheY + r;
        ArrayBeginX = OwnBeginX - MaxR - 1;
        ArrayBeginY = OwnBeginY - MaxR - 1;
        ArrayLenX = OwnEndX + MaxR + 1;
        ArrayLenY = OwnEndY + MaxR + 1;
        //同一个格子内
        for(int i = 0;i < MapDatas[someOne.cacheX,someOne.cacheY].Count;i++) {
            if(someOne.id == MapDatas[someOne.cacheX,someOne.cacheY][i]) continue;
            CacheTrigger.Add(MapDatas[someOne.cacheX,someOne.cacheY][i]);
        }

        for(int i = ArrayBeginX;i <= ArrayLenX;i++) {
            for(int j = ArrayBeginY;j <= ArrayLenY;j++) {
                if(j < 0 || i < 0) continue;
                if(j >= MapX || i >= MapY) continue;
                if(MapDatas[i,j] == null) continue;
                for(int k = 0;k < MapDatas[i,j].Count;k++) {
                    if(someOne.id == MapDatas[i,j][k]) continue;
                    if(someOne.camp == StepColliders[MapDatas[i,j][k]].camp) continue;
                    if(IsEnter(someOne,StepColliders[MapDatas[i,j][k]])) {
                        CacheTrigger.Add(MapDatas[i,j][k]);
                    }
                }
            }
        }
        TriggerEvent(someOne);
    }
    private bool IsEnter(StepCollider someOne,StepCollider other) {
        if(someOne.shapeType == 1 && other.shapeType == 1) {
            return ShapeTrigger.CheckCubeToCube(new Vector4(someOne.pos.x,someOne.pos.y,someOne.width,someOne.width),new Vector4(other.pos.x,other.pos.y,other.width,other.width));
        }
        if(someOne.shapeType == 0 && other.shapeType == 0) {
            return ShapeTrigger.CheckSphereToSphere(new Vector3(someOne.pos.x,someOne.pos.y,someOne.r),new Vector3(other.pos.x,other.pos.y,other.r));
        }
        if(someOne.shapeType == 1) {
            return ShapeTrigger.CheckSphereToCube(new Vector3(other.pos.x, other.pos.y,other.r), new Vector4(someOne.pos.x, someOne.pos.y, someOne.width , someOne.height));
        }
        return ShapeTrigger.CheckSphereToCube(new Vector3(someOne.pos.x,someOne.pos.y,someOne.r),new Vector4(other.pos.x,other.pos.y,other.width ,other.height));
    }
   
    private void TriggerEvent(StepCollider someOne) {
        if(CacheTrigger.Count > 0) {
            CacheTrigger.Add(someOne.id);
        }
        for(ushort i = 0;i < OnTrigger.Count;i++) {
            if(!CacheTrigger.Contains(OnTrigger[i])) {
                //离开碰撞
                StepColliders[OnTrigger[i]].ExitTrigger(someOne);
                Exit.Add(OnTrigger[i]);
                OnTrigger.RemoveAt(i);
            }
        }
        for(ushort i = 0;i < CacheTrigger.Count;i++) {
            OnLeave.Add(CacheTrigger[i]);
            if(OnTrigger.Contains(CacheTrigger[i])) {
                //碰撞中
                //StepColliders[CacheTrigger[i]].OnLeaveTrigger(someOne);
            }
            else {
                OnTrigger.Add(CacheTrigger[i]);
                if(someOne.id != CacheTrigger[i]) {
                    //开始碰撞;
                    StepColliders[CacheTrigger[i]].EnterTrigger(someOne);
                    Enter.Add(CacheTrigger[i]);
                }
            }
        }
    }
    void OnDrawGizmos() {
        if(MapDatas == null)
            return;
        for(int j = 0;j < MapX;j++) {
            for(int i = 0;i < MapY;i++) {
                Gizmos.color = Color.white;
                Vector3 center = new Vector3(j,i,0);
                Gizmos.DrawWireCube(center,Vector2.one);
            }
        }

        for(ushort i = 0;i < StepColliders.Count;i++) {
            if(StepColliders[i].camp == GameCamp.Player)
                Gizmos.color = Color.black;
            else
                Gizmos.color = Color.cyan;
            //if(Exit.Contains(StepColliders[i].Id)) {
            //    Gizmos.color = Color.yellow;
            //}
            //else if(OnLeave.Contains(StepColliders[i].Id)) {
            //    Gizmos.color = Color.green;
            //}
            //else if(Enter.Contains(StepColliders[i].Id)) {
            //    Gizmos.color = Color.blue;
            //}
            if(OnLeave.Contains(StepColliders[i].id)) {
                Gizmos.color = Color.green;
            }
            if(OnRect.Contains(StepColliders[i].id)) {
                Gizmos.color = Color.red;
            }
            if(OnSphere.Contains(StepColliders[i].id)) {
                Gizmos.color = Color.blue;
            }
            Vector2 center = StepColliders[i].pos;
            if(StepColliders[i].shapeType == 1)
                Gizmos.DrawWireCube(center,new Vector2(StepColliders[i].width * 2,StepColliders[i].height * 2));
            else {
                DrawHelper.DrawSphere(center,StepColliders[i].r,Gizmos.color);
            }
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(new Vector2(Rect.x,Rect.y),new Vector2(Rect.z,Rect.w) * 2);
            Gizmos.color = Color.yellow;
            DrawHelper.DrawSphere(new Vector2(Sphere.x,Sphere.y),Sphere.z,Gizmos.color);
        }
    }
}
