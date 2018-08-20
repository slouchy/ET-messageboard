// ToDo 20180820 嘗試將重複的 JavaScript 提出

let ajaxTimeout = null;
$(function () {
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
        isEnabledSubmit: true,
        isEmailOK: false,
        isPWOK: false,
        isUserOK: false,
        isRegisterOK: false,
        pw1: "",
        pw2: "",
        userAccount: "",
        userEmail: "",
        errorList: null,
        registerMsg: ""
    },
    methods: {
        btnCloseDialog() {
            if (this.isRegisterOK) {
                window.location.href = "Login";
            }
        },
        CheckUserEmail(evt) {
            let $this = $(evt.target);
            let errorMsg = "";
            let _this = this;

            if (!isEmailSyntaxOK($this.val())) {
                errorMsg = "信箱檢驗失敗";
                this.isEmailOK = false;
            } else {
                clearTimeout(ajaxTimeout);
                ajaxTimeout = setTimeout(() => {
                    axios.get(`Register/CheckUserEmail/?userEmail=${encodeURI($this.val())}`)
                        .then((result) => {
                            _this.isEmailOK = result.data.isOK;
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
            let _this = this;

            if (userCheck.result) {
                clearTimeout(ajaxTimeout);
                ajaxTimeout = setTimeout(() => {
                    if ($this.val() !== "") {
                        axios.get(`Register/CheckUserExist/?userAccount=${encodeURI($this.val())}`)
                            .then((result) => {
                                _this.isUserOK = result.data.isOK;
                                SetPopover(_this, $this, !result.data.isOK, "使用者帳號已經存在");
                            })
                            .catch((msg) => { console.error(msg); });
                    }
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
            let _this = this;
            $(".user-info").each(function () {
                $(this).focus();
            });

            $(evt.target).focus();
            if (!this.isFieldOK()) {
                evt.preventDefault();
            } else {
                setTimeout(() => {
                    $(evt.target).focus();
                    $(".user-info").prop("disabled", true);
                    axios.post("Register/UserRegister", {
                        userAccount: encodeURI(this.userAccount),
                        userPW1: encodeURI(this.pw1),
                        userPW2: encodeURI(this.pw2),
                        userEmail: encodeURI(this.userEmail)
                    })
                        .then((result) => {
                            $("#dgRegister").modal({
                                keyboard: false
                            });

                            _this.isRegisterOK = result.data.isOK;
                            _this.errorList = result.data.errorList;
                            _this.registerMsg = result.data.msg;
                            if (!_this.isRegisterOK) {
                                $(".user-info").prop("disabled", false);
                            }
                        })
                        .catch((msg) => {
                            console.error(msg);
                        });
                }, 300);
            }
        }
    }
});