mergeInto(LibraryManager.library, {
  ConnectToSocket: function () {
    if (typeof io === 'undefined') {
      console.error("Socket.IO is not loaded.");
      return;
    }

    window.socket = io();

    socket.on('connect', function () {
      console.log(" Socket �����: " + socket.id);

      // Unity�� ���� ID ����
      SendMessage('SocketManager', 'SetMyId', socket.id);

      // ������ �� ID ��� ��û
      socket.emit('MakePlayers', socket.id);
    });

    socket.on('ServerToPos', function (Pos) {
      var pos = UTF8ToString(Pos);
      SendMessage('SocketManager', 'ReceivePos', pos);
    });

    socket.on('MakePlayers', function (Players) {
      var players = UTF8ToString(Players);
      console.log(" ��� �÷��̾� ���� ����:", players);
      SendMessage('SocketManager', 'MakePlayer', players);
    });
  },

  SendPosToServer: function (Pos) {
    var pos = UTF8ToString(Pos);
    socket.emit('SendPos', pos);
  }
});
