using System;

using UnityEngine;

/// <summary>
/// Represents an eased interpolation w/ respect to time.
/// float t, float b, float c, float d
/// </summary>
/// <param name="current">how long into the ease are we</param>
/// <param name="initialValue">starting value if current were 0</param>
/// <param name="totalChange">total change in the value (not the end value, but the end - start)</param>
/// <param name="duration">the total amount of time (when current == duration, the returned value will == initial + totalChange)</param>
public delegate float Ease(float current, float initialValue, float totalChange, float duration);

public enum EaseStyle : int {
	Linear,
	LinearEaseIn,
	LinearEaseOut,
	LinearEaseInOut,
	BackEaseIn,
	BackEaseOut,
	BackEaseInOut,
	BounceEaseIn,
	BounceEaseOut,
	BounceEaseInOut,
	CircleEaseIn,
	CircleEaseOut,
	CircleEaseInOut,
	CubicEaseIn,
	CubicEaseOut,
	CubicEaseInOut,
	ElasticEaseIn,
	ElasticEaseOut,
	ElasticEaseInOut,
	ExpoEaseIn,
	ExpoEaseOut,
	ExpoEaseInOut,
	QuadEaseIn,
	QuadEaseOut,
	QuadEaseInOut,
	QuartEaseIn,
	QuartEaseOut,
	QuartEaseInOut,
	QuintEaseIn,
	QuintEaseOut,
	QuintEaseInOut,
	SineEaseIn,
	SineEaseOut,
	SineEaseInOut,
	StrongEaseIn,
	StrongEaseOut,
	StrongEaseInOut
}

/// <summary>
/// A set of easing methods, to see a visual representation you can check out:
/// https://msdn.microsoft.com/en-us/library/vstudio/Ee308751%28v=VS.100%29.aspx
/// </summary>
public static class ConcreteEaseMethods {
	private const float _2PI = Mathf.PI * 2;
	private const float _HALF_PI = Mathf.PI / 2;

	#region Back Ease
	public static float BackEaseIn(float t, float b, float c, float d) => BackEaseInFull(t, b, c, d);
	public static float BackEaseOut(float t, float b, float c, float d) => BackEaseOutFull(t, b, c, d);
	public static float BackEaseInOut(float t, float b, float c, float d) => BackEaseInOutFull(t, b, c, d);

