namespace WacK.Configuration
{
	/// <summary>
	/// Player-configured settings that affect gameplay mechanics.
	/// Can
	/// </summary>
	public class PlaySettings
	{
		/// <summary>
		/// Scroll speed multiplier.
		/// </summary>
		public static Config<float> playSpeedMultiplier =
			new("PlaySettings", "playSpeedMultiplier", 2f);

		/// <summary>
		/// How much to shift song audio by in seconds.
		/// </summary>
		public static Config<float> audioOffset =
			new("PlaySettings", "audioOffset", 0f);
	}
}