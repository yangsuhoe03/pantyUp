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

// ✅ 트래픽 제한 로직 (socket.id 기반)
const socketRequestMap = {}; // { socket.id: [timestamps] }
function isRateLimited(socket, limit = 20, windowMs = 1000) {
  const now = Date.now();
  if (!socketRequestMap[socket.id]) socketRequestMap[socket.id] = [];
  socketRequestMap[socket.id] = socketRequestMap[socket.id].filter((t) => now - t < windowMs);
  if (socketRequestMap[socket.id].length >= limit) return true;
  socketRequestMap[socket.id].push(now);
  return false;
}
// 🔁 정기적으로 오래된 기록 정리 (메모리 누수 방지)
setInterval(() => {
  const now = Date.now();
  for (const id in socketRequestMap) {
    socketRequestMap[id] = socketRequestMap[id].filter((t) => now - t < 5000);
    if (socketRequestMap[id].length === 0) delete socketRequestMap[id];
  }
}, 600000); // 10분 간격



let Scores = {};
const RoomPlayerStatus = {}; // { roomName: { playerId: { nickname, score } } }
const Rooms = {}; // { roomName: [playerId1, playerId2, ...] }
const PlayerRooms = {}; // { playerId: roomName } - 플레이어가 속한 방 정보
let roomCount = 0;
const MAX_PLAYERS_PER_ROOM = 6;

// ✅ 방별 타이머 관리 시스템
const roomStartTime = {}; // { roomName: 시작시간 } - 각 방의 게임 시작 시간을 저장
const roomTimers = {};    // { roomName: setInterval 핸들 } - 각 방의 타이머 인터벌을 저장
const GAME_DURATION = 10 * 60 * 1000; // 10분 (밀리초 단위) - 게임 지속 시간

const itemSpawnTimers = {}; // { roomName: setInterval 핸들 }

function UpdatePlayerStatus(roomName) {
  if (!RoomPlayerStatus[roomName]) return '';
  
  const statusStr = Object.entries(RoomPlayerStatus[roomName])
    .map(([pid, info]) => `${pid},${info.nickname},${info.score}`)
    .join('|');
  return statusStr;
}

// ✅ 타이머 핵심 함수: 특정 방의 남은 시간을 계산
function getRemainingTime(roomName) {
  const now = Date.now(); // 현재 시간
  const elapsed = now - roomStartTime[roomName]; // 경과 시간 계산
  return Math.max(0, Math.floor((GAME_DURATION - elapsed) / 1000)); // 남은 시간을 초 단위로 반환 (최소 0초)
}

