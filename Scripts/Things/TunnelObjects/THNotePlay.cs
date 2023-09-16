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
			SetSizePos((int)noteData.position, (int)noteData.size);
		}
		
		public void SetSizePos(int pos, int size)
		{
			// TODO: pos + size >= 60
			var nPos = Position;
			nPos.X = pos * (1920f/60) - 12;
			Position = nPos;

			var nSize = Size;
			nSize.X = size * (1920f/60) + 24;
			Size = nSize;
		}
	}
}