namespace Skyline.DataMiner.Library.Protocol.Matrix
{
	internal class MatrixCustomTableInfo
	{
		private readonly int tableParameterId;
		private readonly MatrixCustomTableInfoItem labelId;
		private readonly MatrixCustomTableInfoItem enabledId;
		private readonly MatrixCustomTableInfoItem lockedId;
		private readonly MatrixCustomTableInfoItem connectedId;
		private readonly MatrixCustomTableInfoItem pageId; // optional
		private readonly MatrixCustomTableInfoItem toolTipId; // optional
		private readonly MatrixCustomTableInfoItem lockOverrideId; // optional
		private readonly int maxCount;
		private readonly bool isInput;

		internal MatrixCustomTableInfo(int tableParameterId, MatrixCustomTableInfoItem labelId, MatrixCustomTableInfoItem enabledId, MatrixCustomTableInfoItem lockedId, MatrixCustomTableInfoItem connectedId, int maxCount, bool isInput, MatrixCustomTableInfoItem pageId = null, MatrixCustomTableInfoItem toolTipId = null, MatrixCustomTableInfoItem lockOverrideId = null)
		{
			this.tableParameterId = tableParameterId;
			this.labelId = labelId;
			this.enabledId = enabledId;
			this.lockedId = lockedId;
			this.connectedId = connectedId;
			this.maxCount = maxCount;
			this.isInput = isInput;
			this.pageId = pageId;
			this.toolTipId = toolTipId;
			this.lockOverrideId = lockOverrideId;
		}

		internal int TableParameterId
		{
			get
			{
				return tableParameterId;
			}
		}

		internal uint LabelColumnIdx
		{
			get
			{
				return labelId.ColumnIdx;
			}
		}

		internal int LabelParameterId
		{
			get
			{
				return labelId.ParameterId;
			}
		}

		internal int LabelWriteParameterId
		{
			get
			{
				return labelId.WriteParameterId;
			}
		}

		internal uint EnabledColumnIdx
		{
			get
			{
				return enabledId.ColumnIdx;
			}
		}

		internal int EnabledParameterId
		{
			get
			{
				return enabledId.ParameterId;
			}
		}

		internal int EnabledWriteParameterId
		{
			get
			{
				return enabledId.WriteParameterId;
			}
		}

		internal uint LockedColumnIdx
		{
			get
			{
				return lockedId.ColumnIdx;
			}
		}

		internal int LockedParameterId
		{
			get
			{
				return lockedId.ParameterId;
			}
		}

		internal int LockedWriteParameterId
		{
			get
			{
				return lockedId.WriteParameterId;
			}
		}

		internal uint ConnectedColumnIdx
		{
			get
			{
				return connectedId.ColumnIdx;
			}
		}

		internal int ConnectedParameterId
		{
			get
			{
				return connectedId.ParameterId;
			}
		}

		internal int ConnectedWriteParameterId
		{
			get
			{
				return connectedId.WriteParameterId;
			}
		}

		internal uint LockOverrideColumnIdx
		{
			get
			{
				if (lockOverrideId != null)
				{
					return lockOverrideId.ColumnIdx;
				}
				else
				{
					return 0;
				}
			}
		}

		internal int LockOverrideParameterId
		{
			get
			{
				if (lockOverrideId != null)
				{
					return lockOverrideId.ParameterId;
				}
				else
				{
					return -1;
				}
			}
		}

		internal int LockOverrideWriteParameterId
		{
			get
			{
				if (lockOverrideId != null)
				{
					return lockOverrideId.WriteParameterId;
				}
				else
				{
					return -1;
				}
			}
		}

		internal uint PageColumnIdx
		{
			get
			{
				if (pageId != null)
				{
					return pageId.ColumnIdx;
				}
				else
				{
					return 0;
				}
			}
		}

		internal int PageParameterId
		{
			get
			{
				if (pageId != null)
				{
					return pageId.ParameterId;
				}
				else
				{
					return -1;
				}
			}
		}

		internal int PageWriteParameterId
		{
			get
			{
				if (pageId != null)
				{
					return pageId.WriteParameterId;
				}
				else
				{
					return -1;
				}
			}
		}

		internal uint ToolTipColumnIdx
		{
			get
			{
				if (toolTipId != null)
				{
					return toolTipId.ColumnIdx;
				}
				else
				{
					return 0;
				}
			}
		}

		internal int ToolTipParameterId
		{
			get
			{
				if (toolTipId != null)
				{
					return toolTipId.ParameterId;
				}
				else
				{
					return -1;
				}
			}
		}

		internal int ToolTipWriteParameterId
		{
			get
			{
				if (toolTipId != null)
				{
					return toolTipId.WriteParameterId;
				}
				else
				{
					return -1;
				}
			}
		}

		internal int MaxCount
		{
			get
			{
				return maxCount;
			}
		}

		internal bool IsInput
		{
			get
			{
				return isInput;
			}
		}
	}
}
