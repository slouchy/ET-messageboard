using MessageBoard.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
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

        /// <summary>
        /// 檢查附檔名是否允許
        /// </summary>
        /// <param name="fileName">檔案名稱</param>
        /// <param name="allowRegexStr">待檢查的附檔名正則表達式</param>
        /// <returns>回傳結果</returns>
        public static bool isFileExtensionAllow(string fileName, string allowRegexStr)
        {
            return Regex.IsMatch(fileName, allowRegexStr);
        }

        /// <summary>
        /// 儲存使用者圖示
        /// </summary>
        /// <param name="httpPostedFile">傳遞的檔案</param>
        /// <param name="userID">儲存檔案名稱</param>
        public static void SaveUserPic(HttpPostedFileBase httpPostedFile, int userID)
        {
            try
            {
                string realFileViturePath = $"UserIcon/UserIcon_{userID}{Path.GetExtension(httpPostedFile.FileName)}";
                string tmpFilePath = HttpContext.Current.Server.MapPath($@"~/UserIcon/2_UserIcon{userID}{Path.GetExtension(httpPostedFile.FileName)}");
                string realFilePath = HttpContext.Current.Server.MapPath($@"~/{realFileViturePath}");
                if (File.Exists(tmpFilePath))
                {
                    File.Delete(HttpContext.Current.Server.MapPath(tmpFilePath));
                }

                httpPostedFile.SaveAs(tmpFilePath);
                using (Image originImg = Image.FromFile(tmpFilePath))
                {
                    using (var formattedImg = AddFrame(GetResizeImage(originImg, 400, 400), 400, 400))
                    {
                        formattedImg.Save(realFilePath);
                    }
                }

                if (File.Exists(tmpFilePath))
                {
                    File.Delete(tmpFilePath);
                    MessageBoardEntities messageBoardEntities = new MessageBoardEntities();
                    var user = messageBoardEntities.UserList.Find(userID);
                    user.UserIcon = realFileViturePath;
                    messageBoardEntities.SaveChanges();
                }
                GC.Collect();
            }
            catch (Exception err)
            {
                LogTool.DoErrorLog($"#{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")}:{err.Message}\r\n{err.StackTrace}\r\n");
            }
        }

        public static void SaveMessagePic(HttpPostedFileBase httpPostedFile, int userID, int messageID)
        {
            try
            {
                string fileName = Guid.NewGuid().ToString();
                string realFileViturePath = $"messagePics/origin/{fileName}{Path.GetExtension(httpPostedFile.FileName)}";
                string tmpFilePath = HttpContext.Current.Server.MapPath($@"~/messagePics/{fileName}{Path.GetExtension(httpPostedFile.FileName)}");
                string realFilePath = HttpContext.Current.Server.MapPath($@"~/{realFileViturePath}");

                httpPostedFile.SaveAs(tmpFilePath);
                using (Image originImg = Image.FromFile(tmpFilePath))
                {
                    using (var formattedImg = GetResizeImage(originImg, 60, 60))
                    {
                        formattedImg.Save(realFilePath);
                    }
                }

                MessageBoardEntities messageBoardEntities = new MessageBoardEntities();
                MessagePic messagePic = new MessagePic()
                {
                    CreateDate = DateTime.Now,
                    MessageID = messageID,
                    CreateUserID = userID,
                    PicURL = realFileViturePath
                };
                messageBoardEntities.SaveChanges();
                GC.Collect();
            }
            catch (Exception err)
            {
                LogTool.DoErrorLog($"#{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")}:{err.Message}\r\n{err.StackTrace}\r\n");
            }
        }


        /// <summary>
        /// 在圖片四周加入白邊
        /// </summary>
        /// <param name="imgToAddFrame">圖片</param>
        /// <param name="width">目標圖片寬度</param>
        /// <param name="height">目標圖片高度</param>
        /// <returns>Image</returns>
        private static Image AddFrame(Image imgToAddFrame, int width, int height)
        {
            Image newImg = imgToAddFrame;
            if (imgToAddFrame.Width < width || imgToAddFrame.Height < height)
            {
                ImageFormat format = imgToAddFrame.RawFormat;
                Bitmap bitmap = new Bitmap(width, height);
                Graphics graphics = Graphics.FromImage(bitmap);
                SolidBrush solidBrush = new SolidBrush(format.Equals(ImageFormat.Png) ? Color.Transparent : Color.White);
                Rectangle rectangle = new Rectangle(0, 0, imgToAddFrame.Width, imgToAddFrame.Height);

                graphics.FillRectangle(solidBrush, 0, 0, width, height);
                graphics.DrawImage(imgToAddFrame, (width - imgToAddFrame.Width) / 2, (height - imgToAddFrame.Height) / 2, rectangle, GraphicsUnit.Pixel);
                newImg = bitmap;
            }

            return newImg;
        }

        /// <summary>
        /// 取得縮尺寸的圖片
        /// </summary>
        /// <param name="imgToResize">待縮圖的原圖</param>
        /// <param name="targetWidth">目標寬度</param>
        /// <param name="targetHeight">目標高度</param>
        /// <returns></returns>
        private static Image GetResizeImage(Image imgToResize, int targetWidth, int targetHeight)
        {
            Image newImg = imgToResize;
            if (imgToResize.Width > targetWidth || imgToResize.Height > targetHeight)
            {
                float percent = Math.Min(targetHeight / (float)imgToResize.Height, targetWidth / (float)imgToResize.Width);
                int destHeight = (int)(imgToResize.Height * percent);
                int destWidth = (int)(imgToResize.Width * percent);
                Bitmap bitmap = new Bitmap(destWidth, destHeight);
                Graphics graphics = Graphics.FromImage(bitmap);

                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
                newImg = bitmap;
            }

            return newImg;
        }
    }
}