io.on('connection', (socket) => {
  console.log(' Unity 클라이언트 연결됨', socket.id);

  socket.on('joinRandomRoom', (playerId) => {
    let joinedRoom = null;

    for (const roomName in Rooms) {
      if (Rooms[roomName].length < MAX_PLAYERS_PER_ROOM) {
        Rooms[roomName].push(playerId);
        joinedRoom = roomName;
        break;
      }
    }

    if (!joinedRoom) {
      roomCount++;
      console.log("방 생성: ", roomCount);
      joinedRoom = `room${roomCount}`;
      Rooms[joinedRoom] = [playerId];
      RoomPlayerStatus[joinedRoom] = {};

      if (!itemSpawnTimers[joinedRoom]) {
        itemSpawnTimers[joinedRoom] = setInterval(() => {
          // 6개 스폰포인트에 대해 30% 확률로 1, 아니면 0
          const spawnArray = Array.from({length: 6}, () => Math.random() < 0.2 ? 1 : 0);
          // 배열을 문자열로 변환: "0,1,0,0,1,0"
          const spawnString = spawnArray.join(',');
          io.to(joinedRoom).emit('ServerToItemSpawn', spawnString);
        }, 10000); // 10초
      }
    }

    PlayerRooms[playerId] = joinedRoom;
    socket.join(joinedRoom);

    console.log(`${playerId} joined ${joinedRoom}`);

    socket.emit('joinedRoom', joinedRoom);
    io.to(joinedRoom).emit('roomPlayerList', Rooms[joinedRoom].join(','));
    console.log(`방 ${joinedRoom} 플레이어 목록: `, Rooms[joinedRoom]);
    // 방 입장 시 바로 플레이어 상태 업데이트 전송
    //io.to(joinedRoom).emit('updatePlayerStatus', UpdatePlayerStatus(joinedRoom));

    // ✅ 방 입장 시 타이머 관리 로직
    if (!roomStartTime[joinedRoom]) {
      // 🔹 새로 생성된 방인 경우: 타이머 시작
      roomStartTime[joinedRoom] = Date.now(); // 방의 시작 시간 기록

      // 🔹 방별 독립 타이머 생성 (1초마다 실행)
      roomTimers[joinedRoom] = setInterval(() => {
        const remaining = getRemainingTime(joinedRoom); // 남은 시간 계산
        io.to(joinedRoom).emit('ServerToTimeSync', remaining); // 방의 모든 플레이어에게 남은 시간 전송

        // 🔹 score 10점 이상인 플레이어가 있는지 체크
        let isScoreOver = false;
        if (RoomPlayerStatus[joinedRoom]) {
          for (const pid in RoomPlayerStatus[joinedRoom]) {
            if (RoomPlayerStatus[joinedRoom][pid].score >=10) {
              isScoreOver = true;
              break;
            }
          }
        }

        // 🔹 타이머 종료 조건: 남은 시간이 0초 이하이거나, 3점 이상인 플레이어가 있을 때
        if (remaining <= 0 || isScoreOver) {
          io.to(joinedRoom).emit('GameOver'); // 게임 종료 이벤트 전송
          // 방 정보, 상태, 타이머 즉시 삭제
          if (roomTimers[joinedRoom]) {
            clearInterval(roomTimers[joinedRoom]);
            delete roomTimers[joinedRoom];
          }
          if (itemSpawnTimers[joinedRoom]) {
            clearInterval(itemSpawnTimers[joinedRoom]);
            delete itemSpawnTimers[joinedRoom];
          }
          if(Rooms[joinedRoom] != null){
            delete Rooms[joinedRoom];
             delete RoomPlayerStatus[joinedRoom];
             delete roomStartTime[joinedRoom];
             roomCount--;
          }
        }
      }, 1000); // 1초(1000ms) 간격으로 실행
    } else {
      // 🔹 기존 방에 입장한 경우: 현재 남은 시간만 전송
      socket.emit('ServerToTimeSync', getRemainingTime(joinedRoom));
    }
  });

  socket.on('setNickName', (playerStatus) => {
    console.log("닉네임 설정 받음: ", playerStatus);
    const [id, nickname] = playerStatus.split(',');
    const playerRoom = PlayerRooms[id];
    if (!playerRoom) return;

    if (!RoomPlayerStatus[playerRoom][id]) {
      RoomPlayerStatus[playerRoom][id] = {
        nickname: nickname,
        score: 1
      };
    } else {
      RoomPlayerStatus[playerRoom][id].nickname = nickname;
    }

    console.log("정보 전송: ", UpdatePlayerStatus(playerRoom));
    io.to(playerRoom).emit('updatePlayerStatus', UpdatePlayerStatus(playerRoom));
    io.to(playerRoom).emit('ServerToMakePlayers');
  });

  socket.on('SendPos', (pos) => {
      // if (isRateLimited(socket, 10, 1000)) {
      //   console.log(`\u274C ${socket.id} 과도한 이동 요청`);
      //   socket.disconnect(true);
      //   return; // 과도한 요청은 무시
      // }
    const data = `${socket.id}:${pos}`;
    const playerRoom = PlayerRooms[socket.id];
    if (playerRoom) {
      socket.to(playerRoom).emit('ServerToPos', data);
    }
  });

  socket.on('SendAnimNumber', (data) => {
    const playerRoom = PlayerRooms[socket.id];
    if (playerRoom) {
      socket.to(playerRoom).emit('ServerToAnimNumber', data);
    }
  });

  socket.on('SendItemGet', (playerId) => {
      if (isRateLimited(socket, 10, 1000)) {
        console.log(`\u274C ${socket.id} 과도한 아이템 획득 요청`);
        socket.disconnect(true);
        return; // 과도한 요청은 무시
      }
    const playerRoom = PlayerRooms[playerId];
    if (playerRoom && RoomPlayerStatus[playerRoom][playerId]) {
      RoomPlayerStatus[playerRoom][playerId].score += 1;
      io.to(playerRoom).emit('updatePlayerStatus', UpdatePlayerStatus(playerRoom));
    }
  });

  socket.on('SendAttack', (attacks) => {
      if (isRateLimited(socket, 10, 1000)) {
        console.log(`\u274C ${socket.id} 과도한 공격 요청`);
        socket.disconnect(true);
        return; // 과도한 요청은 무시
      }
    console.log("서버받음: ", attacks);
    const playerRoom = PlayerRooms[socket.id];
    if (playerRoom) {
      io.to(playerRoom).emit('ServerToAttack', attacks);
    }
  });

  socket.on('SendSucceseAttack', (data) => {
      if (isRateLimited(socket, 10, 1000)) {
        console.log(`\u274C ${socket.id} 과도한 공격성공 요청`);
        socket.disconnect(true);
        return; // 과도한 요청은 무시
      }
    const [attackerID, targetID] = data.split(',');
    const attackerRoom = PlayerRooms[attackerID];
    const targetRoom = PlayerRooms[targetID];

    if (!attackerRoom || !targetRoom || attackerRoom !== targetRoom) {
      console.log('다른 방의 플레이어를 공격할 수 없습니다:', { attackerID, targetID });
      return;
    }

    if (!RoomPlayerStatus[attackerRoom][attackerID] || !RoomPlayerStatus[attackerRoom][targetID]) {
      console.log('유효하지 않은 플레이어 ID:', { attackerID, targetID });
      return;
    }

    RoomPlayerStatus[attackerRoom][attackerID].score += RoomPlayerStatus[attackerRoom][targetID].score;
    RoomPlayerStatus[attackerRoom][targetID].score = 1;

    io.to(attackerRoom).emit('ServerToSucceseAttack', data);
    io.to(attackerRoom).emit('updatePlayerStatus', UpdatePlayerStatus(attackerRoom));
  });

  socket.on('SendFaildAttack', (data) => {
    const playerRoom = PlayerRooms[socket.id];
    if (playerRoom) {
      io.to(playerRoom).emit('ServerToFaildAttack', data);
    }
  });

  socket.on('disconnect', () => {
    console.log('클라이언트 연결 종료');
    const disconnectedId = socket.id;
    const playerRoom = PlayerRooms[disconnectedId];

    if (playerRoom && Rooms[playerRoom]) {
      Rooms[playerRoom] = Rooms[playerRoom].filter(id => id !== disconnectedId);
      if (RoomPlayerStatus[playerRoom]) {
        delete RoomPlayerStatus[playerRoom][disconnectedId];
      }

      if (Rooms[playerRoom].length === 0) {
        delete Rooms[playerRoom];
        delete RoomPlayerStatus[playerRoom];

        roomCount--;

        
        // ✅ 방이 비었을 경우 타이머도 정리
        if (roomTimers[playerRoom]) {
          clearInterval(roomTimers[playerRoom]); // 타이머 인터벌 정지
          delete roomTimers[playerRoom]; // 타이머 핸들 삭제
        }
        delete roomStartTime[playerRoom]; // 시작 시간 정보 삭제

        console.log(`방 ${playerRoom} 삭제됨`);
      } else {
        io.to(playerRoom).emit('updatePlayerStatus', UpdatePlayerStatus(playerRoom));
        io.to(playerRoom).emit('roomPlayerList', Rooms[playerRoom].join(','));
        console.log(`플레이어 ${disconnectedId}가 방 ${playerRoom}에서 나감`);
      }
      if (itemSpawnTimers[playerRoom]) {
        clearInterval(itemSpawnTimers[playerRoom]);
        delete itemSpawnTimers[playerRoom];
      }
    }

    delete PlayerRooms[disconnectedId];
    console.log(`플레이어 ${disconnectedId} 정보 정리 완료`);
  });
});

// 🔹 서버 실행
server.listen(PORT, '0.0.0.0', () => {
  console.log('🚀 서버 실행 중');
});

//<script src="https://cdn.socket.io/4.6.1/socket.io.min.js"></script>