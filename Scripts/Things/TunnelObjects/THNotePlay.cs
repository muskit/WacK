using Godot;
using WacK.Data.Chart;

namespace WacK.Things.TunnelObjects
{
	public partial class THNotePlay : Control
	{
		public NotePlay noteData;
		
		public void Init(NotePlay noteData)
		{
			this.noteData = noteData;
			SetSizePos((int)noteData.pos, (int)noteData.size);
		}
		
		public void SetSizePos(int pos, int size)
		{
			if (3 <= size && size <= 59)
			{
				pos += 1;
				size -= 2;				
			}
			else if (size >= 60)
			{
				size = 60;
				// remove padding
			}

			var nPos = Position;
			nPos.X = pos * (Constants.BASE_2D_RESOLUTION/60) - 12;
			Position = nPos;

			var nSize = Size;
			nSize.X = size * (Constants.BASE_2D_RESOLUTION/60) + 24;
			Size = nSize;
		}
	}
}