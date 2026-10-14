// AssetFactory.js - Procedural 3D Model & Canvas Texture Generator for Bihar Surfers
import * as THREE from 'three';

export class AssetFactory {
  constructor() {
    this.textureCache = new Map();
    this.materialsCache = new Map();
    this.textureLoader = new THREE.TextureLoader();
  }

  // --- CANVAS / IMAGE TEXTURE CREATORS ---

  createStationSignTexture(nameHi = 'पटना जंक्शन', nameEn = 'PATNA JN.', code = 'PNBE') {
    const key = `sign_${nameEn}_${nameHi}`;
    if (this.textureCache.has(key)) return this.textureCache.get(key);

    const canvas = document.createElement('canvas');
    canvas.width = 512;
    canvas.height = 128;
    const ctx = canvas.getContext('2d');

    // Indian Railways Classic Station Signboard: Bright Yellow background with black border
    ctx.fillStyle = '#ffcc00';
    ctx.fillRect(0, 0, 512, 128);

    ctx.strokeStyle = '#000000';
    ctx.lineWidth = 10;
    ctx.strokeRect(5, 5, 502, 118);

    ctx.lineWidth = 2;
    ctx.strokeRect(12, 12, 488, 104);

    // Text - Hindi (Top)
    ctx.fillStyle = '#000000';
    ctx.font = 'bold 36px sans-serif';
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.fillText(nameHi, 256, 42);

    // Text - English (Bottom)
    ctx.font = '900 30px "Outfit", sans-serif';
    ctx.fillText(nameEn, 256, 85);

    // Station Code Badge (Left)
    ctx.fillStyle = '#cc0000';
    ctx.fillRect(20, 20, 75, 30);
    ctx.fillStyle = '#ffffff';
    ctx.font = 'bold 16px sans-serif';
    ctx.fillText(code, 57, 37);

    const texture = new THREE.CanvasTexture(canvas);
    texture.needsUpdate = true;
    this.textureCache.set(key, texture);
    return texture;
  }

  createStationBackdropTexture(districtObj) {
    const key = `backdrop_${districtObj ? districtObj.id : 'default'}`;
    if (this.textureCache.has(key)) return this.textureCache.get(key);

    // If an image asset URL is specified for this station, load it directly via TextureLoader!
    if (districtObj && districtObj.image) {
      const texture = this.textureLoader.load(districtObj.image);
      texture.colorSpace = THREE.SRGBColorSpace;
      this.textureCache.set(key, texture);
      return texture;
    }

    // Canvas fallback for procedural backdrop
    const canvas = document.createElement('canvas');
    canvas.width = 1024;
    canvas.height = 512;
    const ctx = canvas.getContext('2d');

    const nameHi = districtObj ? (districtObj.hindiName || districtObj.hindi) : 'पटना जंक्शन';
    const nameEn = districtObj ? (districtObj.englishName || districtObj.station) : 'PATNA JN.';
    const theme = districtObj ? districtObj.theme : 'urban';

    const skyGrad = ctx.createLinearGradient(0, 0, 0, 300);
    if (theme === 'mithila') {
      skyGrad.addColorStop(0, '#ff9980');
      skyGrad.addColorStop(1, '#ffd1b3');
    } else if (theme === 'historic') {
      skyGrad.addColorStop(0, '#ff9900');
      skyGrad.addColorStop(1, '#ffcc66');
    } else if (theme === 'river') {
      skyGrad.addColorStop(0, '#4da6ff');
      skyGrad.addColorStop(1, '#b3daff');
    } else {
      skyGrad.addColorStop(0, '#ffaa00');
      skyGrad.addColorStop(1, '#ffe066');
    }
    ctx.fillStyle = skyGrad;
    ctx.fillRect(0, 0, 1024, 300);

    ctx.fillStyle = 'rgba(60, 40, 20, 0.25)';
    ctx.beginPath();
    ctx.arc(200, 300, 120, Math.PI, 0);
    ctx.arc(820, 300, 140, Math.PI, 0);
    ctx.fillRect(100, 300, 824, 150);
    ctx.fill();

    ctx.fillStyle = '#807366';
    ctx.fillRect(0, 400, 1024, 112);

    ctx.fillStyle = '#ffcc00';
    ctx.fillRect(312, 100, 400, 90);
    ctx.strokeStyle = '#000000';
    ctx.lineWidth = 6;
    ctx.strokeRect(316, 104, 392, 82);

    ctx.fillStyle = '#000000';
    ctx.font = 'bold 36px sans-serif';
    ctx.textAlign = 'center';
    ctx.fillText(nameHi, 512, 138);
    ctx.font = 'bold 28px "Outfit", sans-serif';
    ctx.fillText(nameEn.toUpperCase(), 512, 172);

    const texture = new THREE.CanvasTexture(canvas);
    texture.needsUpdate = true;
    this.textureCache.set(key, texture);
    return texture;
  }

