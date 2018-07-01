namespace imagetool
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            if (!args.Any())
            {
                Console.WriteLine("ImageResize v1.0");
                Console.WriteLine("by Niels Koomans.");
                Console.WriteLine("");
                Console.WriteLine("No arguments specified. Please give a valid path to the file you want to resize and the size you want to resize it to.");
                return;
            }

            if (args[0].Contains("help"))
            {
                Console.WriteLine("ImageResize v1.0");
                Console.WriteLine("by Niels Koomans.");
                Console.WriteLine("");
                Console.WriteLine("imagetool.exe input=[input file name] size=[image size]");
                Console.WriteLine("     Resizes specified input file with the specified size.");
                Console.WriteLine("     Example: imagetool.exe input=dog.png size=24");
                Console.WriteLine("");
                return;
            }

            if (args.Length > 1)
            {
                Console.WriteLine("ImageResize v1.0");
                Console.WriteLine("by Niels Koomans.");
                Console.WriteLine("");
                string input = args[0];
                int sizeInt = Convert.ToInt32(args[1].Split('=')[1]);
                Size size = new Size(sizeInt, sizeInt);

                ResizeImage(input, size);
            }
        }
        private static Size ParseSize(string text)
        {
            int w = Convert.ToInt32(text.Split('x')[0]);
            int h = Convert.ToInt32(text.Split('x')[1]);
            return new Size(w, h);
        }
        private static string GetDirectoryPath(string file)
        {
            string[] pathparts = file.Split('\\');
            string result = string.Empty;

            for (int i = 0; i < pathparts.Length - 1; i++)
                result += pathparts[i] + "\\";

            return result;
        }
        private static string GetFileNameWithoutExt(string file)
        {
            string[] pathparts = file.Split('\\');
            string name = pathparts[pathparts.Length - 1].Split('.')[0];
            return name;
        }
        private static string GetFileExtension(string file)
        {
            string[] pathparts = file.Split('\\');
            string name = pathparts[pathparts.Length - 1].Split('.')[1];
            return name;
        }
        private static void ResizeImage(string ipath, Size size) 
        {
            // Code by Joshua Folkerts (folkertsj) on ASP.NET Forums
            // https://forums.asp.net/t/1186865.aspx?Resizing+Images+with+C+with+no+quality+loss
            // changes (or improvements) were made by me.

            string newPath = $"{GetDirectoryPath(ipath)}{GetFileNameWithoutExt(ipath)}_{size.Width}.{GetFileExtension(ipath)}";
            Console.WriteLine($"Resizing to {newPath} with size {size.Width}{Environment.NewLine}");

            // Load original image and create new bitmap with size parameters
            Image image = Image.FromFile(ipath);
            Bitmap dst = new Bitmap(size.Width, size.Height);
            dst.SetResolution(72, 72);

            // Create graphics object based on previously created bitmap
            Graphics g = Graphics.FromImage(dst);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.CompositingMode = CompositingMode.SourceCopy;

            // Draw image to previously created bitmap
            g.DrawImage(image, 0, 0, size.Width, size.Height);
            g.Dispose();

            // Save with custom codec and encoder parameters
            ImageCodecInfo pngEncoder = GetEncoderInfo(GetFileExtension(ipath).ToUpper());
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Compression, 100);
            dst.Save(newPath, pngEncoder, encoderParameters);
            dst.Dispose();

            image.Dispose();

            Console.WriteLine("Resized succesfully.");
        }
        private static ImageCodecInfo GetEncoderInfo(string v)
        {
            return ImageCodecInfo.GetImageEncoders().Where(f => f.FilenameExtension.Contains(v)).First();
        }
    }
}
