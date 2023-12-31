using Godot;
using System;

namespace WacK.Things.TunnelObjects
{
	public partial class SwipeArrow : Control
	{

		private ShaderMaterial shader;

        public override void _EnterTree()
        {
            shader = (ShaderMaterial) Material;
        }

        public void Init(int pos, int size, bool isCW)
		{
			var p = Position;
			var s = Size;
			
			if (size <= 59)	
			{
				p.X = Constants.BASE_2D_RESOLUTION / 60 * pos + Constants.BASE_2D_RESOLUTION / 120;
				s.Y = Constants.BASE_2D_RESOLUTION / 60 * size - Constants.BASE_2D_RESOLUTION / 60;
			}
			else
			{
				p.X = Constants.BASE_2D_RESOLUTION / 60 * pos;
				s.Y = Constants.BASE_2D_RESOLUTION;

			}

			
			Position = p;
			Size = s;
			shader.SetShaderParameter("TileMult", s.Y / 64);
			shader.SetShaderParameter("isCw", isCW);
		}
	}
}