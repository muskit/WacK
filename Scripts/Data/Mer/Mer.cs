/**
 * Chart.cs
 * Representation of a chart, constructed from a .mer file.
 *
 * by muskit
 * July 1, 2022
 **/

using System;
using System.Collections.Generic;
using Godot;

namespace WacK.Data.Mer
{
    /// <summary>
    /// Structurized representation of a .mer file.
    /// </summary>
    public class Mer
    {
        /// <summary>
        /// HIERARCHY:
        /// Key is measure.
        /// Value is List of (beat/1920, Notes) tuples.
        /// </summary>
        public SortedList<int, List<(int, MerNote)>> notes = new SortedList<int, List<(int, MerNote)>>();

        public int playableNoteCount { get; private set; }

        /// <summary>
        /// Construct Chart from contents of .mer file.
        /// </summary>
        /// <param name="str">Contents of a .mer file.</param>    
        public Mer(string str)
        {
            if (str == String.Empty) return;

            playableNoteCount = 0;

            List<string> lines = new List<string>(str.Split('\n'));
            foreach (var line in lines)
            {
                List<string> tokens = new List<string>(line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
                if (tokens.Count == 0) continue;
                if (tokens[0][0] == '#') continue;

                int currentMeasure = int.Parse(tokens[0]);
                int currentBeat = int.Parse(tokens[1]);

                if (!notes.ContainsKey(currentMeasure))
                {
                    notes[currentMeasure] = new List<(int, MerNote)>();
                }

                switch (tokens[2])
                {
                    case "1": // common note types
                        switch(tokens[3])
                        {
                            case "1": // touch
                                notes[currentMeasure].Add((currentBeat, new MerNote(int.Parse(tokens[5]), int.Parse(tokens[6]), type: MerType.Touch)));
                                ++playableNoteCount;
                                break;
                            case "2": // touch w/ bonus
                            case "20": // TODO: big effect
                                notes[currentMeasure].Add((currentBeat, new MerNote(int.Parse(tokens[5]), int.Parse(tokens[6]), type: MerType.Touch, bonus: true)));
                                ++playableNoteCount;
                                break;
                            case "16": // untimed
                                notes[currentMeasure].Add((currentBeat, new MerNote(int.Parse(tokens[5]), int.Parse(tokens[6]), type: MerType.Untimed)));
                                ++playableNoteCount;
                                break;
                            case "26": // untimed w/ bonus
                                notes[currentMeasure].Add((currentBeat, new MerNote(int.Parse(tokens[5]), int.Parse(tokens[6]), type: MerType.Untimed, bonus: true)));
                                ++playableNoteCount;
                                break;
                            case "3": // swipe in (red)
                                notes[currentMeasure].Add((currentBeat, new MerNote(int.Parse(tokens[5]), int.Parse(tokens[6]), type: MerType.SwipeIn)));
                                ++playableNoteCount;
                                break;
                            case "21": // swipe in w/ bonus
                                notes[currentMeasure].Add((currentBeat, new MerNote(int.Parse(tokens[5]), int.Parse(tokens[6]), type: MerType.SwipeIn, bonus: true)));
                                ++playableNoteCount;
                                break;
                            case "4": // swipe out (blue)
                                notes[currentMeasure].Add((currentBeat, new MerNote(int.Parse(tokens[5]), int.Parse(tokens[6]), type: MerType.SwipeOut)));
                                ++playableNoteCount;
                                break;
                            case "22": // swipe out w/ bonus
                                notes[currentMeasure].Add((currentBeat, new MerNote(int.Parse(tokens[5]), int.Parse(tokens[6]), type: MerType.SwipeOut, bonus: true)));
                                ++playableNoteCount;
                                break;
                            case "7": // swipe CCW
                                notes[currentMeasure].Add((currentBeat, new MerNote(int.Parse(tokens[5]), int.Parse(tokens[6]), type: MerType.SwipeCCW)));
                                ++playableNoteCount;
                                break;
                            case "8": // swipe CCW w/ bonus
                            case "24": // TODO: big effect
                                notes[currentMeasure].Add((currentBeat, new MerNote(int.Parse(tokens[5]), int.Parse(tokens[6]), type: MerType.SwipeCCW, bonus: true)));
                                ++playableNoteCount;
                                break;
                            case "5": // swipe CW
                                notes[currentMeasure].Add((currentBeat, new MerNote(int.Parse(tokens[5]), int.Parse(tokens[6]), type: MerType.SwipeCW)));
                                ++playableNoteCount;
                                break;
                            case "6": // swipe CW w/ bonus
                            case "23": // TODO: big effect
                                notes[currentMeasure].Add((currentBeat, new MerNote(int.Parse(tokens[5]), int.Parse(tokens[6]), type: MerType.SwipeCW, bonus: true)));
                                ++playableNoteCount;
                                break;
                            case "9": // hold start
                                notes[currentMeasure].Add((currentBeat, new MerNote(int.Parse(tokens[5]), int.Parse(tokens[6]), holdIndex: int.Parse(tokens[4]), holdNext: int.Parse(tokens[8]), type: MerType.HoldStart)));
                                break;
                            case "25": // hold start (w/ bonus)
                                notes[currentMeasure].Add((currentBeat, new MerNote(int.Parse(tokens[5]), int.Parse(tokens[6]), holdIndex: int.Parse(tokens[4]), holdNext: int.Parse(tokens[8]), type: MerType.HoldStart, bonus: true)));
                                ++playableNoteCount;
                                break;
                            case "10": // hold middle; value = should draw?
                                notes[currentMeasure].Add((currentBeat, new MerNote(int.Parse(tokens[5]), int.Parse(tokens[6]), value: tokens[7],holdIndex: int.Parse(tokens[4]), holdNext: int.Parse(tokens[8]), type: MerType.HoldMid)));
                                break;
                            case "11": // hold end
                                notes[currentMeasure].Add((currentBeat, new MerNote(int.Parse(tokens[5]), int.Parse(tokens[6]), holdIndex: int.Parse(tokens[4]), type: MerType.HoldEnd)));
                                break;
                            case "12": // BG add
                                notes[currentMeasure].Add((currentBeat, new MerNote(int.Parse(tokens[5]), int.Parse(tokens[6]), value: tokens[8], type: MerType.BGAdd)));
                                break;
                            case "13": // BG rem
                                notes[currentMeasure].Add((currentBeat, new MerNote(int.Parse(tokens[5]), int.Parse(tokens[6]), value: tokens[8], type: MerType.BGRem)));
                                break;
                            case "15": // TODO: "SAME_TIME" (BG instant anim?)
                                break;
                            case "14": // TODO: end of chart
                                break;
                        }
                        break;
                    case "2": // tempo
                        notes[currentMeasure].Add((currentBeat, new MerNote(value: tokens[3], type: MerType.Tempo)));
                        break;
                    case "3": // beats per measure
                        string de = tokens.Count >= 5 ? tokens[4] : "4"; // TODO: use previously-estbalished denominator?
                        notes[currentMeasure].Add((currentBeat, new MerNote(value: $"{tokens[3]} {de}", type: MerType.TimeSignature)));
                        break;
                }
            }
            foreach (var measure in notes)
            {
                measure.Value.Sort((x, y) => x.Item1.CompareTo(y.Item1));
            }
        }
    }
}
