var mixinArray = [];

$(function () {
    ButtonSubmit();
    $(window).scroll(function () {    // this will work when your window scrolled.
        var height = $(window).scrollTop();  //getting the scrolling height of window
        if (height > 0) {
            $(".header-stack").addClass('sticky')
        } else {
            $(".header-stack").removeClass('sticky')
        }
    });

    // auto hide notification
    if ($(".notify").length > 0) {
        setTimeout(function() {
            $(".notify").fadeOut('slow');
        }, 3000);
    }

    $("form").on("change", ".file-upload-field", function () {
        var value = $(this).val().replace(/.*(\/|\\)/, '');
        $(this).parent().find(".file-custom").text(value);
    });

    // var stickyOffset = $('.sticky').offset().top;
    //
    // $(window).scroll(function(){
    //     var sticky = $('.sticky'),
    //         scroll = $(window).scrollTop();
    //
    //     if (scroll >= 100) sticky.addClass('fixed');
    //     else sticky.removeClass('fixed');
    // });

    tinymce.init({
        selector: 'textarea.editor',
        branding: false,
        height: 300,
        menubar: false,
        plugins: [
            'advlist autolink lists link image charmap print preview anchor',
            'searchreplace visualblocks code fullscreen'
        ],
        toolbar: 'bold italic underline | alignleft aligncenter ' +
            'alignright alignjustify | bullist numlist outdent indent | ' +
            'removeformat | code',

    });

    $("#premium-modal").on("show.bs.modal", function() {
        var curModal;
        curModal = this;
        $(".modal").each(function() {
            if (this !== curModal) {
                $(this).modal("hide");
            }
        });
    });

    $("#payment-modal").on("show.bs.modal", function() {
        var curModal;
        curModal = this;
        $(".modal").each(function() {
            if (this !== curModal) {
                $(this).modal("hide");
            }
        });
    });

    var placeholderElement = $('#modal-placeholder');

    $('button[data-toggle="ajax-modal"]').click(function (event) {
        var url = $(this).data('url');
        
        $.get(url).done(function (data) {
            placeholderElement.html(data);
            placeholderElement.find('.modal').modal('show');
        });
    });

    placeholderElement.on('click', '[data-save="modal"]', function (event) {
        event.preventDefault();

        var form = $(this).parents('.modal').find('form');
        var actionUrl = form.attr('action');
        var dataToSend = new FormData(form.get(0));
        var method = form.attr('method');

        $.ajax({
            url: actionUrl, method: method, data: dataToSend, processData: false, contentType: false, success: function (data) {

                if (typeof data == 'object') {
                    switch (data.action) {
                        case 'Redirect':
                            window.location.href = data.url;
                            break;
                        case 'Reload':
                            window.location.reload();
                            break;
                        case 'Js':
                            eval(data.javascriptResponse);
                            placeholderElement.find('.modal').modal('hide');
                            break;
                    }

                } else {
                    var newBody = $('.modal-body', data);
                    placeholderElement.find('.modal-body').replaceWith(newBody);

                    var isValid = newBody.find('[name="IsValid"]').val() == 'True';
                    if (isValid) {
                        placeholderElement.find('.modal').modal('hide');
                        window.location.reload();
                    }
                }
            }
        });
    });

    $('.count').each(function () {
        $(this).prop('Counter',0).animate({
            Counter: $(this).text()
        }, {
            duration: 1500,
            easing: 'linear',
            step: function (now) {
                $(this).text(now.toFixed(1));
            }
        });
    });

    $('.scroll-to').click(function(e){
        var jump = $(this).attr('href');
        var new_position = $(jump).offset();
        $('html, body').stop().animate({ scrollTop: new_position.top }, 500);
        e.preventDefault();
    });

    if ($('[data-typed-text]').length > 0) {
        var typedTxt = $('[data-typed-text]').data('typed-text').split(',');
        var typed = new Typed('[data-typed-text]', {
            strings: typedTxt, // string need to type
            typeSpeed: 80, // type speed in milliseconds
            loop: true, // loop after last string is typed
            backSpeed: 80, // backspacing speed in milliseconds
            showCursor: false // show cursor
        });
    }

    $('.lazy').Lazy({
        scrollDirection: 'vertical',
        effect: 'fadeIn',
        visibleOnly: true,
        onError: function(element) {
            console.log('error loading ' + element.data('src'));
        }
    });
});


