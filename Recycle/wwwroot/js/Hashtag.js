$("#hashtaginput").keypress(function (e) {
    //13 is enter
    // 9 for tab
    let hashtag = $("#hashtaginput").val()

    if (e.which == 13 && hashtag.length > 0) {
        event.preventDefault();
        if (hashtag.indexOf(',') != -1) {
            $('#hashtaginputError').text("',' is not allow in hashtag!")
            return
        }

        let is_already_added = false
        $("#hashtagsList").children().each(function (i) {
            if ($(this).text() == hashtag)
                is_already_added = true
        })

        if (is_already_added) {
            $('#hashtaginputError').text(hashtag + " already added")
            return
        }

        $("#hashtagsList").append('<span class="badge bg-primary" onclick="$(this).remove()">' + hashtag + '</span>')
        $("#hashtaginput").val("")
        $('#hashtaginputError').text("")
    }
})

$('form').submit(() => {
    let hashtagList = []
    $('#hashtagsList').children().each(function (i) {
        hashtagList.push($(this).text())
    });

    $('#hashtaginput').val(hashtagList);
})