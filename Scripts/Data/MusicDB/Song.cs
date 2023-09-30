namespace WacK.MusicDB
{
	public class Song
	{
		string name, artist, category, copyright;
		int tempo;

		// chart path relative to user://songs
		string dirPath;

		// should only hold 4 values
		Difficulty[] diff;
	}
}