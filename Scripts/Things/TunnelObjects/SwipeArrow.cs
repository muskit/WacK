using Godot;
using System;

namespace WacK.Things.TunnelObjects
{
	[Tool]
	public partial class SwipeArrow : Control
	{
		public readonly Color COLOR_CW = new("#FF8000");
		public readonly Color COLOR_CCW = new("#00FF00");

		private ShaderMaterial shader;

        public override void _EnterTree()
        {
            shader = (ShaderMaterial) Material;
        }
		
		public void SetCW(bool isCW)
		{
			shader.SetShaderParameter("ArrowColor", isCW ? COLOR_CW : COLOR_CCW);
			shader.SetShaderParameter("isCwShape", isCW);
		}

        public void SetPosSize(int pos, int size)
		{
			var p = Position;
			p.X = Constants.BASE_2D_RESOLUTION / 60 * pos + Constants.BASE_2D_RESOLUTION / 120;
			Position = p;

			var s = Size;
			s.Y = Constants.BASE_2D_RESOLUTION / 60 * size - Constants.BASE_2D_RESOLUTION / 60;
			Size = s;
			shader.SetShaderParameter("TileMult", s.Y / 64);
		}
	}
}