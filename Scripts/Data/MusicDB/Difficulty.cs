using WacK.Data.Chart;

namespace WacK.MusicDB
{	
	public enum DifficultyLevel
	{
		Normal, Hard, Expert, Inferno
	}

	public struct Difficulty
	{
		DifficultyLevel diffLevel;
		float diffValue;
		
		/// <summary>
		/// % of max score required to clear this chart.
		/// </summary>
		float clearRatio;

		/// <summary>
		/// Path to audio file for this difficulty.
		/// </summary>
		string audioFile;

		string designer;

		float audioPreviewStart, audioPreviewLength;
		float audioOffset; // in seconds
	}
}
