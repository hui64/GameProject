using UnityEngine;
using System.Collections;

public class ShapeTrigger {

    //===矩形跟矩形
    /// <summary>
    /// Vecttor4 x,y对应位置 z: width/2, w: height/2
    /// </summary>
    /// <param name="one"></param>
    /// <param name="two"></param>
    /// <returns></returns>
    public static bool CheckCubeToCube(Vector4 one,Vector4 two) {
        return Mathf.Abs(one.x - two.x) <= (one.z + two.z) && Mathf.Abs(one.y - two.y) <= (one.w + two.w);
    }
    //===圆跟圆
    /// <summary>
    /// Vecttor3 x,y对应位置 z: r
    /// </summary>
    /// <param name="one"></param>
    /// <param name="two"></param>
    /// <returns></returns>
    public static bool CheckSphereToSphere(Vector3 one,Vector3 two) {
        return (one.x - two.x) * (one.x - two.x) + (one.y - two.y) * (one.y - two.y) <= (one.z + two.z) * (one.z + two.z);
    }
    //点跟圆
    public static bool CheckPointToShere(Vector2 pos,Vector3 sphere) {
        return (pos.x - sphere.x) * (pos.x - sphere.x) + (pos.y - sphere.y) * (pos.y - sphere.y) < sphere.z * sphere.z;
    }
    private static byte index1 = 0;
    private static byte index4 = 0;
    private static Vector2 a1,a2,a3,a4;
    //圆跟矩形
    public static bool CheckSphereToCube(Vector3 s,Vector4 c) {
        a1 = new Vector2(c.x - c.z, c.y + c.w); //矩形左上顶点
        a2 = new Vector2(c.x + c.z, c.y + c.w);  //矩形右上顶点
        a3 = new Vector2(c.x - c.z, c.y - c.w);  //矩形左下顶点
        a4 = new Vector2(c.x + c.z, c.y - c.w);  //矩形右下顶点
        //确定物体中心在哪个象限

        index1 = 0;   //判断点一
        if(a1.x < s.x) {
            //左
            index1 += 2;
        }
        else {
            //右
            index1 -= 2;
        }
        if(a1.y > s.y) {
            //上
            index1 += 1;
        }
        else {
            //下
            index1 -= 1;
        }
        index4 = 0;   //判断点四
        if(a4.x < s.x) {
            //左
            index4 += 2;
        }
        else {
            //右
            index4 -= 2;
        }
        if(a4.y > s.y) {
            //上
            index4 += 1;
        }
        else {
            //下
            index4 -= 1;
        }
        //判断这两个点是否在圆的同一象限
        if(index1 == index4) 
        {
            //同一象限
            if(CheckPointToShere(a1,s) || CheckPointToShere(a2,s) || CheckPointToShere(a3,s) || CheckPointToShere(a4,s))
                return true;
            else
                return false;
        }
        //两个象限以上 使用矩形跟矩形碰撞
        return CheckCubeToCube(new Vector4(s.x,s.y,s.z,s.z),c);
    }
}
