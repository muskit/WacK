/**
 * Note.cs
 * A struct representing a note.
 *
 * by muskit
 * July 1, 2022
 **/

namespace WacK.Data.Mer
{
    public enum MerType
    {
        Touch, HoldStart, HoldMid, HoldEnd, Untimed, SwipeIn, SwipeOut, SwipeCW, SwipeCCW, Tempo, TimeSignature, BGAdd, BGRem
    }
    public struct MerNote
    {
        public MerType noteType { get; private set; }
        public bool isBonus { get; private set; }

        // Radial values in minutes
        public int position { get; private set; }
        public int size { get; private set; } // 1 <= size <= 60
        public string value { get; private set; }
        public int holdIdx { get; private set; }
        public int holdNextIdx { get; private set; }

        public MerNote(int position = 0, int size = 1, string value = "", int holdIndex = -1, int holdNext = -1, MerType type = MerType.Touch, bool bonus = false)
        {
            this.position = position;
            this.size = size;
            this.value = value;
            this.holdIdx = holdIndex;
            this.holdNextIdx = holdNext;
            this.noteType = type;
            this.isBonus = bonus;
        }
    }
}