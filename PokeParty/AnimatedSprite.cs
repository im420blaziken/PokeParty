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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

namespace PokeParty
{
    class AnimatedSprite
    {
        private List<Image> _images;
        private Image _currentImage;
        private int _currentImagesIndex;
        private PictureBox _picturebox;
        private EventHandler _eh;
        private PaintEventHandler _peh;
        private int _currentFrame = 0;
        private int _maxFrames = 0;
        private bool _isAnimating = false;
        Random _rand = new Random();
        private object _framedimen_lock = new object();

        public AnimatedSprite(PictureBox p, List<Image> i)
        {
            if (p == null || i == null || i.First() == null) throw new ArgumentNullException();

            this._images = new List<Image>(i);
            this._picturebox = p;
            this._eh = new EventHandler(this.OnFrameChanged);
            this._currentImage = this._images.First();
            this._currentImagesIndex = 0;
            Thread start_thread = new Thread(() => StartAnimation());
            start_thread.IsBackground = true;
            start_thread.Start();
        }

        private void StartAnimation()
        {
            Thread.CurrentThread.Name = "StartAnimation";
            if (_isAnimating)
            {
                // Get random image
                int randi = _rand.Next(this._images.Count() + 2); // Give preference to the idle animation.
                if (randi >= this._images.Count()) randi = 0;     // Looks weird when Pokemon swipe continuously.
                if (randi != this._currentImagesIndex)
                {
                    StopAnimation();
                    
                    this._currentFrame = 0;
                    this._maxFrames = 0;

                    this._currentImage = this._images[randi];
                }
                else return;
            }

            lock (_framedimen_lock)
            {
                try
                {
                    FrameDimension dimension = new FrameDimension(this._currentImage.FrameDimensionsList[0]);
                    this._maxFrames = this._currentImage.GetFrameCount(dimension);
                    ImageAnimator.Animate(this._currentImage, this._eh);
                    this._picturebox.Paint += (this._peh = new System.Windows.Forms.PaintEventHandler(this.Paint));
                    _isAnimating = true;
                }
                catch (System.InvalidOperationException e)
                {
                    Console.WriteLine("InvalidOperationException while getting dimension.");
                }
            }
            
        }

        public void StopAnimation()
        {
            if (_isAnimating)
            {
                _isAnimating = false;
                ImageAnimator.StopAnimate(this._currentImage, this._eh);
                 this._picturebox.Paint -= this._peh;
            }
        }

        public void OnFrameChanged(object o, EventArgs e)
        {
            this._picturebox.Invalidate();

            if (_isAnimating)
            {
                if (_maxFrames > 0 && _maxFrames == ++this._currentFrame)
                {
                    Thread start_thread = new Thread(() => StartAnimation());
                    start_thread.IsBackground = true;
                    start_thread.Start();
                }
            }
        }

        public static Image FixedSize(Image imgPhoto, int Width, int Height, Color? backColor = null)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);
            if (nPercentH < nPercentW) // height is constraining
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((Width -
                              (sourceWidth * nPercent)) / 2);
            }
            else // width is constraining
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((Height -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            if (sourceHeight <= 64 || sourceWidth <= 64)
            {
                int _destWidth = (int)((double)destWidth * 0.75);
                int _destHeight = (int)((double)destHeight * 0.75);
                if (nPercent.Equals(nPercentH))
                {
                    destX += (destWidth - _destWidth) / 2;
                    destY += destHeight - _destHeight;
                }
                else if (nPercent.Equals(nPercentW))
                {
                    destX += (destWidth - _destWidth) / 2;
                    destY += destHeight - _destHeight;
                }
                destWidth = _destWidth;
                destHeight = _destHeight;

                destY = Height - destHeight;
            }

            Bitmap bmPhoto = new Bitmap(Width, Height,
                              PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                             imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(backColor ?? Color.Transparent);
            grPhoto.InterpolationMode =
                    System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

        public void Paint(object sender, PaintEventArgs e)
        {
            if (_isAnimating)
            {
                try
                {
                    ImageAnimator.UpdateFrames();
                    Image image = this._currentImage;

                    var g = e.Graphics;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

                    /* pokemonshowdown has properly cropped sprites.
                     * Rectangle dstRect = new Rectangle(-8, -4, _picturebox.Width, _picturebox.Height);
                     * Rectangle srcRect = new Rectangle((image.Width - 96) / 2, image.Height - 96 - ((image.Height > 100) ? 26 : 0), 96, 96);
                     */
                    /* Rectangle dstRect = new Rectangle(0, 0, _picturebox.Width, _picturebox.Height);
                     * Rectangle srcRect = new Rectangle((image.Width - _picturebox.Width) / 2, image.Height - _picturebox.Height, _picturebox.Width, _picturebox.Height);
                     * Rectangle srcRect = new Rectangle(0, 0, image.Width, image.Height);
                     * g.DrawImage(image, dstRect, srcRect, GraphicsUnit.Pixel); */
                    g.DrawImage(FixedSize(image, _picturebox.Width, _picturebox.Height, _picturebox.BackColor),new Point(0, 0));
                }
                catch (Exception ex) {
                    Console.WriteLine("Error while painting: " + ex);
                }
            }
        }

        public static Image CleanImageFromFile(string path) // Loads an image without resource locks.
        {
            var bytes = File.ReadAllBytes(path);
            var ms = new MemoryStream(bytes);
            var img = Image.FromStream(ms);
            return img;
        }
    }
}
