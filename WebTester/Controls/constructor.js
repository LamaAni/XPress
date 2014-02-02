({
    $: function () {
        console.log("(Test conterol only) constructor triggered for " + this.id);
    },
    TestFunc: function () {
        console.log("Testing this...!! yey construction worked.");
    },
});