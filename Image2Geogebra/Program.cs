using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Image2Geogebra
{
    class Program
    {
        const string Prefix = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<geogebra format=\"5.0\" version=\"5.0.518.0\" app=\"classic\" platform=\"d\" id=\"1ea62ef6-7e82-4847-b18f-ff610bd4273a\"  xsi:noNamespaceSchemaLocation=\"http://www.geogebra.org/ggb.xsd\" xmlns=\"\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" >\r\n<gui>\r\n\t<window width=\"1000\" height=\"750\" />\r\n\t<perspectives>\r\n<perspective id=\"tmp\">\r\n\t<panes>\r\n\t\t<pane location=\"\" divider=\"0.25\" orientation=\"1\" />\r\n\t</panes>\r\n\t<views>\r\n\t\t<view id=\"512\" toolbar=\"0 | 1 501 5 19 , 67 | 2 15 45 18 , 7 37 | 514 3 9 , 13 44 , 47 | 16 | 551 550 11 ,  20 22 21 23 , 55 56 57 , 12 | 69 | 510 511 , 512 513 | 533 531 , 534 532 , 522 523 , 537 536 , 535 | 521 520 | 36 , 38 49 560 | 571 30 29 570 31 33 | 17 | 540 40 41 42 , 27 28 35 , 6 , 502\" visible=\"false\" inframe=\"false\" stylebar=\"false\" location=\"1,1,1\" size=\"500\" window=\"100,100,600,400\" />\r\n\t\t<view id=\"4\" toolbar=\"0 || 2020 , 2021 , 2022 , 66 || 2001 , 2003 , 2002 , 2004 , 2005 || 2040 , 2041 , 2042 , 2044 , 2043\" visible=\"false\" inframe=\"false\" stylebar=\"false\" location=\"1,1\" size=\"300\" window=\"100,100,600,400\" />\r\n\t\t<view id=\"8\" toolbar=\"1001 | 1002 | 1003  || 1005 | 1004 || 1006 | 1007 | 1010 || 1008 1009 || 66 68 || 6\" visible=\"false\" inframe=\"false\" stylebar=\"false\" location=\"1,3\" size=\"300\" window=\"100,100,600,400\" />\r\n\t\t<view id=\"4097\" visible=\"false\" inframe=\"false\" stylebar=\"true\" location=\"1,1\" size=\"395\" window=\"100,100,700,550\" />\r\n\t\t<view id=\"1\" visible=\"true\" inframe=\"false\" stylebar=\"false\" location=\"1\" size=\"716\" window=\"100,100,600,400\" />\r\n\t\t<view id=\"2\" visible=\"true\" inframe=\"false\" stylebar=\"false\" location=\"3\" size=\"250\" window=\"100,100,600,400\" />\r\n\t\t<view id=\"16\" visible=\"false\" inframe=\"false\" stylebar=\"false\" location=\"1\" size=\"150\" window=\"50,50,500,500\" />\r\n\t\t<view id=\"32\" visible=\"false\" inframe=\"false\" stylebar=\"true\" location=\"1\" size=\"150\" window=\"50,50,500,500\" />\r\n\t\t<view id=\"64\" toolbar=\"0\" visible=\"false\" inframe=\"false\" stylebar=\"false\" location=\"1\" size=\"150\" window=\"50,50,500,500\" />\r\n\t\t<view id=\"70\" toolbar=\"0 || 2020 || 2021 || 2022\" visible=\"false\" inframe=\"false\" stylebar=\"true\" location=\"1\" size=\"150\" window=\"50,50,500,500\" />\r\n\t</views>\r\n\t<toolbar show=\"true\" items=\"0 39 73 62 | 1 501 67 , 5 19 , 72 75 76 | 2 15 45 , 18 65 , 7 37 | 4 3 8 9 , 13 44 , 58 , 47 | 16 51 64 , 70 | 10 34 53 11 , 24  20 22 , 21 23 | 55 56 57 , 12 | 36 46 , 38 49  50 , 71  14  68 | 30 29 54 32 31 33 | 25 17 26 60 52 61 | 40 41 42 , 27 28 35 , 6\" position=\"1\" help=\"false\" />\r\n\t<input show=\"true\" cmd=\"true\" top=\"algebra\" />\r\n\t<dockBar show=\"true\" east=\"true\" />\r\n</perspective>\r\n\t</perspectives>\r\n\t<labelingStyle  val=\"0\"/>\r\n\t<font  size=\"16\"/>\r\n</gui>\r\n<euclidianView>\r\n\t<viewNumber viewNo=\"1\"/>\r\n\t<size  width=\"716\" height=\"565\"/>\r\n\t<coordSystem xZero=\"215.0\" yZero=\"315.0\" scale=\"50.0\" yscale=\"50.0\"/>\r\n\t<evSettings axes=\"true\" grid=\"true\" gridIsBold=\"false\" pointCapturing=\"3\" rightAngleStyle=\"1\" checkboxSize=\"26\" gridType=\"3\"/>\r\n\t<bgColor r=\"255\" g=\"255\" b=\"255\"/>\r\n\t<axesColor r=\"0\" g=\"0\" b=\"0\"/>\r\n\t<gridColor r=\"192\" g=\"192\" b=\"192\"/>\r\n\t<lineStyle axes=\"1\" grid=\"0\"/>\r\n\t<axis id=\"0\" show=\"true\" label=\"\" unitLabel=\"\" tickStyle=\"1\" showNumbers=\"true\"/>\r\n\t<axis id=\"1\" show=\"true\" label=\"\" unitLabel=\"\" tickStyle=\"1\" showNumbers=\"true\"/>\r\n</euclidianView>\r\n<algebraView>\r\n\t<mode val=\"3\"/>\r\n</algebraView>\r\n<kernel>\r\n\t<continuous val=\"false\"/>\r\n\t<usePathAndRegionParameters val=\"true\"/>\r\n\t<decimals val=\"2\"/>\r\n\t<angleUnit val=\"degree\"/>\r\n\t<algebraStyle val=\"0\" spreadsheet=\"0\"/>\r\n\t<coordStyle val=\"0\"/>\r\n</kernel>\r\n<tableview min=\"-2.0\" max=\"2.0\" step=\"1.0\"/>\r\n<scripting blocked=\"false\" disabled=\"false\"/>\r\n<construction title=\"\" author=\"\" date=\"\">";
        const string Postfix = "</construction></geogebra>";

        private static string directoryPath;
        private static string geogebra_xmlPath;


        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                log("You need to specify target image :)");
                return 1;
            }
            if (!File.Exists(args[0]))
            {
                log("File does not exist :(");
                return 1;
            }

            Bitmap image = new Bitmap(args[0]);

            #region Comment
            /*
            float width = 80;
            float height = 60;
            var brush = new SolidBrush(Color.White);

            float scale = Math.Min(width / image.Width, height / image.Height);
            Bitmap image2 = new Bitmap((int)width, (int)height);
            var graph = Graphics.FromImage(image2);

            // uncomment for higher quality output
            graph.InterpolationMode = InterpolationMode.High;
            graph.CompositingQuality = CompositingQuality.HighQuality;
            graph.SmoothingMode = SmoothingMode.AntiAlias;


            var scaleWidth = (int)(image.Width * scale);
            var scaleHeight = (int)(image.Height * scale);

            graph.FillRectangle(brush, new RectangleF(0, 0, width, height));
            graph.DrawImage(image, ((int)width - scaleWidth) / 2, ((int)height - scaleHeight) / 2, scaleWidth, scaleHeight);

            image = new Bitmap((int)width, (int)height, graph);
            */
            #endregion

            //Image is normally inverted because of the direction of pixel reading.
            image.RotateFlip(RotateFlipType.Rotate180FlipX);

            InitFile(Path.GetFileNameWithoutExtension(args[0]));

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color pixel = image.GetPixel(x, y);
                    //Ignores white
                    if (!(pixel.R == 255 && pixel.G == 255 && pixel.B == 255))
                    {
                        AddPixelAdvanced(x, y, pixel.R, pixel.G, pixel.B, (float)Math.Round((decimal)pixel.A / 255, 2));
                    }
                }
                Console.Write($"{x+1} // {image.Width} - {Math.Round((decimal)(x+1)/image.Width * 100, 2)}%\r");
            }

            return PackFile() == "" ? 1 : 0;
        }
                
        static void InitFile(string name)
        {
            Directory.CreateDirectory(name);
            geogebra_xmlPath = Path.Combine(Path.GetFullPath(name), "geogebra.xml");
            directoryPath = Path.GetFullPath(name);
            File.WriteAllText(geogebra_xmlPath, Prefix);

            directoryPath = Path.GetFullPath(directoryPath);
            File.WriteAllText(Path.Combine(directoryPath, "geogebra_javascript.js"), "function ggbOnInit() {}");
        }

        static void AddToFile(string data)
        {
            using(StreamWriter sw = File.AppendText(geogebra_xmlPath))
                sw.Write(data);
        }

        static string PackFile()
        {
            AddToFile(Postfix);
            ZipFile.CreateFromDirectory(directoryPath, directoryPath + ".ggb");
            Directory.Delete(directoryPath, true);

            return File.Exists(directoryPath + ".ggb") ? directoryPath + ".ggb" : "";
        }        

        static void AddPixel_old(int x, int y, int red, int green, int blue)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("" +
                $"<command name=\"Polygon\">" +
                $"<input a0=\"({x}, {y})\" a1=\"({x + 1}, {y})\" a2=\"({x + 1}, {y + 1})\" a3=\"({x}, {y + 1})\" />" +
                $"<output a0=\"q1{x}{y}\" a1=\"f{x}{y}\" a2=\"g{x}{y}\" a3=\"h{x}{y}\" a4=\"i{x}{y}\" />" +
                "</command>\n");
            
            sb.Append("" +
                $"<element type=\"polygon\" label=\"q1{x}{y}\">" +
                $"<lineStyle thickness=\"5\" type=\"0\" typeHidden=\"1\" opacity=\"178\"/>" +
                $"<show object=\"true\" label=\"false\"/>" +
                $"<objColor r=\"{red}\" g=\"{green}\" b=\"{blue}\" alpha=\"1.0\"/>" +
                $"<layer val=\"0\"/>" +
                $"<labelMode val=\"0\"/>" +
                "</element>");
            AddToFile(sb.ToString());
            //geogebraData.Add(sb.ToString());
        }

        static void AddPixelAdvanced(int x, int y, int red, int green, int blue, float alpha = 1.0f)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("" +
                $"<command name=\"Polygon\">" +
                $"<input a0=\"({x}, {y})\" a1=\"({x + 1}, {y})\" a2=\"({x + 1}, {y + 1})\" a3=\"({x}, {y + 1})\" />" +
                $"<output a0=\"q1{x}{y}\" a1=\"f{x}{y}\" a2=\"g{x}{y}\" a3=\"h{x}{y}\" a4=\"i{x}{y}\" />" +
                "</command>");

            sb.Append("" +
                $"<element type=\"polygon\" label=\"q1{x}{y}\">" +
                $"<lineStyle thickness=\"5\" type=\"0\" typeHidden=\"1\" opacity=\"178\"/>" +
                $"<show object=\"true\" label=\"false\"/>" +
                $"<objColor r=\"{red}\" g=\"{green}\" b=\"{blue}\" alpha=\"{alpha}\"/>" + //1.0
                $"<layer val=\"0\"/>" +
                $"<labelMode val=\"0\"/>" +
                "</element>");

            sb.Append("" +
                $"<element type=\"segment\" label=\"f{x}{y}\">" +
                "<show object=\"false\" label=\"true\" />" +
                $"<objColor r=\"{red}\" g=\"{green}\" b=\"{blue}\" alpha=\"1.0\"/>" +
                "<layer val=\"0\"/>" +
                "<labelMode val=\"0\" />" +
                "<auxiliary val=\"false\" />" +
                $"<coords x=\"{(float)x}\" y=\"{(float)(y+1)}\" z=\"0.0\" />" +
                "<lineStyle thickness=\"5\" type=\"0\" typeHidden=\"1\" opacity=\"178\" />" +
                "<outlyingIntersections val=\"false\" />" +
                "<keepTypeOnTransform val=\"true\" />" +
                "</element>");

            sb.Append("" +
                $"<element type=\"segment\" label=\"g{x}{y}\">" +
                "<show object=\"false\" label=\"true\" />" +
                $"<objColor r=\"{red}\" g=\"{green}\" b=\"{blue}\" alpha=\"1.0\"/>" +
                "<layer val=\"0\"/>" +
                "<labelMode val=\"0\" />" +
                "<auxiliary val=\"false\" />" +
                $"<coords x=\"{(float)(x-1)}\" y=\"{(float)y}\" z=\"0.0\" />" +
                "<lineStyle thickness=\"5\" type=\"0\" typeHidden=\"1\" opacity=\"178\" />" +
                "<outlyingIntersections val=\"false\" />" +
                "<keepTypeOnTransform val=\"true\" />" +
                "</element>");

            sb.Append("" +
                $"<element type=\"segment\" label=\"h{x}{y}\">" +
                "<show object=\"false\" label=\"true\" />" +
                $"<objColor r=\"{red}\" g=\"{green}\" b=\"{blue}\" alpha=\"1.0\"/>" +
                "<layer val=\"0\"/>" +
                "<labelMode val=\"0\" />" +
                "<auxiliary val=\"false\" />" +
                $"<coords x=\"{(float)x}\" y=\"{(float)(y-1)}\" z=\"0.0\" />" +
                "<lineStyle thickness=\"5\" type=\"0\" typeHidden=\"1\" opacity=\"178\" />" +
                "<outlyingIntersections val=\"false\" />" +
                "<keepTypeOnTransform val=\"true\" />" +
                "</element>");

            sb.Append("" +
                $"<element type=\"segment\" label=\"i{x}{y}\">" +
                "<show object=\"false\" label=\"true\" />" +
                $"<objColor r=\"{red}\" g=\"{green}\" b=\"{blue}\" alpha=\"1.0\"/>" +
                "<layer val=\"0\"/>" +
                "<labelMode val=\"0\" />" +
                "<auxiliary val=\"false\" />" +
                $"<coords x=\"{(float)(x+1)}\" y=\"{(float)y}\" z=\"0.0\" />" +
                "<lineStyle thickness=\"5\" type=\"0\" typeHidden=\"1\" opacity=\"178\" />" +
                "<outlyingIntersections val=\"false\" />" +
                "<keepTypeOnTransform val=\"true\" />" +
                "</element>");

            AddToFile(sb.ToString());
            //geogebraData.Add(sb.ToString());
        }

        private static void log(object stdout) => Console.WriteLine(stdout);
    }
}

/* // I am bad at saying good bye :/
private static void GenerateFile(string filename)
{
    while (Directory.Exists(filename) && filename.Length <= 259)
        filename += "_";

    Directory.CreateDirectory(filename);

    XmlDocument xmldoc = new XmlDocument();
    xmldoc.LoadXml(Prefix + String.Join("", geogebraData) + Postfix);

    File.WriteAllText(Path.Combine(filename, "geogebra_javascript.js"), "function ggbOnInit() {}");
    File.WriteAllText(Path.Combine(filename, "geogebra.xml"), xmldoc.OuterXml);

    if (File.Exists(filename + ".ggb"))
        File.Delete(filename + ".ggb");
    ZipFile.CreateFromDirectory(filename, filename + ".ggb");
    Directory.Delete(filename, true);
}*/
