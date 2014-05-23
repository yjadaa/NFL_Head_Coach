using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace NFL_Head_Coach
{
    class Team
    {
        public enum Behavior
        {
            Attack = 0,
            Defense = 1
        }

        public Behavior TeamBehavoir;

        public Image TeamLogo;
        public Image TeamImage;
        public List<Player> Member = new List<Player>();
        public List<Player> WR = new List<Player>();
        public string Name;

        

        public void Init(Behavior behavior,GameMode mode,Image img)
        {
            if (mode == GameMode.Mode5v5)
            {
                // Clear member and init new member
                Member = new List<Player>();
                for (int i=0;i<5;i++)
                {
                    Player np = new Player();
                    Image img_Clone = new Image();
                    img_Clone.Source = img.Source.Clone();
                    img_Clone.Width = 32;
                    img_Clone.Height = 32;

                    np.PlayerImage = img_Clone;
                    Member.Add(np); 
                }
                
                if (behavior == Behavior.Attack)
                {
                    Member[0].Role = Player.PlayerRole.Quarter_Back;
                    Member[1].Role = Player.PlayerRole.Wide_Receiver;
                    Member[2].Role = Player.PlayerRole.Wide_Receiver;
                    Member[3].Role = Player.PlayerRole.TackleGurad;
                    Member[4].Role = Player.PlayerRole.TackleGurad;

                    WR.Add(Member[1]);
                    WR.Add(Member[2]);
                }
                else if (behavior == Behavior.Defense)
                {
                    Member[0].Role = Player.PlayerRole.MiddleLineBacker;
                    Member[1].Role = Player.PlayerRole.OutsideLineBacker;
                    Member[2].Role = Player.PlayerRole.OutsideLineBacker;
                    Member[3].Role = Player.PlayerRole.Safety;
                    Member[4].Role = Player.PlayerRole.Safety;
                }
            }
            else if (mode == GameMode.Mode7v7)
            {
                Member.Clear();
                for (int i = 0; i < 7; i++)
                {
                    Member.Add(new Player());
                }
                // Not finish
            }
            else if (mode == GameMode.Mode11v11)
            {
                Member.Clear();
                for (int i = 0; i < 11; i++)
                {
                    Member.Add(new Player());
                }
                // Not finish
            }
        }
        public Player Get( Player.PlayerRole role)
        {
            foreach (Player p in Member)
            {
                if (p.Role == role)
                {
                    return p;
                }
            }
            return null;
        }
    }
}
