using System.Collections.Generic;

using Godot;

namespace WacK.MusicDB
{
	public class Database
	{
		public static readonly string SONGS_DIR = "user://songs";
		public List<Song> songs;

		public static Database instance;

		public static void Init()
		{
			if (instance != null) return;

			instance = new Database();
		}

		public Database()
		{
			InitSongsDir();
			BuildDatabase(SONGS_DIR);
		}
		
		private void InitSongsDir()
		{
			GD.Print($"User directory: {OS.GetUserDataDir()}");
			var songDir = DirAccess.Open(SONGS_DIR);
			if (songDir != null)
			{
				GD.Print("Successfully opened songs directory!");
			}
			else
			{
				GD.Print("Could not find songs directory! Creating it...");

				DirAccess.MakeDirAbsolute(SONGS_DIR);
				using var newSongDir = DirAccess.Open(SONGS_DIR);

				if (newSongDir != null)
				{
					GD.Print("Songs folder created successfully!");
					// create note
					var note = "Place song folders here. Nested folders supported for organization.\n";
					using (var f = FileAccess.Open($"{newSongDir.GetCurrentDir()}/note.txt", FileAccess.ModeFlags.Write))
					{
						f.StoreString(note);
					}
				}
				else
				{
					GD.PrintErr($"Could not create the songs directory!\n{DirAccess.GetOpenError()}");
				}

			}
		}
		
		private void BuildDatabase(string path)
		{
			var d = DirAccess.Open(path);
			if (d == null) return;

		}

		public void SaveCache()
		{
			
		}

		public void LoadCache()
		{
			
		}
	}
}