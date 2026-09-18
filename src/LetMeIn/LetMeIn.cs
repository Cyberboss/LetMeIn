using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FrooxEngine;

using HarmonyLib;

using ResoniteModLoader;

using SkyFrost.Base;

namespace LetMeIn
{
	public sealed class LetMeIn : ResoniteMod
	{
		internal const string NameConstant = nameof(LetMeIn);

		internal const string VersionConstant = "1.1.0";

		public override string Name => NameConstant;

		public override string Author => "Dominion";

		public override string Version => VersionConstant;

		public override string Link => $"https://github.com/Cyberboss/{NameConstant}";

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<bool> Enabled = new ModConfigurationKey<bool>("Enabled", "Mod Enabled", () => true);

		private static ModConfiguration? Config;

		public override void OnEngineInit()
		{
			Config = GetConfiguration()!;
			Config.Save(true);

			Harmony harmony = new($"net.dextraspace.{NameConstant}");
			harmony.PatchAll();
		}

		[HarmonyPatch(typeof(Userspace), "LaunchAutoStartSession")]
		public class Userspace_LaunchAutoStartSession_Patches
		{
			public static bool Prefix(Userspace __instance, SessionJoinParameters session, ref Task __result)
			{
				if (!(Config?.GetValue(Enabled) ?? false))
				{
					return true;
				}

				var sessionID = session.SessionID;
				if (String.IsNullOrWhiteSpace(sessionID))
				{
					Warn("An autoJoinSessions entry has no SessionID. Cannot deduce user ID of host!");
					return true;
				}

				var hostIdentifer = sessionID
					.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[0];
				if (!hostIdentifer.StartsWith("S-"))
				{
					Error($"Could not deduce UserID from session ID. Bad host specifier. Expecting 'S-U-XXX:YYY' format: {sessionID}");
					return true;
				}

				var userId = hostIdentifer.Substring(2);
				if (String.IsNullOrWhiteSpace(userId))
				{
					Error($"Bad session user ID in session ID: {sessionID}");
					return true;
				}

				var cloud = __instance.Cloud;

				Msg($"Requesting invite for auto join session ID: {sessionID}");

				var messages = cloud.Messages;
				var userMessages = messages.GetUserMessages(userId);
				var inviteRequestMessage = userMessages.CreateInviteRequest();
				var markMessagesRead = userMessages.UnreadCount == 0;
				Action<Message> callback = receivedMessage =>
				{
					if (receivedMessage.MessageType != SkyFrost.Base.MessageType.SessionInvite)
						return;

					Msg($"Marking message {receivedMessage.Id} from user {userId} as read as it appears to be an automated session invite.");
					userMessages.MarkAllRead();
				};

				async ValueTask AsyncProcess()
				{
					if (markMessagesRead)
						messages.OnMessageReceived += callback;

					await userMessages.SendMessage(inviteRequestMessage);

					if (markMessagesRead)
					{
						await Task.Delay(TimeSpan.FromSeconds(5));
						messages.OnMessageReceived -= callback;
					}
				}

				_ = AsyncProcess();

				return true;
			}
		}
	}
};
