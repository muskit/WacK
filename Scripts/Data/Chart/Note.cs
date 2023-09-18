using System.Collections.Generic;

namespace WacK.Data.Chart
{
    /// <summary>
    /// Base class for various in-play note types.
    /// </summary>
    public abstract class Note
    {
        /// <summary>
        /// Time in milliseconds which the note occurs.
        /// </summary>
        public double time = 0;

        /// <summary>
        /// Time of the note in MeasureBeat.
        /// </summary>
        public MeasureBeat measureBeat;

        /// <summary>
        /// The note's radial position out of 60.
        /// </summary>
        public int? pos;

        /// <summary>
        /// The radial size of the note.
        /// 1 <= size <= 60
        /// </summary>
        public int? size;

        public Note(double time, MeasureBeat measureBeat, int? position = null, int? size = null)
        {
            this.time = time;
            this.measureBeat = measureBeat;
            this.pos = position;
            this.size = size;
        }

        public Note()
        {
        }
    }
}