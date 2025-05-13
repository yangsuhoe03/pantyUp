mergeInto(LibraryManager.library, {
  ConnectToSocket: function () {
    if (typeof io === 'undefined') {
      console.error("Socket.IO is not loaded.");
      return;
    }

    window.socket = io();

    socket.on('connect', function () {
      console.log(" Socket connected.");
    });

    socket.on('serverToUnity', function (data) {
      console.log(" Received from server:", data);
      SendMessage('SocketManager', 'ReceiveMessage', data);
    });
  },

  SendMessageToServer: function (ptr) {
    const message = UTF8ToString(ptr);
    if (window.socket) {
      console.log(" Sending to server:", message);
      socket.emit('unityToServer', message);
    } else {
      console.warn("Socket not connected.");
    }
  }
});