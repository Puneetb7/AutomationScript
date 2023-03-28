namespace Skyline.DataMiner.Library.Protocol.Matrix
{
	using System;

	/// <summary>
	/// Provides data for the event that is raised when the tooltip of a matrix crosspoint has changed.
	/// </summary>
	[Serializable]
	public class MatrixToolTipSetFromUIMessage
	{
		private readonly int _index;
		private readonly string _toolTip;

		internal MatrixToolTipSetFromUIMessage(int index, string toolTip)
		{
			_index = index;
			_toolTip = toolTip;
		}

		/// <summary>
		/// Gets the zero-based port number of the changed tooltip.
		/// </summary>
		/// <value>Zero-based port number.</value>
		public int Index
		{
			get { return _index; }
		}

		/// <summary>
		/// Gets a value indicating the tooltip text.
		/// </summary>
		public string ToolTip
		{
			get { return _toolTip; }
		}
	}
}
