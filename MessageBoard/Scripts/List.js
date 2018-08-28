// VUE NOTE： 
//      ● component 在父子間傳遞資料，使用駝峰式命名會傳遞失敗
//      ● component 需要先被定義才能使用
//      ● v-on 事件綁定要注意不能有「;」，不正確會無法綁定

let $dgMessage;
let $dgDialog;
$(function () {
    $("input[type='text'], input[type='password'], input[type='email'], input[type='file']").popover({
        template: '<div class="popover bg-danger" role="tooltip"><div class="arrow arrow-danger arrow-bottom"></div><h3 class="popover-header text-white"></h3><div class="popover-body text-white"></div></div>',
        placement: "bottom",
        trigger: "manual",
        html: true
    });

    $(document).on('click', '[data-toggle="lightbox"]', function (event) {
        event.preventDefault();
        $(this).ekkoLightbox();
    });

    $dgMessage = $("#dgMessage");
    $dgDialog = $("#dgDialog");
    $dgMessage.modal({
        show: false,
        keyboard: false
    });
    $dgDialog.modal({
        show: false,
        keyboard: false
    });
});

let GetDatetimeFormatted = (orignDatetime) => {
    return moment(orignDatetime).format("YYYY-MM-DD HH:mm");
};

let replyCountTemplate = Vue.component("vue-replycount", {
    template: "#replyCount-template",
    data: function () {
        return {
            showTxt: "展開留言",
            hideTxt: "隱藏留言",
            isShowReply: false,
            isLoading: false,
            replys: []
        };
    },
    props: ["total", "majorid"],
    methods: {
        DeleteMessage(id) {
            this.$emit("clicked-delete-message", id);
        },
        EditMessage(id) {
            this.$emit("clicked-edit", id);
        },
        ReplyMessage(id) {
            this.$emit("clicked-create-message", id);
        },
        ToggleReply(evt) {
            this.isLoading = true;
            this.isShowReply = !this.isShowReply;
            $(evt.target).nextAll(".replays").slideToggle();

            if (this.isShowReply) {
                axios.get(`list/GetMessageList?majorID=${this.majorid}`)
                    .then((result) => {
                        result.data.forEach((item, i) => {
                            item.CreateDate = GetDatetimeFormatted(item.CreateDate);
                            item.content = decodeURI(item.content).replace(/(\r\n|\r|\n)/gi, "<br/>");
                            item.userName = decodeURI(item.userName);
                        });
                        this.replys = result.data;
                        this.isLoading = false;
                    });
            } else {
                this.isLoading = false;
            }
        }
    }
});

let contentTemplate = Vue.component("vue-content", {
    template: "#content-template",
    props: ["message"]
});

let userinfoTemplate = Vue.component("vue-userinfo", {
    template: "#userinfo-template",
    props: ["userinfo", "count"],
    methods: {
        DeleteMessage() {
            if (confirm("是否要刪除留言？")) {
                this.$emit("clicked-delete-message", this.userinfo.MessageID);
            }
        },
        OpenCreateMessage() {
            this.$emit("clicked-create-message", this.userinfo.majorID);
        },
        OpenEditMessage() {
            this.$emit("clicked-edit", this.userinfo.MessageID);
        }
    }
});

let messageTemplate = Vue.component("vue-message", {
    template: "#message-template",
    props: ["message", "count"],
    methods: {
        DeleteMessage() {
            this.$emit("clicked-delete-message", this.message.MessageID);
        },
        EditMessage() {
            this.$emit("clicked-edit", this.message.MessageID);
        },
        ReplyMessage() {
            this.$emit("clicked-create-message", this.message.MajorID);
        }
    }
});

