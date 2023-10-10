using Godot;
using WacK.Data.Chart;

namespace WacK.Things.TunnelObjects
{
	public partial class THNotePlay : Control
	{
		[Export]
		private NinePatchRect noteBase;
		public NotePlay noteData;
		
		public void Init(NotePlay noteData)
		{
			this.noteData = noteData;
			SetPosSize((int)noteData.pos, (int)noteData.size);

			// handle swipe arrow color
			if (noteData.type == NotePlayType.SwipeCW)
			{
				var n = (SwipeArrow) FindChild("SwipeArrow");
				n.SetCW(noteData.type == NotePlayType.SwipeCW);
			}
		}
		
		public void SetPosSize(int pos, int size)
		{
			float pxPerMinute = Constants.BASE_2D_RESOLUTION / 60;

			float posPx = 0;
			float sizePx = 0;

			if (size <= 59)
			{
				if (size >= 3)
				{
					posPx = (pos + 1) * pxPerMinute;
					sizePx = (size - 2) * pxPerMinute;
				}
				else // 2 or smaller
				{ 
					posPx = pos * pxPerMinute;
					sizePx = size * pxPerMinute;
				}
				// end-caps
				posPx -= 12;
				sizePx += 24;
			}
			else // size is 60 or greater
			{
				size = 60;
				sizePx = Constants.BASE_2D_RESOLUTION;
				// remove end-caps
				noteBase.RegionRect = new Rect2(12, 0, new Vector2(488, 36));
				noteBase.PatchMarginLeft = 0;
				noteBase.PatchMarginRight = 0;
			}

			var nPos = noteBase.Position;
			nPos.X = posPx;
			noteBase.SetDeferred("position", nPos);

			var nSize = noteBase.Size;
			nSize.X = sizePx;
			noteBase.SetDeferred("size", nSize);
			
			// handle swipe arrow size
			if (noteData.type == NotePlayType.SwipeCW || noteData.type == NotePlayType.SwipeCCW)
			{
				var n = (SwipeArrow) FindChild("SwipeArrow");
				n.SetPosSize(pos, size);
			}
		}
	}
}