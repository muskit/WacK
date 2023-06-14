using System;

using System.Collections.Generic;

namespace WacK.Data.Chart
{
	public enum NoteEventType
	{
		Tempo,
		TimeSignature,
		ScrollSpeedMultiplier,
		BGAdd,
		BGRem,
	}

	/// <summary>
    /// Represents an unplayable event with some associated data value.
    /// </summary>
    /// <typeparam name="T">The value's type.</typeparam>
	public class NoteEvent<T> : Note
	{
		public NoteEventType type;

		/// <summary>
		/// A value whose function will vary depending on the type of note.
		/// </summary>
		public T value;

		public NoteEvent(double time, MeasureBeat measureBeat, NoteEventType type, int? position = null, int? size = null, T value = default(T)) :
			base(time, measureBeat, position, size)
		{
			this.value = value;
			this.type = type;
		}
	}	
}
