using UnityEngine;
using System.Collections;
namespace LockStep {
    public struct StepVector3 {
        public int x;
        public int y;
        public int z;
        public const int Mod = 1000;

        public const float FloatMod = 1000F;

        public const float ModFactor = 0.001F;

        private static StepVector3 _zero = new StepVector3(0,0,0);
        public static StepVector3 zero { get { return _zero; } }

        public StepVector3(Vector3 position) {
            x = (int)System.Math.Round(position.x * FloatMod);
            y = (int)System.Math.Round(position.y * FloatMod);
            z = (int)System.Math.Round(position.z * FloatMod);
        }

        public StepVector3(int _x,int _y,int _z) {
            x = _x;
            y = _y;
            z = _z;
        }
        public Vector3 Vector3 {
            get {
                return new Vector3(x * ModFactor,y * ModFactor,z * ModFactor);
            }
        }

        public static bool operator ==(StepVector3 lhs,StepVector3 rhs) {
            return lhs.x == rhs.x &&
                   lhs.y == rhs.y &&
                   lhs.z == rhs.z;
        }

        public static bool operator !=(StepVector3 lhs,StepVector3 rhs) {
            return lhs.x != rhs.x ||
                   lhs.y != rhs.y ||
                   lhs.z != rhs.z;
        }

        public static explicit operator StepVector3(Vector3 ob) {
            return new StepVector3(
                (int)System.Math.Round(ob.x * FloatMod),
                (int)System.Math.Round(ob.y * FloatMod),
                (int)System.Math.Round(ob.z * FloatMod)
                );
        }

        public static explicit operator Vector3(StepVector3 ob) {
            return new Vector3(ob.x * ModFactor,ob.y * ModFactor,ob.z * ModFactor);
        }

        public static StepVector3 operator -(StepVector3 lhs,StepVector3 rhs) {
            lhs.x -= rhs.x;
            lhs.y -= rhs.y;
            lhs.z -= rhs.z;
            return lhs;
        }

        public static StepVector3 operator -(StepVector3 lhs) {
            lhs.x = -lhs.x;
            lhs.y = -lhs.y;
            lhs.z = -lhs.z;
            return lhs;
        }

        // REMOVE!
        public static StepVector3 operator +(StepVector3 lhs,StepVector3 rhs) {
            lhs.x += rhs.x;
            lhs.y += rhs.y;
            lhs.z += rhs.z;
            return lhs;
        }

        public static StepVector3 operator *(StepVector3 lhs,int rhs) {
            lhs.x *= rhs;
            lhs.y *= rhs;
            lhs.z *= rhs;

            return lhs;
        }

        public static StepVector3 operator *(StepVector3 lhs,float rhs) {
            lhs.x = (int)System.Math.Round(lhs.x * rhs);
            lhs.y = (int)System.Math.Round(lhs.y * rhs);
            lhs.z = (int)System.Math.Round(lhs.z * rhs);

            return lhs;
        }

        public static StepVector3 operator *(StepVector3 lhs,double rhs) {
            lhs.x = (int)System.Math.Round(lhs.x * rhs);
            lhs.y = (int)System.Math.Round(lhs.y * rhs);
            lhs.z = (int)System.Math.Round(lhs.z * rhs);

            return lhs;
        }

        public static StepVector3 operator *(StepVector3 lhs,Vector3 rhs) {
            lhs.x = (int)System.Math.Round(lhs.x * rhs.x);
            lhs.y = (int)System.Math.Round(lhs.y * rhs.y);
            lhs.z = (int)System.Math.Round(lhs.z * rhs.z);

            return lhs;
        }

        public static StepVector3 operator /(StepVector3 lhs,float rhs) {
            lhs.x = (int)System.Math.Round(lhs.x / rhs);
            lhs.y = (int)System.Math.Round(lhs.y / rhs);
            lhs.z = (int)System.Math.Round(lhs.z / rhs);
            return lhs;
        }

        public int this[int i] {
            get {
                return i == 0 ? x : (i == 1 ? y : z);
            }
            set {
                if(i == 0) x = value;
                else if(i == 1) y = value;
                else z = value;
            }
        }

        /** Angle between the vectors in radians */
        public static float Angle(StepVector3 lhs,StepVector3 rhs) {
            double cos = Dot(lhs,rhs) / ((double)lhs.magnitude * (double)rhs.magnitude);

            cos = cos < -1 ? -1 : (cos > 1 ? 1 : cos);
            return (float)System.Math.Acos(cos);
        }

        public static int Dot(StepVector3 lhs,StepVector3 rhs) {
            return
                lhs.x * rhs.x +
                lhs.y * rhs.y +
                lhs.z * rhs.z;
        }

        public static long DotLong(StepVector3 lhs,StepVector3 rhs) {
            return
                (long)lhs.x * (long)rhs.x +
                (long)lhs.y * (long)rhs.y +
                (long)lhs.z * (long)rhs.z;
        }

