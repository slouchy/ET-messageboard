// ToDo 20180820 嘗試將重複的 JavaScript 提出

let ajaxTimeout = null;
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
            let errorMsg = "";

            this.isEmailOK = true;
            if (!isEmailSyntaxOK($this.val())) {
                errorMsg = "信箱檢驗失敗";
                this.isEmailOK = false;
            } else {
                clearTimeout(ajaxTimeout);
                ajaxTimeout = setTimeout(() => {
                    axios.get(`Register/CheckUserEmail/?userEmail=${$this.val()}`)
                        .then((result) => {
                            SetPopover(this, $this, !result.data.isOK, "使用者信箱已經存在");
                        })
                        .catch((msg) => { console.error(msg); });
                }, 300);
            }

            SetPopover(this, $this, errorMsg !== "", errorMsg);
        },
        CheckUserAccount(evt) {
            let $this = $(evt.target);
            let userCheck = GetUserAccountOK($this.val());

            if (userCheck.result) {
                clearTimeout(ajaxTimeout);
                ajaxTimeout = setTimeout(() => {
                    axios.get(`Register/CheckUserExist/?userAccount=${$this.val()}`)
                        .then((result) => {
                            SetPopover(this, $this, !result.data.isOK, "使用者帳號已經存在");

                            if (result.data.isOK) {
                                this.isUserOK = true;
                            }
                        })
                        .catch((msg) => { console.error(msg); });
                }, 300);
            } else {
                SetPopover(this, $this, !userCheck.result, userCheck.msg);
            }
        },
        CheckUserPw(evt) {
            let $this = $(evt.target);
            let errorMsg = "<ul class='popover-content'>";

            this.isPWOK = true;
            if (!isPwSyntaxOK($this.val())) {
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

            SetPopover(this, $(".user-pw"), errorMsg !== "", errorMsg);
        },
        isFieldOK() {
            return this.isPWOK && this.isEmailOK && this.isUserOK;
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
    }
});