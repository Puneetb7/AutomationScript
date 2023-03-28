namespace Skyline.DataMiner.Library.Protocol.Matrix
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;

	using Net.Messages;

	/// <summary>
	/// Represents the matrix tooltips.
	/// </summary>
	[Skyline.DataMiner.Library.Common.Attributes.DllImport("SLNetTypes.dll")]

	internal class MatrixToolTips : MatrixItems<string>
	{

		internal MatrixToolTips(MatrixPortState portState, IDictionary<int, string> toolTipTableValues) : base(portState, MatrixIOType.Output)
		{
			foreach (KeyValuePair<int, string> kvp in toolTipTableValues)
			{
				OriginalItems[kvp.Key] = kvp.Value;
			}
		}
		/// <summary>
		/// Gets or sets the tooltip of a output (crosspoint).
		/// </summary>
		/// <param name="number">0-based output number.</param>
		/// <returns>The corresponding tooltip.</returns>
		internal string this[int number]
		{
			get
			{
				string tooltip;

				if (UpdatedItems.TryGetValue(number, out tooltip) || OriginalItems.TryGetValue(number, out tooltip))
				{
					return tooltip;
				}
				else
				{
					return string.Empty;
				}
			}

			set
			{
				if (String.IsNullOrEmpty(value) || (number < 0) || (number >= MaxItems))
				{
					return;
				}

				string tooltip;

				if (UpdatedItems.TryGetValue(number, out tooltip))
				{
					if (tooltip != value)
					{
						if (OriginalItems.TryGetValue(number, out tooltip) && tooltip == value)
						{
							UpdatedItems.Remove(number); // If value is again the original value, the set should not be performed.
						}
						else
						{
							UpdatedItems[number] = value;
						}
					}
				}
				else
				{
					if (!OriginalItems.TryGetValue(number, out tooltip) || tooltip != value)
					{
						UpdatedItems[number] = value;
					}
				}
			}
		}

		internal void GetChangedTableItems(IDictionary<string, string> allChangedTableItems)
		{
			List<int> keysToRemove = new List<int>();
			foreach (KeyValuePair<int, string> kvp in UpdatedItems)
			{
				allChangedTableItems[Convert.ToString(kvp.Key + 1, CultureInfo.InvariantCulture)] = kvp.Value;
				OriginalItems[kvp.Key] = kvp.Value;
				keysToRemove.Add(kvp.Key);
			}

			foreach (int key in keysToRemove)
			{
				UpdatedItems.Remove(key);
			}
		}

		internal bool UpdateOriginal(int key, string updatedValue)
		{
			string originalValue;
			if (OriginalItems.TryGetValue(key, out originalValue) && originalValue == updatedValue)
			{
				return false;
			}
			else
			{
				OriginalItems[key] = updatedValue;
				return true;
			}
		}
	}
}
