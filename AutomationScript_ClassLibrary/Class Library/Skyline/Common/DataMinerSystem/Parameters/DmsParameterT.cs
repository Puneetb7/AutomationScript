namespace Skyline.DataMiner.Library.Common
{
    using Net.Messages;
    using Skyline.DataMiner.Library.Common.SLNetHelper;
    using Skyline.DataMiner.Library.Common.Subscription.Waiters;
    using Skyline.DataMiner.Library.Common.Subscription.Waiters.Parameter;

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Base class for parameters.
    /// </summary>
    /// <typeparam name="T">The parameter type.</typeparam>
    internal class DmsParameter<T>
	{
		/// <summary>
		/// Setter delegates.
		/// </summary>
		private static readonly Dictionary<Type, Func<SetParameterMessage, T, bool>> Setters = new Dictionary<Type, Func<SetParameterMessage, T, bool>>
		{
			{ typeof(string), new Func<SetParameterMessage, T, bool>(AddStringValueToSetParameterMessage) },
			{ typeof(int?), new Func<SetParameterMessage, T, bool>(AddNullableIntValueToSetParameterMessage)},
			{ typeof(double?), new Func<SetParameterMessage, T, bool>(AddNullableDoubleValueToSetParameterMessage) },
			{ typeof(DateTime?), new Func<SetParameterMessage, T, bool>(AddNullableDateTimeValueToSetParameterMessage) }
		};

		/// <summary>
		/// The parameter ID.
		/// </summary>
		private readonly int id;

		/// <summary>
		/// The type of the parameter.
		/// </summary>
		/// <remarks>Currently supported types: int?, double?, string, DateTime?.</remarks>
		private readonly Type type;

		/// <summary>
		/// The underlying type (in case of Nullable&lt;T&gt;).
		/// </summary>
		private readonly Type underlyingType;

		/// <summary>
		/// Initializes a new instance of the <see cref="DmsParameter{T}"/> class.
		/// </summary>
		/// <param name="id">The parameter ID.</param>
		/// <exception cref="ArgumentException"><paramref name="id"/> is invalid.</exception>
		protected DmsParameter(int id)
		{
			if (id < 0)
			{
				throw new ArgumentException("Invalid parameter ID", "id");
			}

			this.id = id;
			type = typeof(T);
			underlyingType = Nullable.GetUnderlyingType(type);
		}

		/// <summary>
		/// Gets the parameter ID.
		/// </summary>
		/// <value>The parameter ID.</value>
		public int Id
		{
			get { return id; }
		}

		/// <summary>
		/// Processes the response.
		/// </summary>
		/// <param name="response">The response message.</param>
		/// <returns>The parameter value.</returns>
		internal T ProcessResponse(GetParameterResponseMessage response)
		{
			object interopValue = response.Value.InteropValue;

			T value = underlyingType == null ? ProcessResponseNonNullable(interopValue) : ProcessResponseNullable(interopValue);

			return value;
		}

		/// <summary>
		/// Adds the value to set to the SetParameterMessage.
		/// </summary>
		/// <param name="message">The message to update with the parameter value to set.</param>
		/// <param name="value">The parameter value to set.</param>
		/// <returns>Whether the SetParameterMessage needs to be sent.</returns>
		protected bool AddValueToSetParameterMessage(SetParameterMessage message, T value)
		{
			Func<SetParameterMessage, T, bool> setter;

			if (Setters.TryGetValue(type, out setter))
			{
				return setter(message, value);
			}
			else
			{
				throw new NotSupportedException("Type " + typeof(T) + " is not supported.");
			}
		}

		/// <summary>
		/// Adds a nullable DateTime value to the message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="value">The value.</param>
		/// <returns><c>true</c> if the set message needs to be executed; otherwise, <c>false</c>.</returns>
		private static bool AddNullableDateTimeValueToSetParameterMessage(SetParameterMessage message, T value)
		{
			bool executeSet = true;

			if (!value.Equals(default(T)))
			{
				DateTime valueToSet = (DateTime)Convert.ChangeType(value, typeof(DateTime), CultureInfo.CurrentCulture);

				message.Value = new ParameterValue(valueToSet);
			}
			else
			{
				executeSet = false;
			}

			return executeSet;
		}

		/// <summary>
		/// Adds a nullable double value to the message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="value">The value.</param>
		/// <returns><c>true</c> if the set message needs to be executed; otherwise, <c>false</c>.</returns>
		private static bool AddNullableDoubleValueToSetParameterMessage(SetParameterMessage message, T value)
		{
			bool executeSet = true;

			if (!value.Equals(default(T)))
			{
				double valueToSet = (double)Convert.ChangeType(value, typeof(double), CultureInfo.CurrentCulture);

				message.Value = new ParameterValue(valueToSet);
			}
			else
			{
				executeSet = false;
			}

			return executeSet;
		}

		/// <summary>
		/// Adds a nullable int value to the message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="value">The string value.</param>
		/// <returns><c>true</c> if the set message needs to be executed; otherwise, <c>false</c>.</returns>
		private static bool AddNullableIntValueToSetParameterMessage(SetParameterMessage message, T value)
		{
			bool executeSet = true;

			if (!value.Equals(default(T)))
			{
				int valueToSet = (int)Convert.ChangeType(value, typeof(int), CultureInfo.CurrentCulture);
				message.Value = new ParameterValue(valueToSet);
			}
			else
			{
				executeSet = false;
			}

			return executeSet;
		}

		/// <summary>
		/// Adds a string value to the message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="value">The string value.</param>
		/// <returns><c>true</c> if the set message needs to be executed; otherwise, <c>false</c>.</returns>
		private static bool AddStringValueToSetParameterMessage(SetParameterMessage message, T value)
		{
			message.Value = new ParameterValue((string)Convert.ChangeType(value, typeof(string), CultureInfo.CurrentCulture));

			return true;
		}

		/// <summary>
		/// Processes the response for a non-nullable type.
		/// </summary>
		/// <param name="interopValue">The value.</param>
		/// <returns>The parameter value.</returns>
		private T ProcessResponseNonNullable(object interopValue)
		{
			return SLNetUtility.ProcessResponseNonNullable<T>(interopValue, type);
		}

		/// <summary>
		/// Processes the response for a nullable type.
		/// </summary>
		/// <param name="interopValue">The value.</param>
		/// <returns>The parameter value.</returns>
		private T ProcessResponseNullable(object interopValue)
		{
			return SLNetUtility.ProcessResponseNullable<T>(interopValue, underlyingType);
		}
	}
}