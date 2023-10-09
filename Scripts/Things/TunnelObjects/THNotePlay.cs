using Godot;
using WacK.Data.Chart;

namespace WacK.Things.TunnelObjects
{
	public partial class THNotePlay : Control
	{
		[Export]
		private NinePatchRect noteBase;
		public NotePlay noteData;
		
		public async void Init(NotePlay noteData)
		{
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			
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
			var nbPos = pos;
			var nbSize = size;
			// TODO: end caps peak into bounds
			if (3 <= size && size <= 59)
			{
				nbPos += 1;
				nbSize -= 2;
			}
			else if (size >= 60)
			{
				size = 60;
				nbSize = 60;
				noteBase.RegionRect = new Rect2(12, 0, new Vector2(488, 36));
				noteBase.PatchMarginLeft = 0;
				noteBase.PatchMarginRight = 0;
			}

			var nPos = noteBase.Position;
			nPos.X = nbPos * (Constants.BASE_2D_RESOLUTION/60) - 12;
			noteBase.Position = nPos;

			var nSize = noteBase.Size;
			nSize.X = nbSize * (Constants.BASE_2D_RESOLUTION/60) + 24;
			noteBase.Size = nSize;
			
			// handle swipe arrow size
			if (noteData.type == NotePlayType.SwipeCW || noteData.type == NotePlayType.SwipeCCW)
			{
				var n = (SwipeArrow) FindChild("SwipeArrow");
				n.SetPosSize(pos, size);
			}
		}
	}
}