  createTrainSideTexture(trainName = 'BIHAR EXPRESS') {
    const key = `train_${trainName}`;
    if (this.textureCache.has(key)) return this.textureCache.get(key);

    const canvas = document.createElement('canvas');
    canvas.width = 1024;
    canvas.height = 256;
    const ctx = canvas.getContext('2d');

    ctx.fillStyle = '#0055bb';
    ctx.fillRect(0, 0, 1024, 180);

    ctx.fillStyle = '#ffaa00';
    ctx.fillRect(0, 180, 1024, 76);

    ctx.fillStyle = '#ffcc00';
    ctx.fillRect(0, 120, 1024, 20);

    ctx.fillStyle = '#112244';
    ctx.strokeStyle = '#ffaa00';
    ctx.lineWidth = 4;
    for (let x = 60; x < 960; x += 120) {
      ctx.fillRect(x, 40, 70, 60);
      ctx.strokeRect(x, 40, 70, 60);
      ctx.fillStyle = 'rgba(255, 255, 255, 0.25)';
      ctx.beginPath();
      ctx.moveTo(x + 10, 40);
      ctx.lineTo(x + 35, 40);
      ctx.lineTo(x + 15, 100);
      ctx.lineTo(x, 100);
      ctx.fill();
      ctx.fillStyle = '#112244';
    }

    ctx.fillStyle = '#ffffff';
    ctx.font = 'bold 36px "Outfit", sans-serif';
    ctx.textAlign = 'center';
    ctx.fillText(trainName.toUpperCase(), 512, 160);

    ctx.font = 'bold 20px sans-serif';
    ctx.fillText('S4 - 23109', 100, 160);
    ctx.fillText(' Patna - Delhi ', 900, 160);

    const texture = new THREE.CanvasTexture(canvas);
    texture.needsUpdate = true;
    this.textureCache.set(key, texture);
    return texture;
  }

  createRupeeCoinTexture() {
    const key = 'rupee_coin';
    if (this.textureCache.has(key)) return this.textureCache.get(key);

    const canvas = document.createElement('canvas');
    canvas.width = 256;
    canvas.height = 256;
    const ctx = canvas.getContext('2d');

    const grad = ctx.createRadialGradient(128, 128, 10, 128, 128, 120);
    grad.addColorStop(0, '#fff5a3');
    grad.addColorStop(0.6, '#ffd700');
    grad.addColorStop(1, '#b8860b');

    ctx.fillStyle = grad;
    ctx.beginPath();
    ctx.arc(128, 128, 120, 0, Math.PI * 2);
    ctx.fill();

    ctx.strokeStyle = '#8b6508';
    ctx.lineWidth = 8;
    ctx.stroke();

    ctx.lineWidth = 4;
    ctx.beginPath();
    ctx.arc(128, 128, 100, 0, Math.PI * 2);
    ctx.stroke();

    ctx.fillStyle = '#4a3200';
    ctx.font = 'bold 120px sans-serif';
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.fillText('₹', 128, 132);

    const texture = new THREE.CanvasTexture(canvas);
    texture.needsUpdate = true;
    this.textureCache.set(key, texture);
    return texture;
  }

