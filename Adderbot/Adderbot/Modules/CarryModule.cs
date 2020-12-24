using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adderbot.Models;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Adderbot.Modules
{
    internal class CarryModule : BaseModule
    {
        [Command("trial")]
        [Summary("Calculates the payout for a trial.")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task CalculateTrialAsync(double total)
        {
            if (Double.IsNaN(total))
            {
                await Context.User.SendMessageAsync(
                    Constants.MessageText.Error.NotCorrectGoldFormat);
                return;
            }

            try
            {
                var payoutValues = Constants.CarryHelp.TotalText + String.Format("{0:0,0.0}", total);
                for (int i = 0; i < Constants.CarryHelp.TrialHelp.TrialPayoutTypes.Length; i++)
                    payoutValues += Environment.NewLine + Constants.CarryHelp.TrialHelp.TrialPayoutTypes[i] + String.Format("{0:0,0.0}", total * Constants.CarryHelp.TrialHelp.TrialModifiers[i]);

                await Context.User.SendMessageAsync(payoutValues);
            }
            catch (ArgumentException ae)
            {
                await Context.User.SendMessageAsync(ae.Message);
            }
        }

        [Command("4man")]
        [Summary("Calculates the payout for four man content.")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task CalculateFourManAsync(double total)
        {
            if (Double.IsNaN(total))
            {
                await Context.User.SendMessageAsync(
                    Constants.MessageText.Error.NotCorrectGoldFormat);
                return;
            }

            try
            {
                var payoutValues = Constants.CarryHelp.TotalText + String.Format("{0:0,0.0}", total);
                for (int i = 0; i < Constants.CarryHelp.FourManHelp.FourManPayoutTypes.Length; i++)
                    payoutValues += Environment.NewLine + Constants.CarryHelp.FourManHelp.FourManPayoutTypes[i] + String.Format("{0:0,0.0}", total * Constants.CarryHelp.FourManHelp.FourManModifiers[i]);

                await Context.User.SendMessageAsync(payoutValues);
            }
            catch (ArgumentException ae)
            {
                await Context.User.SendMessageAsync(ae.Message);
            }
        }

        [Command("fourman")]
        [Summary("Calculates the payout for four man content.")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task CalculateFourMan2Async(double total)
        {
            await CalculateFourManAsync(total);
        }

        [Command("dungeon")]
        [Summary("Calculates the payout for a dungeon.")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task CalculateDungeonAsync(double total)
        {
            await CalculateFourManAsync(total);
        }

        [Command("arena")]
        [Summary("Calculates the payout for an arena.")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task CalculateArenaAsync(double total)
        {
            await CalculateFourManAsync(total);
        }
    }
}