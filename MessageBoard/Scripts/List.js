let $dgMessage;
$(function () {
    $("input[type='text'], input[type='password'], input[type='email'], input[type='file']").popover({
        template: '<div class="popover bg-danger" role="tooltip"><div class="arrow arrow-danger arrow-bottom"></div><h3 class="popover-header text-white"></h3><div class="popover-body text-white"></div></div>',
        placement: "bottom",
        trigger: "manual",
        html: true
    });
    $dgMessage = $("#dgMessage");
    $dgMessage.modal({
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
    props: ["userinfo"]
});

let messageTemplate = Vue.component("vue-message", {
    template: "#message-template",
    props: ["message"]
});

let App = new Vue({
    el: "#main",
    data: {
        editMajorID: 0,
        editMessageID: 0,
        file: "",
        messageContent: "",
        isHasMessage: false,
        isOutOverMaxContent: false,
        isFileOK: false,
        messages: [],
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
        ClearMessage() {
            this.editMajorID = 0;
            this.editMessageID = 0;
            this.file = "";
            this.messageContent = "";
        },
        CheckPics(evt) {
            let fileCheck = isFileAllow(evt.target.id, 1, /(jpg|gif|png|bmp|jpeg|jpg2000|svg)$/i);
            if (fileCheck && $(evt.target).val().length > 0) {
                this.file = this.$refs.file.files[0];
                this.isFileOK = fileCheck;
            }

            SetPopover($(evt.target), !this.isFileOK, "檔案檢驗失敗");
        },
        OpenCreateMessage(majorID) {
            $dgMessage.modal("show");
            this.ClearMessage();
            this.editMajorID = majorID;
        },
        OpenEditMessage(messageID) {
            $dgMessage.modal("show");
            this.ClearMessage();
            this.editMessageID = messageID;
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
        }
    }
});