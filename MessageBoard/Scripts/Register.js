// ToDo 20180820 嘗試將重複的 JavaScript 提出

$(function () {
    $("#dgRegister").modal();
    $("input[type='text'], input[type='password'], input[type='email']").popover({
        template: '<div class="popover bg-danger" role="tooltip"><div class="arrow arrow-danger"></div><h3 class="popover-header text-white"></h3><div class="popover-body text-white"></div></div>',
        placement: "right",
        trigger: "manual",
        html: true
    });
});

let app = new Vue({
    el: "#main",
    data: {
        isEnabledSubmit: false,
        isEmailOK: false,
        isPWOK: false,
        isUserOK: false,
        pw1: "",
        pw2: ""
    },
    methods: {
        CheckUserEmail(evt) {
            let $this = $(evt.target);
            let regEmailAddress = /^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z]+$/;
            let errorMsg = "";

            this.isEmailOK = true;
            if (!regEmailAddress.test($this.val())) {
                errorMsg = "信箱檢驗失敗";
                this.isEmailOK = false;
            }

            this.SetPopover($this, errorMsg !== "", errorMsg);
        },
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
            let errorMsg = "<ul class='popover-content'>";

            this.isPWOK = true;
            if (!regPW.test($this.val())) {
                errorMsg += "<li>密碼檢驗失敗</li>";
                this.isPWOK = false;
            }

            if (this.pw1 !== this.pw2) {
                errorMsg += "<li>前後密碼輸入不一致</li>";
                this.isPWOK = false;
            }

            errorMsg += "</ul>";

            if (this.isPWOK) {
                errorMsg = "";
            }

            this.SetPopover($(".user-pw"), errorMsg !== "", errorMsg);
        },
        isFieldOK() {
            return this.isPWOK && this.isEmailOK && this.isUserOK;
        },
        SetPopover($this, isShowMsg, errorMsg) {
            $this.attr("data-content", errorMsg);
            $this.popover(isShowMsg ? "show" : "hide");
            this.isEnabledSubmit = false;
            if (this.isFieldOK()) {
                this.isEnabledSubmit = true;
            }
        },
        UserRegister(evt) {
            $(".user-info").each(function () {
                $(this).focus();
            });

            $(evt.target).focus();
            if (!this.isFieldOK()) {
                evt.preventDefault();
            }
        }
    },
    watch: {}
});