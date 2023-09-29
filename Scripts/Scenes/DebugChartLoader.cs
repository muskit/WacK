using Godot;
using System;

namespace WacK.Scenes
{
	public partial class DebugChartLoader : Node
	{
		[Export]
		private LineEdit pathsLine;

		[Export]
		private OptionButton songsButton;

		[Export]
		private OptionButton soundButton;

		[Export]
		private OptionButton difficultyButton;

		[Export]
		private Button playButton;

		const string SONGS_PATH = $"user://songs";


		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			// --- UI SETUP --- //
			playButton.Pressed += PlayClicked;
			songsButton.ItemSelected += RefreshSounds;
			pathsLine.Text = OS.GetUserDataDir();

			// --- SONGS ENUMERATION --- //
			using var dir = DirAccess.Open(SONGS_PATH);
			if (dir == null)
			{
				GD.PrintErr($"Error occurred opening song folder!\n{DirAccess.GetOpenError()}");
			}
			else
			{
				// populate songsButton
				foreach (var sd in dir.GetDirectories())
				{
					var d = DirAccess.Open($"{SONGS_PATH}/{sd}");
					if (d == null) continue;

					songsButton.AddItem(sd);
				}
			}

			if (songsButton.ItemCount >= 1)
			{
				songsButton.Disabled = false;
				playButton.Disabled = false;
			}
			else
			{
				playButton.Text = "No songs found!";
			}

			RefreshSounds(0);
		}

        private void RefreshSounds(long _)
        {
			soundButton.Clear();
			var dir = DirAccess.Open($"{SONGS_PATH}/{songsButton.Text}");
			foreach (var sf in dir.GetFiles())
			{
				var l = sf.ToLower();
				if (l.EndsWith(".mp3") || l.EndsWith(".wav") || l.EndsWith(".ogg"))
				{
					soundButton.AddItem(sf);
				}
			}
			if (soundButton.ItemCount < 1)
			{
				soundButton.Disabled = true;
				soundButton.Text = "No audio files found!";
			}
			else
			{
				soundButton.Disabled = false;
			}
        }

		private void PlayClicked()
		{
			// TODO: globally accessible verify song folder and chart/audio function
			var songPath = $"user://songs/{songsButton.Text}";
			var chartPath = $"{songPath}/{difficultyButton.Selected}.mer";
			var soundPath = $"{songPath}/{soundButton.Text}";

			// folder check
			using var dir = DirAccess.Open(songPath);
			if (dir == null)
			{
				GD.PrintErr($"Error occurred opening song folder!\n{DirAccess.GetOpenError()}");
				return;
			}

			// chart check
			using var chartFile = FileAccess.Open(chartPath, FileAccess.ModeFlags.Read);
			if (chartFile == null)
			{
				GD.PrintErr($"Error occurred opening chart!\n{FileAccess.GetOpenError()}");
				return;
			}

			// sound check
			using var soundFile = FileAccess.Open(soundPath, FileAccess.ModeFlags.Read);
			if (soundFile == null)
			{
				GD.PrintErr($"Error occurred opening sound!\n{FileAccess.GetOpenError()}");
				return;
			}

			Play.playParams = new PlayParameters(chartPath, soundPath);
			GetTree().ChangeSceneToFile("res://Scenes/Play.tscn");
		}

		public override void _ExitTree()
		{
			songsButton.ItemSelected -= RefreshSounds;
			playButton.Pressed -= PlayClicked;
		}
	}
}
