$(document).ready(function() {

    //this selected "a" tag with data-post attribute
    $("a[data-post]").click(function (e) {
        e.preventDefault(); //stops the default action of an element from occuring

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

    $("[data-slug]").each(function() 
    { //loop thru anything with a data-slug attribute
        
        var $this = $(this);
        var $sendSlugFrom = $($this.data("slug")); //get text field of element we're getting the slug from?

        $sendSlugFrom.keyup(function () {
            console.log("here");
            var slug = $sendSlugFrom.val();
            
            slug = slug.replace(/[^a-zA-Z0-9\s]/g, ""); //remove all special characters
            slug = slug.toLowerCase();
            slug = slug.replace(/\s+/g, "-"); //replace any spaces with -

            if (slug.charAt(slug.length - 1) == "-")
                slug = slug.substr(0, slug.length - 1);
            $this.val(slug);
        });
     });
});