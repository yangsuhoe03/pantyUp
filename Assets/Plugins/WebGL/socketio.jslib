mergeInto(LibraryManager.library, {
  ConnectToSocket: function () {
    if (typeof io === 'undefined') {
      console.error("Socket.IO is not loaded.");
      return;
    }

    window.socket = io(); // 전역 선언

    window.socket.on('connect', function () {
      console.log(" Socket connected.");
      SendMessage('SocketManager', 'SetMySocketID', window.socket.id);
      window.socket.emit('makePlayers', window.socket.id);
    });

    window.socket.on('ServerToPos', function(data){
      SendMessage('SocketManager', 'ReceivePos', data);
    });

    window.socket.on('ServerToMakePlayers', function(players){
      SendMessage('SocketManager', 'MakePlayer', players);
    });

    window.socket.on('ServerToAttack', function(attacks){
      console.log("클라 받음:", attacks);
      SendMessage('SocketManager', 'Attacking', attacks);
    });
  },

  SendPosToServer: function (Pos) {
    var pos = UTF8ToString(Pos); 
    if (window.socket) {
      window.socket.emit('SendPos', pos);
    } 
  },

  SendAttackToServer: function (attacks) {
    var ATK = UTF8ToString(attacks); 
    console.log("공격 전송:", ATK);
    if (window.socket) {
      window.socket.emit('SendAttack', ATK);
    } 
  }
});
