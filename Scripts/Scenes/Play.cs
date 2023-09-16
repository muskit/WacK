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
		public static PackedScene notePlay = GD.Load<PackedScene>("res://Things/TunnelObjects/Notes/NoteTouch.tscn");

		[Export]
		public Control noteDisplay;

		private Chart chart;

		public override void _Ready()
		{ 
			// parse mer and create chart for current play
			chart = new(playParams.chartPath);
			RealizeChart();
		}

		/// <summary>
		/// Instantiates necessary notes onto the noteDisplay for the player to see.
		/// </summary>
		private void RealizeChart()
		{	
			foreach (var msNote in chart.playNotes)
			{
				GD.Print(msNote.Key);
				foreach (var note in msNote.Value)
				{
					THNotePlay nNote;
					switch (note)
					{
						default: // tap note
							nNote = notePlay.Instantiate<THNotePlay>();
							break;
					}
					nNote.Init(note);
					var nPos = nNote.Position;
					nPos.Y = msNote.Key * -1000;
					nNote.Position = nPos;
					noteDisplay.AddChild(nNote);
				}
			}
		}

        public override void _Process(double delta)
        {
			var nPos = noteDisplay.Position;
			nPos.Y += (float)delta * 1000;
			noteDisplay.Position = nPos;
        }

        private void OnDestroy()
		{
			playParams = null;
		}
	}
}