using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchiSteamFarm.Localization;
using ArchiSteamFarm.Steam;
using ArchiSteamFarm.Steam.Data;
using ArchiSteamFarm.Steam.Security;

namespace BoosterManager {
	internal static class GemHandler {
		internal const ulong GemsClassID = 667924416;
		internal const ulong SackOfGemsClassID = 667933237; 

		internal static async Task<string> GetGemCount(Bot bot) {
			HashSet<Asset> inventory;
			try {
				inventory = await bot.ArchiWebHandler.GetInventoryAsync().Where(item => item.Type == Asset.EType.SteamGems).ToHashSetAsync().ConfigureAwait(false);
			} catch (Exception e) {
				bot.ArchiLogger.LogGenericException(e);
				return Commands.FormatBotResponse(bot, Strings.WarningFailed);
			}

			(uint tradable, uint untradable) gems = (0,0);
			(uint tradable, uint untradable) sacks = (0,0);

			foreach (Asset item in inventory) {
				switch (item.ClassID, item.Tradable) {
					case (GemsClassID, true): gems.tradable += item.Amount; break;
					case (GemsClassID, false): gems.untradable += item.Amount; break;
					case (SackOfGemsClassID, true): sacks.tradable += item.Amount; break;
					case (SackOfGemsClassID, false): sacks.untradable += item.Amount; break;
					default: break;
				}
			}

			return Commands.FormatBotResponse(bot, String.Format("Tradable: {0:N0}{1}{2}", gems.tradable, sacks.tradable == 0 ? "" : String.Format(" (+{0:N0} Sacks)", sacks.tradable),
				(gems.untradable + sacks.untradable) == 0 ? "" : String.Format("; Untradable: {0:N0}{1}", gems.untradable, sacks.untradable == 0 ? "" : String.Format(" (+{0:N0} Sacks)", sacks.untradable))
			));
		}
	}
}
