using Discord.Addons.InteractiveCommands;
using Discord.Commands;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using SataniaBot.Services.EmbedExtensions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SataniaBot.Modules
{
    [Name("Games")]
    public class GameModules : InteractiveModuleBase<SocketCommandContext>
    {
        [Command("CoinFlip")]
        public async Task Flip()
        {

            Random flip = new Random();
            var coin = (flip.Next(1, 100));
            string CoinSide = "";
            if (coin <= 50)
            {
                CoinSide = "Heads";
            }
            else if (coin >= 51)
            {
                CoinSide = "Tails";
            }

            await Context.Channel.SendConfirmAsync(CoinSide);

        }
        public class monster
        {
            public string Name { get; set; }
            public int health { get; set; }
            public int attack { get; set; }
            public int resistance { get; set; }
        }
        public class Player
        {
            public int hpCap { get; set; }
            public int health { get; set; }
            public int attack { get; set; }
            public int resistance { get; set; }
        }
        [Command("Rpg", RunMode = RunMode.Async)]
        public async Task rpg()
        {
            var Dragon = new monster() { Name = "Dragon", health = 250, attack = 30, resistance = 15};
            var Player = new Player() { hpCap = 350, health = 350, attack = 35, resistance = 35 };
            while (Dragon.health > 0 && Player.health > 0)
            {
                Player.resistance = 35;
                await Context.Channel.SendMessageAsync("```Choose your action:" + "\n" + "a/attack > you swing your weapon at the monst" + "\n" + "g/guard > you guard yourself from the next attack rising your resistance stat and healing you for 1-10hp```");
                var response = await WaitForMessage(Context.Message.Author, Context.Channel, null, new MessageContainsResponsePrecondition("a", "attack", "g", "guard"));
                if (response.Content.ToString().ToLower() == "a" || response.ToString().ToLower() == "attack")
                {
                    Random Attack = new Random();
                    int AttackDMGIncrease = Attack.Next(0, Player.attack);
                    int DMGDealt = ((Player.attack + ((AttackDMGIncrease / 2))) * (100 - Dragon.resistance) / 100);       //dont ask why I made it like this
                    Dragon.health -= DMGDealt;
                    await ReplyAsync("```you swung your weapon at the monster and did " + $"{DMGDealt}" + " damage to it!" + "\n" + "leaving the " + $"{Dragon.Name}" + " at " + $"{Dragon.health}" + " hp!```");
                    if (Dragon.health > 0)
                    {
                        await Task.Delay(3000);

                        int MonsterDMGIncrease = Attack.Next(0, Dragon.attack);
                        int MonsterDMGDealt = ((Player.attack + ((MonsterDMGIncrease / 2))) * (100 - Dragon.resistance) / 100);      //same formula just used player resistance and (to be)Monster attack
                        Player.health -= MonsterDMGDealt;
                        await ReplyAsync("```The " + $"{Dragon.Name}" + " swung it's claws at you and it did " + $"{MonsterDMGDealt}" + " damage to you" + "\n" + "leaving you at " + $"{Player.health}" + " hp!```");

                        await Task.Delay(3000);
                    }
                }
                else if(response.Content.ToString().ToLower() == "g" || response.Content.ToString().ToLower() == "guard")
                {
                    Player.resistance = 80;
                    Random RNG = new Random();

                    int hpHealed = RNG.Next(1,10);
                    Player.health += hpHealed;

                    if (Player.health > Player.hpCap)
                    {
                        Player.health = Player.hpCap;
                    }

                    await ReplyAsync("```you guarded and raised your resistance stat to: " + $"{Player.resistance}" + " and healed yourself for: " + $"{hpHealed}" + " hp! And now you are at "+ $"{Player.health}/{Player.hpCap}" + "```");

                    await Task.Delay(3000);

                    int MonsterDMGIncrease = RNG.Next(0, Dragon.attack);
                    int MonsterDMGDealt = ((Player.attack + ((MonsterDMGIncrease / 2))) * (100 - Dragon.resistance) / 100);      //same formula just used player resistance and (to be)Monster attack
                    Player.health -= MonsterDMGDealt;
                    await ReplyAsync("```The " + $"{Dragon.Name}" + " swung it's claws at you and it did " + $"{MonsterDMGDealt}" + " damage to you" + "\n" + "leaving you at " + $"{Player.health}" + " hp!```");

                    await Task.Delay(3000);
                };
                if (Dragon.health < 0)
                {
                    await Context.Channel.SendConfirmAsync("congratulations you have defeated the Dragon!");
                }
                else if (Player.health < 0)
                {
                    await Context.Channel.SendErrorAsync("You have been killed by the dragon, better luck next time!");
                }
            }
        }



    }
}

