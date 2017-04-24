using UnityEngine;
using System.Collections;

public class DrawHelper {
    public static void DrawSphere(Vector2 center, float radius, Color color) {
        Circle2d circle2 = new Circle2d(center, radius);
        DrawSphere(ref circle2, color);
    }    
    public static void DrawSphere(ref Circle2d circle,Color color) {
        int count = 40;
        float delta = 2f * Mathf.PI / count;
        Vector3 prev = circle.Eval(0);

        Color tempColor = Gizmos.color;
        Gizmos.color = color;

        for(int i = 1;i <= count;++i) {
            Vector3 curr = circle.Eval(i * delta);
            Gizmos.DrawLine(prev,curr);
            prev = curr;
        }

        Gizmos.color = tempColor;
    }

    protected void DrawCircle(Vector2 center,float radius,Color color) {
        Circle2d circle = new Circle2d(ref center,radius);
        int count = 40;
        float delta = 2f * Mathf.PI / count;
        Vector3 prev = circle.Eval(0);

        Color tempColor = Gizmos.color;
        Gizmos.color = color;

        for(int i = 1;i <= count;++i) {
            Vector3 curr = circle.Eval(i * delta);
            Gizmos.DrawLine(prev,curr);
            prev = curr;
        }

        Gizmos.color = tempColor;
    } 
}
public struct Circle2d {
    public Vector2 Center;
    public float Radius;
    public Circle2d(ref Vector2 center,float radius) {
        this.Center = center;
        this.Radius = radius;
    }

    public Circle2d(Vector2 center,float radius) {
        this.Center = center;
        this.Radius = radius;
    }

    public float CalcPerimeter() {
        return (6.283185f * this.Radius);
    }

    public float CalcArea() {
        return ((3.141593f * this.Radius) * this.Radius);
    }

    public Vector2 Eval(float t) {
        return new Vector2(this.Center.x + (this.Radius * Mathf.Cos(t)),this.Center.y + (this.Radius * Mathf.Sin(t)));
    }

    public Vector2 Eval(float t,float radius) {
        return new Vector2(this.Center.x + (radius * Mathf.Cos(t)),this.Center.y + (radius * Mathf.Sin(t)));
    }
    public bool Contains(ref Vector2 point) {
        Vector2 vector = point - this.Center;
        return (vector.magnitude <= (this.Radius * this.Radius));
    }

    public bool Contains(Vector2 point) {
        Vector2 vector = point - this.Center;
        return (vector.magnitude <= (this.Radius * this.Radius));
    }

    public void Include(ref Circle2d circle) {
        Vector2 vector = circle.Center - this.Center;
        float num = vector.magnitude;
        float num2 = circle.Radius - this.Radius;
        float num3 = num2 * num2;
        if(num3 >= num) {
            if(num2 >= 0f) {
                this = circle;
            }
        }
        else {
            float num4 = Mathf.Sqrt(num);
            if(num4 > 1E-05f) {
                float num5 = (num4 + num2) / (2f * num4);
                this.Center += (Vector2)(num5 * vector);
            }
            this.Radius = 0.5f * ((num4 + this.Radius) + circle.Radius);
        }
    }

    public void Include(Circle2d circle) {
        this.Include(ref circle);
    }
}
