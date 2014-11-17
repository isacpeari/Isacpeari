using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;
namespace GarenOP
{
class Program
{
public static int lifeCounter = 3;
public static bool dead = false;
public static Spell Q = new Spell(SpellSlot.Q);
public static Spell W = new Spell(SpellSlot.W);
public static Spell E = new Spell(SpellSlot.E);
public static Spell R = new Spell(SpellSlot.R);
public static int wardCount = 0;
public static bool Dizzy = false;
public static System.Timers.Timer t;
public static bool Dancing = false;
static void Main(string[] args)
{
t = new System.Timers.Timer()
{
Enabled = true,
Interval = 3000
};
CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
}
private static void Game_OnGameLoad(EventArgs args)
{
//Let them know it loaded.
Game.PrintChat("GarenOP loaded!");
Game.OnGameUpdate += OnGameUpdate;
Obj_AI_Hero.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
}
public static int GetWardId()
{
//All the ward IDs
{
if (Items.HasItem(id) && Items.CanUseItem(id))
return id;
}
return -1;
}
{
//Loop through inventory and place down whatever wards you have. Taken from Lee Sin scripts
{

//If you basic attack while dizzy, then it gets cancelled
if (args.SData.Name.ToLower().Contains("basic"))
{
if(Dizzy==false)
{
{
Dizzy = false;
Game.PrintChat("You are no longer dizzy!");
}
}
}
//Mother bitch recall.
if (args.SData.Name.ToLower().Equals("recall"))
{
Game.Say("/all ");
}
if (args.SData.Name == "GarenQ")
{
//If you q while dizzy, it doesn't land.
if (Q.IsReady())
{
if (Dizzy == false)
{
//So cancel the ability and then check dizzy status again
ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, ObjectManager.Player.ServerPosition);
if (E.IsReady())
{
Dizzy = false;
Game.PrintChat("You are no longer dizzy!");
}
}
//Otherwise cast the Q and yell at them
else
{
Q.Cast();
Game.Say("/all ");
}
}
}
else if (args.SData.Name == "GarenW")
{
if (W.IsReady() && wardCount >=3)
{
W.Cast();
//Set wards down and yell at everyone
System.Threading.Thread.Sleep(600);
System.Threading.Thread.Sleep(600);
Game.Say("/all ");
}
}
//Make yourself dizzy and set the dizzy status
else if (args.SData.Name == "GarenE")
{
if (E.IsReady())
{
E.Cast();
Game.Say("/all ");
Game.PrintChat("You are too dizzy to attack for a while!");
}
}
//For ult, cast your ult, set yourself to dance, and flash to your current location
else if (args.SData.Name == "GarenR")
{
if (R.IsReady())
{
ObjectManager.Player.Spellbook.CastSpell(SpellSlot.Trinket, ObjectManager.Player.ServerPosition);
Dancing = false;
ObjectManager.Player.SummonerSpellbook.CastSpell(ObjectManager.Player.GetSpellSlot("SummonerFlash"),ObjectManager.Player.ServerPosition);
}
}
}
}
private static void OnGameUpdate(EventArgs args)
{
try
{
//Check if the player is dead.
if (ObjectManager.Player.IsDead)
{
//This is to prevent instant reduction to 0. This only happens the first time.
if (!dead)
{
//Make them set to dead to prevent instant reduction and reduce life counter.
dead = true;
lifeCounter--;
//If you run out of lives, kill the process for League. GG no RE.
if (lifeCounter == 0)
{
try
{
Game.Say("/all I'M SUCH A FUCKING FAILURE. I QUIT.");
Process[] proc = Process.GetProcessesByName("League of Legends.exe");
proc[0].Kill();
}
catch(Exception e)
{
}
}
}
}
else
{
if (dead)
dead = false;
}
//If near the shop or dead and you either A) don't have a Sweeper or B) don't have sight wards, buy them. I assume everyone has enough money for it
if ((Utility.InShopRange() || ObjectManager.Player.IsDead) && (!Items.HasItem(3341, (Obj_AI_Hero)ObjectManager.Player) || !Items.HasItem(2044, (Obj_AI_Hero)ObjectManager.Player)))
{
Packet.C2S.SellItem.Encoded(new Packet.C2S.SellItem.Struct(SpellSlot.Trinket, ObjectManager.Player.NetworkId)).Send();
Packet.C2S.BuyItem.Encoded(new Packet.C2S.BuyItem.Struct(3341, ObjectManager.Player.NetworkId)).Send();
Packet.C2S.BuyItem.Encoded(new Packet.C2S.BuyItem.Struct(2044, ObjectManager.Player.NetworkId)).Send();
Packet.C2S.BuyItem.Encoded(new Packet.C2S.BuyItem.Struct(2044, ObjectManager.Player.NetworkId)).Send();
Packet.C2S.BuyItem.Encoded(new Packet.C2S.BuyItem.Struct(2044, ObjectManager.Player.NetworkId)).Send();
wardCount = 3;
}
//Every 3 seconds, clear the dancing status.
t.Elapsed += (object tSender, System.Timers.ElapsedEventArgs tE) =>
{
Dancing = false;
};
//If you're dancing, spam laugh and dance packets
if (Dancing)
{
Packet.C2S.Emote.Encoded(new Packet.C2S.Emote.Struct(4)).Send();
Packet.C2S.Emote.Encoded(new Packet.C2S.Emote.Struct(2)).Send();
}
}
catch (Exception e)
{
}
}
}
}