  createChaiStallTexture() {
    const key = 'chai_stall';
    if (this.textureCache.has(key)) return this.textureCache.get(key);

    const canvas = document.createElement('canvas');
    canvas.width = 512;
    canvas.height = 256;
    const ctx = canvas.getContext('2d');

    ctx.fillStyle = '#b35900';
    ctx.fillRect(0, 0, 512, 256);

    ctx.fillStyle = '#ffcc00';
    ctx.fillRect(20, 20, 472, 60);
    ctx.strokeStyle = '#cc0000';
    ctx.lineWidth = 6;
    ctx.strokeRect(25, 25, 462, 50);

    ctx.fillStyle = '#cc0000';
    ctx.font = 'bold 28px "Outfit", sans-serif';
    ctx.textAlign = 'center';
    ctx.fillText('RAJU CHAI & SNACKS', 256, 58);

    ctx.fillStyle = '#e67300';
    ctx.fillRect(100, 120, 60, 80);
    ctx.fillRect(352, 120, 60, 80);

    ctx.fillStyle = '#ffffff';
    ctx.font = 'bold 20px sans-serif';
    ctx.fillText('HOT CHAI ₹10', 256, 160);
    ctx.fillText('LITTI CHOKHA ₹30', 256, 200);

    const texture = new THREE.CanvasTexture(canvas);
    texture.needsUpdate = true;
    this.textureCache.set(key, texture);
    return texture;
  }

  // --- 3D MESH MODEL BUILDERS ---

  createTrainModel(type = 'express') {
    const trainGroup = new THREE.Group();

    const bodyGeo = new THREE.BoxGeometry(2.4, 3.2, 14);
    let trainTitle = 'BIHAR EXPRESS';
    let baseColor = 0x0044aa;

    if (type === 'local') {
      trainTitle = 'PATNA LOCAL';
      baseColor = 0xd97706;
    } else if (type === 'garibRath') {
      trainTitle = 'GARIB RATH';
      baseColor = 0x047857;
    }

    const sideTex = this.createTrainSideTexture(trainTitle);

    const materials = [
      new THREE.MeshStandardMaterial({ color: baseColor, roughness: 0.25, metalness: 0.6 }),
      new THREE.MeshStandardMaterial({ color: baseColor, roughness: 0.25, metalness: 0.6 }),
      new THREE.MeshStandardMaterial({ color: 0x222222, roughness: 0.8 }),
      new THREE.MeshStandardMaterial({ color: 0x111111, roughness: 0.9 }),
      new THREE.MeshStandardMaterial({ map: sideTex, roughness: 0.25, metalness: 0.5 }),
      new THREE.MeshStandardMaterial({ map: sideTex, roughness: 0.25, metalness: 0.5 })
    ];

    const bodyMesh = new THREE.Mesh(bodyGeo, materials);
    bodyMesh.position.y = 2.0;
    bodyMesh.castShadow = true;
    bodyMesh.receiveShadow = true;
    trainGroup.add(bodyMesh);

    // Roof AC Vents & Ribs
    const roofVentGeo = new THREE.CylinderGeometry(0.32, 0.32, 0.45, 10);
    const ventMat = new THREE.MeshStandardMaterial({ color: 0x777777, metalness: 0.85, roughness: 0.2 });
    for (let z = -5; z <= 5; z += 2.5) {
      const vent = new THREE.Mesh(roofVentGeo, ventMat);
      vent.position.set(0, 3.8, z);
      trainGroup.add(vent);
    }

    // 3D Front WAP-7 Locomotive Headlights
    const headlightGeo = new THREE.CylinderGeometry(0.22, 0.22, 0.2, 12);
    headlightGeo.rotateX(Math.PI / 2);
    const lightMat = new THREE.MeshBasicMaterial({ color: 0xffffff });

    const lightL = new THREE.Mesh(headlightGeo, lightMat);
    lightL.position.set(-0.8, 1.2, 7.05);
    const lightR = new THREE.Mesh(headlightGeo, lightMat);
    lightR.position.set(0.8, 1.2, 7.05);
    trainGroup.add(lightL, lightR);

    // 3D Iron Bogie Wheels & Axles
    const wheelGeo = new THREE.CylinderGeometry(0.42, 0.42, 0.22, 16);
    wheelGeo.rotateZ(Math.PI / 2);
    const wheelMat = new THREE.MeshStandardMaterial({ color: 0x111111, metalness: 0.95, roughness: 0.2 });

    [-5, 5].forEach(zOffset => {
      [-1.15, 1.15].forEach(xOffset => {
        [-0.6, 0.6].forEach(zSub => {
          const wheel = new THREE.Mesh(wheelGeo, wheelMat);
          wheel.position.set(xOffset, 0.4, zOffset + zSub);
          trainGroup.add(wheel);
        });
      });
    });

    return trainGroup;
  }