	public static float BackEaseInFull(float t, float b, float c, float d, float s = 1.70158f) => c * (t /= d) * t * ((s + 1) * t - s) + b;
	public static float BackEaseOutFull(float t, float b, float c, float d, float s = 1.70158f) => c * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1) + b;
	public static float BackEaseInOutFull(float t, float b, float c, float d, float s = 1.70158f) => ((t /= d / 2) < 1) ? (c / 2 * (t * t * (((s *= (1.525f)) + 1) * t - s)) + b) : (c / 2 * ((t -= 2) * t * (((s *= (1.525f)) + 1) * t + s) + 2) + b);
	#endregion

	#region Bounce Ease
	public static float BounceEaseOut(float t, float b, float c, float d) {
		if ((t /= d) < (1 / 2.75f)) {
			return c * (7.5625f * t * t) + b;
		} else if (t < (2 / 2.75)) {
			return c * (7.5625f * (t -= (1.5f / 2.75f)) * t + .75f) + b;
		} else if (t < (2.5f / 2.75f)) {
			return c * (7.5625f * (t -= (2.25f / 2.75f)) * t + .9375f) + b;
		} else {
			return c * (7.5625f * (t -= (2.625f / 2.75f)) * t + .984375f) + b;
		}
	}
	public static float BounceEaseIn(float t, float b, float c, float d) => c - BounceEaseOut(d - t, 0, c, d) + b;
	public static float BounceEaseInOut(float t, float b, float c, float d) => (t < d / 2) ? (BounceEaseIn(t * 2, 0, c, d) * .5f + b) : (BounceEaseOut(t * 2 - d, 0, c, d) * .5f + c * .5f + b);
	#endregion

	#region Circle Ease
	public static float CircleEaseIn(float t, float b, float c, float d) => -c * (Mathf.Sqrt(1 - (t /= d) * t) - 1) + b;
	public static float CircleEaseOut(float t, float b, float c, float d) => c * Mathf.Sqrt(1 - (t = t / d - 1) * t) + b;
	public static float CircleEaseInOut(float t, float b, float c, float d) => ((t /= d / 2) < 1) ? (-c / 2 * (Mathf.Sqrt(1 - t * t) - 1) + b) : (c / 2 * (Mathf.Sqrt(1 - (t -= 2) * t) + 1) + b);
	#endregion

	#region Cubic Ease
	public static float CubicEaseIn(float t, float b, float c, float d) => c * (t /= d) * t * t + b;
	public static float CubicEaseOut(float t, float b, float c, float d) => c * ((t = t / d - 1) * t * t + 1) + b;
	public static float CubicEaseInOut(float t, float b, float c, float d) => ((t /= d / 2) < 1) ? (c / 2 * t * t * t + b) : (c / 2 * ((t -= 2) * t * t + 2) + b);
	#endregion

	#region Elastic Ease
	public static float ElasticEaseIn(float t, float b, float c, float d) => ElasticEaseInFull(t, b, c, d, 0, 0);
	public static float ElasticEaseOut(float t, float b, float c, float d) => ElasticEaseOutFull(t, b, c, d, 0, 0);
	public static float ElasticEaseInOut(float t, float b, float c, float d) => ElasticEaseInOutFull(t, b, c, d, 0, 0);

	public static float ElasticEaseInFull(float t, float b, float c, float d, float a, float p) {
		float s;
		if (t == 0f) return b;
		if ((t /= d) == 1) return b + c;
		if (p == 0f) p = d * 0.3f;
		if (a == 0f || a < Math.Abs(c)) { a = c; s = p / 4; } else s = p / _2PI * Mathf.Asin(c / a);
		return -(a * Mathf.Pow(2, 10 * (t -= 1)) * Mathf.Sin((t * d - s) * _2PI / p)) + b;
	}
	public static float ElasticEaseOutFull(float t, float b, float c, float d, float a = 0, float p = 0) {
		float s;
		if (t == 0f) return b;
		if ((t /= d) == 1) return b + c;
		if (p == 0f) p = d * 0.3f;
		if (a == 0f || a < Math.Abs(c)) { a = c; s = p / 4; } else s = p / _2PI * Mathf.Asin(c / a);
		return (a * Mathf.Pow(2, -10 * t) * Mathf.Sin((t * d - s) * _2PI / p) + c + b);
	}
	public static float ElasticEaseInOutFull(float t, float b, float c, float d, float a = 0, float p = 0) {
		float s;
		if (t == 0f) return b;
		if ((t /= d / 2) == 2) return b + c;
		if (p == 0f) p = d * (0.3f * 1.5f);
		if (a == 0f || a < Math.Abs(c)) { a = c; s = p / 4; } else s = p / _2PI * Mathf.Asin(c / a);
		if (t < 1) return -.5f * (a * Mathf.Pow(2, 10 * (t -= 1)) * Mathf.Sin((t * d - s) * _2PI / p)) + b;
		return a * Mathf.Pow(2, -10 * (t -= 1)) * Mathf.Sin((t * d - s) * _2PI / p) * .5f + c + b;
	}
	#endregion

	#region Expo Ease
	public static float ExpoEaseIn(float t, float b, float c, float d) => (t == 0) ? b : c * Mathf.Pow(2, 10 * (t / d - 1)) + b - c * 0.001f;
	public static float ExpoEaseOut(float t, float b, float c, float d) => (t == d) ? b + c : c * (-Mathf.Pow(2, -10 * t / d) + 1) + b;
	public static float ExpoEaseInOut(float t, float b, float c, float d) {
		if (t == 0) return b;
		if (t == d) return b + c;
		if ((t /= d / 2) < 1) return c / 2 * Mathf.Pow(2, 10 * (t - 1)) + b;
		return c / 2 * (-Mathf.Pow(2, -10 * --t) + 2) + b;
	}
	#endregion

	#region Linear Ease
	public static float LinearEaseNone(float t, float b, float c, float d) => c * t / d + b;
	public static float LinearEaseIn(float t, float b, float c, float d) => c * t / d + b;
	public static float LinearEaseOut(float t, float b, float c, float d) => c * t / d + b;
	public static float LinearEaseInOut(float t, float b, float c, float d) => c * t / d + b;
	#endregion

	#region Quad Ease
	public static float QuadEaseIn(float t, float b, float c, float d) => c * (t /= d) * t + b;
	public static float QuadEaseOut(float t, float b, float c, float d) => -c * (t /= d) * (t - 2) + b;
	public static float QuadEaseInOut(float t, float b, float c, float d) => ((t /= d / 2) < 1) ? (c / 2 * t * t + b) : (-c / 2 * ((--t) * (t - 2) - 1) + b);
	#endregion

	#region Quart Ease
	public static float QuartEaseIn(float t, float b, float c, float d) => c * (t /= d) * t * t * t + b;
	public static float QuartEaseOut(float t, float b, float c, float d) => -c * ((t = t / d - 1) * t * t * t - 1) + b;
	public static float QuartEaseInOut(float t, float b, float c, float d) => ((t /= d / 2) < 1) ? (c / 2 * t * t * t * t + b) : (-c / 2 * ((t -= 2) * t * t * t - 2) + b);
	#endregion

	#region Quint Ease
	public static float QuintEaseIn(float t, float b, float c, float d) => c * (t /= d) * t * t * t * t + b;
	public static float QuintEaseOut(float t, float b, float c, float d) => c * ((t = t / d - 1) * t * t * t * t + 1) + b;
	public static float QuintEaseInOut(float t, float b, float c, float d) => ((t /= d / 2) < 1) ? (c / 2 * t * t * t * t * t + b) : (c / 2 * ((t -= 2) * t * t * t * t + 2) + b);
	#endregion

	#region Sine Ease
	public static float SineEaseIn(float t, float b, float c, float d) => -c * Mathf.Cos(t / d * _HALF_PI) + c + b;
	public static float SineEaseOut(float t, float b, float c, float d) => c * Mathf.Sin(t / d * _HALF_PI) + b;
	public static float SineEaseInOut(float t, float b, float c, float d) => -c / 2 * (Mathf.Cos(Mathf.PI * t / d) - 1) + b;
	#endregion

	#region Strong Ease
	public static float StrongEaseIn(float t, float b, float c, float d) => c * (t /= d) * t * t * t * t + b;
	public static float StrongEaseOut(float t, float b, float c, float d) => c * ((t = t / d - 1) * t * t * t * t + 1) + b;
	public static float StrongEaseInOut(float t, float b, float c, float d) => ((t /= d / 2) < 1) ? (c / 2 * t * t * t * t * t + b) : (c / 2 * ((t -= 2) * t * t * t * t + 2) + b);
	#endregion
}

