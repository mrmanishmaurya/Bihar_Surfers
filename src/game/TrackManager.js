// TrackManager.js - Procedural Endless Runner Engine & Obstacle Generator for Bihar Surfers
import * as THREE from 'three';

export class TrackManager {
  constructor(scene, assetFactory, journeyManager) {
    this.scene = scene;
    this.assetFactory = assetFactory;
    this.journeyManager = journeyManager;

    this.chunks = [];
    this.trains = [];
    this.obstacles = [];
    this.coins = [];
    this.powerUps = [];

    this.backdropMesh = null;

    this.chunkLength = 20;
    this.visibleChunks = 8;
    this.spawnZ = 120;

    this.magnetActive = false;
    this.magnetTimer = 0;
  }

  init(districtObj) {
    this.clearAll();

    // Spawn Station Backdrop Plane in the background
    const backdropTex = this.assetFactory.createStationBackdropTexture(districtObj);
    const backdropGeo = new THREE.PlaneGeometry(80, 40);
    const backdropMat = new THREE.MeshBasicMaterial({ map: backdropTex, depthWrite: false });
    this.backdropMesh = new THREE.Mesh(backdropGeo, backdropMat);
    this.backdropMesh.position.set(0, 18, 110);
    this.scene.add(this.backdropMesh);

    for (let i = -1; i < this.visibleChunks; i++) {
      const zPos = i * this.chunkLength;
      this.spawnChunk(zPos, districtObj, i > 1);
    }
  }

  clearAll() {
    if (this.backdropMesh) {
      this.scene.remove(this.backdropMesh);
      this.backdropMesh = null;
    }

    [...this.chunks, ...this.trains, ...this.obstacles, ...this.coins, ...this.powerUps].forEach(obj => {
      this.scene.remove(obj.mesh);
    });
    this.chunks = [];
    this.trains = [];
    this.obstacles = [];
    this.coins = [];
    this.powerUps = [];
  }

  spawnChunk(zPos, districtObj, spawnEntities = true) {
    const chunkMesh = this.assetFactory.createRailwayTrackChunk(districtObj);
    chunkMesh.position.z = zPos;
    this.scene.add(chunkMesh);

    this.chunks.push({ mesh: chunkMesh, z: zPos });

    if (Math.floor(zPos / this.chunkLength) % 3 === 0) {
      const platformMesh = this.assetFactory.createStationPlatform(districtObj);
      platformMesh.position.z = zPos;
      this.scene.add(platformMesh);
      this.chunks.push({ mesh: platformMesh, z: zPos, isPlatform: true });
    }

    if (spawnEntities) {
      this.generateChunkEntities(zPos);
    }
  }

  generateChunkEntities(zPos) {
    const lanes = [-2.5, 0, 2.5];
    const rand = Math.random();

    if (rand < 0.45) {
      const laneIdx = Math.floor(Math.random() * 3);
      const laneX = lanes[laneIdx];
      const isMoving = Math.random() < 0.4;

      const trainMesh = this.assetFactory.createTrainModel(isMoving ? 'express' : 'local');
      trainMesh.position.set(laneX, 0, zPos);
      if (isMoving) {
        trainMesh.rotation.y = Math.PI;
      }
      this.scene.add(trainMesh);

      this.trains.push({
        mesh: trainMesh,
        laneX,
        z: zPos,
        length: 14,
        height: 3.2,
        isMoving,
        speed: isMoving ? 12 : 0,
        nearMissTriggered: false
      });
    } else if (rand < 0.75) {
      const laneIdx = Math.floor(Math.random() * 3);
      const laneX = lanes[laneIdx];

      const barGeo = new THREE.BoxGeometry(2.0, 1.2, 0.4);
      const barMat = new THREE.MeshStandardMaterial({ color: 0xcc0000, roughness: 0.5 });
      const barMesh = new THREE.Mesh(barGeo, barMat);
      barMesh.position.set(laneX, 0.6, zPos);
      barMesh.castShadow = true;

      const stripeGeo = new THREE.BoxGeometry(2.02, 0.4, 0.42);
      const stripeMat = new THREE.MeshStandardMaterial({ color: 0xffcc00 });
      const stripe = new THREE.Mesh(stripeGeo, stripeMat);
      stripe.position.set(0, 0, 0);
      barMesh.add(stripe);

      this.scene.add(barMesh);
      this.obstacles.push({
        mesh: barMesh,
        laneX,
        z: zPos,
        width: 2.0,
        height: 1.2,
        type: 'barricade',
        nearMissTriggered: false
      });
    }

    const coinLane = lanes[Math.floor(Math.random() * 3)];
    const coinPattern = Math.random();
    if (coinPattern < 0.7) {
      for (let c = 0; c < 5; c++) {
        const coinZ = zPos - 8 + c * 2.2;
        const coinMesh = this.assetFactory.createRupeeCoinModel();
        coinMesh.position.set(coinLane, 1.0, coinZ);
        this.scene.add(coinMesh);

        this.coins.push({
          mesh: coinMesh,
          x: coinLane,
          y: 1.0,
          z: coinZ,
          collected: false
        });
      }
    }

    if (Math.random() < 0.15) {
      const pTypes = ['sattuRocket', 'littiShield', 'biharBoost', 'chaiMagnet', 'gangaGlide', 'superAuto'];
      const pType = pTypes[Math.floor(Math.random() * pTypes.length)];
      const pLane = lanes[Math.floor(Math.random() * 3)];
      const pZ = zPos - 2;

      const pMesh = this.assetFactory.createPowerUpModel(pType);
      pMesh.position.set(pLane, 1.2, pZ);
      this.scene.add(pMesh);

      this.powerUps.push({
        mesh: pMesh,
        type: pType,
        x: pLane,
        y: 1.2,
        z: pZ,
        collected: false
      });
    }
  }

