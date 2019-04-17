/* 
 * Sprite Sheet Packer Engine
 * @author John McWatters
 * An engine to calculate optimal positioning of rectangles in a power of 2 square
 * The heavy lifting is all done by CygonRectanglePacker, 
 * written by Markus Ewald (cygon at nuclex.org)
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using Nuclex.Game.Packing;


namespace spriteSheetPacker
{
    /*
     * Engine class
     */
    public class Engine
    {
        /// <summary>
        /// The packer
        /// </summary>
        CygonRectanglePacker packer;

        /// <summary>
        /// The size of the square, side length
        /// </summary>
        public int size;

        /*
         * Constructor for Engine class
         * @param side, an int of the lenght of a side. Only one as it works with a square
         * @param images, a collection of ImageInfo objects
         * Takes a collection of images and trys to fit them to a square, 
         * increasing the size of square if they don't fit        
         */ 
        public Engine(int side, IEnumerable<ImageInfo> images)
        {
            /// Collection of images not fitting
            List<ImageInfo> dontFit = new List<ImageInfo>();

            /// A check of whether all images are mapped
            bool check = true;
            while (check)
            {
                /// Initalise a packer object
                packer = new CygonRectanglePacker(side, side);

                /// Creating a Point object, for positioning images
                Point placement;

                /// Iterates through images, and trys to place it on the square
                foreach (ImageInfo image in images)
                {
                    /// Checking to see if it fits, else it is added to dontFit
                    if (packer.TryPack(image.Width, image.Height, out placement))
                    {
                        image.Position = placement;
                    }//end if
                    else
                    {
                        dontFit.Add(image);
                    }//end else
                }// end foreach

                /// If images didn't fit, the loop will go again, otherwise it will finsih
                if(dontFit.Count == 0)
                {
                    check = false;
                }// end if
                else
                {
                    side = side * 2;
                    dontFit.Clear();
                }// end else
            }

            /// Initalising size with final square side lenght
            size = side;
        }
    }
}