/// <summary> References to Ease delegates for use, this avoids the garbage of the ease delegate. </summary>
public static class EaseMethods {
	public static Ease GetEase(EaseStyle style) {
		switch (style) {
			case EaseStyle.Linear:
				return EaseMethods.LinearEaseNone;
			case EaseStyle.LinearEaseIn:
				return EaseMethods.LinearEaseIn;
			case EaseStyle.LinearEaseOut:
				return EaseMethods.LinearEaseOut;
			case EaseStyle.LinearEaseInOut:
				return EaseMethods.LinearEaseInOut;

			case EaseStyle.BackEaseIn:
				return EaseMethods.BackEaseIn;
			case EaseStyle.BackEaseOut:
				return EaseMethods.BackEaseOut;
			case EaseStyle.BackEaseInOut:
				return EaseMethods.BackEaseInOut;

			case EaseStyle.BounceEaseIn:
				return EaseMethods.BounceEaseIn;
			case EaseStyle.BounceEaseOut:
				return EaseMethods.BounceEaseOut;
			case EaseStyle.BounceEaseInOut:
				return EaseMethods.BounceEaseInOut;

			case EaseStyle.CircleEaseIn:
				return EaseMethods.CircleEaseIn;
			case EaseStyle.CircleEaseOut:
				return EaseMethods.CircleEaseOut;
			case EaseStyle.CircleEaseInOut:
				return EaseMethods.CircleEaseInOut;

			case EaseStyle.CubicEaseIn:
				return EaseMethods.CubicEaseIn;
			case EaseStyle.CubicEaseOut:
				return EaseMethods.CubicEaseOut;
			case EaseStyle.CubicEaseInOut:
				return EaseMethods.CubicEaseInOut;

			case EaseStyle.ElasticEaseIn:
				return EaseMethods.ElasticEaseIn;
			case EaseStyle.ElasticEaseOut:
				return EaseMethods.ElasticEaseOut;
			case EaseStyle.ElasticEaseInOut:
				return EaseMethods.ElasticEaseInOut;

			case EaseStyle.ExpoEaseIn:
				return EaseMethods.ExpoEaseIn;
			case EaseStyle.ExpoEaseOut:
				return EaseMethods.ExpoEaseOut;
			case EaseStyle.ExpoEaseInOut:
				return EaseMethods.ExpoEaseInOut;

			case EaseStyle.QuadEaseIn:
				return EaseMethods.QuadEaseIn;
			case EaseStyle.QuadEaseOut:
				return EaseMethods.QuadEaseOut;
			case EaseStyle.QuadEaseInOut:
				return EaseMethods.QuadEaseInOut;

			case EaseStyle.QuartEaseIn:
				return EaseMethods.QuartEaseIn;
			case EaseStyle.QuartEaseOut:
				return EaseMethods.QuartEaseOut;
			case EaseStyle.QuartEaseInOut:
				return EaseMethods.QuartEaseInOut;

			case EaseStyle.QuintEaseIn:
				return EaseMethods.QuintEaseIn;
			case EaseStyle.QuintEaseOut:
				return EaseMethods.QuintEaseOut;
			case EaseStyle.QuintEaseInOut:
				return EaseMethods.QuintEaseInOut;

			case EaseStyle.SineEaseIn:
				return EaseMethods.SineEaseIn;
			case EaseStyle.SineEaseOut:
				return EaseMethods.SineEaseOut;
			case EaseStyle.SineEaseInOut:
				return EaseMethods.SineEaseInOut;

			case EaseStyle.StrongEaseIn:
				return EaseMethods.StrongEaseIn;
			case EaseStyle.StrongEaseOut:
				return EaseMethods.StrongEaseOut;
			case EaseStyle.StrongEaseInOut:
				return EaseMethods.StrongEaseInOut;
		}

		return null;
	}

