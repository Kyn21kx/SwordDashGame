using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Auxiliars {
	public static class SpartanMath {

		public const float TAU = 6.2831855f;

		public const float DEG_IN_CIRCLE = 360f;

		public static void Clamp(ref float n, float min, float max) {
			n = n > max ? max : n;
			n = n < min ? min : n;
		}

		public static float Clamp(float n, float min, float max) {
			n = n > max ? max : n;
			return n < min ? min : n;
		}
		
		public static float Sin(float value) => (float)System.Math.Sin(value);
		
		public static float RoundToSignificantFigures(float n, int digits) {
			if (n == 0) return 0;

			float scale = Mathf.Pow(10, Mathf.Floor(Mathf.Log10(Mathf.Abs(n))) + 1);
			return scale * MathF.Round(n / scale, digits);
		}

		public static Vector3 RemapUpToForward(in Vector3 toRemap, bool preserveZ = false) {
			float zValue = preserveZ ? toRemap.z : 0f;
			return new Vector3(toRemap.x, zValue, toRemap.y);
		}

		public static Vector3 RemapUpToForward(in Vector2 toRemap) => new Vector3(toRemap.x, 0f, toRemap.y);

		public static float Exp(float power) => (float)System.Math.Exp(power);

		public static float Ln(float value) => (float)System.Math.Log(value);

		public static float Log(float value, float n) => Ln(value) / Ln(n);

		public static float FastPow(float value, float power) => Exp(power * Ln(value));

		public static float SmoothStart(float from, float to, float t, float n) {
			float powerValue = FastPow(t, n);
			return Lerp(from, to, powerValue);
		}
		public static Vector3 SmoothStart(Vector3 from, Vector3 to, float t, float n) {
			float powerValue = FastPow(t, n);
			Clamp(ref powerValue, 0f, 1f);
			return new Vector3(
				LerpUnclamped(from.x, to.x, powerValue),
				LerpUnclamped(from.y, to.y, powerValue),
				LerpUnclamped(from.z, to.z, powerValue)
			);
		}

		public static float SmoothStop(float from, float to, float t, float n) {
			float powerValue = 1f - FastPow(1f - t, n);
			return Lerp(from, to, powerValue);
		}

		public static Vector3 SmoothStop(Vector3 from, Vector3 to, float t, float n) {
			float powerValue = 1f - FastPow(1f - t, n);
			Clamp(ref powerValue, 0f, 1f);
			return new Vector3(
				LerpUnclamped(from.x, to.x, powerValue),
				LerpUnclamped(from.y, to.y, powerValue),
				LerpUnclamped(from.z, to.z, powerValue)
			);
		}

		public static float Lerp(float from, float to, float t) {
			Clamp(ref t, 0f, 1f);
			return from + (to - from) * t;
		}


		public static Vector3 Lerp(Vector3 from, Vector3 to, float t) {
			Clamp(ref t, 0f, 1f);
			return new Vector3(
				LerpUnclamped(from.x, to.x, t),
				LerpUnclamped(from.y, to.y, t),
				LerpUnclamped(from.z, to.z, t)
			);
		}

		public static Color Lerp(Color from, Color to, float t) {
			Clamp(ref t, 0f, 1f);
			return new Color(
				LerpUnclamped(from.r, to.r, t),
				LerpUnclamped(from.g, to.g, t),
				LerpUnclamped(from.b, to.b, t),
				LerpUnclamped(from.a, to.a, t)
			);
		}

		public static float LerpUnclamped(float from, float to, float t) {
			return from + (to - from) * t;
		}

		public static Vector3 LerpUnclamped(Vector3 from, Vector3 to, float t) {
			return new Vector3(
				LerpUnclamped(from.x, to.x, t),
				LerpUnclamped(from.y, to.y, t),
				LerpUnclamped(from.z, to.z, t)
			);
		}

		/// <summary>
		/// Gets a direction vector in magnitude and angle and returns its cartesian representation
		/// </summary>
		public static Vector2 PolarToCartesian(float magnitude, float degrees) {
			float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
			float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
			return new Vector2(magnitude * cos, magnitude * sin);
		}

		/// <summary>
		/// Gets a direction vector in magnitude and angle and returns its cartesian representation
		/// </summary>
		public static Vector2 PolarToCartesianRadians(float magnitude, float radians) {
			float cos = Mathf.Cos(radians);
			float sin = Mathf.Sin(radians);
			return new Vector2(magnitude * cos, magnitude * sin);
		}

		public static Vector2 ReflectionVector(Vector2 incoming, Vector2 normal) {
			//To get a reflection vector we follow: 2(n . v)n + v
			//Where v: The incoming velocity / direction vector
			//And n: The normal surface vector of the collider we hit
			float dotV = Vector2.Dot(normal, incoming);
			Vector2 tmp = (-2f * dotV) * normal;
			return tmp + incoming;
		}

		public static float InverseLerp(float from, float to, float t) {
			Clamp(ref t, 0f, 1f);
			return (t - from) / to - from;
		}

		/// <summary>
		/// Exclusive Between operation
		/// </summary>
		/// <param name="value">Value to check</param>
		/// <param name="from">Exclusive starting point</param>
		/// <param name="to">Exclusive ending point</param>
		/// <returns>True if it's neither limit, but rather is between them</returns>
		public static bool BetweenEx(float value, float from, float to) {
			return value > from && value < to;
		}

		public static int Sign(float x) => x >= 0f ? 1 : -1;


		public static float DistanceSqr(Vector3 a, Vector3 b) {
			//So, let's substract, right?
			Vector3 towards = a - b;
			return towards.sqrMagnitude;
		}

		public static bool ArrivedAt(Vector3 from, Vector3 to, float threshold = 0.2f) {
			//Get the distance
			float sqrDis = DistanceSqr(from, to);
			if (sqrDis <= threshold * threshold) {
				return true;
			}
			return false;
		}

		public static bool ArrivedAt(float from, float to, float threshold = 0.2f) {
			//Get the distance
			float dis = Mathf.Abs(to - from);
			return dis <= threshold;
		}

		public static bool IsInLayerMask(int value, LayerMask mask) {
			return (mask & 1 << value) == 1 << value;
		}

		public static Quaternion LookTowardsDirection(Vector3 forward, Vector3 direction, float offsetAngle = 90f) {
			float alpha = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - offsetAngle;
			Quaternion result = Quaternion.AngleAxis(alpha, forward);
			return result;
		}

		public static float RandSign() {
			return Mathf.Sign(UnityEngine.Random.Range(-1, 0));
		}

		public static Vector2 Perpendicular(Vector2 inDirection) {
			return new Vector2(0f - inDirection.y, inDirection.x);
		}

		public static Vector2 VectorAlongSurface(Vector2 inDirection) {
			return Perpendicular(inDirection);
		}

		public static Vector2 RotateVector(in Vector2 v, float degrees)
		{
		    float radians = degrees * Mathf.Deg2Rad;
		    float sin = Mathf.Sin(radians);
		    float cos = Mathf.Cos(radians);
    
		    float tx = v.x;
		    float ty = v.y;

		    return new Vector2(
		        cos * tx - sin * ty,
		        sin * tx + cos * ty
		    );
		}
		// Normalize an angle between 0 and 360 degrees
		private static float NormalizeAngle(float angle) {
			angle %= DEG_IN_CIRCLE;
			if (angle < 0) angle += DEG_IN_CIRCLE;
			return angle;
		}

		public static float ClampCircleBack(float value, float min, float max) {
			if (value < min) return max;
			if (value > max) return min;
			return value;
		}

	}
}
