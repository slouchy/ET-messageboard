// VUE NOTE： 
//      ● v-if 改變事件，可能會無法再次觸發其他的 init 事件 (如：jQuery $(function(){}))

let $dgDialog;
$(function () {
    $("input[type='text'], input[type='password'], input[type='email'], input[type='file']").popover({
        template: '<div class="popover bg-danger" role="tooltip"><div class="arrow arrow-danger arrow-right"></div><h3 class="popover-header text-white"></h3><div class="popover-body text-white"></div></div>',
        placement: "right",
        trigger: "manual",
        html: true
    });
    $dgDialog = $("#dgDialog");
});

let vueApp = new Vue({
    el: "#main",
    data: {
        dgMsg: "",
        isFieldOK: false,   // 這個檔案沒有使用，因應共用函式而增加
        isPWOK: false,
        isResetedPW: false,
        isShowEditPW: false,
        isShowRestPW: false,
        pw1: "",
        pw2: "",
        userName: "",
        userEmail: ""
    },
    mounted() { },
    methods: {
        CheckUserAccount(evt) {
            let $this = $(evt.target);
            let userCheck = GetUserAccountOK($this.val());

            SetPopover($this, !userCheck.result, userCheck.msg);
            if (userCheck.result && evt.keyCode === 13) {
                $(".btn-sign-forgetpw").click();
            }
        },
        CheckUserEmail(evt) {
            let $this = $(evt.target);
            let errorMsg = "";

            if (!isEmailSyntaxOK($this.val())) {
                errorMsg = "信箱檢驗失敗";
            } else if (evt.keyCode === 13) {
                $(".btn-sign-forgetpw").click();
            }

            SetPopover($this, errorMsg !== "", errorMsg);
        },
        CheckUserPw(evt) {
            let $this = $(evt.target);
            let errorMsg = "<ul class='popover-content'>";

            this.isPWOK = true;
            if (!isPwSyntaxOK($this.val())) {
                errorMsg += "<li>密碼不符合規則</li>";
                this.isPWOK = false;
            }

            if (this.pw1 !== this.pw2 && this.pw2 !== "") {
                errorMsg += "<li>前後密碼輸入不一致</li>";
                this.isPWOK = false;
            }

            errorMsg += "</ul>";
            if (this.isPWOK) {
                errorMsg = "";
                if (evt.keyCode === 13) {
                    $(".btn-rest-pw").click();
                }
            }

            SetPopover($this, errorMsg !== "", errorMsg);
        },
        CloseDialog() {
            if (this.isResetedPW) {
                window.location.href = "login";
            }

            if (this.isShowRestPW) {
                this.SetFocus("#userPw1");
            } else {
                this.SetFocus("#userAccount");
            }
        },
        ForgetPW(evt) {
            let userCheck = GetUserAccountOK(this.userName);
            let _this = this;
            _this.dgMsg = "";
            if (!userCheck.result) {
                this.dgMsg += "<br/> * 使用者名稱檢驗失敗";
            }

            if (!isEmailSyntaxOK(_this.userEmail)) {
                _this.dgMsg += "<br/> * 使用者信箱檢驗失敗";
            }

            if (this.dgMsg.length !== 0) {
                $dgDialog.modal("show");
            } else {
                $(evt.target).prop("disabled", true);
                axios.get("forgetpw/CheckUserNameEmail", {
                    params: {
                        userEmail: encodeURI(_this.userEmail),
                        userName: encodeURI(_this.userName)
                    }
                })
                    .then((response) => {
                        _this.dgMsg = response.data.msg;
                        $dgDialog.modal("show");
                        if (response.data.isOK) {
                            _this.isShowRestPW = true;
                            $(".user-resetpw").removeClass("hidden");
                        }

                        this.SetFocus(".btn-close-dialog");
                    });
            }
        },
        ResetPW(evt) {
            this.dgMsg = "";
            if (this.pw1 !== this.pw2) {
                this.dgMsg += "<br/> * 密碼前後兩次輸入不相同";
            }

            if (!isPwSyntaxOK(this.pw1)) {
                this.dgMsg += "<br/> * 密碼不符合規則";
            }

            if (this.dgMsg.length !== 0) {
                $dgDialog.modal("show");
            } else {
                $(evt.target).prop("disabled", true);
                axios.get("forgetpw/ResetPW", {
                    params: {
                        userEmail: encodeURI(this.userEmail),
                        userName: encodeURI(this.userName),
                        pw1: encodeURI(this.pw1),
                        pw2: encodeURI(this.pw2)
                    }
                })
                    .then((response) => {
                        this.dgMsg = response.data.msg;
                        this.isResetedPW = response.data.isOK;
                        this.SetFocus(".btn-close-dialog");
                        $dgDialog.modal("show");
                    });
            }
        },
        SetFocus(selector) {
            setTimeout(() => {
                $(selector).focus();
            }, 500);
        }
    },
    computed: {
        isEnabledSubmit() {
            let userCheck = GetUserAccountOK(this.userName);
            return isEmailSyntaxOK(this.userEmail) && userCheck.result;
        },
        isEnabledResetPW() {
            let isSubmit = this.pw1 === this.pw2 && isPwSyntaxOK(this.pw1);
            if (isSubmit) {
                $(".popover").hide();
            }

            return isSubmit;
        },
        isShowMuted() {
            return this.userName.length > 0;
        }
    }
});