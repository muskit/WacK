using System.Windows.Markup;
using System.Xml.Linq;
using Godot;

namespace WacK.Configuration
{
	/// <summary>
	/// A configurable variable that saves to user://settings.cfg.
	/// </summary>
	public class Config<[MustBeVariant] T>
	{
		// --- STATICALLY-TRACKED CONFIG FILE ---
		private static ConfigFile cfgFile = new();
		public static readonly string cfgPath = "user://settings.cfg";

		// Metadata
		string section, name;

		// Callback
		public delegate void OnValueSet();
		private OnValueSet callback;

		// Stored values
		public T _default { get; private set; }
		private T _value;
		public T Value
		{
			get => _value;
			set
			{
				_value = value;
				Save();
				if (callback != null) callback();
			}
		}

		static Config()
		{
			var err = cfgFile.Load("user://settings.cfg");
			if (err != Error.Ok)
			{
				GD.Print("Creating new settings file...");
				cfgFile.Save("user://settings.cfg");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="section">CANNOT have spaces.</param>
		/// <param name="name">CANNOT contain spaces.</param>
		/// <param name="defaultValue">The value to set if not found in the settings file.</param>
		public Config(string section, string name, T defaultValue, OnValueSet callback = null)
		{
			this.section = section;
			this.name = name;
			_default = defaultValue;
			this.callback = callback;
			Fetch();
		}
		
		private void Fetch()
		{
			if (!cfgFile.HasSectionKey(section, name))
			{
				cfgFile.SetValue(section, name, Variant.From(_default));
				Save();
			}
			Value = cfgFile.GetValue(section, name).As<T>();
		}

		private void Save()
		{
			cfgFile.Save(cfgPath);
		}
	}
}