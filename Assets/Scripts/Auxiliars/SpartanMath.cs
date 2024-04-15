using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Auxiliars {
	public static class SpartanMath {

		public static void Clamp(ref float n, float min, float max)
		{
			n = n > max ? max : n;
			n = n < min ? min : n;
		}

		public static float Clamp(float n, float min, float max)
		{
			n = n > max ? max : n;
			return n < min ? min : n;
		}

		public static float RoundToSignificantFigures(float n, int digits)
		{
			if (n == 0) return 0;

			float scale = Mathf.Pow(10, Mathf.Floor(Mathf.Log10(Mathf.Abs(n))) + 1);
			return scale * MathF.Round(n / scale, digits);
		}

		public static Vector3 RemapUpToForward(in Vector3 toRemap, bool preserveZ = false)
		{
			float zValue = preserveZ ? toRemap.z : 0f;
			return new Vector3(toRemap.x, zValue, toRemap.y);
		}

		public static Vector3 RemapUpToForward(in Vector2 toRemap) => new Vector3(toRemap.x, 0f, toRemap.y);

        public static float Sin(float value) => (float)System.Math.Sin(value);
		public static float Exp(float power) => (float)System.Math.Exp(power);

		public static float Ln(float value) => (float)System.Math.Log(value);

		public static float Log(float value, float n) => Ln(value) / Ln(n);

		public static float FastPow(float value, float power) => Exp(power * Ln(value));

		public static float SmoothStart(float from, float to, float t, float n)
		{
			float powerValue = FastPow(t, n);
			return Lerp(from, to, powerValue);
		}
		public static Vector3 SmoothStart(Vector3 from, Vector3 to, float t, float n)
		{
			float powerValue = FastPow(t, n);
			Clamp(ref powerValue, 0f, 1f);
			return new Vector3(
				LerpUnclamped(from.x, to.x, powerValue),
				LerpUnclamped(from.y, to.y, powerValue),
				LerpUnclamped(from.z, to.z, powerValue)
			);
		}

		public static float SmoothStop(float from, float to, float t, float n)
		{
			float powerValue = 1f - FastPow(1f - t, n);
			return Lerp(from, to, powerValue);
		}

		public static Vector3 SmoothStop(Vector3 from, Vector3 to, float t, float n)
		{
			float powerValue = 1f - FastPow(1f - t, n);
			Clamp(ref powerValue, 0f, 1f);
			return new Vector3(
				LerpUnclamped(from.x, to.x, powerValue),
				LerpUnclamped(from.y, to.y, powerValue),
				LerpUnclamped(from.z, to.z, powerValue)
			);
		}

		public static float Lerp(float from, float to, float t)
		{
			Clamp(ref t, 0f, 1f);
			return from + (to - from) * t;
		}


		public static Vector3 Lerp(Vector3 from, Vector3 to, float t)
		{
			Clamp(ref t, 0f, 1f);
			return new Vector3(
				LerpUnclamped(from.x, to.x, t),
				LerpUnclamped(from.y, to.y, t),
				LerpUnclamped(from.z, to.z, t)
			);
		}

		public static float LerpUnclamped(float from, float to, float t)
		{
			return from + (to - from) * t;
		}

		public static Vector3 LerpUnclamped(Vector3 from, Vector3 to, float t)
		{
			return new Vector3(
				LerpUnclamped(from.x, to.x, t),
				LerpUnclamped(from.y, to.y, t),
				LerpUnclamped(from.z, to.z, t)
			);
		}

		public static float InverseLerp(float from, float to, float t)
		{
			Clamp(ref t, 0f, 1f);
			return (t - from) / to - from;
		}

		public static int Sign(float x) => x >= 0f ? 1 : -1;


		public static float DistanceSqr(Vector3 a, Vector3 b)
		{
			//So, let's substract, right?
			Vector3 towards = a - b;
			return towards.sqrMagnitude;
		}

		public static bool ArrivedAt(Vector3 from, Vector3 to, float threshold = 0.2f)
		{
			//Get the distance
			float sqrDis = DistanceSqr(from, to);
			if (sqrDis <= threshold * threshold)
			{
				return true;
			}
			return false;
		}

		public static bool ArrivedAt(float from, float to, float threshold = 0.2f)
		{
			//Get the distance
			float dis = Mathf.Abs(to - from);
			return dis <= threshold;
		}

		public static bool IsInLayerMask(int value, LayerMask mask)
		{
			return (mask & 1 << value) == 1 << value;
		}

        /// <summary>
        /// Code coming from https://stackoverflow.com/a/8659483/9854946
        /// </summary>
		public static class NoiseGenerator
		{
            private static int[] p = new int[512];
            private static int[] permutation = { 151,160,137,91,90,15,
               131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
               190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
               88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
               77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
               102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
               135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
               5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
               223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
               129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
               251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
               49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
               138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
               };

            private static int _octaves;
            private static int _halfLength = 256;
            public static void SetOctaves(int octaves)
            {
                _octaves = octaves;

                var len = (int)Math.Pow(2, octaves);

                permutation = new int[len];

                Reseed();
            }

            private static void CalculateP()
            {
                p = new int[permutation.Length * 2];
                _halfLength = permutation.Length;

                for (int i = 0; i < permutation.Length; i++)
                    p[permutation.Length + i] = p[i] = permutation[i];
            }

            public static void Reseed()
            {
                var random = new System.Random();
                var perm = Enumerable.Range(0, permutation.Length).ToArray();

                for (var i = 0; i < perm.Length; i++)
                {
                    var swapIndex = random.Next(perm.Length);

                    var t = perm[i];

                    perm[i] = perm[swapIndex];

                    perm[swapIndex] = t;
                }

                permutation = perm;

                CalculateP();

            }

            public static float Noise(Vector3 position, int octaves, ref float min, ref float max)
            {
                return Noise(position.x, position.y, position.z, octaves, ref min, ref max);
            }

            public static float Noise(float x, float y, float z, int octaves, ref float min, ref float max)
            {

                var perlin = 0f;
                var octave = 1;

                for (var i = 0; i < octaves; i++)
                {
                    var noise = Noise(x * octave, y * octave, z * octave);

                    perlin += noise / octave;

                    octave *= 2;
                }

                perlin = Math.Abs((float)Math.Pow(perlin, 2));
                max = Math.Max(perlin, max);
                min = Math.Min(perlin, min);

                //perlin = 1f - 2 * perlin;

                return perlin;
            }

            public static float Noise(float x, float y, float z)
            {
                int X = (int)Math.Floor(x) % _halfLength;
                int Y = (int)Math.Floor(y) % _halfLength;
                int Z = (int)Math.Floor(z) % _halfLength;

                if (X < 0)
                    X += _halfLength;

                if (Y < 0)
                    Y += _halfLength;

                if (Z < 0)
                    Z += _halfLength;

                x -= (int)Math.Floor(x);
                y -= (int)Math.Floor(y);
                z -= (int)Math.Floor(z);

                var u = Fade(x);
                var v = Fade(y);
                var w = Fade(z);

                int A = p[X] + Y, AA = p[A] + Z, AB = p[A + 1] + Z,      // HASH COORDINATES OF
                    B = p[X + 1] + Y, BA = p[B] + Z, BB = p[B + 1] + Z;      // THE 8 CUBE CORNERS,


                return SpartanMath.Lerp(
                        SpartanMath.Lerp(
                             SpartanMath.Lerp(
                                Grad(p[AA], x, y, z) // AND ADD
                                ,
                                Grad(p[BA], x - 1, y, z) // BLENDED
                                ,
                                u
                                )
                            ,
                            SpartanMath.Lerp(
                                Grad(p[AB], x, y - 1, z)  // RESULTS
                                ,
                                Grad(p[BB], x - 1, y - 1, z)
                                ,
                                u
                                )
                            ,
                            v
                        )
                        ,
                        SpartanMath.Lerp(
                            SpartanMath.Lerp(
                                Grad(p[AA + 1], x, y, z - 1) // CORNERS
                                ,
                                Grad(p[BA + 1], x - 1, y, z - 1) // OF CUBE
                                ,
                                u
                                )
                            ,
                            SpartanMath.Lerp(
                                Grad(p[AB + 1], x, y - 1, z - 1)
                                ,
                                Grad(p[BB + 1], x - 1, y - 1, z - 1)
                                ,
                                u
                                )
                            ,
                            v
                        )
                        ,
                        w
                    );

            }

            static float Fade(float t) { return t * t * t * (t * (t * 6 - 15) + 10); }

            static float Grad(int hash, float x, float y, float z)
            {
                int h = hash & 15;                      // CONVERT LO 4 BITS OF HASH CODE

                float u = h < 8 ? x : y,                 // INTO 12 GRADIENT DIRECTIONS.
                       v = h < 4 ? y : h == 12 || h == 14 ? x : z;

                return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
            }
        }

	}
}