$(document).ready(function () {
    var walkthrough;
    walkthrough = {
        index: 0,
        nextScreen: function () {
            if (this.index < this.indexMax()) {
                this.index++;
                return this.updateScreen();
            }
        },
        prevScreen: function () {
            if (this.index > 0) {
                this.index--;
                return this.updateScreen();
            }
        },
        updateScreen: function () {
            this.reset();
            this.goTo(this.index);
            return this.setBtns();
        },
        setBtns: function () {
            var $lastBtn, $nextBtn, $prevBtn;
            $nextBtn = $('.next-screen');
            $prevBtn = $('.prev-screen');
            $lastBtn = $('.finish');
            if (walkthrough.index === walkthrough.indexMax()) {
                $nextBtn.prop('disabled', true).addClass('disabled');
                $prevBtn.prop('disabled', false).removeClass('disabled');
                return $lastBtn.addClass('active').prop('disabled', false).removeClass('disabled');
            } else if (walkthrough.index === 0) {
                $nextBtn.prop('disabled', false).removeClass('disabled');
                $prevBtn.prop('disabled', true).addClass('disabled');
                return $lastBtn.removeClass('active').prop('disabled', true).addClass('disabled');
            } else {
                $nextBtn.prop('disabled', false).removeClass('disabled');
                $prevBtn.prop('disabled', false).removeClass('disabled');
                return $lastBtn.removeClass('active').prop('disabled', true).addClass('disabled');
            }
        },
        goTo: function (index) {
            $('.screen').eq(index).addClass('active');
            return $('.dot').eq(index).addClass('active');
        },
        reset: function () {
            return $('.screen, .dot').removeClass('active');
        },
        indexMax: function () {
            return $('.screen').length - 1;
        },
        closeModal: function () {
            $('.walkthrough, .shade').removeClass('reveal');
            return setTimeout(((function (_this) {
                return function () {
                    $('.walkthrough, .shade').removeClass('show');
                    _this.index = 0;
                    return _this.updateScreen();
                };
            })(this)), 200);
        },
        openModal: function () {
            $('.walkthrough, .shade').addClass('show');
            setTimeout(((function (_this) {
                return function () {
                    return $('.walkthrough, .shade').addClass('reveal');
                };
            })(this)), 200);
            return this.updateScreen();
        }
    };
    $('.next-screen').click(function () {
        return walkthrough.nextScreen();
    });
    $('.prev-screen').click(function () {
        return walkthrough.prevScreen();
    });
    $('.get-started').click(function () {
        window.location.href = '/jobs';
        // return walkthrough.closeModal();
    });
    $('.open-walkthrough').click(function () {
        return walkthrough.openModal();
    });
    walkthrough.openModal();
    return $(document).keydown(function (e) {
        switch (e.which) {
            case 37:
                walkthrough.prevScreen();
                break;
            case 38:
                walkthrough.openModal();
                break;
            case 39:
                walkthrough.nextScreen();
                break;
            case 40:
                walkthrough.closeModal();
                break;
            default:
                return;
        }
        e.preventDefault();
    });

    
    
});

function RequestBegin() {
    $("#process-request").show();
}

function RequestComplete() {
    $("#process-request").fadeOut();
}


ButtonSubmit = function() {
    $('button[type="submit"]').click(function() {
        var form = $(this).closest('form');
        if ($(form).valid()) {
            RequestBegin();
            // $(this).addClass("is-loading");
        }
    });

    $('a').click(function() {
        var comp = new RegExp(location.host);
        var url = $(this).attr('href');
        if(comp.test(url) || url.startsWith('/') || url === 'javascript:history.back();'){
            RequestBegin();
        }
    });
};