  createRailwayTrackChunk(districtObj = null) {
    const chunkGroup = new THREE.Group();
    const chunkLength = 20;

    // Ballast Gravel Bed
    const ballastGeo = new THREE.BoxGeometry(9.6, 0.2, chunkLength);
    const ballastMat = new THREE.MeshStandardMaterial({ color: 0x3d352e, roughness: 0.95 });
    const ballast = new THREE.Mesh(ballastGeo, ballastMat);
    ballast.position.set(0, -0.1, 0);
    ballast.receiveShadow = true;
    chunkGroup.add(ballast);

    const lanes = [-2.5, 0, 2.5];
    const railMat = new THREE.MeshStandardMaterial({ color: 0xe2e8f0, metalness: 0.95, roughness: 0.15 });
    const sleeperMat = new THREE.MeshStandardMaterial({ color: 0x475569, roughness: 0.7 });

    lanes.forEach(laneX => {
      // Metallic Steel Rails
      [-0.7, 0.7].forEach(rX => {
        const railGeo = new THREE.BoxGeometry(0.12, 0.16, chunkLength);
        const rail = new THREE.Mesh(railGeo, railMat);
        rail.position.set(laneX + rX, 0.08, 0);
        chunkGroup.add(rail);
      });

      // Concrete Sleepers / Ties
      for (let z = -chunkLength / 2; z <= chunkLength / 2; z += 1.1) {
        const sleeperGeo = new THREE.BoxGeometry(1.85, 0.1, 0.35);
        const sleeper = new THREE.Mesh(sleeperGeo, sleeperMat);
        sleeper.position.set(laneX, 0.02, z);
        sleeper.receiveShadow = true;
        chunkGroup.add(sleeper);
      }
    });

    const poleMat = new THREE.MeshStandardMaterial({ color: 0x3a5a40, metalness: 0.7 });
    const poleGeo = new THREE.CylinderGeometry(0.12, 0.12, 6, 8);

    const leftPole = new THREE.Mesh(poleGeo, poleMat);
    leftPole.position.set(-5.2, 3, 0);
    const rightPole = new THREE.Mesh(poleGeo, poleMat);
    rightPole.position.set(5.2, 3, 0);
    chunkGroup.add(leftPole, rightPole);

    const crossbarGeo = new THREE.BoxGeometry(10.6, 0.1, 0.1);
    const crossbar = new THREE.Mesh(crossbarGeo, poleMat);
    crossbar.position.set(0, 5.8, 0);
    chunkGroup.add(crossbar);

    const signalHousingGeo = new THREE.BoxGeometry(0.4, 1.2, 0.3);
    const signalMat = new THREE.MeshStandardMaterial({ color: 0x111111 });
    const lightGreen = new THREE.MeshBasicMaterial({ color: 0x00ff44 });
    const lightRed = new THREE.MeshBasicMaterial({ color: 0xff0022 });

    lanes.forEach((laneX, i) => {
      const housing = new THREE.Mesh(signalHousingGeo, signalMat);
      housing.position.set(laneX, 5.2, 0);

      const lampGeo = new THREE.CylinderGeometry(0.1, 0.1, 0.1, 8);
      lampGeo.rotateX(Math.PI / 2);
      const lamp = new THREE.Mesh(lampGeo, i === 1 ? lightGreen : lightRed);
      lamp.position.set(laneX, 5.2, 0.15);

      chunkGroup.add(housing, lamp);
    });

    // 3D Overhead Catenary Wires
    const wireMat = new THREE.MeshBasicMaterial({ color: 0x111111 });
    [-1.2, 1.2].forEach(wX => {
      const wireGeo = new THREE.CylinderGeometry(0.015, 0.015, chunkLength, 4);
      wireGeo.rotateX(Math.PI / 2);
      const wire = new THREE.Mesh(wireGeo, wireMat);
      wire.position.set(wX, 5.75, 0);
      chunkGroup.add(wire);
    });

    // 3D Foot Overbridge (FOB) every few track sections
    if (Math.abs(Math.floor(chunkGroup.position.z || 0)) % 60 === 0) {
      const fobGroup = new THREE.Group();
      const spanGeo = new THREE.BoxGeometry(14, 1.2, 2.5);
      const spanMat = new THREE.MeshStandardMaterial({ color: 0x004488, roughness: 0.5 });
      const span = new THREE.Mesh(spanGeo, spanMat);
      span.position.set(0, 7.5, 0);
      fobGroup.add(span);

      const pillarGeo = new THREE.CylinderGeometry(0.3, 0.3, 7.5, 8);
      const pillarMat = new THREE.MeshStandardMaterial({ color: 0x334155, metalness: 0.8 });
      const p1 = new THREE.Mesh(pillarGeo, pillarMat);
      p1.position.set(-6.5, 3.75, 0);
      const p2 = new THREE.Mesh(pillarGeo, pillarMat);
      p2.position.set(6.5, 3.75, 0);
      fobGroup.add(p1, p2);

      chunkGroup.add(fobGroup);
    }

    // 3D Distant Patna City Skyline Buildings along track sides
    [-14, 14].forEach(bX => {
      const bHeight = 12 + Math.random() * 10;
      const bGeo = new THREE.BoxGeometry(6, bHeight, 8);
      const bMat = new THREE.MeshStandardMaterial({ color: bX < 0 ? 0x94a3b8 : 0xcbd5e1, roughness: 0.8 });
      const bld = new THREE.Mesh(bGeo, bMat);
      bld.position.set(bX, bHeight / 2, 0);
      chunkGroup.add(bld);
    });

    return chunkGroup;
  }

