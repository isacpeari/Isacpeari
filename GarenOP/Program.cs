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
        public static bool Dancing = false;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Game.PrintChat("GarenOP loaded!");
            Game.OnGameUpdate += OnGameUpdate;
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
                Game.PrintChat(args.SData.Name);
                if (args.SData.Name == Q.ChargedSpellName)
                {
                    if (Q.IsReady())
                    {
                        Q.Cast();
                        Game.Say("/all SILENZZZ SKRUBZZZ");
                    }
                }
                else if (args.SData.Name == W.ChargedSpellName)
                {
                    if (W.IsReady() && ObjectManager.Player.SightWardsBought + ObjectManager.Player.VisionWardsBought >=3)
                    {
                        W.Cast();
                        Vector2 pos = ObjectManager.Player.ServerPosition.To2D();
                        pos.Y += 20;
                        PutWard(pos);
                        pos.Y -= 40;
                        pos.X += 20;
                        PutWard(pos);
                        pos.X -= 40;
                        PutWard(pos);
                        Game.Say("/all ILLUMINATAYYYYYYYY");
                    }
                }
                //else if (args.SData.Name == ObjectManager.Player.Spellbook.GetSpellSlot(SpellSlot.E).SData.name)
                    
                else if (args.SData.Name == E.ChargedSpellName)
                {
                    if (E.IsReady())
                    {
                        E.Cast();
                        System.Timers.Timer t = new System.Timers.Timer()
                        {
                            Enabled = true,
                            Interval = 3000
                        };

                        t.Elapsed += (object tSender, System.Timers.ElapsedEventArgs tE) =>
                        {
                            Dizzy = true;
                        };

                    }

                }
                else if (args.SData.Name == R.ChargedSpellName)
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
               
                if (Dizzy)
                {
                    Orbwalking.DisableNextAttack = true;
                }
                if (E.IsReady())
                {
                    Dizzy = false;
                }

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
