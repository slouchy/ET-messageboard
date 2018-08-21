using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MessageBoard.Tools
{
    public class PicTool
    {
        /// <summary>
        /// 檢查檔案大小
        /// </summary>
        /// <param name="httpPostedFile">傳遞的檔案</param>
        /// <param name="allowMaxSize">(選填)允許的檔案大小(預設：1MB)</param>
        /// <returns>回傳結果</returns>
        public static bool isFileSizeAllow(HttpPostedFileBase httpPostedFile, int allowMaxSize = 1)
        {
            return (httpPostedFile.ContentLength < allowMaxSize * 1024 * 1024);
        }

        public static bool isFileExtensionAllow(string fileName, string allowRegexStr)
        {
            Regex regexFile = new Regex(allowRegexStr);
            return regexFile.IsMatch(fileName);
        }

        public static void SaveUserPic(HttpPostedFileBase httpPostedFile, string fileName)
        {
            try
            {
                string filePath = $@"~/UserIcon/{fileName}";
                httpPostedFile.SaveAs(HttpContext.Current.Server.MapPath(filePath));
                Image originImg = Image.FromFile(filePath);
                var formattedImg = AddFrame(GetResizeImage(originImg, 400, 400), 400, 400);
            }
            catch (Exception err)
            {
                LogTool.DoErrorLog($"#{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")}:{err.Message}\r\n");
            }
        }

        /// <summary>
        /// 在圖片四周加入白邊
        /// </summary>
        /// <param name="imgToAddFrame">圖片</param>
        /// <param name="width">圖片寬度</param>
        /// <param name="height">圖片高度</param>
        /// <returns>Image</returns>
        private static Image AddFrame(Image imgToAddFrame, int width, int height)
        {
            Image newImg = imgToAddFrame;
            if (imgToAddFrame.Width < width || imgToAddFrame.Height < height)
            {
                using (Bitmap bitmap = new Bitmap(width, height))
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        SolidBrush solidBrush = new SolidBrush(Color.Transparent);//這裏修改顏色
                        graphics.FillRectangle(solidBrush, 0, 0, width, height);
                        Rectangle rectangle = new Rectangle(0, 0, imgToAddFrame.Width, imgToAddFrame.Height);
                        graphics.DrawImage(imgToAddFrame, (width - imgToAddFrame.Width) / 2, (height - imgToAddFrame.Height) / 2, rectangle, GraphicsUnit.Pixel);
                        newImg = bitmap;
                    }
                }
            }

            return newImg;
        }

        private static Image GetResizeImage(Image imgToResize, int targetWidth, int targetHeight)
        {
            Image newImg = imgToResize;
            ImageFormat format = imgToResize.RawFormat;
            if (imgToResize.Width > targetWidth || imgToResize.Height > targetHeight)
            {
                float percent = Math.Min(targetHeight / (float)imgToResize.Height, targetWidth / (float)imgToResize.Width);
                int destHeight = (int)(imgToResize.Height * percent);
                int destWidth = (int)(imgToResize.Width * percent);
                using (Bitmap bitmap = new Bitmap(destWidth, destHeight))
                {
                    using (Graphics graphics = Graphics.FromImage(imgToResize))
                    {
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
                        newImg = bitmap;
                    }
                }
            }

            return newImg;
        }
    }
}