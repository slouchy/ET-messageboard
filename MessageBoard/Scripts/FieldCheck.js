let GetUserAccountOK = (userAccount) => {
    let maxLength = 20;
    let errorMsg = "";
    let result = false;

    if (userAccount.length > maxLength) {
        errorMsg = `輸入長度超過 ${maxLength} 字`;
    } else if (userAccount.length === 0) {
        errorMsg = `請輸入使用者名稱`;
    } else {
        result = true;
    }

    return { result: result, msg: errorMsg };
};

let isPwSyntaxOK = (pwd) => {
    let regPW = /(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])[\w\d]{0,12}/;
    return regPW.test(pwd);
};

let isEmailSyntaxOK = (email) => {
    let regEmailAddress = /^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z]+$/;
    return regEmailAddress.test(email);
};

let SetPopover = (that, $popover, isShowMsg, errorMsg) => {
    $popover.attr("data-content", errorMsg);
    $popover.popover(isShowMsg ? "show" : "hide");
    that.isEnabledSubmit = false;
    if (that.isFieldOK()) {
        that.isEnabledSubmit = true;
    }
};