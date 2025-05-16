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

    socket.on('ServerToPos', function(Pos){
      var pos = UTF8ToString(Pos); 
      //console.log("3", pos);

      SendMessage('SocketManager', 'ReceivePos', pos);
    });

    socket.on('ServerToMakePlayers', function(Players){
        var pos = UTF8ToString(Players); 
        console.log("hihihi", Players);
        SendMessage('SocketManager', 'MakePlayer', Players);
        SendMessage('SocketManager', 'MakePlayer', pos);
    });




  },

  SendPosToServer: function (Pos) {
    var pos = UTF8ToString(Pos); 
    //console.log("1",pos);
    socket.emit('SendPos', pos);
  }
});