	public static float EasedLerp(Ease ease, float from, float to, float t) => ease(t, from, to - from, 1f);

	#region Back Ease
	private static Ease _backEaseIn;
	public static Ease BackEaseIn { get => (_backEaseIn = _backEaseIn ?? ConcreteEaseMethods.BackEaseIn); }

	private static Ease _backEaseOut;
	public static Ease BackEaseOut { get => (_backEaseOut = _backEaseOut ?? ConcreteEaseMethods.BackEaseOut); }

	private static Ease _backEaseInOut;
	public static Ease BackEaseInOut { get => (_backEaseInOut = _backEaseInOut ?? ConcreteEaseMethods.BackEaseInOut); }
	#endregion

	#region Bounce Ease
	private static Ease _bounceEaseIn;
	public static Ease BounceEaseIn { get => (_bounceEaseIn = _bounceEaseIn ?? ConcreteEaseMethods.BounceEaseIn); }

	private static Ease _bounceEaseOut;
	public static Ease BounceEaseOut { get => (_bounceEaseOut = _bounceEaseOut ?? ConcreteEaseMethods.BounceEaseOut); }

	private static Ease _bounceEaseInOut;
	public static Ease BounceEaseInOut { get => (_bounceEaseInOut = _bounceEaseInOut ?? ConcreteEaseMethods.BounceEaseInOut); }
	#endregion

	#region Circle Ease
	private static Ease _circleEaseIn;
	public static Ease CircleEaseIn { get => (_circleEaseIn = _circleEaseIn ?? ConcreteEaseMethods.CircleEaseIn); }

	private static Ease _circleEaseOut;
	public static Ease CircleEaseOut { get => (_circleEaseOut = _circleEaseOut ?? ConcreteEaseMethods.CircleEaseOut); }

	private static Ease _circleEaseInOut;
	public static Ease CircleEaseInOut { get => (_circleEaseInOut = _circleEaseInOut ?? ConcreteEaseMethods.CircleEaseInOut); }
	#endregion

	#region Cubic Ease
	private static Ease _cubicEaseIn;
	public static Ease CubicEaseIn { get => (_cubicEaseIn = _cubicEaseIn ?? ConcreteEaseMethods.CubicEaseIn); }

