"use strict";
var userId = getCookieValue('FemoriUserId');
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub?UserId="+userId+"").build(); //for hub connection

connection.start();

function getCookieValue(a) {
    debugger;
    var b = document.cookie.match('(^|[^;]+)\\s*' + a + '\\s*=\\s*([^;]+)');
    return b ? b.pop() : '';
}

//connection should be started now. beware of exceptions though

$(".messages").animate({ scrollTop: $(document).height() }, "fast");

    $("#profile-img").click(function () {
        $("#status-options").toggleClass("active");
    });

    $(".expand-button").click(function () {
        $("#profile").toggleClass("expanded");
        $("#contacts").toggleClass("expanded");
    });

    $("#status-options ul li").click(function () {
        $("#profile-img").removeClass();
        $("#status-online").removeClass("active");
        $("#status-away").removeClass("active");
        $("#status-busy").removeClass("active");
        $("#status-offline").removeClass("active");
        $(this).addClass("active");

        if ($("#status-online").hasClass("active")) {
            $("#profile-img").addClass("online");
        } else if ($("#status-away").hasClass("active")) {
            $("#profile-img").addClass("away");
        } else if ($("#status-busy").hasClass("active")) {
            $("#profile-img").addClass("busy");
        } else if ($("#status-offline").hasClass("active")) {
            $("#profile-img").addClass("offline");
        } else {
            $("#profile-img").removeClass();
        };

        $("#status-options").removeClass("active");
    });



$('.userlist').off('click').on('click', function (e) {
    e.preventDefault();
    
    $('.userlist').removeClass('active');
    $(this).addClass('active');
    var conversationId = $(this).attr('data-conversationid');
    var frienduserid = $(this).attr('data-userid');
    var friendimagepath = $(this).children('.wrap').children('img').attr('src');
    var friendname = $(this).children('.wrap').children('.meta').children().children().children('p').text();
    $('.messages').attr('data-chatconversId', conversationId);
    if (conversationId != null && conversationId != "") {
        var url = "/Chat/Getmessagehistory";
        $.get(url, {
            conversationId: conversationId
        },
            function (data) {
               
                $("#messagelist").html('');
                $("#messagelist").hide().html(data).fadeIn(100);
            });
    }
    else
    {
        var curruserId = $('#profile').attr('data-curruserId');
       
        $("#messagelist").hide().html("").fadeIn(100);
        
    }
    $('#messagelist').attr('data-curruserId', curruserId);
    $('#messagelist').attr('data-frienduserid', frienduserid);
    $('#messagelist').attr('data-chatconversId', conversationId);
    $('.contact-profile').children('img').attr('src', friendimagepath);
    $('.contact-profile').children('p').text(friendname);
    
});

$('.contacticon').off().on('click', function () {
    $('.messagediv').slideToggle();
    $('.suggestiondiv').slideToggle();
});
function checkTime(i) {
    if (i < 10) {
        i = '0' + i;
    }
    return i;
}

function dayOfWeek(dayIndex) {
    return ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"][dayIndex];
}
function boldString(str, find) {
    var re = new RegExp(find, 'g');
    return str.replace(re, '<b>' + find + '</b>');
}

function newMessage() {
   
    var fromuser = $('#messagelist').attr('data-curruserId');
    var touser = $('#messagelist').attr('data-frienduserid');
    var fromuserimage = $('#profile').children('div').children('img').attr('src');
    var message = $('#messageInput').val();
    var ChatconversId = $('#messagelist').attr('data-chatconversId');
    if ($.trim(message) == '') {
        return false;
    }

    connection.invoke("SendMessage", fromuser, touser, ChatconversId, fromuserimage, message).catch(function (err) {
        return console.error(err.toString());
    });

    event.preventDefault();
}

$('.submit').off().on('click',function () {
    newMessage();
   
});

$(window).on('keydown', function (e) {
    if (e.which == 13) {
        newMessage();
        return false;
    }
});


