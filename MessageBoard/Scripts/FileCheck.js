/**
 * 檢驗檔案大小、檔案格式，適用於非IE的瀏覽器
 * @param {string} oControlID - 待檢驗的上傳ID
 * @param {int} oMaxSize - 最大大小(MB)
 * @param {string} oFileType - 檔案類型正規表達式
 * @returns {bool} 檢查結果
 */
function isFileAllow(oControlID, oMaxSize, oFileType) {
    var sUserAgent = window.navigator.userAgent.toLowerCase();
    var x = document.getElementById(oControlID);
    var isFileChecked = true;
    oFileType = new RegExp(oFileType);

    if (sUserAgent.indexOf("msie") < 0 && 'files' in x) {
        if (x.files.length !== 0) {
            for (var i = 0; i < x.files.length; i++) {
                var file = x.files[i];
                if ('name' in file) {
                    var FileType = file.name.substring(file.name.lastIndexOf(".") + 1).toLowerCase();
                    if (!oFileType.test(FileType)) {
                        isFileChecked = false;
                    }
                }
                if ('size' in file) {
                    var FileSize = file.size / 1024 / 1024;
                    if (FileSize > oMaxSize) {
                        isFileChecked = false;
                    }
                }
            }
        }
    }

    return isFileChecked;
}