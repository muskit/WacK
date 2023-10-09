using Godot;
using Godot.Collections;
using System;

public partial class BGM : AudioStreamPlayer
{
	// latency compensation
	private ulong timeStartUsec;
	private float timeDelay;
	
	public float CurTime
	{
		get
		{
			float time = (Time.GetTicksUsec() - timeStartUsec) / 1000000f;
			return time - timeDelay;
		}
	}

	public void LoadFromUser(string path)
	{
		if (!path.StartsWith("user://"))
		{
			GD.PrintErr("Tried to load audio that isn't in user directory.");
			return;
		}

		var f = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		if (f == null)
		{
			GD.PrintErr($"Unable to open {path} for loading audio! {FileAccess.GetOpenError()}");
			return;
		}
		
		var ext = path.Split('.')[^1].ToLower();
		switch (ext)
		{
			case "mp3":
				GD.Print($"audio is MP3");
				var mp3 = new AudioStreamMP3()
				{
					Data = f.GetBuffer((long)f.GetLength())
				};
				Stream = mp3;
				break;
			case "wav":
			case "wave":
				GD.Print("audio is WAV");
				var buffer = f.GetBuffer((long)f.GetLength());

				/// WAV HEADER PARSING ///
				/// https://medium.com/swlh/reversing-a-wav-file-in-c-482fc3dfe3c4
				
				// bit format
				var bf = new byte[]{ buffer[34], buffer[35] };
				var bitFormat = BitConverter.ToUInt16(bf) switch
				{
					8 => AudioStreamWav.FormatEnum.Format8Bits,
					16 => AudioStreamWav.FormatEnum.Format16Bits,
					_ => AudioStreamWav.FormatEnum.ImaAdpcm
				};
				GD.Print($"Bit format: {bitFormat}");

				// sample rate
				var sr = new byte[] { buffer[24], buffer[25], buffer[26], buffer[27] };
				var sampleRate = BitConverter.ToUInt32(sr);
				GD.Print($"Sample rate: {sampleRate}");

				// stereo or mono
				var c = new byte[] { buffer[22], buffer[23] };
				var channels = BitConverter.ToUInt16(c);
				GD.Print($"Channels: {channels}");

				var wav = new AudioStreamWav()
				{
					Data = buffer,
					Format = bitFormat,
					MixRate = (int)sampleRate,
					Stereo = channels <= 1 ? false : true
				};
				Stream = wav;
				break;
			case "ogg":
				// TODO: implement
				GD.PrintErr("External OGGs not supported in Godot 4.1...");
				break;
			default:
				GD.PrintErr("Unknown audio!");
				break;
		}
	}

	public void Play()
	{
		timeStartUsec = Time.GetTicksUsec();
		timeDelay = (float) (AudioServer.GetTimeToNextMix() + AudioServer.GetOutputLatency());
		base.Play();
	}
}
