using System;

namespace WacK.Data.Chart
{
	/// <summary>
	/// Represents a non-timed chart position in measure and beat as a fraction of the measure.
	/// </summary>
	public class MeasureBeat
	{
		public const int DENOMINATOR = 1920;
		
		public int measure { get; private set; }
		/// <summary>
		/// Beat in the measure, represented as beat/1920 of a measure.
		/// </summary>
		public int beat { get; private set; }

		public MeasureBeat(int measure, int beat)
		{
			this.measure = measure;
			this.beat = beat;
			Normalize();
		}

		public void Normalize()
		{
			// beats larger than denominator
			if (Math.Abs(beat) >= DENOMINATOR)
			{
				measure += beat / DENOMINATOR;
				beat %= DENOMINATOR;
			}
			// positive measure, negative beats
			if (beat < 0 && measure > 0)
			{
				measure--;
				beat += DENOMINATOR;
			}
		}

		public override string ToString()
		{
			return $"MeasureBeat({this.measure}, {this.beat}/{DENOMINATOR})";
		}

		public override int GetHashCode()
		{
			return measure*DENOMINATOR + beat;
		}

		public override bool Equals(object tgt)
		{
			if (tgt == null) return false;
			
			return this == (MeasureBeat)tgt;
		}

		/* STATIC CONSTANTS */
		public readonly static MeasureBeat ZERO = new MeasureBeat(0, 0);

		/* STATIC OPERATORS */
		public static MeasureBeat operator +(MeasureBeat a) => a;
		public static MeasureBeat operator -(MeasureBeat a)
			=> new MeasureBeat(-a.measure, -a.beat);
		
		public static MeasureBeat operator +(MeasureBeat a, MeasureBeat b)
		{
			var meas = a.measure + b.measure;
			var beat = a.beat + b.beat;
			return new MeasureBeat(meas, beat);
		}

		public static MeasureBeat operator -(MeasureBeat a, MeasureBeat b)
		{
			return a + -b;
		}

		public static bool operator ==(MeasureBeat a, MeasureBeat b)
		{
			return a.measure == b.measure && a.beat == b.beat;
		}

		public static bool operator !=(MeasureBeat a, MeasureBeat b)
		{
			return !(a == b);
		}

		public static bool operator <(MeasureBeat a, MeasureBeat b)
		{
			if (a.measure < b.measure) return true;

			if (a.measure == b.measure)
			{
				return a.beat < b.beat;
			}

			return false;
		}

		public static bool operator >(MeasureBeat a, MeasureBeat b)
		{
			if (a != b)
			{
				return !(a < b);
			}
			return false;
		}

		public static bool operator <=(MeasureBeat a, MeasureBeat b)
		{
			if (a == b) return true;
			return a < b;
		}

		public static bool operator >=(MeasureBeat a, MeasureBeat b)
		{
			if (a == b) return true;
			return a > b;
		}
	}
}