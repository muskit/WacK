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
		/// Name of audio file for this level.
		/// </summary>
		string audioFile;

		/// <summary>
		/// Name of chart file for this level.
		/// </summary>
		string chartFile;

		string designer;

		float audioPreviewStart, audioPreviewLength;
		float audioOffset; // in seconds
	}
}
