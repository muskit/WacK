/**
 * Background.cs
 * Set various properties of the drawn background.
 *
 * by muskit
 * July 1, 2022
 **/

using Godot;
using System.Collections.Generic;

namespace WacK.Things.TunnelObjects
{

	public enum DrawDirection {
		CounterClockwise, Clockwise, Center
	}
	public partial class Background : Node
	{
		[Export]
		private TextureRect firstSegment;
		private List<TextureRect> segments = new(60);

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			var segmentsNode = FindChild("Segment Masks");
			firstSegment.Visible = false;
			segments.Add(firstSegment);
			for (int i = 1; i < 60; ++i)
			{
				var n = (TextureRect)firstSegment.Duplicate();
				segmentsNode.AddChild(n);
				segments.Add(n);
				n.Name = i.ToString();
				n.SetPosition(new Vector2(i * Constants.BASE_2D_RESOLUTION / 60, Constants.BASE_2D_RESOLUTION));
			}
		}

		// draw in 6/60 frames (0.1s)
		// TODO: figure out how WACCA handles animation speed
		public async void SetSegments(int pos, int size, bool state, DrawDirection direction)
		{
			// GD.Print($"{direction} = {state}. Even? {size % 2 == 0}");

			double timer = 0;
			double time = .1f;

			int centerSeg = pos + size/2;
			while (timer < time)
			{
				timer = Mathf.Clamp(timer + GetProcessDeltaTime(), 0, time);
				var timerRatio = (float)(timer / time);
				int steps = Mathf.CeilToInt(size*timerRatio);

				switch(direction)
				{
					case DrawDirection.CounterClockwise:
						for (int i = 0; i < steps; ++i)
						{
							segments[(i + pos)%60].Visible = state;
						}
						break;
					case DrawDirection.Center: // add: center to edge. rem: edge to center.
						for (int i = centerSeg; i < Util.InterpInt(centerSeg, pos+size, timerRatio); ++i)
						{
							segments[i % 60].Visible = state;
						}
						for (int i = centerSeg; i >= Util.InterpInt(centerSeg, pos, timerRatio); --i)
						{
							segments[i % 60].Visible = state;
						}
						break;
					case DrawDirection.Clockwise:
						for (int i = 0; i < steps; ++i)
						{
							segments[(pos + size - i - 1)%60].Visible = state;
						}
						break;
				}
				await ToSignal(GetTree(), "process_frame");
			}
			GD.Print("Finished BG anim!");
		}
	}
}
