namespace Skyline.DataMiner.Library.Protocol.Matrix
{
	using System;


	public class MatrixPageSetFromUIMessage
	{
		private readonly int index;
		private readonly MatrixIOType type;
		private readonly string page;

		internal MatrixPageSetFromUIMessage(int index, MatrixIOType type, string page)
		{
			this.index = index;
			this.type = type;
			this.page = page;
		}

		/// <summary>
		/// Gets the zero-based port number of the changed page.
		/// </summary>
		/// <value>Zero-based port number.</value>
		public int Index
		{
			get { return index; }
		}

		/// <summary>
		/// Gets the <see cref="MatrixIOType"/> to determine if the changed page is on an output or input port.
		/// </summary>
		/// <value><see cref="MatrixIOType"/> that determines if it is an output or input port.</value>
		public MatrixIOType Type
		{
			get { return type; }
		}

		/// <summary>
		/// Gets the changed page of this port.
		/// </summary>
		/// <value>The changed page of this port.</value>
		public string Page
		{
			get { return page; }
		}
	}
}
