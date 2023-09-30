using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using Godot;
using WacK.Configuration;
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
			GD.Print($"Chart: {chartPath}\nSound: {soundPath}");
		}
	}
	public partial class Play : Node
	{
		// initialized by another scene, BEFORE loading this one!
		public static PlayParameters playParams;

		// TunnelObjects we can instantiate
		public static PackedScene noteTouch = GD.Load<PackedScene>("res://Things/TunnelObjects/Notes/NoteTouch.tscn");
		public static PackedScene noteHold = GD.Load<PackedScene>("res://Things/TunnelObjects/Notes/NoteHold.tscn");
		public static PackedScene noteChain = GD.Load<PackedScene>("res://Things/TunnelObjects/Notes/NoteChain.tscn");
		public static PackedScene noteCW = GD.Load<PackedScene>("res://Things/TunnelObjects/Notes/NoteSwipeCW.tscn");
		public static PackedScene noteCCW = GD.Load<PackedScene>("res://Things/TunnelObjects/Notes/NoteSwipeCCW.tscn");
		public static PackedScene noteIn = GD.Load<PackedScene>("res://Things/TunnelObjects/Notes/NoteSnapIn.tscn");
		public static PackedScene noteOut = GD.Load<PackedScene>("res://Things/TunnelObjects/Notes/NoteSnapOut.tscn");

		[ExportCategory("Audio")]
		[Export]
		private BGM bgmController;
		[Export]
		private SFX sfxController;

		[ExportCategory("2D")]
		[ExportSubgroup("2D Things")]
		[Export]
		public Control noteDisplay;
		[Export]
		public Control scrollDisplay;
		[Export]
		public Background background;

		[ExportSubgroup("Out-of-bounds Viewports")]
		[Export]
		public Viewport mainViewport;
		[Export]
		public Viewport leftViewport;
		[Export]
		public Viewport rightViewport;

		private Chart chart;
		// Indices point to the NEXT thing in chart to look for. We process
		// that thing once the song time is at or later than the thing's time.
		private int playNextIdx = 0;
		private int eventNextIdx = 0;

		// base scroll speed, which we can apply multipliers on
		public static readonly float BASE_PIXELS_PER_SECOND = 800;
		public static float ScrollPxPerSec
		{
			get
			{
				return BASE_PIXELS_PER_SECOND * PlaySettings.playSpeedMultiplier.Value;
			}
		}

		public override void _Ready()
		{
			// so we can see objects outside of the 0-60min. region
			leftViewport.World2D = mainViewport.World2D;
			rightViewport.World2D = mainViewport.World2D;

			// parse mer and create chart for current play
			chart = new(playParams.chartPath);
			InstantiateChartVisuals();

			// // audio setup
			bgmController.LoadFromUser(playParams.soundPath);
			bgmController.Play();

			// TestBGAnim();
		}

		/// <summary>
		/// Instantiates necessary notes onto noteDisplay for the player to see.
		/// </summary>
		private void InstantiateChartVisuals()
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
						case NotePlayType.Chain:
							nNote = noteChain.Instantiate<THNotePlay>();
							break;
						case NotePlayType.SwipeCW:
							nNote = noteCW.Instantiate<THNotePlay>();
							break;
						case NotePlayType.SwipeCCW:
							nNote = noteCCW.Instantiate<THNotePlay>();
							break;
						case NotePlayType.SnapIn:
							nNote = noteIn.Instantiate<THNotePlay>();
							break;
						case NotePlayType.SnapOut:
							nNote = noteOut.Instantiate<THNotePlay>();
							break;
						default:
							continue;
					}
					nNote.Init(note);
					var nPos = nNote.Position;
					nPos.Y = msNote.Key * -ScrollPxPerSec;
					nNote.Position = nPos;
					noteDisplay.AddChild(nNote);
				}
			}
		}

		/// <summary>
		/// Process current game state. Should only run if playing a chart and unpaused.
		/// </summary>
		private void PlayLoop()
		{
			float time = bgmController.CurTime;

			// check next event
			while (eventNextIdx < chart.events.Count && time >= chart.events.Keys[eventNextIdx])
			{
				var t = chart.events.Keys[eventNextIdx];
				var l = chart.events[t];
				
				foreach (var e in l)
				{
					GD.Print($"Passed event {e.type} at {t}");
					switch (e.type)
					{
						case NoteEventType.BGAdd:
							background.SetSegments((int)e.pos, (int)e.size, true, (DrawDirection)e.value);
							break;
						case NoteEventType.BGRem:
							background.SetSegments((int)e.pos, (int)e.size, false, (DrawDirection)e.value);
							break;
					}
				}
				
				eventNextIdx++;
			}
			
			// set scroll
			var nPos = noteDisplay.Position;
			nPos.Y = time * ScrollPxPerSec;
			noteDisplay.Position = nPos;
			scrollDisplay.Position = nPos;
		}

        public override void _Process(double delta)
        {
			PlayLoop();
        }

        private void OnDestroy()
		{
			playParams = null;
		}
	}
}