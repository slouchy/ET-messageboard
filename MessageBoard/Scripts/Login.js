$(function () {
    $("#dgLoginFail").modal();
    $("input[type='text'], input[type='password']").popover({
        template: '<div class="popover bg-danger" role="tooltip"><div class="arrow arrow-danger"></div><h3 class="popover-header text-white"></h3><div class="popover-body text-white"></div></div>',
        placement: "right",
        trigger: "manual"
    });
});

let vueApp = new Vue({
    el: "#main",
    data: {
        isEnabledSubmit: true,
        isPWOK: false,
        isUserOK: false
    },
    methods: {
        CheckUserName(evt) {
            let $this = $(evt.target);
            let maxLength = 20;
            let errorMsg = "";

            this.isUserOK = false;
            if ($this.val().length > maxLength) {
                errorMsg = `輸入長度超過 ${maxLength} 字`;
            } else if ($this.val().length === 0) {
                errorMsg = `請輸入使用者名稱`;
            } else {
                this.isUserOK = true;
            }

            this.SetPopover($this, errorMsg !== "", errorMsg);
        },
        CheckUserPw(evt) {
            let $this = $(evt.target);
            let regPW = /(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])[\w\d]{0,12}/;
            let errorMsg = "";

            this.isPWOK = true;
            if (!regPW.test($this.val())) {
                errorMsg = "密碼檢驗失敗";
                this.isPWOK = false;
            }

            this.SetPopover($this, errorMsg !== "", errorMsg);
        },
        SetPopover($this, isShowMsg, errorMsg) {
            $this.attr("data-content", errorMsg);
            $this.popover(isShowMsg ? "show" : "hide");
            this.isEnabledSubmit = false;
            if (this.isPWOK && this.isUserOK) {
                this.isEnabledSubmit = true;
            }
        },
        UserLogin(evt) {
            $(".user-info").each(function () {
                $(this).focus();
            });

            $(evt.target).focus();
            if (!(this.isUserOK && this.isPWOK)) {
                evt.preventDefault();
            }
        }
    }
});