mergeInto(LibraryManager.library, {
  ConnectToSocket: function () {
    if (typeof io === 'undefined') {
      console.error("Socket.IO is not loaded.");
      return;
    }

    window.socket = io();

    socket.on('connect', function () {
      console.log(" Socket connssssssssssssssssssssected.");
    });

    socket.on('ServerToPos', function(Pos){
      var pos = UTF8ToString(Pos); 
      console.log("4");
      SendMessage('SocketManager', 'ReceivePos', pos);
    })
  },

  SendPosToServer: function (Pos) {
    var pos = UTF8ToString(Pos); 
    console.log("2");
    socket.emit('SendPos', pos);
  }
});