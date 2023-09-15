using Godot;
using WacK.Data.Chart;
using WacK.Data.Mer;

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

		private Chart chart;

		public override void _Ready()
		{ 
			chart = new(playParams.chartPath);
		} 
		private void OnDestroy()
		{
			playParams = null;
		}
	}
}