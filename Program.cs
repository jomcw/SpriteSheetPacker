/* 
 * John's Sprite Sheet Packer
 * @author John McWatters
 * This application runs in your OS's version of a command line,
 * it can either pack a bunch of pngs into a a single sprite sheet or
 * unpack a sprite sheet you have already made.
 */

using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using Claunia.PropertyList;
using spriteSheetPacker;
using static System.Console;

/* 
 * Application class for Sprite Sheet Packer
 * Takes input from command line, and then completes the appropriate tasks
 */
public class Program
{
    /*
     * Main Method
     * Reads commands and calls methods to complete tasks
     */
    static void Main(string[] args)
    {
        WriteLine("\nWelcome to John's Sprite Sheet Packer");
        WriteLine("What would you like to do? \nHint: you can drag a file into here instead of typing the path!");
        WriteLine("Commands: pack, unpack, help, done");

        /// Checks to see if the user is finished with the application
        bool done = false;

        /// Runs until the user is done, taking input and calling methods
        while (!done)
        {
            ///Taking a command from console
            string cmd = ReadLine();
            switch (cmd)
            {
                //help case, explains the other commands
                case "?":
                case "h":
                case "help":
                    WriteLine("pack   :: You've got some sprites to pack together");
                    WriteLine("unpack :: You packed some sprites together, and want to seperate them");
                    WriteLine("done   :: You're all done packing and unpacking");
                    break;
                // pack command
                case "p":
                case "pack":  // source directory path
                    Pack();
                    break;
                //unpack command
                case "u":
                case "unpack":  // destination directory path
                    UnPack();
                    break;
                // quit command        
                case "d":
                case "done":
                    done = true;
                    WriteLine("Thanks for using John's Sprite Sheet Packer");
                    break;

                default:
                    break;
            }//end switch
        }//end while

    }

    /*
     * Packing function
     * Creates a collection of images, a rectangle object, and sends it through an engine instance, 
     * which plots all images it to a power of 2 square. These are then drawn to a single spritesheet
     */
    public static void Pack()
    {
        ///Creating an instance Rectangles, an collection of images from a file
        Rectangles rectangles = new Rectangles();

        /// Calculates the area of all the images
        int totalAreaAllImages = 0;
        foreach (ImageInfo image in rectangles)
        {
            totalAreaAllImages += image.Width * image.Height;
        }//end foreach

        ///Calculate the closest power of 2 square that could enclose the images
        int side = 2;
        while (side < Math.Sqrt(totalAreaAllImages))
        {
            side = side * 2;
        }//end while

        ///Creating an instance of engine, which plots the image objects to a position on the spritesheet
        Engine engine = new Engine(side, rectangles);

        /// Creating NSObjects to let us create a plist file
        NSDictionary locations;
        NSDictionary root = new NSDictionary();
        NSArray imageArray = new NSArray(rectangles.fileCount);

        /// A bitmap to draw all of the images to
        Bitmap sheet = new Bitmap(engine.size, engine.size);

        /// Drawing images to bitmap sheet
        using (Graphics gfx = Graphics.FromImage(sheet))
        {
            foreach (ImageInfo image in rectangles)
            {
                Write(".");

                //Writing image location data for plist file
                locations = new NSDictionary();
                locations.Add("Width", image.Width);
                locations.Add("Height", image.Height);
                locations.Add("X", image.Position.X);
                locations.Add("Y", image.Position.Y);
                imageArray.Add(locations);

                //Drawing method
                gfx.DrawImage(image.Img, image.Position);

            }//end foreach
        }//end using graphics

        /// Creating plist file of image info on spritesheet
        root.Add("Frames", imageArray);
        PropertyListParser.SaveAsXml(root, new FileInfo(rectangles.outputPath + ".plist"));
        WriteLine("\nSaving:\n{0}", rectangles.outputPath);

        /// Saving spritesheet
        sheet.Save(rectangles.outputPath);
        WriteLine("What would you like to do now?");
    }

