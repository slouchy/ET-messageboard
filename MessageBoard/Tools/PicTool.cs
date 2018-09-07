using MessageBoard.Models;
using MessageBoard.Models.Interface;
using MessageBoard.Models.Repository;
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
        /// 檢查上傳檔案
        /// </summary>
        /// <param name="httpPostedFile">postFile</param>
        /// <param name="allowRegexStr">允許的副檔名</param>
        /// <param name="allowMaxSize">(選填)允許的檔案大小(預設：1MB)</param>
        /// <returns></returns>
        public static Tuple<bool, List<string>> CheckUplaodFiles(HttpPostedFileBase httpPostedFile, string allowRegexStr, int allowMaxSize = 1)
        {
            bool result = true;
            List<string> errors = new List<string>();

            if (!isFileSizeAllow(httpPostedFile, allowMaxSize))
            {
                errors.Add($"檔案大於 {allowMaxSize}MB");
                result = false;
            }


            if (!isFileExtensionAllow(httpPostedFile.FileName, allowRegexStr))
            {
                errors.Add($"不是圖像檔案");
                result = false;
            }

            return Tuple.Create(result, errors);
        }

        /// <summary>
        /// 儲存訊息圖片
        /// </summary>
        /// <param name="httpPostedFile">Post file</param>
        /// <param name="userID">使用者 ID</param>
        /// <param name="messageID">訊息 ID</param>
        public static void SaveMessagePic(HttpPostedFileBase httpPostedFile, int userID, int messageID)
        {
            string fileName = Guid.NewGuid().ToString();
            string originPicViturePath = $"messagePics/origin/{fileName}{Path.GetExtension(httpPostedFile.FileName)}";
            string resizePicViturePath = $"messagePics/{fileName}{Path.GetExtension(httpPostedFile.FileName)}";
            DoSaveImage(httpPostedFile, resizePicViturePath, originPicViturePath, 60);

            IMessagePic interfaceMessagePic = new MessagePicRepository(new MessageBoardEntities());
            MessagePic messagePic = new MessagePic()
            {
                CreateDate = DateTime.Now,
                MessageID = messageID,
                CreateUserID = userID,
                PicURL = originPicViturePath,
                picStatus = true
            };
            interfaceMessagePic.Create(messagePic);
        }

        /// <summary>
        /// 儲存使用者圖示
        /// </summary>
        /// <param name="httpPostedFile">傳遞的檔案</param>
        /// <param name="userID">儲存檔案名稱</param>
        public static void SaveUserPic(HttpPostedFileBase httpPostedFile, int userID)
        {
            string realFileViturePath = $"UserIcon/UserIcon_{userID}{Path.GetExtension(httpPostedFile.FileName)}";
            string tmpFilePath = $"UserIcon/2_UserIcon{userID}{Path.GetExtension(httpPostedFile.FileName)}";
            DoSaveImage(httpPostedFile, realFileViturePath, tmpFilePath, 100, true);

            tmpFilePath = HttpContext.Current.Server.MapPath($"~/{tmpFilePath}");
            if (File.Exists(tmpFilePath))
            {
                File.Delete(tmpFilePath);
                UserTool userTool = new UserTool();
                userTool.SaveUserIconPath(userID, realFileViturePath);
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
        /// 儲存圖片
        /// </summary>
        /// <param name="httpPostedFile">post file</param>
        /// <param name="formatedFilePath">被格式化的圖片路徑</param>
        /// <param name="originFilePath">原始圖片路徑</param>
        /// <param name="imgSize">格式化成方形的大小</param>
        /// <param name="isAddFrame">是否需要加白邊</param>
        private static void DoSaveImage(HttpPostedFileBase httpPostedFile, string formatedFilePath, string originFilePath, int imgSize, bool isAddFrame = false)
        {
            try
            {
                formatedFilePath = HttpContext.Current.Server.MapPath($@"~/{formatedFilePath}");
                originFilePath = HttpContext.Current.Server.MapPath($@"~/{originFilePath}");
                if (File.Exists(originFilePath))
                {
                    File.Delete(HttpContext.Current.Server.MapPath(originFilePath));
                }

                httpPostedFile.SaveAs(originFilePath);
                using (Image originImg = Image.FromFile(originFilePath))
                {
                    using (var resizedImg = GetResizeImage(originImg, imgSize, imgSize))
                    {
                        if (isAddFrame)
                        {
                            using (var framedImg = AddFrame(resizedImg, imgSize, imgSize))
                            {
                                framedImg.Save(formatedFilePath);
                            }
                        }
                        else
                        {
                            resizedImg.Save(formatedFilePath);
                        }
                    }
                }

                GC.Collect();
            }
            catch (Exception err)
            {
                LogTool.DoErrorLog($"#{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")}:{err.Message}\r\n{err.StackTrace}\r\n");
                throw;
            }
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

        /// <summary>
        /// 檢查附檔名是否允許
        /// </summary>
        /// <param name="fileName">檔案名稱</param>
        /// <param name="allowRegexStr">待檢查的附檔名正則表達式</param>
        /// <returns>回傳結果</returns>
        private static bool isFileExtensionAllow(string fileName, string allowRegexStr)
        {
            return Regex.IsMatch(fileName, allowRegexStr);
        }

        /// <summary>
        /// 檢查檔案大小
        /// </summary>
        /// <param name="httpPostedFile">傳遞的檔案</param>
        /// <param name="allowMaxSize">(選填)允許的檔案大小(預設：1MB)</param>
        /// <returns>回傳結果</returns>
        private static bool isFileSizeAllow(HttpPostedFileBase httpPostedFile, int allowMaxSize = 1)
        {
            return (httpPostedFile.ContentLength < allowMaxSize * 1024 * 1024);
        }
    }
}