// VUE NOTE： 
//      ● component 在父子間傳遞資料，使用駝峰式命名會傳遞失敗
//      ● component 需要先被定義才能使用

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
        SetMajorID(id) {
            this.$emit("clicked-create-message", id);
        },
        SetMessageID(id) {
            this.$emit("clicked-edit", id);
        },
        ToggleReply(evt) {
            this.isLoading = true;
            this.isShowReply = !this.isShowReply;
            $(evt.target).nextAll(".replays").slideToggle();

            if (this.isShowReply) {
                axios.get(`data/reply.json`)
                    .then((result) => {
                        // ToDo 2018.08.22 測試使用，串完資料後不需要 filter
                        let showReplys = result.data.filter((reply, i) => {
                            return i < this.total;
                        });

                        this.replys = showReplys;
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
    props: ["userinfo"],
    methods: {
        DeleteMessage(messageID) {

        },
        OpenCreateMessage(majorID) {
            this.$emit("clicked-create-message", majorID);
        },
        OpenEditMessage(messageID) {
            this.$emit("clicked-edit", messageID);
        }
    }
});

let messageTemplate = Vue.component("vue-message", {
    template: "#message-template",
    props: ["message"],
    methods: {
        SetMajorID(id) {
            this.$emit("clicked-create-message", id);
        },
        SetMessageID(id) {
            this.$emit("clicked-edit", id);
        }
    }
});

let App = new Vue({
    el: "#main",
    data: {
        editMajorID: 0,
        editMessageID: 0,
        editExistID: 0,
        file: "",
        isFileOK: true,
        isHasMessage: false,
        isOutOverMaxContent: false,
        isSaveOK: false,
        messageContent: "",
        messages: [],
        serverMsg: "",
        replyItems: [Math.ceil(Math.random() * 5), Math.ceil(Math.random() * 5), Math.ceil(Math.random() * 5),
        Math.ceil(Math.random() * 20), Math.ceil(Math.random() * 5), Math.ceil(Math.random() * 5)]
    },
    mounted() {
        let _this = this;
        $.get("data/messageList.json")
            .then((result) => {
                _this.messages = result;
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
            this.editExistID = 0;
            this.editMajorID = 0;
            this.editMessageID = 0;
            this.file = "";
            this.messageContent = "";
        },
        CloseDialog() {
            if (this.isSaveOK) {
                window.location.reload();
            }
        },
        DeleteMessage(evt) {
            axios.post("List/DeleteMessage", {
                id: this.editMessageID
            })
                .then((result) => {
                    $dgDialog.modal("show");
                    _this.serverMsg = result.data.isOK;
                    _this.saveMsg = result.data.msg;
                });
        },
        DeleteMessagePic(evt) {
            if (confirm("是否要刪除圖片？")) {
                axios.post("List/DeleteMessagePic", {
                    pic: this.editExistID
                })
                    .then((result) => {
                        $dgDialog.modal("show");
                        _this.serverMsg = result.data.isOK;
                        _this.saveMsg = result.data.msg;
                    });
            }
        },
        OpenCreateMessage(majorID) {
            this.SetMajorID(majorID);
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
                        postURL = "List/GetMajorMessage";
                        formData.append("majorID", this.editMajorID);
                    } else {
                        postURL = "List/UpdateMessage";
                        formData.append("messageID", this.editMessageID);
                    }

                    $(".user-message").prop("disabled", false);
                    axios.post(postURL, formData)
                        .then((result) => {
                        })
                        .catch((msg) => {
                            console.error(msg);
                        });
                }, 300);
            }
        },
        SetMajorID(id) {
            $dgMessage.modal("show");
            this.ClearMessage();
            this.editMajorID = id;
        },
        SetMessageID(id) {
            $dgMessage.modal("show");
            this.ClearMessage();
            this.editMessageID = id;
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