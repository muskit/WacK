namespace WacK.Configuration
{
	/// <summary>
	/// Configuration that affects the general application.
	/// </summary>
	public class GameSettings
	{
		/// --- GRAPHICS --- ///
		
		/// <summary>
		/// Value to multiply the canvas resolution by.
		/// </summary>
		public static Config<float> canvasResolutionMult =
			new("Graphics", "canvasResolutionMultiplier", 1f);
		public static Config<bool> vsync =
			new("Graphics", "vsync", true);
	}
}