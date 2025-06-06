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


app.use(express.static(path.join(__dirname)));


app.get('/', (req, res) => {
  res.sendFile(path.join(__dirname, 'index.html'));
});


let Players = [];
let Scores = {};
let allPlayerStatus = {};
io.on('connection', (socket) => {
  console.log(' Unity 클라이언트 연결됨', socket.id);

  socket.on('setNickName', (playerStatus) => {






  });


  socket.on('makePlayers', (PlayerID) => {
    if (!Players.includes(PlayerID)) {// 중복 방지
      Players.push(PlayerID);
      Scores[PlayerID] = 0; // 점수 초기화
    }

    // 쉼표로 이어 붙인 문자열로 보냄
    io.emit('ServerToMakePlayers', Players.join(','));



  });



  socket.on('SendPos', (pos) => {
    const data = `${socket.id}:${pos}`;

    //console.log(2, pos);
    socket.broadcast.emit('ServerToPos', data);


  });
  socket.on('SendAnimNumber', (data) => {


    //console.log(2, pos);
    socket.broadcast.emit('ServerToAnimNumber', data);


  });


  socket.on('SendAttack', (attacks) => {
    console.log("서버받음: ", attacks);

    io.emit('ServerToAttack', attacks);
  });

  socket.on('SendSucceseAttack', (data) => {
    // 공격 성공 처리: "공격자ID,피공격자ID" 형식의 문자열을 받아옴
    const [attackerID, targetID] = data.split(',');

    // 공격자 ID가 존재하고, 점수 테이블(Scores)에 해당 ID가 있으면
    if (attackerID && Scores[attackerID] !== undefined) {
      // 공격자의 점수를 1 증가시킴
      Scores[attackerID] += 1;
      Scores[targetID] = 0; // 피공격자의 점수는 0으로 초기화
    }
    else {
      console.error("공격자 ID가 유효하지 않거나 점수 테이블에 존재하지 않습니다:", attackerID);
    }

    // 현재 공격자의 점수를 콘솔에 출력 (디버깅용)
    console.log("공격 성공 처리:", attackerID, "=>", Scores[attackerID]);

    // 전체 플레이어들의 점수를 문자열로 변환
    // 예: {id1: 3, id2: 0} → "id1:3,id2:0"
    const scoreData = Object.entries(Scores) // [ [id1, 3], [id2, 0], ... ]
      .map(([id, score]) => `${id}:${score}`) // 각 항목을 "id:score" 형식 문자열로 만듦
      .join(','); // 배열을 쉼표로 이어 붙임
    console.log("점수 데이터:", scoreData); // 디버깅용

    io.emit('ServerToSucceseAttack', data);
    io.emit('ServerToScoreUpdate', scoreData); // 점수 업데이트 이벤트 전송
  });


  socket.on('SendFaildAttack', (data) => {
    io.emit('ServerToFaildAttack', data);

  });






  socket.on('disconnect', () => {
    console.log('클라이언트 연결 종료');
    // 연결 종료 시 목록에서 제거
    Players = Players.filter(id => id !== socket.id);
    delete Scores[socket.id]; // 점수도 제거

  });
});

// 🔹 서버 실행
server.listen(PORT, () => {
  console.log('🚀 서버 실행 중');
});
//<script src="https://cdn.socket.io/4.6.1/socket.io.min.js"></script>

