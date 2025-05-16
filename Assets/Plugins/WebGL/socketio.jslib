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
      console.log("3", pos);

      SendMessage('SocketManager', 'ReceivePos', pos);
    });

    socket.on('MakePlayers', function(Players){
        var players = UTF8ToString(Players); 
        console.log(Players);
        console.log(players);
        SendMessage('SocketManager', 'MakePlayer', Players);
        SendMessage('SocketManager', 'MakePlayer', players);
    });




  },

  SendPosToServer: function (Pos) {
    var pos = UTF8ToString(Pos); 
    console.log("1",pos);
    socket.emit('SendPos', pos);
  }
});