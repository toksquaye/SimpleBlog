$(document).ready(function () {
    var $tagEditor = $(".post-tag-editor");

    //add a live event, not a click event.
    //live event works even after an element is dynamically added to the list
    //click event doesnt work for elements dynamically added during runtime

    $tagEditor
        .find(".tag-select") //find ul that has all our tags
        .on("click", "> li > a", function (e) { //click event will fire only on an a tag that descends from li that descennds from the ul tag 
            //-making it activate for past, present or future elements
            e.preventDefault(); // prevents browser from going to the href="#" link in html code. this allows javascript to take over instead of default behavior

            var $this = $(this);
            var $tagParent = $this.closest("li");
            $tagParent.toggleClass("selected"); //toggles selected class on parent li

            var selected = $tagParent.hasClass("selected");
            $tagParent.find(".selected-input").val(selected); //updates hidden form field that allows server to know if particular tag is selected
        });

    var $addTagButton = $tagEditor.find(".add-tag-button");
    var $newTagName = $tagEditor.find(".new-tag-name");

    $addTagButton.click(function (e) {
        e.preventDefault();
        addTag($newTagName.val());
    });

    $newTagName
        .keyup(function () {
            if ($newTagName.val().trim().length > 0) 
                $addTagButton.prop("disabled", false);
            else
                $addTagButton.prop("disabled",true);
        })
        .keydown(function (e) {
            if (e.which != 13)  //enter key
                return;

            e.preventDefault();
            addTag($newTagName.val());
        });
    function addTag(name) {
        var newIndex = $tagEditor.find(".tag-select > li").size() - 1; //get # of li elements under tag select editors
    
        $tagEditor
            .find(".tag-select > li.template") //get template attribute of li element that descends from tag-select
            .clone()
            .removeClass("template")
            .addClass("selected")
            .find(".name").text(name).end()
            .find(".name-input").val(name).attr("name", "Tags[" + newIndex + "].Name").end()
            .find(".selected-input").attr("name", "Tags[" + newIndex + "].IsChecked").val(true).end()
            .appendTo($tagEditor.find(".tag-select"));

        $newTagName.val("");
        $addTagButton.prop("disabled", true);
    }
});