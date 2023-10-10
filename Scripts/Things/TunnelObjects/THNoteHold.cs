using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Godot;
using WacK.Configuration;
using WacK.Data.Chart;
using WacK.Scenes;

namespace WacK.Things.TunnelObjects
{
	public partial class THNoteHold : THNotePlay
	{
		public NoteHold holdData;
		private Node2D longThing;

        public void InitHold(NoteHold noteData, Control holdScroll)
		{
			holdData = noteData;
			BuildLongThing(holdScroll);
		}

		// Create longThing in segments.
        private void BuildLongThing(Control holdScroll)
        {
            longThing = new Node2D();
            holdScroll.AddChild(longThing);
            longThing.Position = new Vector2(0, (float)-holdData.time * Play.ScrollPxPerSec);

            // don't draw invisible hold-mids
            var drawableMids = holdData.points.Values.Where(e => e.type != NotePlayType.HoldMidInvis).ToList();
            if (drawableMids.Count > 0)
            {
                var lastMid = holdData.points.Values[^1];
                if (drawableMids[^1] != lastMid) drawableMids.Add(lastMid);
            }
            else
                // would most likely happen if HoldEnd is missing
                drawableMids = holdData.points.Values.ToList();

            if (drawableMids.Count() > 0)
            {
                NotePlay lastHold = holdData;
                float segmentPos = 0;
				foreach (var curNote in drawableMids)
				{
                    var curLength = Play.ScrollPxPerSec * (float)(curNote.time - lastHold.time);
                    var segment = CreateSegment(lastHold, curNote);
                    longThing.AddChild(segment);
                    segment.Position = new Vector2(0, segmentPos);

                    segmentPos -= curLength;
                    lastHold = curNote;
                }
            }
            else
            {
				GD.PrintErr("Tried to create a Hold note's long with no drawable segments!");
            }
        }

        private Polygon2D CreateSegment(NotePlay origin, NotePlay destination)
        {
            var length = Play.ScrollPxPerSec * (float)(destination.time - origin.time);
            var verts = new Vector2[4];

            int destPosNearest = Util.NearestMinute((int) origin.pos, (int) destination.pos);

            var (originPosPx, originSizePx) = Util.PixelizeNote((int)origin.pos, (int)origin.size);
            var (destPosPx, destSizePx) = Util.PixelizeNote(destPosNearest, (int)destination.size);

            verts[0] = new Vector2(originPosPx, 0);
            verts[1] = new Vector2(verts[0].X + originSizePx, 0);
            verts[2] = new Vector2(destPosPx + destSizePx, -length);
            verts[3] = new Vector2(destPosPx, -length);
            var segment = new Polygon2D() { Polygon = verts, Antialiased = true };

            // draw overflow
            var originFinalPos = origin.pos + origin.size;
            var destinationFinalPos = destPosNearest + destination.size;
            if (originFinalPos > 60 || destinationFinalPos > 60)
            {
                GD.Print("overflowed to the right!");
                var subSegment = new Polygon2D
                {
                    Polygon = verts,
                    Antialiased = true,
                    Position = new Vector2(-Constants.BASE_2D_RESOLUTION, 0)
                };
                segment.AddChild(subSegment);
            }
            if (originFinalPos < 0 || destinationFinalPos < 0)
            {
                GD.Print("overflowed to the left!");
                var subSegment = new Polygon2D
                {
                    Polygon = verts,
                    Antialiased = true,
                    Position = new Vector2(Constants.BASE_2D_RESOLUTION, 0)
                };
                segment.AddChild(subSegment);
            }
            segment.Modulate = new Color("#FFFFFFD0");
            return segment;
        }
    }
}