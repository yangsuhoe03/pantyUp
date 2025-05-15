const express = require('express');
const http = require('http');
const { Server } = require('socket.io');
const path = require('path');
const app = express();
const server = http.createServer(app);
const PORT = process.env.PORT || 3000;

const io = new Server(server, {
  cors: {
    origin: "*", // 개발 시엔 전체 허용, 배포 시엔 특정 도메인으로 제한
    methods: ["GET", "POST"]
  }
});

// 🔹 Brotli 압축 파일 MIME 설정
app.use((req, res, next) => {
  if (req.url.endsWith('.js.br')) {
    res.set('Content-Encoding', 'br');
    res.set('Content-Type', 'application/javascript');
  } else if (req.url.endsWith('.wasm.br')) {
    res.set('Content-Encoding', 'br');
    res.set('Content-Type', 'application/wasm');
  } else if (req.url.endsWith('.data.br')) {
    res.set('Content-Encoding', 'br');
    res.set('Content-Type', 'application/octet-stream');
  }
  next();
});

// 🔹 정적 파일 서빙
app.use(express.static(path.join(__dirname)));

// 🔹 index.html 서빙
app.get('/', (req, res) => {
  res.sendFile(path.join(__dirname, 'index.html'));
});

// 🔹 Socket.IO 통신 처리
io.on('connection', (socket) => {
  console.log('✅ Unity 클라이언트 연결됨');


  
    socket.on('SendPos', (pos) => {
    console.log('3');
    socket.emit('ServerToPos', pos);

  });


  socket.on('disconnect', () => {
    console.log('❌ 클라이언트 연결 종료');
  });
});

// 🔹 서버 실행
server.listen(PORT, () => {
  console.log(`🚀 서버 실행 중: http://localhost:${PORT}`);
});
//<script src="https://cdn.socket.io/4.6.1/socket.io.min.js"></script>
