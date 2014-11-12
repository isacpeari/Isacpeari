using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;

namespace GarenOP
{

    class Program
    {
        public static Spell Q = new Spell(SpellSlot.Q);
        public static Spell W = new Spell(SpellSlot.W);
        public static Spell E = new Spell(SpellSlot.E);
        public static Spell R = new Spell(SpellSlot.R);
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
            Game.PrintChat("GarenOP loaded!");
            Game.OnGameUpdate += OnGameUpdate;
            Obj_AI_Hero.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
        }

        public static int GetWardId()
        {
            int[] wardIds = { 3340, 3350, 3205, 3207, 2049, 2045, 2044, 3361, 3154, 3362, 3160, 2043 };
            foreach (int id in wardIds)
            {
                if (Items.HasItem(id) && Items.CanUseItem(id))
                    return id;
            }
            return -1;
        }


        public static bool PutWard(Vector2 pos)
        {
            int wardItem;
            if ((wardItem = GetWardId()) != -1)
            {
                foreach (var slot in ObjectManager.Player.InventoryItems.Where(slot => slot.Id == (ItemId)wardItem))
                {
                    slot.UseItem(pos.To3D());
                    return true;
                }
            }
            return false;
        }


        static void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name.ToLower().Contains("basic"))
                {
                    
                   if(Dizzy==true)
                    {
                        ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo,ObjectManager.Player.ServerPosition);
                        if (E.IsReady())
                    {
                        Dizzy = false;
                         Game.PrintChat("You are no longer dizzy!");
                    }
                    }

                    if (args.SData.Name.ToLower().Equals("recall"))
                    {
                        Game.Say("/all FUCK THIS I'M GOING HOME MOTHER BITCH.");
                    }
                }
                if (args.SData.Name == "GarenQ")
                {
                    if (Q.IsReady())
                    {
                        Q.Cast();
                        Game.Say("/all SILENZZZ SKRUBZZZ");
                    }
                }
                else if (args.SData.Name == "GarenW")
                {
                    if (W.IsReady() && ObjectManager.Player.SightWardsBought + ObjectManager.Player.VisionWardsBought >=3)
                    {
                        W.Cast();
                        Vector2 pos = ObjectManager.Player.ServerPosition.To2D();
                        pos.Y += 80;
                        PutWard(pos);
                        System.Threading.Thread.Sleep(600);
                        pos.Y -= 160;
                        pos.X += 80;
                        PutWard(pos);
                        System.Threading.Thread.Sleep(600);
                        pos.X -= 160;
                        PutWard(pos);
                        Game.Say("/all ILLUMINATAYYYYYYYY");
                    }
                }
                //else if (args.SData.Name == ObjectManager.Player.Spellbook.GetSpellSlot(SpellSlot.E).SData.name)
                    
                else if (args.SData.Name == "GarenE")
                {
                    if (E.IsReady())
                    {
                        E.Cast();


                        Dizzy = true;
                        Game.Say("/all I'M TOO DIZZY. I CANNOT SEE!!!!11");

                        Game.PrintChat("You are too dizzy to attack for a while!");
                    }

                }
                else if (args.SData.Name == "GarenR")
                {
                    if (R.IsReady())
                    {
                        Dancing = true;
                        ObjectManager.Player.SummonerSpellbook.CastSpell(ObjectManager.Player.GetSpellSlot("SummonerFlash"),ObjectManager.Player.ServerPosition);

                    }

                }

            }

        }


        private static void OnGameUpdate(EventArgs args)
        {
            try
            {

                
                t.Elapsed += (object tSender, System.Timers.ElapsedEventArgs tE) =>
                {
                    Dancing = false;
                };

                if (Dancing)
                {
                    Packet.C2S.Emote.Encoded(new Packet.C2S.Emote.Struct(2)).Send();
                    Packet.C2S.Emote.Encoded(new Packet.C2S.Emote.Struct(4)).Send();
                }
            }
            catch (Exception e)
            {
               
            }
           
        }
    }
}