  createStationPlatform(districtObj) {
    const platformGroup = new THREE.Group();
    const nameHi = districtObj ? (districtObj.hindiName || districtObj.hindi || districtObj.stationHi) : 'पटना जंक्शन';
    const nameEn = districtObj ? (districtObj.englishName || districtObj.station || districtObj.stationEn) : 'PATNA JN.';
    const code = districtObj ? districtObj.code : 'PNBE';

    // 1. Concrete Platform Surface & Tactile Edge Strip
    const platformGeo = new THREE.BoxGeometry(4.0, 1.0, 20);
    const platformMat = new THREE.MeshStandardMaterial({ color: 0x7c7c7c, roughness: 0.8 });
    const platform = new THREE.Mesh(platformGeo, platformMat);
    platform.position.set(6.2, 0.5, 0);
    platform.receiveShadow = true;
    platformGroup.add(platform);

    const edgeGeo = new THREE.BoxGeometry(0.4, 1.02, 20);
    const edgeMat = new THREE.MeshStandardMaterial({ color: 0xffcc00, roughness: 0.5 });
    const edge = new THREE.Mesh(edgeGeo, edgeMat);
    edge.position.set(4.2, 0.51, 0);
    platformGroup.add(edge);

    // 2. Pillars & Covered Passenger Shelter Roof
    const pillarGeo = new THREE.CylinderGeometry(0.15, 0.15, 4, 8);
    const pillarMat = new THREE.MeshStandardMaterial({ color: 0x004488, metalness: 0.8 });
    [-8, 0, 8].forEach(z => {
      const p1 = new THREE.Mesh(pillarGeo, pillarMat);
      p1.position.set(5.5, 2.5, z);
      const p2 = new THREE.Mesh(pillarGeo, pillarMat);
      p2.position.set(7.5, 2.5, z);
      platformGroup.add(p1, p2);
    });

    const roofGeo = new THREE.BoxGeometry(4.5, 0.2, 20);
    const roofMat = new THREE.MeshStandardMaterial({ color: 0x003366, roughness: 0.4 });
    const roof = new THREE.Mesh(roofGeo, roofMat);
    roof.position.set(6.5, 4.5, 0);
    platformGroup.add(roof);

    // 3. Indian Railways Yellow Station Signboard
    const signTex = this.createStationSignTexture(nameHi, nameEn, code);
    const signGeo = new THREE.BoxGeometry(5.0, 1.25, 0.2);
    const signMat = new THREE.MeshStandardMaterial({ map: signTex, roughness: 0.3 });
    const sign = new THREE.Mesh(signGeo, signMat);
    sign.position.set(6.5, 3.5, -4);
    platformGroup.add(sign);

    // 4. Grand Indo-Saracenic 3D Station Palace Building Facade
    const bldgGeo = new THREE.BoxGeometry(4.0, 8.0, 20);
    const bldgMat = new THREE.MeshStandardMaterial({ color: 0xe0a96d, roughness: 0.7 }); // Sandstone color
    const bldg = new THREE.Mesh(bldgGeo, bldgMat);
    bldg.position.set(10.2, 4.0, 0);
    bldg.castShadow = true;
    bldg.receiveShadow = true;
    platformGroup.add(bldg);

    // Arched Windows on Building
    const archGeo = new THREE.CylinderGeometry(0.6, 0.6, 0.4, 16, 1, false, 0, Math.PI);
    archGeo.rotateX(Math.PI / 2);
    const archMat = new THREE.MeshStandardMaterial({ color: 0x1a2634, metalness: 0.9, roughness: 0.1 });
    [-6, 0, 6].forEach(z => {
      const arch = new THREE.Mesh(archGeo, archMat);
      arch.position.set(8.15, 5.0, z);
      platformGroup.add(arch);
    });

    // Royal Domes on Top Towers
    [-8, 8].forEach(z => {
      const towerGeo = new THREE.BoxGeometry(2.5, 3.0, 2.5);
      const tower = new THREE.Mesh(towerGeo, bldgMat);
      tower.position.set(10.2, 9.5, z);

      const domeGeo = new THREE.SphereGeometry(1.4, 16, 16, 0, Math.PI * 2, 0, Math.PI / 1.8);
      const domeMat = new THREE.MeshStandardMaterial({ color: 0xd97706, roughness: 0.4 });
      const dome = new THREE.Mesh(domeGeo, domeMat);
      dome.position.set(10.2, 11.0, z);

      const spireGeo = new THREE.ConeGeometry(0.15, 1.2, 8);
      const spireMat = new THREE.MeshStandardMaterial({ color: 0xf59e0b, metalness: 0.9 });
      const spire = new THREE.Mesh(spireGeo, spireMat);
      spire.position.set(10.2, 12.6, z);

      platformGroup.add(tower, dome, spire);
    });

    // 5. 3D Raju Chai & Snacks Vendor Stall
    const chaiGroup = this.createChaiStall();
    chaiGroup.position.set(6.5, 1.0, 2);
    platformGroup.add(chaiGroup);

    // 6. 3D Auto-Rickshaw
    const autoGroup = this.createAutoRickshaw();
    autoGroup.position.set(8.5, 1.0, 6);
    autoGroup.rotation.y = -Math.PI / 4;
    platformGroup.add(autoGroup);

    // 7. Platform Benches & Luggage Suitcase Stacks
    const benchGeo = new THREE.BoxGeometry(1.6, 0.4, 0.6);
    const benchMat = new THREE.MeshStandardMaterial({ color: 0x854d0e, roughness: 0.6 });
    const bench = new THREE.Mesh(benchGeo, benchMat);
    bench.position.set(6.5, 1.2, -7);
    platformGroup.add(bench);

    const luggageGeo = new THREE.BoxGeometry(0.7, 0.5, 0.4);
    const luggageMat = new THREE.MeshStandardMaterial({ color: 0x991b1b, roughness: 0.5 });
    const luggage = new THREE.Mesh(luggageGeo, luggageMat);
    luggage.position.set(6.5, 1.25, -2);
    platformGroup.add(luggage);

    // 8. 3D Platform Passengers / Station Vendor NPCs
    [-5, 3].forEach((z, idx) => {
      const npcGroup = this.createPassengerNPC(idx % 2 === 0 ? 0xd97706 : 0x0284c7);
      npcGroup.position.set(6.2, 1.0, z);
      npcGroup.rotation.y = Math.PI / 2 + (idx % 2 === 0 ? 0.3 : -0.3);
      platformGroup.add(npcGroup);
    });

    // 9. 3D Background Trees along perimeter
    const treeTrunkGeo = new THREE.CylinderGeometry(0.2, 0.25, 2.5, 8);
    const trunkMat = new THREE.MeshStandardMaterial({ color: 0x451a03, roughness: 0.9 });
    const leavesGeo = new THREE.ConeGeometry(1.2, 3.5, 8);
    const leavesMat = new THREE.MeshStandardMaterial({ color: 0x15803d, roughness: 0.6 });

    [-9, 9].forEach(z => {
      const trunk = new THREE.Mesh(treeTrunkGeo, trunkMat);
      trunk.position.set(13.5, 1.25, z);
      const leaves = new THREE.Mesh(leavesGeo, leavesMat);
      leaves.position.set(13.5, 3.8, z);
      platformGroup.add(trunk, leaves);
    });

    return platformGroup;
  }

