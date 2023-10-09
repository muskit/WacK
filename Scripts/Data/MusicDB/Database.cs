using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Godot;
using WacK.Configuration;

namespace WacK.MusicDB
{
	public class Database
	{
		// TODO: relocate?
		public static readonly string[] DifficultyStr = {
			((DifficultyLevel)0).ToString(),
			((DifficultyLevel)1).ToString(),
			((DifficultyLevel)2).ToString(),
			((DifficultyLevel)3).ToString(),
		};
		public static readonly string SONGS_DIR = "user://songs";
		public static Database instance;

		public List<Song> songs;

		public static void Init()
		{
			if (instance != null) return;

			instance = new Database();
		}

		public Database()
		{
			TryCreateSongsDir();
			AddSongDirs(SONGS_DIR);
		}
		
		private void TryCreateSongsDir()
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
		
		/// <summary>
		/// Recursively find and add song folders.
		/// </summary>
		/// <param name="path"></param>
		private void AddSongDirs(string path)
		{
			var dir = DirAccess.Open(path);
			if (dir == null) return;

			// recursively check subdirectories
			foreach (var d in dir.GetDirectories())
			{
				AddSongDirs($"{path}/{d}");
			}

			// don't do anything if there isn't a song.ini
			if (!dir.FileExists("song.ini")) return;

			/// AT THIS POINT, WE'RE IN A SONG FOLDER ///
			GD.Print($"{path} -------------------------");
			
			var songIni = new ConfigFile();
			songIni.Load($"{path}/song.ini");
			try
			{
				var merRegex = @"\d+.*\.mer$";
				// song info
				var name = songIni.GetValue("Song", "name").As<string>();
				var artist = songIni.GetValue("Song", "artist").As<string>();
				var category = songIni.GetValue("Song", "category").As<string>();
				var copyright = songIni.GetValue("Song", "copyright").As<string>();
				GD.Print($"Song(name={name},artist={artist},category={category},copyright={copyright})");
				
				// per-difficulty info
				foreach (var f in dir.GetFiles())
				{
					var m = Regex.Match(f, merRegex, RegexOptions.IgnoreCase);
					if (m.Captures.Count > 0)
					{
						var substr = m.Captures[0].ToString();
						var num = int.Parse(new string(substr.SkipWhile(c => !Char.IsDigit(c)).TakeWhile(Char.IsDigit).ToArray()));

						GD.Print($"{f}: is diff {DifficultyStr[num]}");
					}
					else
					{
						GD.Print($"{f} is not a chart");
					}
				}
			}
			catch
			{
				GD.Print($"Error parsing {path}/song.ini, skipping!");
				return;
			}
		}

		public void SaveCache()
		{
			
		}

		public void LoadCache()
		{
			
		}
	}
}