  disposeMesh(group) {
    if (!group) return;
    group.traverse(child => {
      if (child.isMesh) {
        if (child.geometry) child.geometry.dispose();
        if (child.material) {
          if (Array.isArray(child.material)) {
            child.material.forEach(m => {
              if (m.map) m.map.dispose();
              m.dispose();
            });
          } else {
            if (child.material.map) child.material.map.dispose();
            child.material.dispose();
          }
        }
      }
    });
  }

  update(delta, runSpeed, playerBounds) {
    const moveDist = runSpeed * delta;

    for (let i = this.chunks.length - 1; i >= 0; i--) {
      const chunk = this.chunks[i];
      chunk.z -= moveDist;
      chunk.mesh.position.z = chunk.z;

      if (chunk.z < -35) {
        this.scene.remove(chunk.mesh);
        this.disposeMesh(chunk.mesh);
        this.chunks.splice(i, 1);
      }
    }

    const lastChunk = this.chunks[this.chunks.length - 1];
    if (lastChunk && lastChunk.z < this.spawnZ) {
      const newZ = lastChunk.z + this.chunkLength;
      this.spawnChunk(newZ, this.journeyManager.getCurrentDistrict(), true);
    }

    for (let i = this.trains.length - 1; i >= 0; i--) {
      const t = this.trains[i];
      t.z -= (moveDist + t.speed * delta);
      t.mesh.position.z = t.z;

      if (t.z < -35) {
        this.scene.remove(t.mesh);
        this.disposeMesh(t.mesh);
        this.trains.splice(i, 1);
      }
    }

    for (let i = this.obstacles.length - 1; i >= 0; i--) {
      const obs = this.obstacles[i];
      obs.z -= moveDist;
      obs.mesh.position.z = obs.z;

      if (obs.z < -35) {
        this.scene.remove(obs.mesh);
        this.disposeMesh(obs.mesh);
        this.obstacles.splice(i, 1);
      }
    }

    for (let i = this.coins.length - 1; i >= 0; i--) {
      const c = this.coins[i];
      c.z -= moveDist;
      c.mesh.position.z = c.z;
      c.mesh.rotation.y += 3.0 * delta;

      if (this.magnetActive && !c.collected) {
        const distToPlayer = Math.hypot(c.x - playerBounds.minX, c.z - playerBounds.z);
        if (distToPlayer < 8.0) {
          c.x += (playerBounds.minX - c.x) * 10 * delta;
          c.y += (playerBounds.minY + 0.8 - c.y) * 10 * delta;
          c.mesh.position.set(c.x, c.y, c.z);
        }
      }

      if (c.z < -10 || c.collected) {
        this.scene.remove(c.mesh);
        this.coins.splice(i, 1);
      }
    }

    for (let i = this.powerUps.length - 1; i >= 0; i--) {
      const p = this.powerUps[i];
      p.z -= moveDist;
      p.mesh.position.z = p.z;
      p.mesh.rotation.y += 2.0 * delta;

      if (p.z < -10 || p.collected) {
        this.scene.remove(p.mesh);
        this.powerUps.splice(i, 1);
      }
    }
  }

  checkNearMiss(playerBounds) {
    for (const t of this.trains) {
      if (t.nearMissTriggered) continue;
      const dx = Math.abs(t.laneX - (playerBounds.minX + playerBounds.maxX) / 2);
      const dz = Math.abs(t.z - playerBounds.z);

      // Passed very close without colliding
      if (dx >= 1.1 && dx < 2.8 && dz < 2.0) {
        t.nearMissTriggered = true;
        return true;
      }
    }
    return false;
  }

  checkCoinCollisions(playerBounds) {
    let collectedCount = 0;
    for (const c of this.coins) {
      if (c.collected) continue;
      const dx = Math.abs(c.x - (playerBounds.minX + playerBounds.maxX) / 2);
      const dz = Math.abs(c.z - playerBounds.z);
      const dy = Math.abs(c.y - playerBounds.minY);

      if (dx < 0.8 && dz < 1.2 && dy < 1.6) {
        c.collected = true;
        collectedCount++;
      }
    }
    return collectedCount;
  }

  checkPowerUpCollisions(playerBounds) {
    for (const p of this.powerUps) {
      if (p.collected) continue;
      const dx = Math.abs(p.x - (playerBounds.minX + playerBounds.maxX) / 2);
      const dz = Math.abs(p.z - playerBounds.z);

      if (dx < 1.0 && dz < 1.5) {
        p.collected = true;
        return p.type;
      }
    }
    return null;
  }

  checkObstacleCollisions(playerBounds) {
    for (const obs of this.obstacles) {
      const dx = Math.abs(obs.laneX - (playerBounds.minX + playerBounds.maxX) / 2);
      const dz = Math.abs(obs.z - playerBounds.z);

      if (dx < 0.9 && dz < 0.8) {
        if (playerBounds.minY > obs.height - 0.2) {
          continue;
        }
        return true;
      }
    }

    for (const t of this.trains) {
      const dx = Math.abs(t.laneX - (playerBounds.minX + playerBounds.maxX) / 2);
      const dz = Math.abs(t.z - playerBounds.z);

      if (dx < 1.1 && dz < t.length / 2 - 0.5) {
        if (playerBounds.minY >= t.height - 0.2) {
          continue;
        }
        return true;
      }
    }

    return false;
  }
}