	private static Ease _cubicEaseOut;
	public static Ease CubicEaseOut { get => (_cubicEaseOut = _cubicEaseOut ?? ConcreteEaseMethods.CubicEaseOut); }

	private static Ease _cubicEaseInOut;
	public static Ease CubicEaseInOut { get => (_cubicEaseInOut = _cubicEaseInOut ?? ConcreteEaseMethods.CubicEaseInOut); }
	#endregion

	#region Elastic Ease
	private static Ease _elasticEaseIn;
	public static Ease ElasticEaseIn { get => (_elasticEaseIn = _elasticEaseIn ?? ConcreteEaseMethods.ElasticEaseIn); }

	private static Ease _elasticEaseOut;
	public static Ease ElasticEaseOut { get => (_elasticEaseOut = _elasticEaseOut ?? ConcreteEaseMethods.ElasticEaseOut); }

	private static Ease _elasticEaseInOut;
	public static Ease ElasticEaseInOut { get => (_elasticEaseInOut = _elasticEaseInOut ?? ConcreteEaseMethods.ElasticEaseInOut); }
	#endregion

	#region Expo Ease
	private static Ease _expoEaseIn;
	public static Ease ExpoEaseIn { get => (_expoEaseIn = _expoEaseIn ?? ConcreteEaseMethods.ExpoEaseIn); }

	private static Ease _expoEaseOut;
	public static Ease ExpoEaseOut { get => (_expoEaseOut = _expoEaseOut ?? ConcreteEaseMethods.ExpoEaseOut); }

	private static Ease _expoEaseInOut;
	public static Ease ExpoEaseInOut { get => (_expoEaseInOut = _expoEaseInOut ?? ConcreteEaseMethods.ExpoEaseInOut); }
	#endregion

	#region Linear Ease
	private static Ease _linearEaseNone;
	public static Ease LinearEaseNone { get => (_linearEaseNone = _linearEaseNone ?? ConcreteEaseMethods.LinearEaseNone); }

	private static Ease _linearEaseIn;
	public static Ease LinearEaseIn { get => (_linearEaseIn = _linearEaseIn ?? ConcreteEaseMethods.LinearEaseIn); }

	private static Ease _linearEaseOut;
	public static Ease LinearEaseOut { get => (_linearEaseOut = _linearEaseOut ?? ConcreteEaseMethods.LinearEaseOut); }

	private static Ease _linearEaseInOut;
	public static Ease LinearEaseInOut { get => (_linearEaseInOut = _linearEaseInOut ?? ConcreteEaseMethods.LinearEaseInOut); }
	#endregion

	#region Quad Ease
	private static Ease _quadEaseIn;
	public static Ease QuadEaseIn { get => (_quadEaseIn = _quadEaseIn ?? ConcreteEaseMethods.QuadEaseIn); }

	private static Ease _quadEaseOut;
	public static Ease QuadEaseOut { get => (_quadEaseOut = _quadEaseOut ?? ConcreteEaseMethods.QuadEaseOut); }

	private static Ease _quadEaseInOut;
	public static Ease QuadEaseInOut { get => (_quadEaseInOut = _quadEaseInOut ?? ConcreteEaseMethods.QuadEaseInOut); }
	#endregion

	#region Quart Ease
	private static Ease _quartEaseIn;
	public static Ease QuartEaseIn { get => (_quartEaseIn = _quartEaseIn ?? ConcreteEaseMethods.QuartEaseIn); }

	private static Ease _quartEaseOut;
	public static Ease QuartEaseOut { get => (_quartEaseOut = _quartEaseOut ?? ConcreteEaseMethods.QuartEaseOut); }

	private static Ease _quartEaseInOut;
	public static Ease QuartEaseInOut { get => (_quartEaseInOut = _quartEaseInOut ?? ConcreteEaseMethods.QuartEaseInOut); }
	#endregion

	#region Quint Ease
	private static Ease _quintEaseIn;
	public static Ease QuintEaseIn { get => (_quintEaseIn = _quintEaseIn ?? ConcreteEaseMethods.QuintEaseIn); }

	private static Ease _quintEaseOut;
	public static Ease QuintEaseOut { get => (_quintEaseOut = _quintEaseOut ?? ConcreteEaseMethods.QuintEaseOut); }

	private static Ease _quintEaseInOut;
	public static Ease QuintEaseInOut { get => (_quintEaseInOut = _quintEaseInOut ?? ConcreteEaseMethods.QuintEaseInOut); }
	#endregion

