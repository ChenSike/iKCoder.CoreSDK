using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.DrawingCore;

namespace iKCoderSDK
{
    public class Basic_CheckCode
    {

		
		public static string NewCode(int length = 4)
		{
			Random objRnd = new Random();
			string rndCodeResult = "";
			for (int i = 1; i <= length; i++)
			{
				int value = objRnd.Next(1, 9);
				rndCodeResult = rndCodeResult + value.ToString();
			}
			return rndCodeResult;
		}



		public static MemoryStream NewCheckCode(out string code, int length = 4, int codewidth = 72, int codeheight = 32)
		{
			code = NewCode(length);
			Bitmap objImg = null;
			Graphics objGraphics = null;
			Random random = new Random();
			MemoryStream objMemStream = null;
			Random objRnd = new Random();
			System.DrawingCore.Color[] activeColors = { System.DrawingCore.Color.Black, System.DrawingCore.Color.Red, System.DrawingCore.Color.DarkBlue, System.DrawingCore.Color.Green, System.DrawingCore.Color.Orange, System.DrawingCore.Color.Brown, System.DrawingCore.Color.Purple };
			string[] fonts = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial" };
			objImg = new Bitmap(codewidth, codeheight);
			objGraphics = Graphics.FromImage(objImg);
			objGraphics.Clear(System.DrawingCore.Color.White);
			for (int i = 0; i < 100; i++)
			{
				int x = random.Next(objImg.Width);
				int y = random.Next(objImg.Height);
				objGraphics.DrawRectangle(new Pen(System.DrawingCore.Color.LightGray, 0), x, y, 1, 1);
			}
			for (int i = 0; i < code.Length; i++)
			{
				Font f = new Font(fonts[3], 15, FontStyle.Bold);
				Brush b = new SolidBrush(activeColors[0]);
				int ii = 4;
				if ((i + 1) % 2 == 0)
				{
					ii = 2;
				}
				objGraphics.DrawString(code.Substring(i, 1), f, b, 3 + (i * 12), ii);
			}
			objMemStream = new MemoryStream();
			objImg.Save(objMemStream, System.DrawingCore.Imaging.ImageFormat.Png);
			objGraphics.Dispose();
			objImg.Dispose();
			return objMemStream;
		}
	}
}
