using Godot;
using System;

namespace WacK.Scenes
{
	public partial class Startup : Node
	{
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			GD.Print($"User directory: {OS.GetUserDataDir()}");
			using var songDir = DirAccess.Open("user://songs");
			if (songDir != null)
			{
				GD.Print("Successfully opened songs directory!");
				// load songs
			}
			else
			{
				GD.Print("Could not find songs directory! Creating it...");

				DirAccess.MakeDirAbsolute("user://songs");
				using var newSongDir = DirAccess.Open("user://songs");

				if (newSongDir != null)
				{
					GD.Print("Songs folder created successfully!");
					// create note
					var note = "Place song folders here. Nested folders supported for organization.\n";
					using var f = FileAccess.Open($"{newSongDir.GetCurrentDir()}/note.txt", FileAccess.ModeFlags.Write);
					f.StoreString(note);

					// TODO: add in-game notice
				}
				else
				{
					GD.PrintErr($"Could not create the songs directory!\n{DirAccess.GetOpenError()}");
				}

			}

			// Change scenes
			GetTree().ChangeSceneToFile("res://Scenes/DebugChartLoader.tscn");
		}
	}
}
