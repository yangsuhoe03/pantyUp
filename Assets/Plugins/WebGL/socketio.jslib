mergeInto(LibraryManager.library, {
  ConnectToSocket: function () {
    if (typeof io === 'undefined') {
      console.error("Socket.IO is not loaded.");
      return;
    }

    window.socket = io();

    socket.on('connect', function () {
      console.log(" Socket 연결됨: " + socket.id);

      // Unity에 본인 ID 전송
      SendMessage('SocketManager', 'SetMyId', socket.id);

      // 서버에 내 ID 등록 요청
      socket.emit('MakePlayers', socket.id);
    });

    socket.on('ServerToPos', function (Pos) {
      var pos = UTF8ToString(Pos);
      SendMessage('SocketManager', 'ReceivePos', pos);
    });

    socket.on('MakePlayers', function (Players) {
      var players = UTF8ToString(Players);
      console.log(" 모든 플레이어 정보 수신:", players);
      SendMessage('SocketManager', 'MakePlayer', players);
    });
  },

  SendPosToServer: function (Pos) {
    var pos = UTF8ToString(Pos);
    socket.emit('SendPos', pos);
  }
});
