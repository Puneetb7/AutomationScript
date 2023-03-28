namespace Skyline.DataMiner.Library.Protocol.Matrix
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;

	using Net.Messages;

	using Skyline.DataMiner.Scripting;

	/// <summary>
	/// Represents the state of a matrix port.
	/// </summary>
	[Skyline.DataMiner.Library.Common.Attributes.DllImport("SLManagedScripting.dll")]
	[Skyline.DataMiner.Library.Common.Attributes.DllImport("SLNetTypes.dll")]
	[Skyline.DataMiner.Library.Common.Attributes.DllImport("QActionHelperBaseClasses.dll")]
	internal class MatrixPortState
	{
		private readonly bool isTableCapable;
		private readonly bool isMatrixCapable;

		private readonly MatrixConnections connections;

		private readonly MatrixDisplayType detectedDisplayType;

		private readonly MatrixLabels inputLabels;
		private readonly MatrixLabels outputLabels;

		private readonly MatrixIOStates inputStates;
		private readonly MatrixIOStates outputStates;

		private readonly MatrixLocks inputLocks;
		private readonly MatrixLocks outputLocks;

		private readonly MatrixPages inputpages;
		private readonly MatrixPages outputpages;

		private readonly MatrixToolTips toolTips;

		private readonly MatrixSizeInfo maxSize;

		internal MatrixPortState(MatrixSizeInfo maxSize, ParameterInfo matrixReadParameterInfo, string connectionBuffer)
		{
			this.maxSize = maxSize;
			isTableCapable = false;
			isMatrixCapable = true;

			connections = new MatrixConnections(connectionBuffer);

			inputLabels = new MatrixLabels(this, MatrixIOType.Input, matrixReadParameterInfo);
			outputLabels = new MatrixLabels(this, MatrixIOType.Output, matrixReadParameterInfo);

			inputStates = new MatrixIOStates(this, MatrixIOType.Input, matrixReadParameterInfo);
			outputStates = new MatrixIOStates(this, MatrixIOType.Output, matrixReadParameterInfo);

			inputLocks = new MatrixLocks(this, MatrixIOType.Input, matrixReadParameterInfo);
			outputLocks = new MatrixLocks(this, MatrixIOType.Output, matrixReadParameterInfo);
		}

		internal MatrixPortState(SLProtocol protocol, MatrixSizeInfo maxSize, ParameterInfo matrixReadParameterInfo, MatrixCustomTableInfo inputTableInfo, MatrixCustomTableInfo outputTableInfo, string connectionBuffer, out MatrixSizeInfo maxFoundSize)
		{
			this.maxSize = maxSize;
			isTableCapable = true;
			isMatrixCapable = true;

			connections = new MatrixConnections(connectionBuffer);

			Dictionary<int, string> inputLabelTableCollection = new Dictionary<int, string>();
			Dictionary<int, bool> inputEnabledValuesTableCollection = new Dictionary<int, bool>();
			Dictionary<int, bool> inputLockedValuesTableCollection = new Dictionary<int, bool>();
			Dictionary<int, string> outputLabelTableCollection = new Dictionary<int, string>();
			Dictionary<int, bool> outputEnabledValuesTableCollection = new Dictionary<int, bool>();
			Dictionary<int, bool> outputLockedValuesTableCollection = new Dictionary<int, bool>();

			int maxFoundInput = -1;
			int maxFoundOutput = -1;

			if (GetColumnValues(protocol, inputTableInfo, inputLabelTableCollection, inputEnabledValuesTableCollection, inputLockedValuesTableCollection, maxSize.Inputs, out maxFoundInput) &&
				GetColumnValues(protocol, outputTableInfo, outputLabelTableCollection, outputEnabledValuesTableCollection, outputLockedValuesTableCollection, maxSize.Outputs, out maxFoundOutput))
			{
				bool isInputLabelMatches;
				bool isOutputLabelMatches;
				bool isInputEnabledMatches;
				bool isOutputEnabledMatches;
				bool isInputLockedMatches;
				bool isOutputLockedMatches;
				inputLabels = new MatrixLabels(this, MatrixIOType.Input, matrixReadParameterInfo, inputLabelTableCollection, out isInputLabelMatches);
				outputLabels = new MatrixLabels(this, MatrixIOType.Output, matrixReadParameterInfo, outputLabelTableCollection, out isOutputLabelMatches);

				inputStates = new MatrixIOStates(this, MatrixIOType.Input, matrixReadParameterInfo, inputEnabledValuesTableCollection, out isInputEnabledMatches);
				outputStates = new MatrixIOStates(this, MatrixIOType.Output, matrixReadParameterInfo, outputEnabledValuesTableCollection, out isOutputEnabledMatches);

				inputLocks = new MatrixLocks(this, MatrixIOType.Input, matrixReadParameterInfo, inputLockedValuesTableCollection, out isInputLockedMatches);
				outputLocks = new MatrixLocks(this, MatrixIOType.Output, matrixReadParameterInfo, outputLockedValuesTableCollection, out isOutputLockedMatches);

				bool isEnabledMatch = isInputEnabledMatches && isOutputEnabledMatches;
				bool isLabelMatch = isInputLabelMatches && isOutputLabelMatches;
				bool isLockMatch = isInputLockedMatches && isOutputLockedMatches;
				if (isEnabledMatch && isLabelMatch && isLockMatch)
				{
					detectedDisplayType = MatrixDisplayType.MatrixAndTables;    // data matches in matrix and tables
				}
				else
				{
					detectedDisplayType = MatrixDisplayType.Tables; // tables have different data compared with matrix, taking tables data as source
				}
			}
			else
			{
				detectedDisplayType = MatrixDisplayType.Matrix; // no rows in tables, data of matrix is being used
				inputLabels = new MatrixLabels(this, MatrixIOType.Input, matrixReadParameterInfo);
				outputLabels = new MatrixLabels(this, MatrixIOType.Output, matrixReadParameterInfo);

				inputStates = new MatrixIOStates(this, MatrixIOType.Input, matrixReadParameterInfo);
				outputStates = new MatrixIOStates(this, MatrixIOType.Output, matrixReadParameterInfo);

				inputLocks = new MatrixLocks(this, MatrixIOType.Input, matrixReadParameterInfo);
				outputLocks = new MatrixLocks(this, MatrixIOType.Output, matrixReadParameterInfo);
			}

			maxFoundSize = new MatrixSizeInfo(maxFoundInput, maxFoundOutput);
		}

		internal MatrixPortState(SLProtocol protocol, MatrixSizeInfo maxSize, MatrixCustomTableInfo inputTableInfo, MatrixCustomTableInfo outputTableInfo, string connectionBuffer, out MatrixSizeInfo maxFoundSize)
		{
			this.maxSize = maxSize;
			isTableCapable = true;
			isMatrixCapable = false;

			connections = new MatrixConnections(connectionBuffer);

			Dictionary<int, string> inputLabelCollection = new Dictionary<int, string>();
			Dictionary<int, bool> inputEnabledValuesCollection = new Dictionary<int, bool>();
			Dictionary<int, bool> inputLockedValuesCollection = new Dictionary<int, bool>();
			Dictionary<int, string> inputPageValuesCollection = new Dictionary<int, string>();
			Dictionary<int, string> outputLabelCollection = new Dictionary<int, string>();
			Dictionary<int, bool> outputEnabledValuesCollection = new Dictionary<int, bool>();
			Dictionary<int, bool> outputLockedValuesCollection = new Dictionary<int, bool>();
			Dictionary<int, string> outputPageValuesCollection = new Dictionary<int, string>();
			Dictionary<int, string> outputToolTipValuesCollection = new Dictionary<int, string>();
			int maxFoundInput = -1;
			int maxFoundOutput = -1;
			GetOrInitTableValues(protocol, inputTableInfo, inputLabelCollection, inputEnabledValuesCollection, inputLockedValuesCollection, out maxFoundInput, pageValues: inputPageValuesCollection);
			GetOrInitTableValues(protocol, outputTableInfo, outputLabelCollection, outputEnabledValuesCollection, outputLockedValuesCollection, out maxFoundOutput, toolTipValues: outputToolTipValuesCollection, pageValues: outputPageValuesCollection);

			maxFoundSize = new MatrixSizeInfo(maxFoundInput, maxFoundOutput, maxSize.IsDynamic);

			if (maxSize.IsDynamic)
			{
				this.maxSize = maxFoundSize;
			}

			inputLabels = new MatrixLabels(this, MatrixIOType.Input, inputLabelCollection);
			outputLabels = new MatrixLabels(this, MatrixIOType.Output, outputLabelCollection);

			inputStates = new MatrixIOStates(this, MatrixIOType.Input, inputEnabledValuesCollection);
			outputStates = new MatrixIOStates(this, MatrixIOType.Output, outputEnabledValuesCollection);

			inputLocks = new MatrixLocks(this, MatrixIOType.Input, inputLockedValuesCollection);
			outputLocks = new MatrixLocks(this, MatrixIOType.Output, outputLockedValuesCollection);

			inputpages = new MatrixPages(this, MatrixIOType.Input, inputPageValuesCollection);
			outputpages = new MatrixPages(this, MatrixIOType.Input, outputPageValuesCollection);

			toolTips = new MatrixToolTips(this, outputToolTipValuesCollection);
		}

		internal MatrixConnections Connections
		{
			get { return connections; }
		}

		internal MatrixDisplayType DetectedDisplayType
		{
			get { return detectedDisplayType; }
		}

		internal MatrixLabels InputLabels
		{
			get { return inputLabels; }
		}

		internal MatrixLocks InputLocks
		{
			get { return inputLocks; }
		}

		internal MatrixIOStates InputStates
		{
			get { return inputStates; }
		}

		internal int MaxInputs
		{
			get { return maxSize.Inputs; }
		}

		internal int MaxOutputs
		{
			get { return maxSize.Outputs; }
		}

		internal bool IsDynamicSize
		{
			get { return maxSize.IsDynamic; }
		}

		internal bool IsTableCapable
		{
			get { return isTableCapable; }
		}

		internal bool IsMatrixCapable
		{
			get { return isMatrixCapable; }
		}

		internal MatrixLabels OutputLabels
		{
			get { return outputLabels; }
		}

		internal MatrixLocks OutputLocks
		{
			get { return outputLocks; }
		}

		internal MatrixIOStates OutputStates
		{
			get { return outputStates; }
		}

		internal MatrixToolTips ToolTips
		{
			get { return toolTips; }
		}

		internal MatrixPages InputPages
		{
			get { return inputpages; }
		}

		internal MatrixPages OutputPages
		{
			get { return outputpages; }
		}

		private static void GetOrInitTableValues(SLProtocol protocol, MatrixCustomTableInfo matrixTableInfo, IDictionary<int, string> labels, IDictionary<int, bool> enabledValues, IDictionary<int, bool> lockedValues, out int maxFoundKey, IDictionary<int, string> toolTipValues = null, IDictionary<int, string> pageValues = null)
		{
			if (!GetColumnValues(protocol, matrixTableInfo, labels, enabledValues, lockedValues, matrixTableInfo.MaxCount, out maxFoundKey, toolTipValues: toolTipValues, pageValues: pageValues))
			{
				bool hasToolTip = toolTipValues != null && matrixTableInfo.ToolTipColumnIdx > 0;
				bool hasPage = pageValues != null && matrixTableInfo.PageColumnIdx > 0;

				object[] indexCol = new object[matrixTableInfo.MaxCount];
				object[] labelCol = new object[matrixTableInfo.MaxCount];
				object[] enabledCol = new object[matrixTableInfo.MaxCount];
				object[] lockedCol = new object[matrixTableInfo.MaxCount];
				object[] toolTipCol = new object[matrixTableInfo.MaxCount];
				object[] pageCol = new object[matrixTableInfo.MaxCount];

				for (int i = 0; i < matrixTableInfo.MaxCount; i++)
				{
					string key = Convert.ToString(i + 1, CultureInfo.InvariantCulture);
					string label = (matrixTableInfo.IsInput ? "Input " : "Output ") + key;
					labels[i] = label;
					enabledValues[i] = true;
					lockedValues[i] = false;

					if (hasToolTip)
					{
						toolTipValues[i] = String.Empty;
					}

					if (hasPage)
					{
						pageValues[i] = String.Empty;
					}

					indexCol[i] = key;
					labelCol[i] = label;
					enabledCol[i] = "1";
					lockedCol[i] = "0";

					if (hasToolTip)
					{
						toolTipCol[i] = String.Empty;
					}

					if (hasPage)
					{
						pageCol[i] = String.Empty;
					}
				}

				maxFoundKey = matrixTableInfo.MaxCount - 1; // why -1 ? 
				List<object> setTable = new List<object>(6);
				setTable.Add(indexCol);
				setTable.Add(labelCol);
				setTable.Add(enabledCol);
				setTable.Add(lockedCol);

				if (hasToolTip)
				{
					setTable.Add(toolTipCol);
				}

				if (hasPage)
				{
					setTable.Add(pageCol);
				}


				List<object> parameterIds = new List<object>(6);
				parameterIds.Add(matrixTableInfo.TableParameterId);
				parameterIds.Add(matrixTableInfo.LabelParameterId);
				parameterIds.Add(matrixTableInfo.EnabledParameterId);
				parameterIds.Add(matrixTableInfo.LockedParameterId);

				if (hasToolTip)
				{
					parameterIds.Add(matrixTableInfo.ToolTipParameterId);
				}

				if (hasPage)
				{
					parameterIds.Add(matrixTableInfo.PageParameterId);
				}

				protocol.NotifyProtocol((int)NotifyType.NT_FILL_ARRAY_WITH_COLUMN, parameterIds.ToArray(), setTable.ToArray());
			}
		}

		private static void AddMissingKeys(SLProtocol protocol, MatrixCustomTableInfo matrixTableInfo, IDictionary<int, string> labels, IDictionary<int, bool> enabledValues, IDictionary<int, bool> lockedValues, int maxKey, IDictionary<int, string> toolTipValues = null, IDictionary<int, string> pageValues = null)
		{
			HashSet<int> missingKeys = new HashSet<int>();
			for (int i = 0; i < maxKey; i++)
			{
				if (!labels.ContainsKey(i))
				{
					missingKeys.Add(i);
				}
			}

			if (missingKeys.Count == 0)
			{
				return;
			}

			bool hasToolTip = toolTipValues != null && matrixTableInfo.ToolTipColumnIdx > 0;
			bool hasPage = pageValues != null && matrixTableInfo.PageColumnIdx > 0;

			// there seem to be gaps in the table keys, filling up with default values
			object[] setIndexCol = new object[missingKeys.Count];
			object[] setLabelCol = new object[missingKeys.Count];
			object[] setEnabledCol = new object[missingKeys.Count];
			object[] setLockedCol = new object[missingKeys.Count];
			object[] setToolTipCol = new object[missingKeys.Count];
			object[] setPageCol = new object[missingKeys.Count];

			int count = 0;
			foreach (int portNumber in missingKeys)
			{
				string key = Convert.ToString(portNumber + 1, CultureInfo.InvariantCulture);
				string sLabel = (matrixTableInfo.IsInput ? "Input " : "Output ") + key;
				labels[portNumber] = sLabel;
				enabledValues[portNumber] = true;
				lockedValues[portNumber] = false;

				if (hasToolTip)
				{
					toolTipValues[portNumber] = string.Empty;
				}

				if (hasPage)
				{
					pageValues[portNumber] = string.Empty;
				}

				setIndexCol[count] = key;
				setLabelCol[count] = sLabel;
				setEnabledCol[count] = "1";
				setLockedCol[count] = "0";

				if (hasToolTip)
				{
					setToolTipCol[count] = string.Empty;
				}

				if (hasPage)
				{
					setPageCol[count] = string.Empty;
				}
				count++;
			}

			List<object> setTable = new List<object>(6);
			setTable.Add(setIndexCol);
			setTable.Add(setLabelCol);
			setTable.Add(setEnabledCol);
			setTable.Add(setLockedCol);

			if (hasToolTip)
			{
				setTable.Add(setToolTipCol);
			}

			if (hasPage)
			{
				setTable.Add(setPageCol);
			}

			List<object> parameterIds = new List<object>(6);
			parameterIds.Add(matrixTableInfo.TableParameterId);
			parameterIds.Add(matrixTableInfo.LabelParameterId);
			parameterIds.Add(matrixTableInfo.EnabledParameterId);
			parameterIds.Add(matrixTableInfo.LockedParameterId);

			if (hasToolTip)
			{
				parameterIds.Add(matrixTableInfo.ToolTipParameterId);
			}

			if (hasPage)
			{
				parameterIds.Add(matrixTableInfo.PageParameterId);
			}

			protocol.NotifyProtocol((int)NotifyType.NT_FILL_ARRAY_WITH_COLUMN, parameterIds.ToArray(), setTable.ToArray());
			;
		}

		private static bool GetColumnValues(SLProtocol protocol, MatrixCustomTableInfo matrixTableInfo, IDictionary<int, string> labels, IDictionary<int, bool> enabledValues, IDictionary<int, bool> lockedValues, int maxAllowedSize, out int maxKey, IDictionary<int, string> toolTipValues = null, IDictionary<int, string> pageValues = null)
		{
			bool hasValues = false;
			bool hasToolTip = toolTipValues != null && matrixTableInfo.ToolTipColumnIdx > 0;
			bool hasPage = pageValues != null && matrixTableInfo.PageColumnIdx > 0;
			maxKey = -1;
			List<uint> paramIdx = new List<uint>(6);
			paramIdx.Add(0);
			paramIdx.Add(matrixTableInfo.LabelColumnIdx);
			paramIdx.Add(matrixTableInfo.EnabledColumnIdx);
			paramIdx.Add(matrixTableInfo.LockedColumnIdx);

			if (hasToolTip)
			{
				paramIdx.Add(matrixTableInfo.ToolTipColumnIdx);
			}

			if (hasPage)
			{
				paramIdx.Add(matrixTableInfo.PageColumnIdx);
			}

			object[] tableCols = (object[])protocol.NotifyProtocol((int)NotifyType.NT_GET_TABLE_COLUMNS, matrixTableInfo.TableParameterId, paramIdx.ToArray());

			if (!CheckValidTable(tableCols, paramIdx.Count))
			{
				return hasValues;
			}

			int index = 0;
			object[] indexCol = (object[])tableCols[index++];
			object[] labelCol = (object[])tableCols[index++];
			object[] enabledCol = (object[])tableCols[index++];
			object[] lockedCol = (object[])tableCols[index++];
			object[] toolTipCol = null;
			object[] pageCol = null;

			if (hasToolTip)
			{
				toolTipCol = (object[])tableCols[index++];
			}

			if (hasPage)
			{
				pageCol = (object[])tableCols[index++];
			}

			HashSet<string> rowsToBeDeleted = new HashSet<string>();
			for (int i = 0; i < indexCol.Length; i++)
			{
				string key = Convert.ToString(indexCol[i], CultureInfo.InvariantCulture);
				int portNumber;
				if (Int32.TryParse(key, out portNumber))
				{
					hasValues = true;
					portNumber--;
					if (portNumber >= maxAllowedSize && maxAllowedSize != -1)
					{
						rowsToBeDeleted.Add(key);
						continue;
					}

					labels[portNumber] = Convert.ToString(labelCol[i], CultureInfo.InvariantCulture);
					enabledValues[portNumber] = Convert.ToString(enabledCol[i], CultureInfo.InvariantCulture) != "0";
					lockedValues[portNumber] = Convert.ToString(lockedCol[i], CultureInfo.InvariantCulture) != "0";

					if (hasToolTip)
					{
						toolTipValues[portNumber] = Convert.ToString(toolTipCol[i], CultureInfo.InvariantCulture);
					}

					if (hasPage)
					{
						pageValues[portNumber] = Convert.ToString(pageCol[i], CultureInfo.InvariantCulture);
					}

					if (portNumber > maxKey)
					{
						maxKey = portNumber;
					}
				}
				else
				{
					if (!String.IsNullOrEmpty(key))
					{
						rowsToBeDeleted.Add(key);
					}
				}
			}

			AddMissingKeys(protocol, matrixTableInfo, labels, enabledValues, lockedValues, maxKey, toolTipValues, pageValues);
			MatrixHelper.DeleteRows(protocol, matrixTableInfo.TableParameterId, rowsToBeDeleted);
			return hasValues;
		}

		private static bool CheckValidColumns(object[] columns)
		{
			for (int i = 0; i < columns.Length; i++)
			{
				if (columns[i] == null)
				{
					return false;
				}

				if (i == 0)
				{
					continue;
				}

				object[] previousCol = (object[])columns[i - 1];
				object[] currentCol = (object[])columns[i];
				if (previousCol.Length != currentCol.Length)
				{
					return false;
				}
			}

			return true;
		}

		private static bool CheckValidTable(object[] columns, int expectedSize)
		{
			if (columns == null || columns.Length < expectedSize)
			{
				return false;
			}

			return CheckValidColumns(columns);
		}
	}
}
