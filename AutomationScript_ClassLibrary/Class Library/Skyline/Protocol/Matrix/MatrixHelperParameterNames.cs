namespace Skyline.DataMiner.Library.Protocol.Matrix
{
    using System.Collections.Generic;
    using System.Globalization;
    using Net.Messages;

	[Skyline.DataMiner.Library.Common.Attributes.DllImport("SLManagedScripting.dll")]
	[Skyline.DataMiner.Library.Common.Attributes.DllImport("SLNetTypes.dll")]
	[Skyline.DataMiner.Library.Common.Attributes.DllImport("QActionHelperBaseClasses.dll")]
	internal class MatrixHelperParameterNames
	{
		//mappings
		internal static string MATRIX_MAPPING_TYPE_PID = "pid"; //type
		internal static string MATRIX_MAPPING_NAME_INDEX = "index";
		internal static string MATRIX_MAPPING_NAME_LABEL = "label";
		internal static string MATRIX_MAPPING_NAME_STATE = "state";
		internal static string MATRIX_MAPPING_NAME_LOCK = "lock";
		internal static string MATRIX_MAPPING_NAME_PAGE = "page";
		internal static string MATRIX_MAPPING_NAME_CONNECTED_INPUT = "connectedInput";
		internal static string MATRIX_MAPPING_NAME_TOOLTIP = "tooltip";
		internal static string MATRIX_MAPPING_NAME_LOCK_OVERRIDE = "lockOverride";

		//options
		internal static string MATRIX_OPTION_TYPE_VALUE = "value"; //type
		internal static string MATRIX_OPTION_VALUE_AUTO = "auto"; //value
		internal static string MATRIX_OPTION_NAME_LAYOUT = "matrixLayout";
		internal static string MATRIX_OPTION_NAME_PAGES = "pages";
		internal static string MATRIX_OPTION_NAME_SMALL_PAGES = "evenSmallPages";
		internal static string MATRIX_OPTION_NAME_MINIMUM_CONNECTED_INPUTS_PER_OUTPUT = "minimumConnectedInputsPerOutput";
		internal static string MATRIX_OPTION_NAME_MAXIMUM_CONNECTED_INPUTS_PER_OUTPUT = "maximumConnectedInputsPerOutput";
		internal static string MATRIX_OPTION_NAME_MINIMUM_CONNECTED_OUTPUTS_PER_INPUT = "minimumConnectedOutputsPerInput";
		internal static string MATRIX_OPTION_NAME_MAXIMUM_CONNECTED_OUTPUTS_PER_INPUT = "maximumConnectedOutputsPerInput";

		internal string MatrixParameterName { get; set; }

		internal string InputsLabelParameterName { get; set; }

		internal string InputsIsEnabledParameterName { get; set; }

		internal string InputsIsLockedParameterName { get; set; }

		internal string InputsPageParameterName { get; set; }

		internal string OutputsLabelParameterName { get; set; }

		internal string OutputsIsEnabledParameterName { get; set; }

		internal string OutputsIsLockedParameterName { get; set; }

		internal string OutputsConnectedInputParameterName { get; set; }

		internal string OutputPageParameterName { get; set; }

		internal string OutputsToolTipParameterName { get; set; }

		internal string OutputsLockOverrideParameterName { get; set; }

		internal string OutputsTableVirtualSetParameterName { get; set; }

		internal string OutputsTableSerializedWritesParameterName { get; set; }

		internal void SetInputNames(ParameterInfo parameter, IDictionary<int, uint> inputColumns)
		{
			string tableName = parameter.Name.ToLower(CultureInfo.InvariantCulture);
			InputsLabelParameterName = tableName + "label";
			InputsIsEnabledParameterName = tableName + "isenabled";
			InputsIsLockedParameterName = tableName + "islocked";
			uint idx = 0;
			foreach (TableColumnDefinition tableColumnDefinition in parameter.TableColumnDefinitions)
			{
				inputColumns[tableColumnDefinition.ParameterID] = idx++;
			}
		}

		internal void SetOutputNames(ParameterInfo parameter, IDictionary<int, uint> outputColumns)
		{
			string tableName = parameter.Name.ToLower(CultureInfo.InvariantCulture);
			OutputsLabelParameterName = tableName + "label";
			OutputsIsEnabledParameterName = tableName + "isenabled";
			OutputsIsLockedParameterName = tableName + "islocked";
			OutputsConnectedInputParameterName = tableName + "connectedinput";
			OutputsTableVirtualSetParameterName = tableName + "virtualsets";
			OutputsTableSerializedWritesParameterName = tableName + "serializedsets";
			uint idx = 0;
			foreach (TableColumnDefinition tableColumnDefinition in parameter.TableColumnDefinitions)
			{
				outputColumns[tableColumnDefinition.ParameterID] = idx++;
			}
		}
	}
}