    /*
     * UnPack Function
     * Takes a spritesheet and corosponding plist file as input from console, and 
     * seperates the images from the sprite sheet based on their plist information.
     * Paths can be generated by dropping a file into terminal
     */
    public static void UnPack()
    {
        WriteLine("Please enter the sprite sheet source image path:");
        string path = ReadLine();
        path = path.TrimEnd(' ');//cutting white space that can be generated by dragging and dropping into terminal.
        Image source = Image.FromFile(path);
        WriteLine("Please enter the path of the associated plist:");
        string sourcePath = ReadLine();
        sourcePath = sourcePath.TrimEnd(' ');//cutting white space that can be generated by dragging and dropping into terminal.
        WriteLine("Please enter the path of the destination directory:");
        string destPath = ReadLine();
        destPath = destPath.TrimEnd(' ');//cutting white space that can be generated by dragging and dropping into terminal.

        /// Creates destination directory if it does not already exist
        if (!Directory.Exists(destPath))
        {
            Directory.CreateDirectory(destPath);
        }//end if

        /// Reading from plist file into NSObjects
        FileInfo file = new FileInfo(sourcePath);
        NSDictionary rootDict = (NSDictionary)PropertyListParser.Parse(file);
        NSArray array = (Claunia.PropertyList.NSArray)rootDict.ObjectForKey("Frames");

        /// Index count for saving file info
        int i = 0;
        WriteLine("Seperating {0} images", array.Count);

        /// Reading info out of NSObjects into useful types
        foreach (NSDictionary dict in array)
        {
            int X = (int)dict.ObjectForKey("X").ToObject();
            int Y = (int)dict.ObjectForKey("Y").ToObject();
            int Width = (int)dict.ObjectForKey("Width").ToObject();
            int Height = (int)dict.ObjectForKey("Height").ToObject();

            //Creates a new image to be drawn, sourcing from a section of the spritesheet
            Bitmap sheet = new Bitmap(Width, Height);
            using (Graphics gfx = Graphics.FromImage(sheet))
            {

                Rectangle srcRect = new Rectangle(X, Y, Width, Height);//Sets source location on the spritesheet
                Rectangle destRect = new Rectangle(0, 0, Width, Height);

                //Drawing function
                gfx.DrawImage(source, destRect, srcRect, GraphicsUnit.Pixel);
            }//end using graphics
            Write(".");
            sheet.Save(destPath + "/" + i + ".png");
            i++;
        }//end foreach
        WriteLine("\nAll sprites seperated into directory {0}", destPath);
        WriteLine("What would you like to do now?");
    }

}

/*
 * Rectangle Enumerable class
 * Rectangles is a collection of ImageInfo data type
 */
public class Rectangles : IEnumerable<ImageInfo>
{
    /// The practical collection of ImageInfo
    private readonly ImageInfo[] images;

    /// Where the spritesheet will be stored
    public string outputPath;

    /// Count of items in images, useful for creating other collections
    public int fileCount = 0;

    ///Constructor for Rectangles class
    ///Takes no input, as it creates requests for user input
    public Rectangles()
    {
        /// Standard spritesheet name
        string output = "SpriteSheet.png";
        WriteLine("Path of PNG Directory");
        string path = ReadLine();
        path = path.TrimEnd(' ');//cutting white space that can be generated by dragging and dropping into terminal.

        /// A collection of png paths
        string[] files = Directory.GetFiles(path, "*.png");

        /// Setting fileCount field
        fileCount = files.Length;

        /// Intialising images
        images = new ImageInfo[fileCount];

        /// Setting outputPath to something unique
        outputPath = path + output;

        /// Creates a file exists at the output, deletes it
        if (File.Exists(outputPath)) File.Delete(outputPath);

        /// Assigns image info to ImageInfo type collection, images
        if (fileCount > 0)
        {
            WriteLine("{0} files found, analyzing dimensions.", files.Length);// Showing progress

            for (int i = 0; i < fileCount; i++)
            {
                Image img = Image.FromFile(files[i]);
                images[i] = new ImageInfo(img, files[i]);

            }//end for
        }//end if
    }

    /// <summary>
    /// System.s the collections. generic. IE numerable<ImageInfo>. get enumerator.
    /// </summary>
    /// <returns>The collections. generic. IE numerable<ImageInfo>. get enumerator.</returns>
    IEnumerator<ImageInfo> IEnumerable<ImageInfo>.GetEnumerator()
    {
        foreach (ImageInfo image in images)
        {
            yield return image;
        }
    }

    /// <summary>
    /// System.s the collections. IE numerable. get enumerator.
    /// </summary>
    /// <returns>The collections. IE numerable. get enumerator.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return images.GetEnumerator();
    }
}



