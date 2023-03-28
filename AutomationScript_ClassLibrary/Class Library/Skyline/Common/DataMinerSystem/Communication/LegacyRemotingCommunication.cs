namespace Skyline.DataMiner.Library.Common
{
	using Net;
	using Net.Messages;

	using System;
	using System.Reflection;

	/// <summary>
	/// Represents a communication interface implementation using the <see cref="LegacyRemotingConnection"/> class.
	/// </summary>
	internal class LegacyRemotingCommunication : ICommunication
	{
		/// <summary>
		/// The remote connection.
		/// </summary>
		private readonly LegacyRemotingConnection LegacyRemotingConnection;

		/// <summary>
		/// Initializes a new instance of the <see cref="LegacyRemotingCommunication"/> class using an instance of the <see cref="LegacyRemotingConnection"/> class.
		/// </summary>
		/// <param name="LegacyRemotingConnection">The <see cref="LegacyRemotingConnection"/> instance.</param>
		/// <exception cref="ArgumentNullException"><paramref name="LegacyRemotingConnection"/> is <see langword="null"/>.</exception>
		public LegacyRemotingCommunication(LegacyRemotingConnection LegacyRemotingConnection)
		{
			if (LegacyRemotingConnection == null)
			{
				throw new ArgumentNullException("LegacyRemotingConnection");
			}

			this.LegacyRemotingConnection = LegacyRemotingConnection;
		}

		/// <summary>
		/// Add an SLNet Subscription.
		/// </summary>
		/// <param name="handler">The handler containing the action to be performed when the event triggers.</param>
		/// <exception cref="ArgumentNullException"><paramref name="handler"/> is <see langword="null"/>.</exception>
		public void AddSubscriptionHandler(NewMessageEventHandler handler)
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}

			LegacyRemotingConnection.OnNewMessage += handler;
		}

		/// <summary>
		/// Add an SLNet Subscription.
		/// </summary>
		/// <param name="handler">The handler containing the action to be performed when the event triggers.</param>
		/// <param name="handleGuid">A unique identifier for the handler.</param>
		/// <param name="subscriptions">All the subscription filters that define when to trigger an event.</param>
		/// <exception cref="ArgumentNullException"><paramref name="handler"/>, <paramref name="handleGuid"/> or <paramref name="subscriptions"/> is <see langword="null"/>.</exception>
		public void AddSubscriptions(NewMessageEventHandler handler, string handleGuid, SubscriptionFilter[] subscriptions)
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}

			if (handleGuid == null)
			{
				throw new ArgumentNullException("handleGuid");
			}

			if (subscriptions == null)
			{
				throw new ArgumentNullException("subscriptions");
			}

			LegacyRemotingConnection.OnNewMessage += handler;
			LegacyRemotingConnection.AddSubscription(handleGuid, subscriptions);
			System.Diagnostics.Debug.WriteLine("Created Subscriptions:" + handleGuid);
		}

		/// <summary>
		/// Clear an SLNet Subscription.
		/// </summary>
		/// <param name="handler">The handler containing the action to be cleared.</param>
		///  <exception cref="ArgumentNullException"><paramref name="handler"/> is <see langword="null"/>.</exception>
		public void ClearSubscriptionHandler(NewMessageEventHandler handler)
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}

			LegacyRemotingConnection.OnNewMessage -= handler;
		}

		/// <summary>
		/// Clear an SLNet Subscription.
		/// </summary>
		/// <param name="handler">The handler containing the action to be cleared.</param>
		/// <param name="handleGuid">A unique identifier for the handler to be cleared.</param>
		/// <param name="replaceWithEmpty">Indicates if the handler should be replaced with an empty handler after removal.</param>
		///  <exception cref="ArgumentNullException"><paramref name="handler"/>, <paramref name="handleGuid"/> is <see langword="null"/>.</exception>
		public void ClearSubscriptions(NewMessageEventHandler handler, string handleGuid, bool replaceWithEmpty = false)
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}

			if (handleGuid == null)
			{
				throw new ArgumentNullException("handleGuid");
			}

			if (replaceWithEmpty || GetDelegateCount(LegacyRemotingConnection) == 1)
			{
				// Workaround to fix Exceptions and SLNet Connection crashes due to events triggering after the subscriptions was cleared.
				// We need an empty handler to 'trigger' if events still come in.
				NewMessageEventHandler emptyHandler = (sender, e) =>
				{ };
				LegacyRemotingConnection.OnNewMessage += emptyHandler;
			}

			LegacyRemotingConnection.ClearSubscriptions(handleGuid);
			LegacyRemotingConnection.OnNewMessage -= handler;
		}


		internal static int GetDelegateCount(Connection connection)
		{
			string eventName = "OnNewMessage";

			var baseType = typeof(Connection);

			BindingFlags bf = BindingFlags.Instance | BindingFlags.NonPublic;
			var field = baseType.GetField(eventName, bf);

			MulticastDelegate dlg = (MulticastDelegate)field.GetValue(connection);

			if (dlg == null)
			{
				return 0;
			}
			else
			{
				Delegate[] subscribers = dlg.GetInvocationList();
				return subscribers.Length;
			}
		}

		/// <summary>
		/// Sends a message to the SLNet process.
		/// </summary>
		/// <param name="message">The message to be sent.</param>
		/// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
		/// <returns>The message responses.</returns>
		public DMSMessage[] SendMessage(DMSMessage message)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}

			if (LegacyRemotingConnection != null)
			{
				return LegacyRemotingConnection.HandleMessage(message);
			}

			return new DMSMessage[] { };
		}

		/// <summary>
		/// Sends a message to the SLNet process.
		/// </summary>
		/// <param name="message">The message to be sent.</param>
		/// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null" />.</exception>
		/// <returns>The message response.</returns>
		public DMSMessage SendSingleResponseMessage(DMSMessage message)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}

			if (LegacyRemotingConnection != null)
			{
				return LegacyRemotingConnection.HandleSingleResponseMessage(message);
			}

			return null;
		}

		/// <summary>
		/// Sends a message to the SLNet process.
		/// </summary>
		/// <param name="message">The message to be sent.</param>
		/// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null" />.</exception>
		/// <returns>The message response.</returns>
		public DMSMessage SendSingleRawResponseMessage(DMSMessage message)
		{
			return SendSingleResponseMessage(message);
		}
	}
}