        /** Normal in 2D space (XZ).
         * Equivalent to Cross(this, Int3(0,1,0) )
         * except that the Y coordinate is left unchanged with this operation.
         */
        public StepVector3 Normal2D() {
            return new StepVector3(z,y,-x);
        }
        public static float Distance(StepVector3 a,StepVector3 b) {
            return (a - b).magnitude;
        }
        public float magnitude {
            get {
                //It turns out that using doubles is just as fast as using ints with Mathf.Sqrt. And this can also handle larger numbers (possibly with small errors when using huge numbers)!

                double _x = x;
                double _y = y;
                double _z = z;

                return (float)System.Math.Sqrt(_x * _x + _y * _y + _z * _z);
            }
        }
        public int costMagnitude {
            get {
                return (int)System.Math.Round(magnitude);
            }
        }
        [System.Obsolete("This property is deprecated. Use magnitude or cast to a Vector3")]
        public float worldMagnitude {
            get {
                double _x = x;
                double _y = y;
                double _z = z;

                return (float)System.Math.Sqrt(_x * _x + _y * _y + _z * _z) * ModFactor;
            }
        }

        /** The squared magnitude of the vector */
        public float sqrMagnitude {
            get {
                double _x = x;
                double _y = y;
                double _z = z;
                return (float)(_x * _x + _y * _y + _z * _z);
            }
        }

        /** The squared magnitude of the vector */
        public long sqrMagnitudeLong {
            get {
                long _x = x;
                long _y = y;
                long _z = z;
                return (_x * _x + _y * _y + _z * _z);
            }
        }

        public static implicit operator string(StepVector3 ob) {
            return ob.ToString();
        }

        /** Returns a nicely formatted string representing the vector */
        public override string ToString() {
            return "( " + x + ", " + y + ", " + z + ")";
        }

        public override bool Equals(System.Object o) {
            if(o == null) return false;

            var rhs = (StepVector3)o;

            return x == rhs.x &&
                   y == rhs.y &&
                   z == rhs.z;
        }

        public override int GetHashCode() {
            return x * 73856093 ^ y * 19349663 ^ z * 83492791;
        }
    }

    /** Two Dimensional Integer Coordinate Pair */
    public struct StepVector2 {
        public int x;
        public int y;

        public StepVector2(int x,int y) {
            this.x = x;
            this.y = y;
        }
        public Vector2 Vector2 {
            get {
                return new Vector2(x * StepVector3.ModFactor,y * StepVector3.ModFactor);
            }
        }
        public long sqrMagnitudeLong {
            get {
                return (long)x * (long)x + (long)y * (long)y;
            }
        }

        public static StepVector2 operator +(StepVector2 a,StepVector2 b) {
            return new StepVector2(a.x + b.x,a.y + b.y);
        }

        public static StepVector2 operator -(StepVector2 a,StepVector2 b) {
            return new StepVector2(a.x - b.x,a.y - b.y);
        }

        public static bool operator ==(StepVector2 a,StepVector2 b) {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(StepVector2 a,StepVector2 b) {
            return a.x != b.x || a.y != b.y;
        }

        /** Dot product of the two coordinates */
        public static long DotLong(StepVector2 a,StepVector2 b) {
            return (long)a.x * (long)b.x + (long)a.y * (long)b.y;
        }

        public override bool Equals(System.Object o) {
            if(o == null) return false;
            var rhs = (StepVector2)o;

            return x == rhs.x && y == rhs.y;
        }

        public override int GetHashCode() {
            return x * 49157 + y * 98317;
        }

        private static readonly int[] Rotations = {
			1, 0,  //Identity matrix
			0, 1,

			0, 1,
			-1, 0,

			-1, 0,
			0, -1,

			0, -1,
			1, 0
		};

        [System.Obsolete("Deprecated becuase it is not used by any part of the A* Pathfinding Project")]
        public static StepVector2 Rotate(StepVector2 v,int r) {
            r = r % 4;
            return new StepVector2(v.x * Rotations[r * 4 + 0] + v.y * Rotations[r * 4 + 1],v.x * Rotations[r * 4 + 2] + v.y * Rotations[r * 4 + 3]);
        }

        public static StepVector2 Min(StepVector2 a,StepVector2 b) {
            return new StepVector2(System.Math.Min(a.x,b.x),System.Math.Min(a.y,b.y));
        }

        public static StepVector2 Max(StepVector2 a,StepVector2 b) {
            return new StepVector2(System.Math.Max(a.x,b.x),System.Math.Max(a.y,b.y));
        }

        public static StepVector2 FromInt3XZ(StepVector3 o) {
            return new StepVector2(o.x,o.z);
        }

        public static StepVector3 ToInt3XZ(StepVector2 o) {
            return new StepVector3(o.x,0,o.y);
        }

        public override string ToString() {
            return "(" + x + ", " + y + ")";
        }
    }
}
