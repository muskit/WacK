using System;
using System.Collections.Generic;
using System.Linq;

namespace WacK.Data.Chart
{
	/// <summary>
    /// A hold note. Notably contains data about hold points.
    /// </summary>
	public class NoteHold : NotePlay
	{
		public SortedList<float, NotePlay> points;

		public NoteHold(double time, MeasureBeat measureBeat, int position, int size, int holdIndex, int holdNext, bool bonus = false) 
			: base(time, measureBeat, position, size,holdIndex, holdNext, type: NotePlayType.HoldStart, bonus: false) 
		{
			// points = (SortedList<float, Note>)holdPoints.Skip(1);
		}
	}
}
