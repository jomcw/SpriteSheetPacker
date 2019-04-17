/* 
 * ImageInfo
 * @author John McWatters
 * Useful data type for storing image information, 
 * allows image locations to be calculated without having to repeatedly draw images.
 */

using System.Drawing;

/*
 * ImageInfo class
 */
public class ImageInfo
{
    /// <summary>
    /// Gets the image.
    /// </summary>
    /// <value>The image.</value>
    public Image Img { get; private set; }

    /// <summary>
    /// Gets the width.
    /// </summary>
    /// <value>The width.</value>
    public int Width { get; private set; }

    /// <summary>
    /// Gets the height.
    /// </summary>
    /// <value>The height.</value>
    public int Height { get; private set; }

    /// <summary>
    /// Gets the filename.
    /// </summary>
    /// <value>The filename.</value>
    public string Filename { get; private set; }

    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    /// <value>The position.</value>
    public Point Position { get; set; }

    /*
     * Constructor of ImageInfo
     * @param Img, an Image data type
     * @param filename, a string of the filename
     * Many of the fields are set by taking info from the image itself, to make it more accessable    
     */
    public ImageInfo(Image img, string filename)
    {
        this.Img = img;
        this.Width = img.Width;
        this.Height = img.Height;
        this.Filename = filename;
    }
}
