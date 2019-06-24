using Game4Freak.AdvancedZones;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Enumerations;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Game4Freak.CashBank
{
    public class CashBank : RocketPlugin<CashBankConfiguration>
    {
        public static CashBank Instance;
        public const string VERSION = "0.2.2.2";
        public const string cashBankFlag = "cashBank";
        public const string notifyBalanceFlag = "notifyBalance";

        protected override void Load()
        {
            Instance = this;
            Logger.Log("CashBank v" + VERSION);

            if (Configuration.Instance.useAdvancedZones)
            {
                register();
            }

            fr34kyn01535.Uconomy.Uconomy.Instance.OnPlayerPay += onPayed;
        }

        private void register()
        {
            Logger.Log("Using AdvancedZones");
            AdvancedZones.AdvancedZones.Instance.addCustomFlag("cashBank", 100, "Deposit or withdraw your money in the zone");
            AdvancedZones.AdvancedZones.Instance.addCustomFlag("notifyBalance", 101, "Notifies you about your balance when entering zone with cashBank flag");
            AdvancedZones.AdvancedZones.onZoneEnter += onZoneEntered;
        }

        private void unregister()
        {
            AdvancedZones.AdvancedZones.onZoneEnter -= onZoneEntered;
        }

        protected override void Unload()
        {
            if (Configuration.Instance.useAdvancedZones)
                unregister();

            fr34kyn01535.Uconomy.Uconomy.Instance.OnPlayerPay -= onPayed;
        }

        public override TranslationList DefaultTranslations => new TranslationList
        {
            {"balance_notification", "Your current balance is: {0} {1} {2}" },
            {"taxation_notification", "You paid taxes in amount of: {0} {1} {2} ({3}%)" },
            {"command_invalid", "Invalid! Try {0} {1}" },
            {"amount_invalid", "Invalid Amount: {0}" },
            {"note_invalid", "Invalid Bank Note: {0}" },
            {"withdraw", "You have withdrew: {0} {1} {2}" },
            {"withdraw_cant_afford", "Your balance does not allow this withdraw" },
            {"zone_invalid", "You are in no bank" },
            {"deposit", "You have deposited: {0} {1} {2}" },
            {"deposit_invalid", "You have nothing to deposit" }
        };

        private void onZoneEntered(UnturnedPlayer player, Zone zone, Vector3 lastPos)
        {
            if (zone.hasFlag(notifyBalanceFlag))
                UnturnedChat.Say(player, Translate("balance_notification", fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.MoneySymbol, fr34kyn01535.Uconomy.Uconomy.Instance.Database.GetBalance(player.CSteamID.ToString()), fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.MoneyName), UnturnedChat.GetColorFromName(CashBank.Instance.Configuration.Instance.messageColor, Color.green));
        }

        private void onPayed(UnturnedPlayer sender, string receiver, decimal amt)
        {
            if (Configuration.Instance.usePayTaxation)
            {
                UnturnedChat.Say(sender, Translate("taxation_notification", fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.MoneySymbol, amt * Configuration.Instance.payTaxes , fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.MoneyName, Configuration.Instance.payTaxes * 100), UnturnedChat.GetColorFromName(CashBank.Instance.Configuration.Instance.messageColor, Color.green));
                fr34kyn01535.Uconomy.Uconomy.Instance.Database.IncreaseBalance(sender.CSteamID.ToString(), -(amt * Configuration.Instance.payTaxes));
            }
        }

    }
}