  createPassengerNPC(shirtHex = 0xd97706) {
    const group = new THREE.Group();

    const bodyGeo = new THREE.CylinderGeometry(0.2, 0.18, 0.6, 10);
    const bodyMat = new THREE.MeshStandardMaterial({ color: shirtHex, roughness: 0.6 });
    const body = new THREE.Mesh(bodyGeo, bodyMat);
    body.position.y = 0.6;
    group.add(body);

    const headGeo = new THREE.SphereGeometry(0.16, 12, 12);
    const headMat = new THREE.MeshStandardMaterial({ color: 0xe0ac69, roughness: 0.7 });
    const head = new THREE.Mesh(headGeo, headMat);
    head.position.y = 1.05;
    group.add(head);

    const legGeo = new THREE.CylinderGeometry(0.07, 0.06, 0.5, 8);
    const legMat = new THREE.MeshStandardMaterial({ color: 0x1e293b, roughness: 0.8 });

    const lLeg = new THREE.Mesh(legGeo, legMat);
    lLeg.position.set(-0.1, 0.25, 0);
    const rLeg = new THREE.Mesh(legGeo, legMat);
    rLeg.position.set(0.1, 0.25, 0);
    group.add(lLeg, rLeg);

    return group;
  }

  createChaiStall() {
    const group = new THREE.Group();
    const counterGeo = new THREE.BoxGeometry(2.2, 1.0, 1.2);
    const chaiTex = this.createChaiStallTexture();
    const counterMat = new THREE.MeshStandardMaterial({ map: chaiTex, roughness: 0.6 });
    const counter = new THREE.Mesh(counterGeo, counterMat);
    counter.position.y = 0.5;
    group.add(counter);

    const kettleGeo = new THREE.CylinderGeometry(0.2, 0.25, 0.3, 10);
    const kettleMat = new THREE.MeshStandardMaterial({ color: 0xcccccc, metalness: 0.9 });
    const kettle = new THREE.Mesh(kettleGeo, kettleMat);
    kettle.position.set(-0.6, 1.15, 0);
    group.add(kettle);

    return group;
  }

