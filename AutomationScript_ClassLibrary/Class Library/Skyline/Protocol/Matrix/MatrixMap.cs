namespace Skyline.DataMiner.Library.Protocol.Matrix
{
	public class MatrixMap
	{
		public MatrixMap(string name, int internalIndex, int tableColumnPid, int tableColumnWritePid)
		{
			this.Name = name;
			this.InternalIndex = internalIndex;
			this.TableColumnPid = tableColumnPid;
			this.TableColumnWritePid = tableColumnWritePid;
		}
		public int TableColumnPid { get; private set; }
		public int TableColumnWritePid { get; private set; }
		public int InternalIndex { get; private set; }
		public string Name { get; private set; }
	}
}
