using Godot;
using System;

namespace WacK.Scenes
{
	public partial class DebugChartLoader : Node
	{
		[Export]
		private LineEdit inputField;

		[Export]
		private LineEdit soundField;

		[Export]
		private OptionButton difficultyButton;

		[Export]
		private Button playButton;


		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			playButton.Pressed += PlayClicked;
		}

		private void PlayClicked()
		{
			// TODO: globally accessible verify song folder and chart/audio function
			var songPath = $"user://songs/{inputField.Text}";
			var chartPath = $"{songPath}/{difficultyButton.Selected}.mer";
			var soundPath = $"{songPath}/{soundField.Text}";
			GD.Print($"Song: {songPath}\nChart: {chartPath}\nSound: {soundPath}");

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
			playButton.Pressed -= PlayClicked;
		}
	}
}
