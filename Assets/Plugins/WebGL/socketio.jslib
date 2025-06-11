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
    });

    window.socket.on('ServerToPos', function(data){
      SendMessage('SocketManager', 'ReceivePos', data);
    });

    window.socket.on('ServerToAnimNumber', function(data){
      SendMessage('SocketManager', 'ReceiveAnim', data);
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



    window.socket.on('updatePlayerStatus',function(data){
      
      SendMessage('SocketManager', 'ReceiveUpdatePlayerStatus', data);
    });






  },


  SendMakePlayers: function (playerID) {
    if (window.socket) {
      window.socket.emit('makePlayers', UTF8ToString(playerID));
    }
  },

  SendMyNickName: function (data) {
    var nickName = UTF8ToString(data); 
    if (window.socket) {
      window.socket.emit('setNickName', nickName);
    } 
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

  SendAnimToServer: function (data) {
    var animNumber = UTF8ToString(data); 
    
    if (window.socket) {
      window.socket.emit('SendAnimNumber', animNumber);
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
      window.socket.emit('SendFaildAttack', ATK);
    } 
  }



});
