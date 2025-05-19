mergeInto(LibraryManager.library, {
  ConnectToSocket: function () {
    if (typeof io === 'undefined') {
      console.error("Socket.IO is not loaded.");
      return;
    }

    window.socket = io();
    socket.on('connect', function () {
      console.log(" Socket connssssssssssssssssssssected.");
      socket.emit('makePlayers', socket.id);
    });

    socket.on('ServerToPos', function(data){

      SendMessage('SocketManager', 'ReceivePos', data);

    });


    socket.on('ServerToMakePlayers', function(Players){


        SendMessage('SocketManager', 'MakePlayer', Players);



    });




  },

  SendPosToServer: function (Pos) {

    var pos = UTF8ToString(Pos); 
    socket.emit('SendPos', pos);

  }
  
});