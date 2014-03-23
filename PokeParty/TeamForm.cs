/**
 * This file is part of PokeParty.
 * 
 * Copyright (C) 2014 Ashlee Katzenbaer
 * All Rights Reserved.
 * 
 * @github im420blaziken
 *  
 * PokeParty is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * PokeParty is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with PokeParty.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Resources;
using System.Drawing.Drawing2D;
using System.Threading;

namespace PokeParty
{
    public partial class TeamForm : Form
    {
        private List<PictureBox> slots = new List<PictureBox>();
        private List<Label> slotLevels = new List<Label>();

        private List<Image>[] _images = new List<Image>[6];

        public TeamForm(Orientation layout)
        {
            InitializeComponent();

            this.Orientation = layout;
        }

        private void PokemonParty_Load(object sender, EventArgs e)
        {
            slots.Add(pbSlotOne);
            slots.Add(pbSlotTwo);
            slots.Add(pbSlotThree);
            slots.Add(pbSlotFour);
            slots.Add(pbSlotFive);
            slots.Add(pbSlotSix);
            slotLevels.Add(lblSlotOneLevel);
            slotLevels.Add(lblSlotTwoLevel);
            slotLevels.Add(lblSlotThreeLevel);
            slotLevels.Add(lblSlotFourLevel);
            slotLevels.Add(lblSlotFiveLevel);
            slotLevels.Add(lblSlotSixLevel);

            foreach (PictureBox pb in this.slots) {
                pb.BackColor = this.BackColor;
                pb.ForeColor = this.ForeColor;
            }
            foreach (Label label in this.slotLevels)
            {
                label.BackColor = this.pbLevelBackground.BackColor;
                label.ForeColor = this.pbLevelBackground.ForeColor;
            }

            for (int i = 0; i < this._images.Count(); i++) _images[i] = new List<Image>();

            this.Orientation = this.Orientation; // Refresh layout
        }

        public Color ImageBackColor
        {
            get {
                return this.BackColor;
            }
            set {
                this.BackColor = value;
                foreach (PictureBox pb in this.slots)
                {
                    pb.BackColor = value;
                }
            }
        }

        private Color _textSubColor;
        public Color TextSubColor
        {
            get
            {
                return this._textSubColor;
            }
            set
            {
                this._textSubColor = value;
                foreach (Label label in this.slotLevels)
                {
                    if (label.ForeColor != this.TextForeColor) label.ForeColor = value;
                }
            }
        }

        public Color TextBackColor
        {
            get {
                return this.pbLevelBackground.BackColor;
            }
            set {
                this.pbLevelBackground.BackColor = value;
                foreach (Label label in this.slotLevels)
                {
                    label.BackColor = value;
                }
            }
        }

        public Color TextForeColor
        {
            get
            {
                return this.pbLevelBackground.ForeColor;
            }
            set
            {
                foreach (Label label in this.slotLevels)
                {
                    if (label.ForeColor == this.pbLevelBackground.ForeColor) label.ForeColor = value;
                }
                this.pbLevelBackground.ForeColor = value;
            }
        }

        private Orientation _Orientation = Orientation.Horizontal;
        public Orientation Orientation
        {
            get
            {
                return _Orientation;
            }
            set
            {
                this._Orientation = value;
                if (value == Orientation.Horizontal)
                {
                    int maxwidth = 0;
                    int maxheight = 0;
                    int width = 0;

                    // Move Team sprites
                    for (int i = 0; i < this.slots.Count(); i++)
                    {
                        PictureBox pb = this.slots[i];
                        if (i == 0)
                        {
                            maxheight = pb.Height;
                            width = pb.Width;
                        }
                        pb.Location = new Point((width + 6) * i, 0);
                    }
                    maxwidth = this.slots.Count() * (width + 6) - 6;

                    // Move Level sprites
                    this.pbLevelBackground.Location = new Point(0, maxheight);
                    for (int i = 0; i < this.slotLevels.Count(); i++)
                    {
                        Label label = this.slotLevels[i];
                        label.Location = new Point((width + 6) * i, maxheight);
                        if (i + 1 == this.slotLevels.Count())
                        {
                            maxheight += label.Height;
                            maxwidth += label.Width - width;
                        }
                        label.BringToFront();
                    }
                    this.pbLevelBackground.SendToBack();

                    maxwidth = Math.Max(32, maxwidth);
                    maxheight = Math.Max(32, maxheight);
                    // Set Form size
                    this.ClientSize = new Size(maxwidth, maxheight);
                }
                else if (value == Orientation.Vertical)
                {
                    int maxwidth = 0;
                    int maxheight = 0;
                    int height = 0;

                    // Move Team sprites
                    for (int i = 0; i < this.slots.Count(); i++)
                    {
                        PictureBox pb = this.slots[i];
                        if (i == 0)
                        {
                            maxwidth = pb.Width;
                            height = pb.Height;
                        }
                        pb.Location = new Point(0, (height + 6) * i);
                    }
                    maxheight = this.slots.Count() * (height + 6) - 6;

                    // Move Level sprites
                    this.pbLevelBackground.Location = new Point(maxwidth, 0);
                    for (int i = 0; i < this.slotLevels.Count(); i++)
                    {
                        Label label = this.slotLevels[i];
                        label.Location = new Point(maxwidth, (height + 6) * i);
                        if (i + 1 == this.slotLevels.Count()) {
                            maxwidth += label.Width;
                            maxheight += label.Height - height;
                        }
                        label.SendToBack();
                    }
                    this.pbLevelBackground.SendToBack();

                    maxwidth = Math.Max(32, maxwidth);
                    maxheight = Math.Max(32, maxheight);
                    // Set Form size
                    this.ClientSize = new Size(maxwidth, maxheight);
                }
            }
        }

        public bool ShowTeam(List<Pokemon> pokes)
        {
            foreach (PictureBox slot in slots)
            {
                // Stop existing animations
                AnimatedSprite sprite = (slot.Tag is AnimatedSprite) ? (AnimatedSprite)slot.Tag : null;
                if (sprite == null) continue;
                else
                {
                    sprite.StopAnimation();
                    slot.Invalidate();
                }
            }

            try
            {
                foreach (Pokemon p in pokes)
                {
                    if (p.Species == 0) continue;
                    Console.WriteLine("Found Pkmn #" + p.Species + "!");

                    //string sprite_name = p.DexNumber.ToString().PadLeft(3, '0') + ".png";
                    string local_resource = @"poke_sprites/" + p.Name.ToLower() + ".gif";
                    if (!Directory.Exists(@"poke_sprites"))
                    {
                        Console.WriteLine("PokemonParty sprites directory does not exist. Creating one...");
                        Directory.CreateDirectory(@"poke_sprites");
                    }
                    if (!File.Exists(local_resource))
                    {
                        /*using (WebClient Client = new WebClient())
                        {
                            Client.DownloadFile(@"http://www.serebii.net/pokedex-xy/icon/" + sprite_name, sprite_file);
                        }*/
                        //string remote_url = @"http://www.pkparaiso.com/imagenes/xy/sprites/animados/";
                        string remote_url = @"http://play.pokemonshowdown.com/sprites/xyani/";
                        using (MinimalWebClient Client = new MinimalWebClient())
                        {
                            string remote_resource_name = p.Name.ToLower() + ".gif";
                            Client.DownloadFile(remote_url + remote_resource_name, @"poke_sprites/" + remote_resource_name);
                        }
                        /*using (HeadClient Client = new HeadClient())
                        {
                            for (int i = 2; i <= 5; i++)
                            {
                                string remote_resource_name = p.Name.ToLower() + "-" + i.ToString() + ".gif";
                                Client.HeadOnly = true;
                                try
                                {
                                    Client.DownloadData(remote_url + remote_resource_name);
                                    Client.HeadOnly = false;
                                    Client.DownloadFile(remote_url + remote_resource_name, @"poke_sprites/" + remote_resource_name);
                                }
                                catch
                                {
                                    Console.WriteLine("Couldn't find remote resource: " + remote_resource_name);
                                    break;
                                }
                            }
                        }*/
                        Console.WriteLine("Done downloading available sprites.");
                    }
                }
            }
            catch
            {
                Console.WriteLine("Unable to download pkmn team sprites");
            }

            try
            {
                for (int i = 0; i < pokes.Count; i++)
                {

                    //string sprite_name = pokes[i].DexNumber.ToString().PadLeft(3, '0') + ".png";
                    string sprite_name = pokes[i].Name.ToLower() + ".gif";
                    string sprite_file = @"poke_sprites/" + sprite_name;
                    //slots[i].ImageLocation = sprite_file;

                    if (!File.Exists(sprite_file))
                    {
                        Console.WriteLine("Sprite file does not exist: " + sprite_file);
                    }
                    else
                    {
                        Image image = null;
                        try
                        {
                            image = AnimatedSprite.CleanImageFromFile(sprite_file);
                        }
                        catch
                        {
                            Console.WriteLine("Error loading sprite image.");
                        }

                        _images[i].Clear();
                        if (image != null)
                        {
                            _images[i].Add(image);
                            /*for (int alt = 2; alt <= 5; alt++)
                            {
                                string local_resource_name = @"poke_sprites/" + pokes[i].Name.ToLower() + "-" + alt.ToString() + ".gif";
                                if (File.Exists(local_resource_name))
                                {
                                    _images[i].Add(Image.FromFile(local_resource_name));
                                }
                                else break;
                            }*/

                            // Let Paint draw it.
                            slots[i].Image = null;
                            try
                            {
                                slots[i].Tag = new AnimatedSprite(slots[i], _images[i]);
                            }
                            catch
                            {
                                slots[i].Tag = null;
                                Console.WriteLine("Unable to create AnimatedSprite.");
                            };
                        }
                    }

                    int max_chars = slotLevels[i].Width / 8;
                    string hp_percent = ((int)((double)pokes[i].CurrentHP / (double)pokes[i].TotalHP * (double)100)).ToString() + "%";
                    string status_condition = (pokes[i].CurrentHP == 0) ? "FNT" : Pokemon.GetStatusShorthand(pokes[i].ProminentStatusCondition);

                    // Pokemon Levels
                    Action updateInterface = () =>
                    {
                        if (pokes[i].Level != 0)
                        {
                            slotLevels[i].Text = (pokes[i].Nickname != null) ? pokes[i].Nickname : "";
                            slotLevels[i].Text += Environment.NewLine + (":L" + pokes[i].Level.ToString()).PadRight(max_chars - status_condition.Length) + status_condition;
                            slotLevels[i].Text += Environment.NewLine + ":HP".PadRight(max_chars - hp_percent.Length) + hp_percent;

                            if (pokes[i].CurrentHP == 0) slotLevels[i].ForeColor = this.TextSubColor;
                            else if (pokes[i].StatusCondition != Pokemon.StatusType.NONE) slotLevels[i].ForeColor = Pokemon.GetStatusColor(pokes[i].ProminentStatusCondition);
                            else slotLevels[i].ForeColor = this.TextForeColor;
                        }
                        else slotLevels[i].Text = ":Egg";
                        slotLevels[i].Visible = true;
                    };

                    if (this.InvokeRequired) this.Invoke(updateInterface);
                    else if (Program.IsMainThread) updateInterface();
                }

                for (int i = pokes.Count; i < 6; i++)
                {
                    Action hideLabel = () =>
                    {
                        slotLevels[i].Visible = false;
                    };

                    if (this.InvokeRequired) this.Invoke(hideLabel);
                    else if (Program.IsMainThread) hideLabel();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to set pkmn team sprites: " + e.Message);
            }
            return true;
        }
    }
}
