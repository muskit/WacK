using Godot;
using System;
using WacK.MusicDB;

namespace WacK.Scenes
{
	public partial class Startup : Node
	{
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			Database.Init();

			// Change scenes
			GetTree().ChangeSceneToFile("res://Scenes/DebugChartLoader.tscn");
		}
	}
}
