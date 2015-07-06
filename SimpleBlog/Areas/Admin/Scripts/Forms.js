$(document).ready(function() {

    $("a[data-post]").click(function (e) {
        e.preventDefault();

        var $this = $(this);
        var message = $this.data("post");

        if (message && !confirm(message))
            return;

        var antiforgeryToken = $('#anti-forgery-form input');
        var antiforgeryInput = $("<input type='hidden'>").attr("name", antiforgeryToken.attr("name")).val(antiforgeryToken.val());
        $("<form>")
            .attr("method", "post")
            .attr("action", $this.attr("href"))
            .append(antiforgeryInput)
            .appendTo(document.body)
            .submit();
    });
});