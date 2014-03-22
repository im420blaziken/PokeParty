using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokeParty
{
    public partial class BadgesForm : Form
    {
        private int _maxBadges = 0;
        private byte _badgeFlags = 0;
        private List<PictureBox> _badges = new List<PictureBox>();
        private List<string> _badgeNames = new List<string>();
        private List<AnimatedSprite> _animators = new List<AnimatedSprite>();

        public BadgesForm(Orientation orientation)
        {
            InitializeComponent();

            this.Orientation = orientation;
        }

        public string BadgeName(int i)
        {
            if (i >= _badgeNames.Count()) return null;
            else return _badgeNames[i];
        }

        public int MaxBadges
        {
            get
            {
                return _maxBadges;
            }
        }

        private void LayoutBadges(Orientation value) {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(
                    () =>
                    {
                        foreach (PictureBox pb in this._badges) this.Controls.Remove(pb);
                    })
                );
            }
            else if (Program.IsMainThread)
            {
                foreach (PictureBox pb in this._badges) this.Controls.Remove(pb);
            }
            
            this._badges.Clear();
            this._animators.Clear();

            int badgewidth = 50, badgeheight = 50;
            int maxwidth = 0, maxheight = 0;
            if (value == Orientation.Horizontal)
            {
                maxwidth = this.MaxBadges * (badgewidth + 3) - 3;
                maxheight = badgeheight;

                for (int i = 0; i < this.MaxBadges; i++)
                {
                    if ((_badgeFlags & (1 << i)) > 0)
                    {
                        PictureBox pb = new PictureBox();
                        pb.Size = new Size(badgewidth, badgeheight);
                        pb.SizeMode = PictureBoxSizeMode.CenterImage;
                        pb.Location = new Point(i * (badgewidth + 3), 0);
                        pb.Tag = BadgeName(i);
                        this._badges.Add(pb);
                    }
                }
            }
            else if (value == Orientation.Vertical)
            {
                maxwidth = badgewidth;
                maxheight = this.MaxBadges * (badgeheight + 3) - 3;

                for (int i = 0; i < this.MaxBadges; i++)
                {
                    if ((_badgeFlags & (1 << i)) > 0)
                    {
                        PictureBox pb = new PictureBox();
                        pb.Size = new Size(badgewidth, badgeheight);
                        pb.SizeMode = PictureBoxSizeMode.CenterImage;
                        pb.Location = new Point(0, i * (badgeheight + 3));
                        pb.Tag = BadgeName(i);
                        this._badges.Add(pb);
                    }
                }
            }

            maxwidth = Math.Max(32, maxwidth);
            maxheight = Math.Max(32, maxheight);

            if (this.InvokeRequired)
            {
                this.Invoke(new Action(
                    () =>
                    {
                        this.ClientSize = new Size(maxwidth, maxheight);
                        foreach (PictureBox pb in this._badges) this.Controls.Add(pb);
                    })
                );
            }
            else if (Program.IsMainThread)
            {
                this.ClientSize = new Size(maxwidth, maxheight);
                foreach (PictureBox pb in this._badges) this.Controls.Add(pb);
            }
        }

        Orientation _orientation;
        public Orientation Orientation
        {
            get { return _orientation; }
            set {
                this._orientation = value;

                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(
                        () =>
                        {
                            LayoutBadges(value);
                        })
                    );
                }
                else if (Program.IsMainThread)
                {
                    LayoutBadges(value);
                }
                ShowBadges(this._badgeFlags, this._game);
            }
        }

        private GameType _game = GameType.UNKNOWN;
        public void ShowBadges(byte badges, GameType game)
        {
            this._badgeFlags = badges;
            this._game = game;

            if (game == GameType.CRYSTAL) // Include GOLD & SILVER
                _maxBadges = 16;
            else
                _maxBadges = 8;

            _badgeNames.Clear();
            switch (game)
            {
                case GameType.EMERALD:
                    _badgeNames.AddRange(new List<string>() { "Stone Badge", "Knuckle Badge", "Dynamo Badge", "Heat Badge", "Balance Badge", "Feather Badge", "Mind Badge", "Rain Badge" });
                    break;
                // TODO Other games
            }

            LayoutBadges(this._orientation); // refresh data

            try
            {
                for (int i = 0; i < this._badges.Count(); i++) {
                    PictureBox badge = this._badges[i];
                    if (badge.Tag == null) continue;

                    Console.WriteLine("Found " + badge.Tag + "! At location: " + badge.Location.ToString() + ", size: " + badge.Size.ToString());

                    string webname = badge.Tag.ToString().Split(' ').First().ToLower();

                    string local_resource = @"poke_sprites/" + webname + ".png";
                    if (!Directory.Exists(@"poke_sprites"))
                    {
                        Console.WriteLine("PokemonParty sprites directory does not exist. Creating one...");
                        Directory.CreateDirectory(@"poke_sprites");
                    }
                    if (!File.Exists(local_resource))
                    {
                        string remote_url = @"http://stegriff.co.uk/content/badges/files/RSE/";
                        using (MinimalWebClient Client = new MinimalWebClient())
                        {
                            string remote_resource_name = webname + ".png";
                            Client.DownloadFile(remote_url + remote_resource_name, @"poke_sprites/" + remote_resource_name);
                        }
                    }

                    if (File.Exists(local_resource))
                    {
                        Image image = null;
                        try
                        {
                            image = AnimatedSprite.CleanImageFromFile(local_resource);
                        }
                        catch
                        {
                            Console.WriteLine("Error loading sprite image.");
                        }

                        if (image == null) Console.WriteLine("Loaded null badge image.");

                        try
                        {
                            image = AnimatedSprite.FixedSize(image, badge.Width - 16, badge.Height - 16, this.BackColor);

                            if (this.InvokeRequired)
                            {
                                this.Invoke(new Action(
                                    () =>
                                    {
                                        badge.Image = image;
                                        Console.WriteLine("is not main thread.");
                                    })
                                );
                            }
                            else if (Program.IsMainThread)
                            {
                                badge.Image = image;
                                Console.WriteLine("is main thread.");
                            }
                            else
                            {
                                Console.WriteLine("is not main thread2.");
                            }
                            Console.WriteLine("Showing Badge!");
                        }
                        catch
                        {
                            Console.WriteLine("Unable to crop badge.");
                        };
                    }
                    else
                        Console.WriteLine("Unable to download badge sprites, even though the url exists?");
                }
            }
            catch
            {
                Console.WriteLine("Unable to download badges sprites");
            }
        }

        public override Color BackColor
        {
            set
            {
                base.BackColor = value;
                if (_game != GameType.UNKNOWN) ShowBadges(_badgeFlags, _game);
            }
            get { return base.BackColor; }
        }
    }
}
