let App = new Vue({
    el: "#main",
    data: {
        items: [
            { "order": 0, "userName": "使用者名稱", "ip": "111.111.111.111", "createDate": "2018/08/21 15:31", "isShowDelete": true, "isShowEdit": false },
            { "order": 1, "userName": "使用者名稱", "ip": "111.111.111.111", "createDate": "2018/08/21 15:31", "isShowDelete": true, "isShowEdit": true },
            { "order": 2, "userName": "使用者名稱", "ip": "111.111.111.111", "createDate": "2018/08/21 15:31", "isShowDelete": false, "isShowEdit": false },
            { "order": 3, "userName": "使用者名稱", "ip": "111.111.111.111", "createDate": "2018/08/21 15:31", "isShowDelete": false, "isShowEdit": false },
            { "order": 4, "userName": "使用者名稱", "ip": "111.111.111.111", "createDate": "2018/08/21 15:31", "isShowDelete": true, "isShowEdit": true },
            { "order": 5, "userName": "使用者名稱", "ip": "111.111.111.111", "createDate": "2018/08/21 15:31", "isShowDelete": true, "isShowEdit": true }],
        replyItems: [Math.ceil(Math.random() * 5), Math.ceil(Math.random() * 5), Math.ceil(Math.random() * 5),
        Math.ceil(Math.random() * 20), Math.ceil(Math.random() * 5), Math.ceil(Math.random() * 5)]
    },
    methods: {
        ToggleReply(evt) {
            let $this = $(evt.target);
            let currentStatus = parseInt($this.attr("data-show"));

            $this.parents(".row").next(".row").slideToggle();
            currentStatus = (currentStatus + 1) % 2;
            $this.attr("data-show", currentStatus);
            if (currentStatus === 1) {
                $this.text($this.text().replace("收合", "展開"));
            } else {
                $this.text($this.text().replace("展開", "收合"));
            }
        }
    }
});