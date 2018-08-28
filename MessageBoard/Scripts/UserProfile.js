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
        isLogout: false,
        isModifyMode: false,
        isUpdated: false,
        originPW: "",
        pw1: "",
        pw2: "",
        file: "",
        userInfo: [],
        userIcon: ""
    },
    methods: {
        AxiosCheckLogined() {
            return axios.get("userprofile/isUserLogined");
        },
        AxiosUserCheck(originPW) {
            return axios.get(`userprofile/UserCheck?pw=${originPW}`);
        },
        AxiosSaveUserProfile(postOptions) {
            return axios.post("UserProfile/SavePorfile", postOptions);
        },
        CheckUserPw(evt) {
            let $this = $(evt.target);
            let errorMsg = "";

            if (!isPwSyntaxOK($this.val())) {
                errorMsg += "密碼不符合規則";
            } else {
                if (evt.keyCode === 13) {
                    $(".btn-check-pw").prop("disabled", true);
                    $(".btn-check-pw").click();
                }
            }

            SetPopover($this, errorMsg !== "", errorMsg);
        },
        CheckUserNewPw(evt) {
            let $this = $(evt.target);
            let errorMsg = "<ul class='popover-content'>";

            this.isPWOK = true;
            if ($this.val() !== "" && !isPwSyntaxOK($this.val())) {
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
                    $(".btn-save").click();
                }
            }

            SetPopover($this, errorMsg !== "", errorMsg);
        },
        CheckPics(evt) {
            let fileCheck = isFileAllow(evt.target.id, 1, /(jpg|gif|png|bmp|jpeg|jpg2000|svg)$/i);

            this.file = "";
            this.isFileOK = fileCheck;
            if (fileCheck && $(evt.target).val().length > 0) {
                this.file = this.$refs.file.files[0];
            }

            SetPopover($(evt.target), !this.isFileOK, "檔案檢驗失敗");
        },
        ClearInput() {
            $("input[type='text'], input[type='password'], input[type='email'], input[type='file']").each(function () {
                $(this).val("");
            });
        },
        CloseDialog() {
            this.ClearInput();
            if (this.isLogout) {
                window.location.href = "login";
            }  else if (this.isModifyMode) {
                this.SetFocus("#userPw1");
            } else {
                this.SetFocus("#userOriginPW");
            }

            if (this.isUpdated) {
                window.location.reload();
            }
        },
        UserCheck(evt) {
            if (!isPwSyntaxOK(this.originPW)) {
                this.dgMsg = "密碼不符合規則";
                $("#dgDialog").modal("show");
            } else {
                $(".user-info").each(function () {
                    $(this).val("");
                });
                $(evt.target).prop("disabled", true);
                this.AxiosCheckLogined()
                    .then((result) => {
                        if (!result.data.isOK) {
                            this.dgMsg = result.data.msg;
                            this.SetFocus(".btn-close-dialog");
                            this.isLogout = true;
                            $dgDialog.modal("show");
                        } else {
                            return this.AxiosUserCheck(encodeURI(this.originPW));
                        }
                    })
                    .then((response) => {
                        this.dgMsg = response.data.msg;
                        this.isModifyMode = response.data.isOK;
                        this.userInfo = response.data.data[0];
                        this.SetFocus(".btn-close-dialog");
                        $dgDialog.modal("show");
                        if (!this.isModifyMode) {
                            $(evt.target).prop("disabled", false);
                        }
                    })
                    .catch((msg) => {
                        console.error(msg);
                    });
            }
        },
        SaveUserProfile(evt) {
            this.dgMsg = "";
            if (this.pw1 !== this.pw2) {
                this.dgMsg += "<br/> * 密碼前後兩次輸入不相同";
            }

            if (this.pw1 !== "" && !isPwSyntaxOK(this.pw1)) {
                this.dgMsg += "<br/> * 密碼不符合規則";
            }

            if (this.dgMsg.length !== 0) {
                $dgDialog.modal("show");
            } else {
                $(evt.target).prop("disabled", true);
                $(".user-info").prop("disabled", true);
                this.AxiosCheckLogined()
                    .then((result) => {
                        if (!result.data.isOK) {
                            this.dgMsg = result.data.msg;
                            this.SetFocus(".btn-close-dialog");
                            this.isLogout = true;
                            $dgDialog.modal("show");
                        } else {
                            let formData = new FormData();
                            formData.append("pw1", encodeURI(this.pw1));
                            formData.append("pw2", encodeURI(this.pw2));
                            formData.append("userIcon", this.file);
                            formData.append("userID", this.userInfo.UserID);
                            return this.AxiosSaveUserProfile(formData);
                        }
                    })
                    .then((response) => {
                        this.dgMsg = response.data.msg;
                        this.isUpdated = response.data.isOK;
                        this.SetFocus(".btn-close-dialog");
                        $(".user-info").prop("disabled", false);
                        $dgDialog.modal("show");
                    })
                    .catch((msg) => {
                        console.error(msg);
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
        isEnabledEditUserProfile() {
            return isPwSyntaxOK(this.originPW);
        },
        isEnabledSaveUserProfile() {
            return this.pw1 === this.pw2 && isPwSyntaxOK(this.pw1) || this.file !== "";
        },
        isShowMuted() {
            return this.originPW.length > 0;
        }
    }
});