let App = new Vue({
    el: "#main",
    data: {
        editMajorID: 0,
        editMessageID: 0,
        editExistID: 0,
        editMessage: "",
        editPics: [],
        editTitle: "建立文章",
        file: "",
        isFileOK: true,
        isHasMessage: false,
        isOutOverMaxContent: false,
        isEditDeleteFile: false,
        messageContent: "",
        messages: [],
        serverMsg: "",
        serverCode: 0,
        testPics: []
    },
    mounted() {
        let _this = this;
        axios.get("list/GetMajorMessage")
            .then((result) => {
                result.data.forEach((item, i) => {
                    item.CreateDate = GetDatetimeFormatted(item.CreateDate);
                    item.content = decodeURI(item.content);
                    item.userName = decodeURI(item.userName);
                });
                _this.messages = result.data;
                $.unblockUI();
            })
            .catch((msg) => { console.log(msg); });
    },
    methods: {
        CheckPics(evt) {
            let fileCheck = isFileAllow(evt.target.id, 1, /(jpg|gif|png|bmp|jpeg|jpg2000|svg)$/i);

            this.isFileOK = fileCheck;
            if (fileCheck && $(evt.target).val().length > 0) {
                this.file = this.$refs.file.files[0];
            }

            SetPopover($(evt.target), !this.isFileOK, "檔案檢驗失敗");
        },
        ClearMessage() {
            this.editMajorID = 0;
            this.editMessageID = 0;
            this.editTitle = "建立文章";
            this.file = "";
            this.messageContent = "";
        },
        CloseDialog() {
            switch (this.serverCode) {
                case 0:                         // 文章異動
                    window.location.reload();
                    break;
                case 1:                         // 刪除編輯中的圖片

                    break;
                case 2:                         // 刪除文章
                    window.location.reload();
                    break;
                case 3:                         // 讀取編輯文章發生錯誤
                    break;
                default:
                    break;
            }
        },
        CloseMessage() {
            if (this.isEditDeleteFile) {
                window.location.reload();
            }
        },
        CreateMessage(id) {
            $dgMessage.modal("show");
            this.ClearMessage();
            this.editMajorID = id;
        },
        DeleteMessage(id) {
            this.editMessageID = id;
            $.blockUI({ css: { "z-index": "1099" } });
            axios.post("List/DeleteMessage", {
                messageID: this.editMessageID
            })
                .then((result) => {
                    $.unblockUI();
                    $dgDialog.modal("show");
                    this.serverMsg = result.data.msg;
                    this.serverCode = -1;
                    if (result.data.isOK) {
                        this.serverCode = 2;
                    }
                })
                .catch((msg) => {
                    console.error(msg);
                });
        },
        DeleteMessagePic(evt, id) {
            if (confirm("是否要刪除圖片？")) {
                let $this = $(evt.target);
                $.blockUI({ css: { "z-index": "1099" } });
                $this.prop("disabled", true);
                axios.post("List/DeleteMessagePic", {
                    picID: id
                })
                    .then((response) => {
                        $.unblockUI();
                        $dgDialog.modal("show");
                        this.serverMsg = response.data.msg;
                        this.serverCode = 1;
                        this.isEditDeleteFile = true;
                        $this.parents("li").remove();
                        $this.prop("disabled", false);
                    })
                    .catch((msg) => {
                        $dgDialog.modal("show");
                        this.serverMsg = "發生錯誤";
                        console.error(msg);
                    });
            }
        },
        EditMessage(id) {
            this.ClearMessage();
            this.editMessageID = id;
            if (id !== 0) {
                axios.get(`list/GetUniqueMessage?messageID=${id}`)
                    .then((response) => {
                        if (response.data.isOK) {
                            this.editTitle = "編輯文章";
                            this.editMessage = response.data.data[0];
                            this.messageContent = decodeURI(response.data.data[0].Message1);
                            this.editPics = response.data.data[0].pics;
                            $dgMessage.modal("show");
                        } else {
                            this.serverCode = 3;
                            this.serverMsg = response.data.msg;
                            $dgDialog.modal("show");
                        }
                    })
                    .catch((msg) => {
                        console.error(msg);
                    });
            }
        },
        OpenCreateMessage(majorID) {
            this.CreateMessage(majorID);
        },
        SaveMessage(evt) {
            let _this = this;
            let $this = $(evt.target);
            $this.prop("disabled", true);
            $(".user-message").each(function () {
                $(this).focus();
            });

            $this.focus();
            if (!this.isEnabledSave) {
                evt.preventDefault();
            } else {
                $.blockUI({ css: { "z-index": "1099" } });
                setTimeout(() => {
                    let postURL = "";
                    let formData = new FormData();
                    $this.focus();
                    $(".user-message").prop("disabled", true);
                    formData.append("content", encodeURI(_this.messageContent));
                    formData.append("picList", _this.file);

                    if (this.editMessageID === 0) {
                        postURL = "List/AddMessage";
                        formData.append("majorID", _this.editMajorID);
                    } else {
                        postURL = "List/UpdateMessage";
                        formData.append("messageID", _this.editMessageID);
                    }

                    $(".user-message").prop("disabled", false);
                    axios.post(postURL, formData)
                        .then((response) => {
                            $.unblockUI();
                            _this.isSaveOK = response.data.isOK;
                            _this.serverMsg = response.data.msg;
                            _this.serverCode = 0;
                            $dgDialog.modal("show");
                            if (!_this.isSaveOK) {
                                $(".user-message").prop("disabled", false);
                            }
                        })
                        .catch((msg) => {
                            console.error(msg);
                            _this.serverMsg = "發生錯誤";
                            $dgDialog.modal("show");
                        });
                }, 300);
            }
        },
        ToggleReply(evt) {
            let $this = $(evt.target);
            let currentStatus = parseInt($this.attr("data-show"));

            currentStatus = (currentStatus + 1) % 2;
            $this.parents(".row").next(".row").slideToggle();
            $this.attr("data-show", currentStatus);
            if (currentStatus === 1) {
                $this.text($this.text().replace("收合", "展開"));
            } else {
                $this.text($this.text().replace("展開", "收合"));
            }
        }
    },
    computed: {
        messageCount() {
            if (this.messageContent.length > 0) {
                this.isHasMessage = true;
            } else {
                this.isHasMessage = false;
            }

            if (this.messageContent.length > 300) {
                this.isOutOverMaxContent = true;
            } else {
                this.isOutOverMaxContent = false;
            }
            return this.messageContent.length;
        },
        isEnabledSave() {
            return !this.isOutOverMaxContent && this.isHasMessage && this.isFileOK;
        }
    }
});