  createAutoRickshaw() {
    const group = new THREE.Group();

    const roofGeo = new THREE.BoxGeometry(1.6, 0.8, 2.2);
    const roofMat = new THREE.MeshStandardMaterial({ color: 0xffcc00, roughness: 0.4 });
    const roof = new THREE.Mesh(roofGeo, roofMat);
    roof.position.set(0, 1.5, 0);
    group.add(roof);

    const cabinGeo = new THREE.BoxGeometry(1.5, 0.8, 2.0);
    const cabinMat = new THREE.MeshStandardMaterial({ color: 0x008844, roughness: 0.5 });
    const cabin = new THREE.Mesh(cabinGeo, cabinMat);
    cabin.position.set(0, 0.7, 0);
    group.add(cabin);

    const wheelGeo = new THREE.CylinderGeometry(0.3, 0.3, 0.15, 12);
    wheelGeo.rotateZ(Math.PI / 2);
    const wheelMat = new THREE.MeshStandardMaterial({ color: 0x111111 });

    const wFront = new THREE.Mesh(wheelGeo, wheelMat);
    wFront.position.set(0, 0.3, 0.9);
    const wBackL = new THREE.Mesh(wheelGeo, wheelMat);
    wBackL.position.set(-0.7, 0.3, -0.7);
    const wBackR = new THREE.Mesh(wheelGeo, wheelMat);
    wBackR.position.set(0.7, 0.3, -0.7);
    group.add(wFront, wBackL, wBackR);

    return group;
  }

