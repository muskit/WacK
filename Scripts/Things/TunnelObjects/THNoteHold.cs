using Godot;
using WacK.Data.Chart;

namespace WacK.Things.TunnelObjects
{
	public partial class THNoteHold : THNotePlay
	{
		public new NoteHold noteData;

        public void Init(NoteHold noteData)
		{
			base.Init(noteData);
			this.noteData = noteData;
			
			// TODO: setup other Nodes to render hold note properly
		}
    }
}