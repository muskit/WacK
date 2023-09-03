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
        private bool isReady = false;
        private float _drawLength;
        private List<Node3D> segments = new List<Node3D>();
        private StandardMaterial3D bgMaterial;
        [Export]
        public float DrawLength
        {
            set
            {
                _drawLength = value;
                if (!isReady) return;

                bgMaterial.DistanceFadeMinDistance = _drawLength;
                bgMaterial.DistanceFadeMaxDistance = 0;
                foreach (Node segment in segments)
                {
                    segment.GetChild<Node3D>(1).Scale = new Vector3(1, _drawLength, 1);
                }
            }
            get { return _drawLength; }
        }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            foreach (Node3D segment in GetChildren())
            {
                segments.Add(segment);
            }
            bgMaterial = (StandardMaterial3D) segments[0].GetChild<CsgPolygon3D>(1).Material;

            isReady = true;
            // DrawLength = DrawLength;
        }

        // draw in 6/60 frames (0.1s)
        // TODO: figure out how WACCA handles animation speed
        public async void SetSegments(int pos, int size, bool state, DrawDirection direction)
        {
            // GD.Print($"{direction} = {state}. Even? {size % 2 == 0}");

            double timer = 0;
            double time = 0.1f;

            int centerSeg = pos + size/2;
            while (timer < 0.1f)
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
                await ToSignal(GetTree(), "idle_frame");
            }
        }
    }
}