	#region Sine Ease
	private static Ease _sineEaseIn;
	public static Ease SineEaseIn { get => (_sineEaseIn = _sineEaseIn ?? ConcreteEaseMethods.SineEaseIn); }

	private static Ease _sineEaseOut;
	public static Ease SineEaseOut { get => (_sineEaseOut = _sineEaseOut ?? ConcreteEaseMethods.SineEaseOut); }

	private static Ease _sineEaseInOut;
	public static Ease SineEaseInOut { get => (_sineEaseInOut = _sineEaseInOut ?? ConcreteEaseMethods.SineEaseInOut); }
	#endregion

	#region Strong Ease
	private static Ease _strongEaseIn;
	public static Ease StrongEaseIn { get => (_strongEaseIn = _strongEaseIn ?? ConcreteEaseMethods.StrongEaseIn); }

	private static Ease _strongEaseOut;
	public static Ease StrongEaseOut { get => (_strongEaseOut = _strongEaseOut ?? ConcreteEaseMethods.StrongEaseOut); }

	private static Ease _strongEaseInOut;
	public static Ease StrongEaseInOut { get => (_strongEaseInOut = _strongEaseInOut ?? ConcreteEaseMethods.StrongEaseInOut); }
	#endregion

	#region AnimationCurve
	/// <summary> Returns an Ease method that ignores start and end. Instead just returning the value in the curve for 'c', as if you called Evaluate(c). </summary>
	/// <param name="curve"></param>
	public static Ease FromAnimationCurve(AnimationCurve curve) => (c, s, e, d) => curve.Evaluate(c);

	/// <summary> This treats the curve as if it's a scaling factor. The vertical from 0->1 is the value s->e. And the horizontal is just 'c'. 'd' is ignored in favor of the duration of the curve. </summary>
	/// <param name="curve"></param>
	public static Ease FromVerticalScalingAnimationCurve(AnimationCurve curve) => (c, s, e, d) => (d <= 0f) ? e : Mathf.LerpUnclamped(s, e, curve.Evaluate(c / d));

	/// <summary> This treats the curve as if it's a scaling factor. The vertical from 0->1 is the value s->e. And the horizontal from 0->1 is the time from c->d. </summary>
	/// <param name="curve"></param>
	public static Ease FromScalingAnimationCurve(AnimationCurve curve) => (c, s, e, d) => (d <= 0f) ? e : Mathf.LerpUnclamped(s, e, curve.Evaluate(c / d));
	#endregion

	#region Configurable Cubic Bezier
	public static Ease CubicBezier(float p0, float p1, float p2, float p3) =>
		(c, s, e, d) => {
			var t = c / d;
			var it = 1f - t;
			var r = (Mathf.Pow(it, 3f) * p0) +
				(3f * Mathf.Pow(it, 2f) * t * p1) +
				(3f * it * Mathf.Pow(t, 2f) * p2) +
				(Mathf.Pow(t, 3f) * p3);
			return s + e * r;
		};

	#endregion

	public static Vector2 EaseVector2(Ease ease, Vector2 start, Vector2 end, float t, float dur) => (ease(t, 0, 1, dur) * (end - start)) + start;
	//return new Vector2(ease(t, start.x, end.x - start.x, dur), ease(t, start.y, end.y - start.y, dur));

	public static Vector3 EaseVector3(Ease ease, Vector3 start, Vector3 end, float t, float dur) => (ease(t, 0, 1, dur) * (end - start)) + start;
	//return new Vector3(ease(t, start.x, end.x - start.x, dur), ease(t, start.y, end.y - start.y, dur), ease(t, start.z, end.z - start.z, dur));

	public static Vector4 EaseVector4(Ease ease, Vector4 start, Vector4 end, float t, float dur) => (ease(t, 0, 1, dur) * (end - start)) + start;
	//return new Vector4(ease(t, start.x, end.x - start.x, dur), ease(t, start.y, end.y - start.y, dur), ease(t, start.z, end.z - start.z, dur), ease(t, start.w, end.w - start.w, dur));

	public static Quaternion EaseQuaternion(Ease ease, Quaternion start, Quaternion end, float t, float dur) => Quaternion.Slerp(start, end, ease(t, 0, 1, dur));
}
