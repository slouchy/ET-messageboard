﻿// VUE NOTE： 
//      ● component 在父子間傳遞資料，使用駝峰式命名會傳遞失敗
//      ● component 需要先被定義才能使用
//      ● v-on 事件綁定要注意「;」，不正確會無法綁定

let $dgMessage;
let $dgDialog;
$(function () {
    $("input[type='text'], input[type='password'], input[type='email'], input[type='file']").popover({
        template: '<div class="popover bg-danger" role="tooltip"><div class="arrow arrow-danger arrow-bottom"></div><h3 class="popover-header text-white"></h3><div class="popover-body text-white"></div></div>',
        placement: "bottom",
        trigger: "manual",
        html: true
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
                axios.get(`list/GetMessageList/?majorID=${this.majorid}`)
                    .then((result) => {
                        // ToDo 2018.08.22 測試使用，串完資料後不需要 filter
                        //let showReplys = result.data.filter((reply, i) => {
                        //    return i < this.total;
                        //});

                        //this.replys = showReplys;
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
            console.log("InEdit");
            this.$emit("clicked-edit", this.userinfo.MessageID);
        }
    }
});

let messageTemplate = Vue.component("vue-message", {
    template: "#message-template",
    props: ["message", "count"],
    methods: {
        DeleteMessage(id) {
            this.$emit("clicked-delete-message", id);
        },
        EditMessage(id) {
            this.$emit("clicked-edit", id);
        },
        ReplyMessage(id) {
            this.$emit("clicked-create-message", id);
        }
    }
});

let App = new Vue({
    el: "#main",
    data: {
        editMajorID: 0,
        editMessageID: 0,
        editExistID: 0,
        editMessage: null,
        file: "",
        isFileOK: true,
        isHasMessage: false,
        isOutOverMaxContent: false,
        messageContent: "",
        messages: [],
        serverMsg: "",
        serverCode: 0
    },
    mounted() {
        let _this = this;
        axios.get("list/GetMajorMessage")
            .then((result) => {
                result.data.forEach((item, i) => {
                    item.CreateDate = GetDatetimeFormatted(item.CreateDate);
                });
                _this.messages = result.data;
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
            }
        },
        CreateMessage(id) {
            $dgMessage.modal("show");
            this.ClearMessage();
            this.editMajorID = id;
        },
        DeleteMessage(id) {
            this.editMessageID = id;
            axios.post("List/DeleteMessage", {
                messageID: this.editMessageID
            })
                .then((result) => {
                    $dgDialog.modal("show");
                    this.serverMsg = result.data.msg;
                })
                .catch((msg) => {
                    console.error(msg);
                });
        },
        DeleteMessagePic(evt) {
            // ToDo 20180824 給定 pic ID 值
            if (confirm("是否要刪除圖片？")) {
                axios.post("List/DeleteMessagePic", {
                    picID: 0
                })
                    .then((response) => {
                        $dgDialog.modal("show");
                        this.serverMsg = response.data.msg;
                        $(evt.target).parents("li").remove();
                    });
            }
        },
        EditMessage(id) {
            this.ClearMessage();
            this.editMessageID = id;
            if (id !== 0) {
                axios.get(`list/GetUniqueMessage/?messageID=${id}`)
                    .then((response) => {
                        if (response.data.length > 0) {
                            this.editMessage = response.data[0];
                            $dgMessage.modal("show");
                        } else {
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
            $(".user-message").each(function () {
                $(this).focus();
            });

            $(evt.target).focus();
            if (!this.isEnabledSave) {
                evt.preventDefault();
            } else {
                setTimeout(() => {
                    let postURL = "";
                    let formData = new FormData();
                    $(evt.target).focus();
                    $(".user-message").prop("disabled", true);
                    formData.append("content", encodeURI(this.messageContent));
                    formData.append("picList", encodeURI(this.file));

                    if (this.editMessageID === 0) {
                        postURL = "List/AddMessage";
                        formData.append("majorID", this.editMajorID);
                    } else {
                        postURL = "List/UpdateMessage";
                        formData.append("messageID", this.editMessageID);
                    }

                    $(".user-message").prop("disabled", false);
                    axios.post(postURL, formData)
                        .then((response) => {
                            _this.isSaveOK = response.data.isOK;
                            _this.serverMsg = response.data.msg;
                            $dgDialog.modal("show");
                            if (!_this.isSaveOK) {
                                $(".user-message").prop("disabled", false);
                            }
                        })
                        .catch((msg) => {
                            console.error(msg);
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