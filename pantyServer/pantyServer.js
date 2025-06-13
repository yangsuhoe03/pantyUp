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


let Scores = {};
let allPlayerStatus = {};
const Rooms = {}; // { roomName: [playerId1, playerId2, ...] }
const PlayerRooms = {}; // { playerId: roomName } - 플레이어가 속한 방 정보
let roomCount = 0;
const MAX_PLAYERS_PER_ROOM = 6;


function UpdatePlayerStatus(){
  const statusStr = Object.entries(allPlayerStatus)
  .map(([pid, info]) => `${pid},${info.nickname},${info.score}`)
  .join('|');
  return statusStr;
} //allPlayerStatus 문자열로 바꾸는 함수

io.on('connection', (socket) => {
  console.log(' Unity 클라이언트 연결됨', socket.id);

  // 방 자동 입장
  socket.on('joinRandomRoom', (playerId) => {
    let joinedRoom = null;

    // 방들 중 인원 미달인 방 탐색
    for (const roomName in Rooms) {
      if (Rooms[roomName].length < MAX_PLAYERS_PER_ROOM) {
        Rooms[roomName].push(playerId);
        joinedRoom = roomName;
        break;
      }
    }

    // 적절한 방이 없으면 새 방 생성
    if (!joinedRoom) {
      roomCount++;
      joinedRoom = `room${roomCount}`;
      Rooms[joinedRoom] = [playerId];
    }

    // 플레이어의 방 정보 저장
    PlayerRooms[playerId] = joinedRoom;

    // 소켓도 해당 방에 join
    socket.join(joinedRoom);

    console.log(`${playerId} joined ${joinedRoom}`);
    socket.emit('joinedRoom', joinedRoom); // 클라이언트에게 알림

    // 같은 방 모든 유저에게 방 참가 정보 전송
    io.to(joinedRoom).emit('roomPlayerList', Rooms[joinedRoom].join(','));
  });

  socket.on('setNickName', (playerStatus) => {
    console.log("닉네임 설정 받음: ", playerStatus);
    const [id, nickname] = playerStatus.split(',');

    // 플레이어 목록 및 점수 관리
    
    // 닉네임 및 점수 상태 관리
    if (!allPlayerStatus[id]) {
      allPlayerStatus[id] = {
        nickname: nickname,
        score: 1
      };
    } else {
      allPlayerStatus[id].nickname = nickname;
    }

    // 🔹 전체 상태 문자열 구성: "id,nickname,score|id2,nickname2,score2"
    console.log("정보 전송: ", UpdatePlayerStatus());

    // 플레이어가 속한 방 정보 가져오기
    const playerRoom = PlayerRooms[id];
    if (playerRoom) {
      // 해당 방의 플레이어들에게만 상태 업데이트 전송
      io.to(playerRoom).emit('updatePlayerStatus', UpdatePlayerStatus());
      io.to(playerRoom).emit('ServerToMakePlayers');
    }
  });

  socket.on('SendPos', (pos) => {
    const data = `${socket.id}:${pos}`;
    //console.log(2, pos);
    // 같은 방의 다른 플레이어들에게만 위치 정보 전송
    const playerRoom = PlayerRooms[socket.id];
    if (playerRoom) {
      socket.to(playerRoom).emit('ServerToPos', data);
    }
  });

  socket.on('SendAnimNumber', (data) => {
    //console.log(2, pos);
    // 같은 방의 다른 플레이어들에게만 애니메이션 정보 전송
    const playerRoom = PlayerRooms[socket.id];
    if (playerRoom) {
      socket.to(playerRoom).emit('ServerToAnimNumber', data);
    }
  });

  socket.on('SendAttack', (attacks) => {
    console.log("서버받음: ", attacks);
    // 같은 방의 플레이어들에게만 공격 정보 전송
    const playerRoom = PlayerRooms[socket.id];
    if (playerRoom) {
      io.to(playerRoom).emit('ServerToAttack', attacks);
    }
  });

  socket.on('SendSucceseAttack', (data) => {
    // 공격 성공 처리: "공격자ID,피공격자ID" 형식의 문자열을 받아옴
    const [attackerID, targetID] = data.split(',');

    // 공격자 ID가 존재하고, 점수 테이블(Scores)에 해당 ID가 있으면

    // 전체 플레이어들의 점수를 문자열로 변환
    // 예: {id1: 3, id2: 0} → "id1:3,id2:0"
    // const scoreData = Object.entries(Scores) // [ [id1, 3], [id2, 0], ... ]
    //   .map(([id, score]) => `${id}:${score}`) // 각 항목을 "id:score" 형식 문자열로 만듦
    //   .join(','); // 배열을 쉼표로 이어 붙임
    // console.log("점수 데이터:", scoreData); // 디버깅용


    allPlayerStatus[attackerID].score = allPlayerStatus[attackerID].score + allPlayerStatus[targetID].score;
    allPlayerStatus[targetID].score = 1;

    // 공격자와 타겟이 같은 방에 있는지 확인
    const attackerRoom = PlayerRooms[attackerID];
    const targetRoom = PlayerRooms[targetID];

    if (attackerRoom && attackerRoom === targetRoom) {
      // 같은 방의 플레이어들에게만 공격 성공 정보 전송
      io.to(attackerRoom).emit('ServerToSucceseAttack', data);
      io.to(attackerRoom).emit('updatePlayerStatus', UpdatePlayerStatus());
    }
  });


  socket.on('SendFaildAttack', (data) => {
    // 같은 방의 플레이어들에게만 공격 실패 정보 전송
    const playerRoom = PlayerRooms[socket.id];
    if (playerRoom) {
      io.to(playerRoom).emit('ServerToFaildAttack', data);
    }
  });






  socket.on('disconnect', () => {
    console.log('클라이언트 연결 종료');
    const disconnectedId = socket.id;
    
    // 플레이어가 속한 방 정보 가져오기
    const playerRoom = PlayerRooms[disconnectedId];
    if (playerRoom) {
      // 방에서 플레이어 제거
      Rooms[playerRoom] = Rooms[playerRoom].filter(id => id !== disconnectedId);
      
      // 방이 비었으면 삭제
      if (Rooms[playerRoom].length === 0) {
        delete Rooms[playerRoom];
      } else {
        // 같은 방의 다른 플레이어들에게 업데이트된 상태 전송
        io.to(playerRoom).emit('updatePlayerStatus', UpdatePlayerStatus());
        io.to(playerRoom).emit('roomPlayerList', Rooms[playerRoom].join(','));
      }
    }

    // 플레이어 정보 정리
    delete allPlayerStatus[disconnectedId];
    delete PlayerRooms[disconnectedId];
  });
});

// 🔹 서버 실행
server.listen(PORT, () => {
  console.log('🚀 서버 실행 중');
});
//<script src="https://cdn.socket.io/4.6.1/socket.io.min.js"></script>

