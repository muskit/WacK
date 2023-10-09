using System.Collections.Generic;

namespace WacK.Data.Chart
{
	public enum NotePlayType
	{
		Touch,
		HoldStart,
		HoldMid,
		HoldMidInvis,
		HoldEnd,
		Chain,
		SnapIn,
		SnapOut,
		SwipeCW,
		SwipeCCW,
	}

	/// <summary>
    /// Represents playable notes.
    /// </summary>
	public class NotePlay : Note
	{
		public NotePlayType type { get; private set; }
		public bool isBonus { get; private set; }
		public int holdIdx { get; private set; }
		public int holdNextIdx { get; private set; }

		public NotePlay(double time, MeasureBeat measureBeat, int position, int size, int holdIndex = -1, int holdNext = -1,
			NotePlayType type = NotePlayType.Touch, bool bonus = false) 
			: base(time, measureBeat, position, size)
		{
			this.type = type;
			this.isBonus = bonus;
			this.holdIdx = holdIndex;
			this.holdNextIdx = holdNext;
		}
	}
}