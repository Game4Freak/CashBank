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
    public class CommandWithdraw : IRocketCommand
    {
        public string Name
        {
            get { return "withdraw"; }
        }
        public string Help
        {
            get { return "Withdraw money from your account"; }
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
            get { return "<amount> <maxBankNote>"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "cashbank.withdraw", "cashbank.withdraw.notes" };
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
            if (CashBank.Instance.Configuration.Instance.useAdvancedZones && !AdvancedZones.AdvancedZones.Instance.playerInZoneType(player, CashBank.cashBankFlag))
            {
                UnturnedChat.Say(caller, CashBank.Instance.Translate("zone_invalid"), UnturnedChat.GetColorFromName(CashBank.Instance.Configuration.Instance.messageColor, Color.green));
                return;
            }
            decimal amount;
            if (!decimal.TryParse(command[0], out amount) || amount <= 0 || amount % CashBank.Instance.Configuration.Instance.bankNotes.Last().worth != 0)
            {
                UnturnedChat.Say(caller, CashBank.Instance.Translate("amount_invalid", command[0]), UnturnedChat.GetColorFromName(CashBank.Instance.Configuration.Instance.messageColor, Color.green));
                return;
            }
            if (fr34kyn01535.Uconomy.Uconomy.Instance.Database.GetBalance(player.CSteamID.ToString()) < amount)
            {
                UnturnedChat.Say(caller, CashBank.Instance.Translate("withdraw_cant_afford"), UnturnedChat.GetColorFromName(CashBank.Instance.Configuration.Instance.messageColor, Color.green));
                return;
            }
            if (command.Length == 1)
            {
                UnturnedChat.Say(caller, CashBank.Instance.Translate("withdraw", "$", amount, fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.MoneyName), UnturnedChat.GetColorFromName(CashBank.Instance.Configuration.Instance.messageColor, Color.green));
                fr34kyn01535.Uconomy.Uconomy.Instance.Database.IncreaseBalance(player.CSteamID.ToString(), -amount);
                List<Note> notes = CashBank.Instance.Configuration.Instance.bankNotes;
                while (amount > 0)
                {
                    for (int i = 0; i < notes.Count; i++)
                    {
                        if (amount >= notes[i].worth)
                        {
                            player.GiveItem(notes[i].ID, 1);
                            amount -= notes[i].worth;
                            break;
                        }
                    }
                }
            }
            else if (command.Length > 1)
            {
                if (!player.HasPermission(Permissions[1]))
                {
                    UnturnedChat.Say(caller, "You dont have enough permissions to do that!", Color.red);
                    return;
                }
                decimal maxNote = 0;
                foreach (var banknote in CashBank.Instance.Configuration.Instance.bankNotes)
                {
                    if (command[1].ToLower() == banknote.name.ToLower())
                    {
                        maxNote = banknote.worth;
                    }
                }
                if (maxNote == 0)
                {
                    if (!decimal.TryParse(command[1], out maxNote))
                    {
                        UnturnedChat.Say(caller, CashBank.Instance.Translate("note_invalid", command[1]), UnturnedChat.GetColorFromName(CashBank.Instance.Configuration.Instance.messageColor, Color.green));
                        return;
                    }
                }
                List<Note> notes = CashBank.Instance.Configuration.Instance.bankNotes;
                for (int x = 0; x < notes.Count; x++)
                {
                    if (maxNote == notes[x].worth)
                    {
                        UnturnedChat.Say(caller, CashBank.Instance.Translate("withdraw", "$", amount, fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.MoneyName), UnturnedChat.GetColorFromName(CashBank.Instance.Configuration.Instance.messageColor, Color.green));
                        fr34kyn01535.Uconomy.Uconomy.Instance.Database.IncreaseBalance(player.CSteamID.ToString(), -amount);
                        while (amount > 0)
                        {
                            for (int i = x; i < notes.Count; i++)
                            {
                                if (amount >= notes[i].worth)
                                {
                                    player.GiveItem(notes[i].ID, 1);
                                    amount -= notes[i].worth;
                                    break;
                                }
                            }
                        }
                        return;
                    }
                }
                UnturnedChat.Say(caller, CashBank.Instance.Translate("note_invalid", command[1]), UnturnedChat.GetColorFromName(CashBank.Instance.Configuration.Instance.messageColor, Color.green));
                return;
            }
        }
    }
}
