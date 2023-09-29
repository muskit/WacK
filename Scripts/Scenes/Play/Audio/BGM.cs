using Godot;
using Godot.Collections;
using System;

public partial class BGM : AudioStreamPlayer
{
	public void LoadFromUser(string path)
	{
		if (!path.StartsWith("user://"))
		{
			GD.Print("Tried to load audio that isn't in user directory.");
			return;
		}

		var f = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		if (f == null)
		{
			GD.PrintErr($"Unable to open {path} for loading audio! {FileAccess.GetOpenError()}");
			return;
		}
		GD.Print("hi");
		var ext = path.Split('.')[^1].ToLower();
		switch (ext)
		{
			case "mp3":
				var mp3 = new AudioStreamMP3()
				{
					Data = f.GetBuffer((long)f.GetLength())
				};
				Stream = mp3;
				break;
			case "wav":
			case "wave":
				var wav = new AudioStreamWav()
				{
					Data = f.GetBuffer((long)f.GetLength())
				};
				Stream = wav;
				break;
			case "ogg":
				// TODO: implement
				GD.PrintErr("External OGGs not supported in Godot 4.1...");
				break;
		}
	}
}
