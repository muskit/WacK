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
		public NoteHold holdNoteData;
		private Node2D longThing;

        public void InitHold(NoteHold noteData, Control holdScroll)
		{
			holdNoteData = noteData;
			BuildLongThing(holdScroll);
		}

		// Create longThing in segments.
        public void BuildLongThing(Control holdScroll)
        {
            longThing = new Node2D();
            holdScroll.AddChild(longThing);
            longThing.Position = new Vector2(0, (float)-holdNoteData.time * Play.scrollPxPerSec);

			GD.Print($"{holdNoteData.points.Count}");
            if (holdNoteData.points.Count > 0)
            {
                NotePlay lastHold = holdNoteData;
                float segmentPos = 0;
				foreach (var (_, curNote) in holdNoteData.points)
				{
                    var curLength = Play.scrollPxPerSec * (float)(curNote.time - lastHold.time);
                    var segment = CreateSegment(lastHold, curNote);
                    longThing.AddChild(segment);
                    segment.Position = new Vector2(0, segmentPos);

                    segmentPos -= curLength;
                    lastHold = curNote;
                }
            }
            else
            {
				GD.PrintErr("Tried to create a long note with no segments!");
            }
        }

        private Polygon2D CreateSegment(NotePlay origin, NotePlay destination)
        {
			float minuteSize = Constants.BASE_2D_RESOLUTION / 60;

            var length = Play.scrollPxPerSec * (float)(destination.time - origin.time);
            var verts = new Vector2[4];

            int originPos;
            int originSize;
            int destPos;
            int destSize;
            if (3 <= origin.size && origin.size <= 59)
            {
                originPos = ((int)origin.pos + 1)%60;
                originSize = (int)origin.size - 2;
            }
            else
            {
                originPos = (int)origin.pos;
                originSize = (int)origin.size;
            }

            if (3 <= destination.size && destination.size <= 59)
            {
                destPos = ((int)destination.pos + 1)%60;
                destSize = (int)destination.size - 2;
            }
            else
            {
                destPos = (int)destination.pos;
                destSize = (int)destination.size;
            }

            destPos = (int)Util.NearestMinute(originPos, destPos);
            verts[0] = new Vector2(originPos * minuteSize, 0);
            verts[1] = new Vector2(verts[0].X + originSize * minuteSize, 0);
            verts[2] = new Vector2(minuteSize * (destPos + destSize), -length);
            verts[3] = new Vector2(minuteSize * destPos, -length);
            var segment = new Polygon2D() { Polygon = verts, Antialiased = true };

            // draw overflow
            var originFinalPos = originPos + originSize;
            var destinationFinalPos = destPos + destSize;
            if (originFinalPos > 60 || destinationFinalPos > 60)
            {
                var subSegment = new Polygon2D() { Polygon = verts, Antialiased = true };
                subSegment.Translate(new Vector2(-60 * minuteSize, 0));
                segment.AddChild(subSegment);
            }
            if (originFinalPos < 60 || destinationFinalPos < 60)
            {
                var subSegment = new Polygon2D() { Polygon = verts, Antialiased = true };
                subSegment.Translate(new Vector2(60 * minuteSize, 0));
                segment.AddChild(subSegment);
            }

            return segment;
        }
    }
}