connection.on("ReceiveMessage", function (fromuser, touser, ChatconversId, fromuserimagestring, message, strdate) {  //on message receive

    $('.messages').attr('data-chatconversId', ChatconversId);
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var today = new Date();
    var hours = today.getHours();
    var minutes = today.getMinutes();
    var seconds = today.getSeconds();
    var day = dayOfWeek(today.getDay());
    var date = today.getDate();

    minutes = checkTime(minutes);
    seconds = checkTime(seconds);
    var fromuserdiv, touserdiv;
    var time = " " + hours + ":" + minutes + ":" + seconds + " | " + day + " " + date + " ";
    if (fromuser.toString() === '') {
        fromuser = 'Anonymous';
    }

    var ConversId = $('#messagelist').attr('data-chatconversId');

    if (ConversId != null && ConversId != '') {
        fromuserdiv = $('#messagelist[data-chatconversId=' + ChatconversId + ']' + '[data-curruserId=' + fromuser + ']');
        touserdiv = $('#messagelist[data-chatconversId=' + ChatconversId + ']' + '[data-curruserId=' + touser + ']');
    }
    else {
        fromuserdiv = $('#messagelist[data-curruserId=' + fromuser + ']');
        touserdiv = $('#messagelist[data-curruserId=' + touser + ']');
    }
     
    var frommessage = '<li class="replies"><img src="' + fromuserimagestring + '" alt="" /><p>' + message + '<br/>' + strdate + '</p></li>';
    var tomessage = '<li class="sent"><img src="' + fromuserimagestring + '" alt="" /><p>' + message + '<br/>' + strdate + '</p></li>';
    $(frommessage).appendTo($(fromuserdiv));
    $(tomessage).appendTo($(touserdiv));
    $('.message-input input').val(null);
    //$('.contact.active .preview').html('<span>You: </span>' + message);
    $(".messages").animate({ scrollTop: $(document).height() }, "fast");
   
});



connection.on("UpdateConnectedSignaling", function (UserIds) {  //on user connection
    debugger;
    $.each(UserIds, function (j, UserId) {
        if (UserId != null && UserId!="")
        {
            var wrapper = $('.userlist[data-userid=' + UserId + ']').children('div').children('span');
            $(wrapper).removeClass('busy');
            $(wrapper).addClass('online');
        }
       
    });
});

connection.on("UpdateDisConnectedSignaling", function (UserId) {  //on user connection
    debugger;
        if (UserId != null && UserId != "") {
            var wrapper = $('.userlist[data-userid=' + UserId + ']').children('div').children('span');
            $(wrapper).removeClass('online');
            $(wrapper).addClass('busy');
        }

   
});

//function newMessage() {
//    message = $(".message-input input").val();
//    if ($.trim(message) == '') {
//        return false;
//    }
//    $('<li class="replies"><img src="http://emilcarlsson.se/assets/mikeross.png" alt="" /><p>' + message + '</p></li>').appendTo($('.messages ul'));
//    $('.message-input input').val(null);
//    $('.contact.active .preview').html('<span>You: </span>' + message);
//    $(".messages").animate({ scrollTop: $(document).height() }, "fast");
//};


    //var encodedMsg = user + ": " + msg;

    //var li = document.createElement("li");
    //li.setAttribute("id", "onlineMessages");
    //li.className = "list-group-item list-group-item-info";

    //var span = document.createElement("span");
    //span.className = "label label-primary label-as-badge";
    //span.textContent = time;
    //span.style.cssFloat = "right";


    //li.textContent = encodedMsg;

    //var colorValue = document.getElementById("valueInputColor").value;
    //li.style.color = colorValue;
    //document.getElementById("messagesList").appendChild(li);
    //document.getElementById("onlineMessages").appendChild(span);
    //document.getElementById("onlineMessages").innerHTML = boldString(document.getElementById("onlineMessages").innerHTML, user);
    //li.setAttribute("id", "temp");
    //li.scrollIntoView();
