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
      SendMessage('SocketManager', 'ReceiveAttacking', attacks);
    });

    window.socket.on('ServerToSucceseAttack', function(attacks){
      SendMessage('SocketManager', 'ReceiveSucceseAttack', attacks);
    });

    window.socket.on('ServerToFaildAttack', function(attacks){
      SendMessage('SocketManager', 'ReceiveFaildAttack', attacks);
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
    if (window.socket) {
      window.socket.emit('SendAttack', ATK);
    } 
  },


  ScoreUp: function (attacks) {
    var ATK = UTF8ToString(attacks); 
      window.socket.emit('SendSucceseAttack', ATK);
      console.log("스코어 업");
  },

  SendAttackToFaild: function (attacks) {
    var ATK = UTF8ToString(attacks); 
    if (window.socket) {
      window.socket.emit('SendAttackFaild', ATK);
    } 
  }



});
