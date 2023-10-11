using Microsoft.VisualBasic.CompilerServices;
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
		}
		
		public void SetPosSize(int pos, int size)
		{
			var (posPx, sizePx) = Util.PixelizeNote(pos, size);

			if (size >= 60)
			{
				size = 60;
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
			
			// handle swipe arrow
			if (noteData.type == NotePlayType.SwipeCW || noteData.type == NotePlayType.SwipeCCW)
			{
				var n = (SwipeArrow) FindChild("SwipeArrow");
				n.CallDeferred("Init", pos, size, noteData.type == NotePlayType.SwipeCW);
			}

			// handle snap arrow
			if (noteData.type == NotePlayType.SnapIn || noteData.type == NotePlayType.SnapOut)
			{
				var n = (SnapArrows)FindChild("SnapArrows");
				n.CallDeferred("Init", pos, size, noteData.type == NotePlayType.SnapIn);
			}
		}
	}
}