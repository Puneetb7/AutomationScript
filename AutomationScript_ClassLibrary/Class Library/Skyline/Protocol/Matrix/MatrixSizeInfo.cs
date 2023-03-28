namespace Skyline.DataMiner.Library.Protocol.Matrix
{
	internal class MatrixSizeInfo
	{
		private readonly int inputs;
		private readonly int outputs;
		private readonly bool isDynamic = false;
		internal MatrixSizeInfo(int inputs, int outputs, bool isDynamic = false)
		{
			this.inputs = inputs;
			this.outputs = outputs;
			this.isDynamic = isDynamic;
		}

		internal int Inputs
		{
			get
			{
				return inputs;
			}
		}

		internal int Outputs
		{
			get
			{
				return outputs;
			}
		}

		internal bool IsDynamic
		{
			get
			{
				return isDynamic;
			}
		}
	}
}
