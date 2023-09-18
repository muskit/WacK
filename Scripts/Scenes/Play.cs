using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using Godot;
using WacK.Data.Chart;
using WacK.Data.Mer;
using WacK.Things.TunnelObjects;

namespace WacK.Scenes
{
	public class PlayParameters
	{
		/* TODO: store song ID from internal database
		public string songID;
		public Difficulty diff;
		*/
		public string chartPath;
		public string soundPath;

		public PlayParameters(string chPath, string snPath)
		{
			chartPath = chPath;
			soundPath = snPath;
		}
	}
	public partial class Play : Node
	{
		// initialized by another scene, BEFORE loading this one!
		public static PlayParameters playParams;

		// TunnelObjects we can instantiate
		public static PackedScene noteTouch = GD.Load<PackedScene>("res://Things/TunnelObjects/Notes/NoteTouch.tscn");
		public static PackedScene noteHold = GD.Load<PackedScene>("res://Things/TunnelObjects/Notes/NoteHold.tscn");

		[Export]
		public Control noteDisplay;
		[Export]
		public Control scrollDisplay;
		[Export]
		public Background background;

		[Export]
		public Viewport mainViewport;
		[Export]
		public Viewport leftViewport;
		[Export]
		public Viewport rightViewport;

		private Chart chart;

		// base scroll speed, which we can apply multipliers on
		public static readonly float BASE_PIXELS_PER_SECOND = 800;
		public static float scrollPxPerSec = BASE_PIXELS_PER_SECOND * 3.5f;

		public override void _Ready()
		{
			// so we can see objects outside of the 0-60min. region
			leftViewport.World2D = mainViewport.World2D;
			rightViewport.World2D = mainViewport.World2D;

			// parse mer and create chart for current play
			chart = new(playParams.chartPath);
			RealizeChart();
		}

		/// <summary>
		/// Instantiates necessary notes onto noteDisplay for the player to see.
		/// </summary>
		private void RealizeChart()
		{	
			foreach (var msNote in chart.playNotes)
			{
				foreach (var note in msNote.Value)
				{
					THNotePlay nNote;
					switch (note.type)
					{
						case NotePlayType.HoldStart:
							nNote = noteHold.Instantiate<THNoteHold>();
							((THNoteHold)nNote).InitHold((NoteHold)note, scrollDisplay);
							break;
						case NotePlayType.Touch:
							nNote = noteTouch.Instantiate<THNotePlay>();
							break;
						default:
							continue;
					}
					nNote.Init(note);
					var nPos = nNote.Position;
					nPos.Y = msNote.Key * -scrollPxPerSec;
					nNote.Position = nPos;
					noteDisplay.AddChild(nNote);
				}
			}
		}

		// private void RealizeHolds(NoteHold note)
		// {
		// 	List<Vector2> verts = new(note.points.Count*2 + 2);

		// 	// HoldStart's pos
		// 	verts.Add(new Vector2((float)note.pos * 1920/60, (float)note.time * -scrollPxPerSec));
		// 	// ascending -- "left" side
		// 	foreach (var (t, n) in note.points)
		// 	{
		// 		verts.Add(new Vector2((float)n.pos * 1920/60, t * -scrollPxPerSec));
		// 	}
		// 	// descending -- "right" side
		// 	foreach (var (t, n) in note.points.Reverse())
		// 	{
		// 		verts.Add(new Vector2((float)((int)n.pos + (int)n.size) * 1920/60, t * -scrollPxPerSec));
		// 	}
		// 	// HoldStart's pos + size
		// 	verts.Add(new Vector2((float)((int)note.pos + (int)note.size) * 1920/60, (float)note.time * -scrollPxPerSec));

		// 	var p2d = new Polygon2D
		// 	{
		// 		Polygon = verts.ToArray(),
		// 		Antialiased = true,
		// 		Modulate = new Color("#FFFFFFD0")
        //     };
        //     scrollDisplay.AddChild(p2d);
		// }

        public override void _Process(double delta)
        {
			var nPos = noteDisplay.Position;
			nPos.Y += (float)delta * scrollPxPerSec;
			noteDisplay.Position = nPos;
			scrollDisplay.Position = nPos;
        }

        private void OnDestroy()
		{
			playParams = null;
		}
	}
}