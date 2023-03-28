namespace Skyline.DataMiner.Library.Protocol.Matrix
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using global::Skyline.DataMiner.Library.Protocol.Matrix;
	using global::Skyline.DataMiner.Net.Messages;
	using global::Skyline.DataMiner.Scripting;

	public class TableMatrixHelper : MatrixHelper
	{
		public TableMatrixHelper(SLProtocol protocol, int matrixDummyParameterId, int matrixTablesVirtualSetsParameterId = -2, int matrixTablesSerializedSetsParameterId = -2, int matrixConnectionsBufferParameterId = -2, int matrixSerializedParameterId = -2, int maxInputCount = 0, int maxOutputCount = 0) : this(protocol, CreateMatrixHelperParamerIds(protocol, matrixDummyParameterId, matrixTablesVirtualSetsParameterId, matrixTablesSerializedSetsParameterId, matrixConnectionsBufferParameterId, matrixSerializedParameterId), maxInputCount, maxOutputCount)
		{

		}

		internal TableMatrixHelper(SLProtocol protocol, MatrixHelperParameterIds parameterIds, int maxInputCount, int maxOutputCount) : base(
		protocol: protocol,
		maxInputCount: maxInputCount,
		maxOutputCount: maxOutputCount,
		matrixHelperParameters: parameterIds)
		{
			SetMatrixOptions(protocol, parameterIds.MatrixDummyReadParameterId);
		}

		#region Static Private Methods

		private void SetMatrixOptions(SLProtocol protocol, int matrixReadParameterId)
		{
			// retreiving the protocol info
			GetElementProtocolResponseMessage protocolInfo = GetProtocolInfo(protocol, protocol.DataMinerID, protocol.ElementID);

			//parsing the read param info in order to get mappings and options
			if (protocolInfo != null && matrixReadParameterId != -1)
			{
				var dummyParameterInfo = protocolInfo.FindParameter(matrixReadParameterId); // finding the matrix read param info

				if (dummyParameterInfo != null && dummyParameterInfo.IsTableMatrix)
				{
					//check for matrix options 
					if (dummyParameterInfo.TableMatrixOptions != null && dummyParameterInfo.TableMatrixOptions.Any())
					{
						foreach (MatrixOption option in dummyParameterInfo.TableMatrixOptions)
						{
							if (option != null && option.Type != null && option.Type.Equals(MatrixHelperParameterNames.MATRIX_OPTION_TYPE_VALUE, StringComparison.InvariantCultureIgnoreCase) &&
								option.Name != null && option.Value != null)
							{
								//Limits
								if (option.Name.Equals(MatrixHelperParameterNames.MATRIX_OPTION_NAME_MINIMUM_CONNECTED_INPUTS_PER_OUTPUT, StringComparison.InvariantCultureIgnoreCase) &&
									!String.IsNullOrEmpty(option.Value) && Int32.TryParse(option.Value, out int minInputsPerOutput))
								{
									MinConnectedInputsPerOutput = minInputsPerOutput;
								}

								if (option.Name.Equals(MatrixHelperParameterNames.MATRIX_OPTION_NAME_MAXIMUM_CONNECTED_INPUTS_PER_OUTPUT, StringComparison.InvariantCultureIgnoreCase) &&
									!String.IsNullOrEmpty(option.Value) && Int32.TryParse(option.Value, out int maxInputsPerOutput))
								{
									MaxConnectedInputsPerOutput = 1; // always 1 due to the nature of a table matrix
								}

								if (option.Name.Equals(MatrixHelperParameterNames.MATRIX_OPTION_NAME_MINIMUM_CONNECTED_OUTPUTS_PER_INPUT, StringComparison.InvariantCultureIgnoreCase) &&
									!String.IsNullOrEmpty(option.Value) && Int32.TryParse(option.Value, out int minOutputsPerInput))
								{
									MinConnectedOutputsPerInput = minOutputsPerInput;
								}

								if (option.Name.Equals(MatrixHelperParameterNames.MATRIX_OPTION_NAME_MAXIMUM_CONNECTED_OUTPUTS_PER_INPUT, StringComparison.InvariantCultureIgnoreCase) &&
								   !String.IsNullOrEmpty(option.Value))
								{
									if (option.Value.Equals(MatrixHelperParameterNames.MATRIX_OPTION_VALUE_AUTO, StringComparison.InvariantCultureIgnoreCase))
									{
										// auto option means -1 internally
										MaxConnectedOutputsPerInput = -2;
									}
									else if (Int32.TryParse(option.Value, out int maxOutputsPerInput))
									{
										MaxConnectedOutputsPerInput = maxOutputsPerInput;
									}
								}
							}
						}
					}
				}
			}
		}

		private static GetElementProtocolResponseMessage GetProtocolInfo(SLProtocol protocol, int agentId, int elementId)
		{
			GetElementProtocolMessage message = new GetElementProtocolMessage(agentId, elementId);
			var responseMessage = protocol.SLNet.SendSingleResponseMessage(message);
			if (responseMessage != null)
			{
				return responseMessage as GetElementProtocolResponseMessage;
			}
			return null;
		}

		private static MatrixHelperParameterIds CreateMatrixHelperParamerIds(SLProtocol protocol, int matrixReadParameterId, int matrixTablesVirtualSetsParameterId, int matrixTablesSerializedSetsParameterId, int matrixConnectionsBufferParameterId, int matrixSerializedParameterId)
		{
			// retreiving the protocol info
			GetElementProtocolResponseMessage protocolInfo = GetProtocolInfo(protocol, protocol.DataMinerID, protocol.ElementID);


			//parsing the read param info in order to get mappings and options
			if (protocolInfo != null && matrixReadParameterId != -1)
			{
				var matrixReadParameterInfo = protocolInfo.FindParameter(matrixReadParameterId); // finding the matrix read param info

				if (matrixReadParameterInfo != null && matrixReadParameterInfo.IsTableMatrix)
				{
					//retrieve the param info for both tables
					var _inputsTableParameterInfo = protocolInfo.FindParameter(matrixReadParameterInfo.MatrixInputsTablePid); // inputs table
					var _outputsTableParameterInfo = protocolInfo.FindParameter(matrixReadParameterInfo.MatrixOutputsTablePid); // outputs table

					// resolve the mappings 
					if (_inputsTableParameterInfo != null && _outputsTableParameterInfo != null)
					{
						//input mappings
						var inputMappings = MapColumnsToInternalId(protocolInfo, _inputsTableParameterInfo, matrixReadParameterInfo.MatrixInputsTableMappings);

						//output mappings
						var outputMappings = MapColumnsToInternalId(protocolInfo, _outputsTableParameterInfo, matrixReadParameterInfo.MatrixOutputsTableMappings);


						return new MatrixHelperParameterIds(
						matrixConnectionsBufferParameterId: matrixConnectionsBufferParameterId,
						matrixReadParameterId: -1,
						discreetInfoParameterId: -1,
						inputsTableParameterId: _inputsTableParameterInfo.ID,
						outputsTableParameterId: _outputsTableParameterInfo.ID,
						matrixSerializedParameterId: matrixSerializedParameterId,
						matrixDummyReadParameterId: matrixReadParameterId,
						intputMappings: inputMappings,
						outputMappings: outputMappings)
						{
							TableVirtualSetParameterId = matrixTablesVirtualSetsParameterId,
							TableSerializedParameterId = matrixTablesSerializedSetsParameterId
						};
					}
				}
			}

			return new MatrixHelperParameterIds();
		}

		private static List<MatrixMap> MapColumnsToInternalId(GetElementProtocolResponseMessage protocolInfo, ParameterInfo tableParameterInfo, List<MatrixMapping> mappings)
		{
			List<MatrixMap> internalIndexColumns = new List<MatrixMap>();

			// raw columns
			var rawColumns = ParseRawColumns(tableParameterInfo);

			if (protocolInfo != null && rawColumns != null)
			{
				//mappings
				foreach (MatrixMapping mapping in mappings)
				{
					if (mapping != null && mapping.Type != null && mapping.Type.Equals(MatrixHelperParameterNames.MATRIX_MAPPING_TYPE_PID, StringComparison.InvariantCultureIgnoreCase))
					{
						if (!String.IsNullOrEmpty(mapping.Name))
						{
							int indexColumnPid;

							if (Int32.TryParse(mapping.Value, out indexColumnPid) && rawColumns.ContainsKey(indexColumnPid))
							{
								var internalIndex = rawColumns[indexColumnPid];

								if (!internalIndexColumns.Any(item => item.Name == mapping.Name))
								{
									var param = protocolInfo.FindParameter(indexColumnPid);
									var writeparam = param != null ? protocolInfo.FindAssociatedWriteParameter(param) : null;
									int writeparamId = writeparam != null ? writeparam.ID : -1;
									internalIndexColumns.Add(new MatrixMap(mapping.Name, internalIndex, indexColumnPid, writeparamId));
								}
							}
						}
					}
				}
			}

			return internalIndexColumns;
		}


		private static Dictionary<int, int> ParseRawColumns(ParameterInfo tableParameterInfo)
		{
			// parse raw columns
			Dictionary<int, int> rawColumns = new Dictionary<int, int>();
			if (tableParameterInfo != null && tableParameterInfo.TableColumnDefinitions != null && tableParameterInfo.TableColumnDefinitions.Any() == true)
			{
				for (int i = 0; i < tableParameterInfo.TableColumnDefinitions.Length; i++)
				{
					int columnPID = tableParameterInfo.TableColumnDefinitions[i].ParameterID;

					if (!rawColumns.ContainsKey(columnPID))
						rawColumns.Add(columnPID, i);
				}
			}

			return rawColumns;
		}
		#endregion
	}
}
