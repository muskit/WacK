using System;
using System.Linq;
using System.Collections.Generic;

using WacK.Data.Mer;

using Godot;

namespace WacK.Data.Chart
{
    /// <summary>
    /// Chart data.
    /// </summary>
    public class Chart
    {
        // Key of dictionaries are in seconds
        // List is for chords
        public SortedList<float, List<NotePlay>> playNotes { get; private set; }
        public SortedList<float, NoteEvent<(int, int)>> timeSigChgs { get; private set; }
        public SortedList<float, NoteEvent<float>> tempoChgs { get; private set; }
        public SortedList<float, NoteEvent<int>> events { get; private set; }

        public Chart(string chartPath)
        {
			var file = FileAccess.Open(chartPath, FileAccess.ModeFlags.Read);
            if (file == null)
            {
                GD.PrintErr("Couldn't load chart in play!");
                return;
            }
			var str = file.GetAsText();
            file.Close();

            var mer = new Mer.Mer(str);
            Load(mer);
        }

        // place notes and events relative to the previous
        private void Load(Mer.Mer chart)
        {
            playNotes = new();
            timeSigChgs = new();
            tempoChgs = new();
            events = new();

            List<float> tempo = new List<float>();
            List<int> tempoChangeMeasures = new List<int>();
            List<int> tempoChangeBeats = new List<int>();
            
            // TODO: switch to MeasureBeat
            List<int> beatsPerMeasure = new List<int>();
            List<int> bpmChangeMeasures = new List<int>();

            tempo.Add(0);
            tempoChangeMeasures.Add(0);
            tempoChangeBeats.Add(0);

            beatsPerMeasure.Add(0);
            bpmChangeMeasures.Add(0);

            float queuedTempo = -1;
            int queuedBPM = -1;

            // timing info of the previous beat
            float prevTime = 0;
            int prevMeasure = 0;
            int prevBeat = 0; // (/1920 beats per measure)

            Note prevNote = null;
            Note curNote = null;
            var prevHoldPoint = new Dictionary<int, NotePlay>(); // <note idx, previous point of hold>
            var curHoldNote = new Dictionary<int, NoteHold>(); // <next hold idx, HoldStart>

            // Notes and Events //
            foreach (var measure in chart.notes) // `measure` = measure: List
            {
                foreach (var chartNote in measure.Value) // `chartNote` = beat, ChartNote
                {
                    var curTime = prevTime + Util.NoteTime(measure.Key - prevMeasure, chartNote.Item1 - prevBeat, tempo.Last<float>(), beatsPerMeasure.Last<int>());
					var mb = new MeasureBeat(measure.Key, chartNote.Item1);

					if (prevMeasure != measure.Key && prevBeat != chartNote.Item1)
                    {
                        if (queuedTempo != -1)
                        {
                            tempo.Add(queuedTempo);
                            tempoChangeMeasures.Add(measure.Key);
							tempoChangeBeats.Add(chartNote.Item1);
							queuedTempo = -1;
                        }

                        if (queuedBPM != -1)
                        {
                            beatsPerMeasure.Add(queuedBPM);
                            bpmChangeMeasures.Add(measure.Key);
                            queuedBPM = -1;
                        }
                    }

                    // notetype-dependent operations
                    switch (chartNote.Item2.noteType)
                    {
                        // Beat map data
                        case MerType.Tempo:
                            if (tempo.Count == 1)
                            {
                                tempo.Add(float.Parse(chartNote.Item2.value));
                                tempoChangeMeasures.Add(measure.Key);
                                tempoChangeBeats.Add(chartNote.Item1);
                            }
                            else
                                queuedTempo = float.Parse(chartNote.Item2.value);
							this.tempoChgs.Add(
                                curTime,
                                new NoteEvent<float> (
                                    curTime, mb,
                                    NoteEventType.Tempo,
                                    value: float.Parse(chartNote.Item2.value)
                                ));
							break;
                        case MerType.TimeSignature:
							var words = chartNote.Item2.value.Split();
							var nu = int.Parse(words[0]);
							var de = int.Parse(words[1]);
							if (beatsPerMeasure.Count == 1)
                            {
                                // TODO: handle denominator (note that gets the beat)
                                beatsPerMeasure.Add(nu);
                                bpmChangeMeasures.Add(measure.Key);
                            }
                            else
                                // TODO: handle denominator (note that gets the beat)
                                queuedBPM = int.Parse(chartNote.Item2.value.Split()[0]);
                            timeSigChgs.Add(
                                curTime,
                                new NoteEvent<(int, int)> (
                                    curTime, mb,
                                    NoteEventType.TimeSignature,
                                    value: (nu, de)
                                ));
                            break;
                        // Playable notes
                        case MerType.Touch:
                            curNote = new NotePlay(
                                curTime, mb,
                                chartNote.Item2.position, chartNote.Item2.size
                            );
                            break;
                        case MerType.HoldStart:
                            curNote = new NoteHold(
                                curTime, mb,
                                chartNote.Item2.position, chartNote.Item2.size,
                                holdIndex: chartNote.Item2.holdIdx,
                                holdNext: chartNote.Item2.holdNextIdx
                            );
							var nh = curNote as NoteHold;
							prevHoldPoint[chartNote.Item2.holdNextIdx] = (NotePlay) curNote;
                            curHoldNote[chartNote.Item2.holdNextIdx] = nh;
                            break;
                        case MerType.HoldMid:
                            curNote = new NotePlay(
                                curTime, mb,
                                chartNote.Item2.position, chartNote.Item2.size,
                                type: NotePlayType.HoldMid,
                                holdIndex: chartNote.Item2.holdIdx,
                                holdNext: chartNote.Item2.holdNextIdx
                            );
                            prevHoldPoint[chartNote.Item2.holdNextIdx] = (NotePlay) curNote;
                            curHoldNote[chartNote.Item2.holdNextIdx] = curHoldNote[chartNote.Item2.holdIdx];
                            break;
                        case MerType.HoldEnd: // TODO: draw end note on cone texture
                            curNote = new NotePlay(
                                curTime, mb,
                                chartNote.Item2.position, chartNote.Item2.size,
                                type: NotePlayType.HoldEnd,
                                holdIndex: chartNote.Item2.holdIdx
                            );
                            break;
                        case MerType.Untimed:
                            curNote = new NotePlay(
                                curTime, mb,
                                chartNote.Item2.position, chartNote.Item2.size,
                                type: NotePlayType.Untimed
                            );
                            break;
                        case MerType.SwipeIn:
                            curNote = new NotePlay(
                                curTime, mb,
                                chartNote.Item2.position, chartNote.Item2.size,
                                type: NotePlayType.SwipeIn
                            );
                            break;
                        case MerType.SwipeOut:
                            curNote = new NotePlay(
                                curTime, mb,
                                chartNote.Item2.position, chartNote.Item2.size,
                                type: NotePlayType.SwipeOut
                            );
                            break;
                        case MerType.SwipeCW:
                            curNote = new NotePlay(
                                curTime, mb,
                                chartNote.Item2.position, chartNote.Item2.size,
                                type: NotePlayType.SwipeCW
                            );
                            break;
                        case MerType.SwipeCCW:
                            curNote = new NotePlay(
                                curTime, mb,
                                chartNote.Item2.position, chartNote.Item2.size,
                                type: NotePlayType.SwipeCCW
                            );
                            break;
                        // Events (invisible modifier notes)
                        case MerType.BGAdd:
                            curNote = new NoteEvent<int>(
                                curTime, mb,
                                NoteEventType.BGAdd,
                                chartNote.Item2.position, chartNote.Item2.size,
                                value: int.Parse(chartNote.Item2.value)
                            );
                            break;
                        case MerType.BGRem:
                            curNote = new NoteEvent<int>(
                                curTime, mb,
                                NoteEventType.BGRem,
                                chartNote.Item2.position, chartNote.Item2.size,
                                value: int.Parse(chartNote.Item2.value)
                            );
                            break;
                    }

                    if (curNote == null || curNote == prevNote) continue;

                    /* Handle our crafted curNote, storing it somewhere in this Chart */
					// NotePlay
					var np = curNote as NotePlay;
                    if (np != null)
                    {
                        // hold point handling
                        if (np.type == NotePlayType.HoldMid || np.type == NotePlayType.HoldEnd)
                        {
							curHoldNote[np.holdIdx].points[curTime] = np;
						}
                        else
                        {
                            // only add notes that aren't part of the hold
                            if (!playNotes.ContainsKey(curTime))
                            {
                                playNotes[curTime] = new List<NotePlay>();
                            }
                            playNotes[curTime].Add(np);
                        }
                    }

					// NoteEvent<float> -- tempo changes
					var nef = curNote as NoteEvent<float>;
                    if (nef != null)
                    {
                        if (nef.type == NoteEventType.Tempo)
                            this.tempoChgs[curTime] = nef;
                        else
                            GD.Print($"Didn't add NoteEvent<float> of type {nef.type}");
					}

					// NoteEvent<(int, int)> -- time signature changes
					var neii = curNote as NoteEvent<(int, int)>;
                    if (neii != null)
                    {
						this.timeSigChgs[curTime] = neii;
					}

                    // NoteEvent<int>
					var nei = curNote as NoteEvent<int>;
                    if (nei != null)
                    {
						this.events[curTime] = nei;
					}

					// update previous states
					prevNote = curNote;
                    prevTime = curTime;
                    prevBeat = chartNote.Item1;
                    prevMeasure = measure.Key;
                }
            }

            // chords
            foreach (KeyValuePair<float, List<NotePlay>> pair in playNotes)
            {
                List<Note> chordableNotes = new List<Note>();
                foreach (NotePlay n in pair.Value)
                {
                    if (n.type != NotePlayType.HoldEnd && n.type != NotePlayType.Untimed)
                    if (!(new NotePlayType[] { NotePlayType.HoldEnd, NotePlayType.Untimed, NotePlayType.HoldMid }).Contains(n.type))
                        chordableNotes.Add(n);
                }
                if (chordableNotes.Count >= 2)
                {
					// GD.Print($"Found chord: {string.Join(", ", chordableNotes)}");
					// TODO: draw chord indicators "Chordify"
				}
            }

            // Measure Lines //
            // TODO: adapt to tempo changes in the middle of a measure
            // int tempoIdx = 1;
            // int bpmIdx = 1;
            // for (int curMeasure = 0; curMeasure < chart.notes.Count; curMeasure++)
            // {
            //     while (curMeasure >= bpmChangeMeasures[bpmIdx] && bpmIdx < bpmChangeMeasures.Count - 1)
            //         ++bpmIdx;
            //     GD.Print($"{curMeasure}: {bpmIdx}");

            //     // last tempo change / only one tempo change exists
            //     if (tempoIdx == tempoChangeMeasures.Count - 1)
            //     {
            //         float pos = tempoChangePositions[tempoIdx] + Util.NotePosition(curMeasure - tempoChangeMeasures[tempoIdx], 0, tempo.Last(), beatsPerMeasure[bpmIdx]);
            //         var ml = measureLine.Instance<MeasureLine>();
            //         measureScroll.AddChild(ml);
            //         ml.Translation = new Vector3(0, 0, pos);
            //         ml.Text = $"{curMeasure}";
            //     }
            //     else if (tempoIdx < tempoChangeMeasures.Count)
            //     {
            //         // TODO: adapt to key signature changes
            //         while (curMeasure == tempoChangeMeasures[tempoIdx])
            //         {
            //             int measuresToCreate = tempoChangeMeasures[tempoIdx] - tempoChangeMeasures[tempoIdx - 1];
            //             for (int i = 0; i < measuresToCreate; ++i)
            //             {
            //                 int measureNum = tempoChangeMeasures[tempoIdx - 1] + i;
            //                 // GD.Print($"{tempoIdx} / {tempoChangePositions.Count}, {tempo.Count}");
            //                 float pos = Util.InterpFloat(tempoChangePositions[tempoIdx - 1], tempoChangePositions[tempoIdx], (float)i/measuresToCreate);

            //                 var ml = measureLine.Instance<MeasureLine>();
            //                 measureScroll.AddChild(ml);
            //                 ml.Translation = new Vector3(0, 0, pos);
            //                 ml.Text = $"{curMeasure}";
            //             }
            //             tempoIdx = Mathf.Clamp(tempoIdx + 1, 0, tempo.Count - 1);
            //         }
            //     }
            // }

            GD.Print("Finished forming Chart!");
        }
    }
}
