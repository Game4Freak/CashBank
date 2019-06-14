using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Core;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Unturned.Player;
using UnityEngine;

namespace Game4Freak.CashBank
{
    public class CommandDeposit : IRocketCommand
    {
        public string Name
        {
            get { return "deposit"; }
        }
        public string Help
        {
            get { return "Deposit money to your account"; }
        }

        public AllowedCaller AllowedCaller
        {
            get
            {
                return AllowedCaller.Player;
            }
        }

        public string Syntax
        {
            get { return "<bankNote|all> <amount>"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "cashbank.deposit", "cashbank.deposit.all" };
            }
        }

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (command.Length < 1)
            {
                UnturnedChat.Say(caller, CashBank.Instance.Translate("command_invalid", Name, Syntax), UnturnedChat.GetColorFromName(CashBank.Instance.Configuration.Instance.messageColor, Color.green));
                return;
            }
            if (CashBank.Instance.Configuration.Instance.useAdvancedZones && !inZone(player, CashBank.cashBankFlag))
            {
                UnturnedChat.Say(caller, CashBank.Instance.Translate("zone_invalid"), UnturnedChat.GetColorFromName(CashBank.Instance.Configuration.Instance.messageColor, Color.green));
                return;
            }
            if (command[0].ToLower() == "all")
            {
                decimal amount = 0;
                foreach (var i in player.Inventory.items)
                {
                    if (i == null) continue;
                    for (byte w = 0; w < i.width; w++)
                    {
                        for (byte h = 0; h < i.height; h++)
                        {
                            try
                            {
                                byte index = i.getIndex(w, h);
                                if (index == 255) continue;
                                foreach (var banknote in CashBank.Instance.Configuration.Instance.bankNotes)
                                {
                                    if (i.getItem(index).item.id == banknote.ID)
                                    {
                                        amount += banknote.worth;
                                        i.removeItem(index);
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }
                if (amount == 0)
                {
                    UnturnedChat.Say(caller, CashBank.Instance.Translate("deposit_invalid"), UnturnedChat.GetColorFromName(CashBank.Instance.Configuration.Instance.messageColor, Color.green));
                    return;
                }
                else
                {
                    UnturnedChat.Say(caller, CashBank.Instance.Translate("deposit", fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.MoneySymbol, amount, fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.MoneyName), UnturnedChat.GetColorFromName(CashBank.Instance.Configuration.Instance.messageColor, Color.green));
                    fr34kyn01535.Uconomy.Uconomy.Instance.Database.IncreaseBalance(player.CSteamID.ToString(), amount);
                    return;
                }
            }
            decimal note = 0;
            foreach (var banknote in CashBank.Instance.Configuration.Instance.bankNotes)
            {
                if (command[0].ToLower() == banknote.name.ToLower())
                {
                    note = banknote.worth;
                }
            }
            if (note == 0)
            {
                if (!decimal.TryParse(command[0], out note))
                {
                    UnturnedChat.Say(caller, CashBank.Instance.Translate("note_invalid", command[0]), UnturnedChat.GetColorFromName(CashBank.Instance.Configuration.Instance.messageColor, Color.green));
                    return;
                }
            }
            int amountNotes = 0;
            if (command.Length > 1 && !int.TryParse(command[1], out amountNotes))
            {
                UnturnedChat.Say(caller, CashBank.Instance.Translate("amount_invalid", command[1]), UnturnedChat.GetColorFromName(CashBank.Instance.Configuration.Instance.messageColor, Color.green));
                return;
            }
            foreach (var banknote in CashBank.Instance.Configuration.Instance.bankNotes)
            {
                if (note == banknote.worth)
                {
                    decimal amount = 0;
                    bool fin = false;
                    foreach (var i in player.Inventory.items)
                    {
                        if (fin) break;
                        if (i == null) continue;
                        for (byte w = 0; w < i.width; w++)
                        {
                            for (byte h = 0; h < i.height; h++)
                            {
                                try
                                {
                                    byte index = i.getIndex(w, h);
                                    if (index == 255) continue;
                                    if (i.getItem(index).item.id == banknote.ID)
                                    {
                                        if (!fin)
                                        {
                                            amount += banknote.worth;
                                            i.removeItem(index);
                                            if (amountNotes != 0 && amount == banknote.worth * amountNotes)
                                                fin = true;
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                    if (amount == 0)
                    {
                        UnturnedChat.Say(caller, CashBank.Instance.Translate("deposit_invalid"), UnturnedChat.GetColorFromName(CashBank.Instance.Configuration.Instance.messageColor, Color.green));
                        return;
                    }
                    else
                    {
                        UnturnedChat.Say(caller, CashBank.Instance.Translate("deposit", fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.MoneySymbol, amount, fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.MoneyName), UnturnedChat.GetColorFromName(CashBank.Instance.Configuration.Instance.messageColor, Color.green));
                        fr34kyn01535.Uconomy.Uconomy.Instance.Database.IncreaseBalance(player.CSteamID.ToString(), amount);
                        return;
                    }
                }
            }
            UnturnedChat.Say(caller, CashBank.Instance.Translate("note_invalid", command[1]), UnturnedChat.GetColorFromName(CashBank.Instance.Configuration.Instance.messageColor, Color.green));
            return;
        }

        private bool inZone(UnturnedPlayer player, string zoneID)
        {
            return AdvancedZones.AdvancedZones.Instance.playerInZoneType(player, zoneID);
        }
    }
}
