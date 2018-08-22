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
        isEnabledSubmit: false,
        isPWOK: false,
        isUserOK: true
    },
    methods: {
        CheckUserAccount(evt) {
            let $this = $(evt.target);
            let userCheck = GetUserAccountOK($this.val());

            SetPopover($this, !userCheck.result, userCheck.msg);
            this.isEnabledSubmit = this.isFieldOK();
            if (userCheck.result) {
                this.isUserOK = true;
            }
        },
        CheckUserPw(evt) {
            let $this = $(evt.target);
            let errorMsg = "";

            this.isPWOK = true;
            if (!isPwSyntaxOK($this.val())) {
                errorMsg = "密碼不符合規則";
                this.isPWOK = false;
            }

            SetPopover($this, errorMsg !== "", errorMsg);
            this.isEnabledSubmit = this.isFieldOK();
        },
        isFieldOK() {
            return this.isPWOK && this.isUserOK;
        },
        UserLogin(evt) {
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