  createRupeeCoinModel() {
    const group = new THREE.Group();
    const coinGeo = new THREE.CylinderGeometry(0.5, 0.5, 0.1, 24);
    coinGeo.rotateX(Math.PI / 2);

    const coinTex = this.createRupeeCoinTexture();
    const coinMat = new THREE.MeshStandardMaterial({
      map: coinTex,
      metalness: 0.95,
      roughness: 0.15,
      color: 0xffd700
    });

    const coinMesh = new THREE.Mesh(coinGeo, coinMat);
    coinMesh.castShadow = true;
    group.add(coinMesh);
    return group;
  }

  createPowerUpModel(type = 'sattuRocket') {
    const group = new THREE.Group();

    if (type === 'sattuRocket') {
      // 3D Terracotta Clay Sattu Pot Main Body
      const potGeo = new THREE.SphereGeometry(0.5, 20, 20);
      const potMat = new THREE.MeshStandardMaterial({ color: 0xd97706, roughness: 0.6, metalness: 0.1 });
      const pot = new THREE.Mesh(potGeo, potMat);
      pot.position.y = 0.55;
      group.add(pot);

      // Pot Rim Neck
      const rimGeo = new THREE.TorusGeometry(0.28, 0.05, 10, 20);
      rimGeo.rotateX(Math.PI / 2);
      const rimMat = new THREE.MeshStandardMaterial({ color: 0x92400e, roughness: 0.5 });
      const rim = new THREE.Mesh(rimGeo, rimMat);
      rim.position.y = 1.0;
      group.add(rim);

      // Rocket Thruster Nose Cone & Fins
      const noseGeo = new THREE.ConeGeometry(0.35, 0.6, 12);
      const noseMat = new THREE.MeshStandardMaterial({ color: 0xef4444, metalness: 0.7, roughness: 0.2 });
      const nose = new THREE.Mesh(noseGeo, noseMat);
      nose.position.y = 1.3;
      group.add(nose);

      const finGeo = new THREE.ConeGeometry(0.25, 0.6, 4);
      const finMat = new THREE.MeshStandardMaterial({ color: 0x3b82f6, roughness: 0.3 });
      for (let i = 0; i < 3; i++) {
        const angle = (i * Math.PI * 2) / 3;
        const fin = new THREE.Mesh(finGeo, finMat);
        fin.position.set(Math.cos(angle) * 0.48, 0.3, Math.sin(angle) * 0.48);
        fin.rotation.z = -Math.cos(angle) * 0.5;
        group.add(fin);
      }

      // Flame Glow Thruster
      const flameGeo = new THREE.ConeGeometry(0.25, 0.5, 10);
      flameGeo.rotateX(Math.PI);
      const flameMat = new THREE.MeshBasicMaterial({ color: 0xffaa00, transparent: true, opacity: 0.85 });
      const flame = new THREE.Mesh(flameGeo, flameMat);
      flame.position.y = 0.05;
      group.add(flame);

      // Glowing Aura Ring
      const auraGeo = new THREE.RingGeometry(0.65, 0.85, 24);
      auraGeo.rotateX(-Math.PI / 2);
      const auraMat = new THREE.MeshBasicMaterial({ color: 0xff9900, side: THREE.DoubleSide, transparent: true, opacity: 0.7 });
      const aura = new THREE.Mesh(auraGeo, auraMat);
      aura.position.y = 0.55;
      group.add(aura);
    } else if (type === 'chaiMagnet') {
      const magnetGeo = new THREE.TorusGeometry(0.4, 0.15, 12, 24, Math.PI);
      const magnetMat = new THREE.MeshStandardMaterial({ color: 0xff0044, metalness: 0.8 });
      const magnet = new THREE.Mesh(magnetGeo, magnetMat);
      magnet.position.y = 0.5;
      group.add(magnet);
    } else {
      const orbGeo = new THREE.IcosahedronGeometry(0.5, 2);
      const orbMat = new THREE.MeshStandardMaterial({ color: 0x00e5ff, roughness: 0.2, metalness: 0.8 });
      const orb = new THREE.Mesh(orbGeo, orbMat);
      orb.position.y = 0.5;
      group.add(orb);
    }

    return group;
  }
}
