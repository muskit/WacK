using Godot;
using System;
using System.Linq;
using WacK;

namespace WacK.Things.TunnelObjects
{
	public partial class SnapArrows : HBoxContainer
	{
		private TextureRect[] arrows = new TextureRect[20];

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			var c = GetChildren();
			for(int i = 0; i < 20; ++i)
			{
				arrows[i] = (TextureRect) c[i];
			}
		}

		/// <summary>
		/// Make sure to run as CallDeferred if constructing!
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="size"></param>
		/// <param name="isIn"></param>
		public void Init(int pos, int size, bool isIn)
		{
			var nVis = Math.Clamp(size / 3, 1, 20);
				
			for(int i = 0; i < arrows.Count(); ++i)
			{
				if (i < nVis)
				{
					arrows[i].Visible = true;
					((ShaderMaterial)arrows[i].Material)
						.SetShaderParameter("isIn", isIn);
				}
				else
				{
					arrows[i].Visible = false;
				}
			}

			// shrink
			var s = Size;
			s.X = 0;
			Size = s;

			var (posPx, sizePx) = Util.PixelizeNote(pos, size);
			var noteCtrPos = posPx + sizePx / 2;

			// reposition
			var p = Position;
			p.X = -Size.X / 2 + noteCtrPos;
			Position = p;
		}
	}
}
