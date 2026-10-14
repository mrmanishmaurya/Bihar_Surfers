// CharacterManager.js - High-Quality 3D Cartoon Player Character & Skeletal Animation Engine for Bihar Surfers
import * as THREE from 'three';

export class CharacterManager {
  constructor(scene, assetFactory) {
    this.scene = scene;
    this.assetFactory = assetFactory;

    this.characterGroup = new THREE.Group();
    this.scene.add(this.characterGroup);

    // Ground Drop Shadow Mesh
    this.shadowMesh = this.createDropShadowMesh();
    this.scene.add(this.shadowMesh);

    // Speed Trail Particles for Power-Ups
    this.speedTrailGroup = new THREE.Group();
    this.scene.add(this.speedTrailGroup);

    // Mesh hierarchy pointers for procedural skeletal animation
    this.skeleton = {
      root: null,
      torso: null,
      head: null,
      hair: null,
      backpack: null,
      leftUpperArm: null,
      leftForeArm: null,
      rightUpperArm: null,
      rightForeArm: null,
      leftThigh: null,
      leftCalf: null,
      leftShoe: null,
      rightThigh: null,
      rightCalf: null,
      rightShoe: null,
      auraGlow: null
    };

    // Movement & state variables
    this.currentLane = 1; // 0 = Left (-2.5), 1 = Center (0), 2 = Right (2.5)
    this.targetX = 0;
    this.currentX = 0;

    this.isJumping = false;
    this.jumpY = 0;
    this.jumpVelocity = 0;
    this.gravity = -36;

    this.isSliding = false;
    this.slideTimer = 0;

    this.isHit = false;
    this.hitTimer = 0;

    this.animTime = 0;
    this.currentCharacterId = 'raj';

    this.buildCharacter('raj');
  }

  createDropShadowMesh() {
    const canvas = document.createElement('canvas');
    canvas.width = 128;
    canvas.height = 128;
    const ctx = canvas.getContext('2d');

    const grad = ctx.createRadialGradient(64, 64, 0, 64, 64, 60);
    grad.addColorStop(0, 'rgba(0, 0, 0, 0.7)');
    grad.addColorStop(0.5, 'rgba(0, 0, 0, 0.3)');
    grad.addColorStop(1, 'rgba(0, 0, 0, 0)');

    ctx.fillStyle = grad;
    ctx.fillRect(0, 0, 128, 128);

    const shadowTex = new THREE.CanvasTexture(canvas);
    const shadowGeo = new THREE.PlaneGeometry(1.6, 1.6);
    shadowGeo.rotateX(-Math.PI / 2);

    const shadowMat = new THREE.MeshBasicMaterial({
      map: shadowTex,
      transparent: true,
      depthWrite: false
    });

    const shadowMesh = new THREE.Mesh(shadowGeo, shadowMat);
    shadowMesh.position.set(0, 0.03, 0);
    return shadowMesh;
  }

  createJerseyTexture(numberStr = '23', nameStr = 'RAJ', baseHex = 0xff6600, trimHex = 0x0055ff) {
    const canvas = document.createElement('canvas');
    canvas.width = 512;
    canvas.height = 512;
    const ctx = canvas.getContext('2d');

    // Vibrant Orange Base Jersey Color (#ff6600)
    const baseColor = '#' + baseHex.toString(16).padStart(6, '0');
    const trimColor = '#' + trimHex.toString(16).padStart(6, '0'); // Royal Blue (#0055ff)

    ctx.fillStyle = baseColor;
    ctx.fillRect(0, 0, 512, 512);

    // Dynamic Royal Blue Side Racing Stripes
    ctx.fillStyle = trimColor;
    ctx.fillRect(0, 0, 75, 512);
    ctx.fillRect(437, 0, 75, 512);

    // White Accent Lines
    ctx.fillStyle = '#ffffff';
    ctx.fillRect(75, 0, 12, 512);
    ctx.fillRect(425, 0, 12, 512);

    // V-Neck Collar Trim (White & Blue)
    ctx.fillStyle = '#ffffff';
    ctx.beginPath();
    ctx.moveTo(170, 0);
    ctx.lineTo(256, 110);
    ctx.lineTo(342, 0);
    ctx.fill();

    ctx.fillStyle = trimColor;
    ctx.beginPath();
    ctx.moveTo(190, 0);
    ctx.lineTo(256, 85);
    ctx.lineTo(322, 0);
    ctx.fill();

    // Front Main Title: BIHAR SURFERS
    ctx.fillStyle = '#ffffff';
    ctx.font = '900 46px "Outfit", sans-serif';
    ctx.textAlign = 'center';
    ctx.shadowColor = 'rgba(0, 0, 0, 0.6)';
    ctx.shadowBlur = 8;
    ctx.shadowOffsetY = 4;
    ctx.fillText('BIHAR SURFERS', 256, 175);
    ctx.shadowBlur = 0;

    // Big Bold #23 Jersey Number Badge
    ctx.fillStyle = trimColor;
    ctx.beginPath();
    ctx.roundRect(166, 205, 180, 155, 16);
    ctx.fill();
    ctx.strokeStyle = '#ffffff';
    ctx.lineWidth = 10;
    ctx.stroke();

    ctx.fillStyle = '#ffffff';
    ctx.font = '900 115px "Teko", sans-serif';
    ctx.fillText(numberStr, 256, 320);

    // Back Name & Number
    ctx.fillStyle = '#ffffff';
    ctx.font = '900 38px "Outfit", sans-serif';
    ctx.fillText(nameStr, 256, 425);
    ctx.font = '900 65px "Teko", sans-serif';
    ctx.fillText(`#${numberStr}`, 256, 485);

    const tex = new THREE.CanvasTexture(canvas);
    tex.colorSpace = THREE.SRGBColorSpace;
    return tex;
  }

  createEyeTexture(irisHex = 0x1d4ed8) {
    const canvas = document.createElement('canvas');
    canvas.width = 256;
    canvas.height = 256;
    const ctx = canvas.getContext('2d');

    // Sclera (Smooth White Oval)
    ctx.fillStyle = '#ffffff';
    ctx.beginPath();
    ctx.ellipse(128, 128, 110, 110, 0, 0, Math.PI * 2);
    ctx.fill();
    ctx.strokeStyle = '#222222';
    ctx.lineWidth = 6;
    ctx.stroke();

    // Iris Ring
    const irisColor = '#' + irisHex.toString(16).padStart(6, '0');
    ctx.fillStyle = irisColor;
    ctx.beginPath();
    ctx.arc(128, 128, 72, 0, Math.PI * 2);
    ctx.fill();

    // Deep Pupil
    ctx.fillStyle = '#050510';
    ctx.beginPath();
    ctx.arc(128, 128, 44, 0, Math.PI * 2);
    ctx.fill();

    // Dual Specular Catchlights (Shine dots for cartoon expression)
    ctx.fillStyle = '#ffffff';
    ctx.beginPath();
    ctx.arc(96, 96, 24, 0, Math.PI * 2);
    ctx.fill();
    ctx.beginPath();
    ctx.arc(150, 150, 12, 0, Math.PI * 2);
    ctx.fill();

    const tex = new THREE.CanvasTexture(canvas);
    tex.colorSpace = THREE.SRGBColorSpace;
    return tex;
  }

  buildCharacter(charId = 'raj') {
    this.currentCharacterId = charId;

    // Completely wipe old placeholder player mesh hierarchy
    while (this.characterGroup.children.length > 0) {
      const child = this.characterGroup.children[0];
      this.characterGroup.remove(child);
    }

    // Character Presets Configuration matching concept design
    const presets = {
      raj: {
        name: 'RAJ',
        number: '23',
        skinHex: 0xffdbac,
        jerseyHex: 0xff6600, // Vibrant Orange
        trimHex: 0x0055ff,   // Royal Blue
        shortsHex: 0x0044bb,
        hairHex: 0x1a110b,   // Natural Black
        hairStyle: 'spiky',
        shoesHex: 0xffffff,
        packHex: 0x1e293b,
        eyeHex: 0x1d4ed8
      },
      vikram: {
        name: 'VIKRAM',
        number: '07',
        skinHex: 0xf1c27d,
        jerseyHex: 0xd97706,
        trimHex: 0x15803d,
        shortsHex: 0x166534,
        hairHex: 0x111111,
        hairStyle: 'headband',
        shoesHex: 0xfacc15,
        packHex: 0x451a03,
        eyeHex: 0x374151
      },
      ananya: {
        name: 'ANANYA',
        number: '10',
        skinHex: 0xffe0bd,
        jerseyHex: 0x0d9488,
        trimHex: 0xf59e0b,
        shortsHex: 0x0f766e,
        hairHex: 0x451a03,
        hairStyle: 'ponytail',
        shoesHex: 0xf43f5e,
        packHex: 0x881337,
        eyeHex: 0x047857
      },
      arjun: {
        name: 'ARJUN',
        number: '99',
        skinHex: 0xe0ac69,
        jerseyHex: 0x2563eb,
        trimHex: 0xffffff,
        shortsHex: 0x1d4ed8,
        hairHex: 0x0f172a,
        hairStyle: 'cap',
        shoesHex: 0x38bdf8,
        packHex: 0x0369a1,
        eyeHex: 0x1e3a8a
      }
    };

    const config = presets[charId] || presets.raj;
    const rootGroup = new THREE.Group();

    // ----------------------------------------------------
    // 1. ATHLETIC TORSO & JERSEY
    // ----------------------------------------------------
    const torsoGeo = new THREE.CylinderGeometry(0.34, 0.28, 0.78, 20);
    const jerseyTex = this.createJerseyTexture(config.number, config.name, config.jerseyHex, config.trimHex);
    const torsoMat = new THREE.MeshStandardMaterial({
      map: jerseyTex,
      roughness: 0.45,
      metalness: 0.1
    });

    const torso = new THREE.Mesh(torsoGeo, torsoMat);
    torso.position.y = 1.18;
    torso.castShadow = true;
    torso.receiveShadow = true;
    rootGroup.add(torso);
    this.skeleton.torso = torso;

    // Collar V-Neck Ring
    const collarGeo = new THREE.TorusGeometry(0.19, 0.035, 10, 24);
    collarGeo.rotateX(Math.PI / 2);
    const collarMat = new THREE.MeshStandardMaterial({ color: config.trimHex, roughness: 0.3 });
    const collar = new THREE.Mesh(collarGeo, collarMat);
    collar.position.set(0, 0.39, 0);
    torso.add(collar);

    // ----------------------------------------------------
    // 2. DETAILED CARTOON HEAD & EXPRESSIVE FACE
    // ----------------------------------------------------
    const skinMat = new THREE.MeshStandardMaterial({ color: config.skinHex, roughness: 0.55 });
    const neckGeo = new THREE.CylinderGeometry(0.12, 0.14, 0.18, 14);
    const neck = new THREE.Mesh(neckGeo, skinMat);
    neck.position.set(0, 0.46, 0);
    torso.add(neck);

    const headGeo = new THREE.SphereGeometry(0.32, 32, 32);
    const head = new THREE.Mesh(headGeo, skinMat);
    head.position.set(0, 0.25, 0);
    head.castShadow = true;
    neck.add(head);
    this.skeleton.head = head;

    // 3D Sculpted Ears
    const earGeo = new THREE.SphereGeometry(0.07, 12, 12);
    earGeo.scale(0.5, 1.3, 0.9);
    const earL = new THREE.Mesh(earGeo, skinMat);
    earL.position.set(-0.33, 0, 0);
    const earR = new THREE.Mesh(earGeo, skinMat);
    earR.position.set(0.33, 0, 0);
    head.add(earL, earR);

    // 3D Cartoon Expressive Eyes
    const eyeTex = this.createEyeTexture(config.eyeHex);
    const eyeMat = new THREE.MeshBasicMaterial({ map: eyeTex });
    const eyeGeo = new THREE.PlaneGeometry(0.16, 0.16);

    const eyeL = new THREE.Mesh(eyeGeo, eyeMat);
    eyeL.position.set(-0.12, 0.04, 0.29);
    eyeL.rotation.y = -0.16;
    const eyeR = new THREE.Mesh(eyeGeo, eyeMat);
    eyeR.position.set(0.12, 0.04, 0.29);
    eyeR.rotation.y = 0.16;
    head.add(eyeL, eyeR);

    // 3D Stylized Eyebrows
    const browGeo = new THREE.BoxGeometry(0.13, 0.03, 0.02);
    const hairMat = new THREE.MeshStandardMaterial({ color: config.hairHex, roughness: 0.8 });
    const browL = new THREE.Mesh(browGeo, hairMat);
    browL.position.set(-0.12, 0.15, 0.3);
    browL.rotation.z = -0.12;

    const browR = new THREE.Mesh(browGeo, hairMat);
    browR.position.set(0.12, 0.15, 0.3);
    browR.rotation.z = 0.12;
    head.add(browL, browR);

    // 3D Nose & Confident Cheerful Smile
    const noseGeo = new THREE.ConeGeometry(0.045, 0.09, 8);
    noseGeo.rotateX(-Math.PI / 4);
    const nose = new THREE.Mesh(noseGeo, skinMat);
    nose.position.set(0, -0.02, 0.32);
    head.add(nose);

    const mouthGeo = new THREE.TorusGeometry(0.075, 0.02, 10, 16, Math.PI);
    mouthGeo.rotateX(Math.PI);
    const mouthMat = new THREE.MeshBasicMaterial({ color: 0x991122 });
    const mouth = new THREE.Mesh(mouthGeo, mouthMat);
    mouth.position.set(0, -0.13, 0.3);
    head.add(mouth);

    // White Teeth Line in Smile
    const teethGeo = new THREE.BoxGeometry(0.1, 0.02, 0.02);
    const teethMat = new THREE.MeshBasicMaterial({ color: 0xffffff });
    const teeth = new THREE.Mesh(teethGeo, teethMat);
    teeth.position.set(0, -0.12, 0.305);
    head.add(teeth);

    // ----------------------------------------------------
    // 3. STYLIZED 3D NATURAL BLACK SPIKY / SIDE-SWEPT HAIR
    // ----------------------------------------------------
    const hairGroup = new THREE.Group();
    if (config.hairStyle === 'spiky') {
      const mainCapGeo = new THREE.SphereGeometry(0.33, 24, 24, 0, Math.PI * 2, 0, Math.PI / 1.7);
      const mainCap = new THREE.Mesh(mainCapGeo, hairMat);
      hairGroup.add(mainCap);

      // Sculpted 3D Spikes Layer
      for (let i = 0; i < 11; i++) {
        const spikeGeo = new THREE.ConeGeometry(0.095, 0.28, 6);
        const spike = new THREE.Mesh(spikeGeo, hairMat);
        const angle = (i / 11) * Math.PI - Math.PI / 2;
        spike.position.set(Math.sin(angle) * 0.22, 0.3 + Math.cos(angle) * 0.06, Math.cos(angle) * 0.15);
        spike.rotation.z = -angle * 0.55;
        spike.rotation.x = -0.35;
        hairGroup.add(spike);
      }
    } else if (config.hairStyle === 'headband') {
      const mainCapGeo = new THREE.SphereGeometry(0.33, 24, 24, 0, Math.PI * 2, 0, Math.PI / 2);
      const mainCap = new THREE.Mesh(mainCapGeo, hairMat);
      hairGroup.add(mainCap);

      const bandGeo = new THREE.CylinderGeometry(0.335, 0.335, 0.075, 24, 1, true);
      const bandMat = new THREE.MeshStandardMaterial({ color: 0xfacc15, roughness: 0.4 });
      const band = new THREE.Mesh(bandGeo, bandMat);
      band.position.y = 0.09;
      hairGroup.add(band);
    } else if (config.hairStyle === 'ponytail') {
      const mainCapGeo = new THREE.SphereGeometry(0.33, 24, 24, 0, Math.PI * 2, 0, Math.PI / 1.6);
      const mainCap = new THREE.Mesh(mainCapGeo, hairMat);
      hairGroup.add(mainCap);

      const tailGeo = new THREE.ConeGeometry(0.12, 0.48, 8);
      tailGeo.rotateX(Math.PI / 3);
      const tail = new THREE.Mesh(tailGeo, hairMat);
      tail.position.set(0, 0.13, -0.34);
      hairGroup.add(tail);
    } else {
      const capGeo = new THREE.SphereGeometry(0.335, 24, 24, 0, Math.PI * 2, 0, Math.PI / 2);
      const capMat = new THREE.MeshStandardMaterial({ color: config.jerseyHex, roughness: 0.5 });
      const cap = new THREE.Mesh(capGeo, capMat);
      hairGroup.add(cap);

      const visorGeo = new THREE.BoxGeometry(0.3, 0.02, 0.24);
      const visor = new THREE.Mesh(visorGeo, capMat);
      visor.position.set(0, 0.02, -0.37);
      visor.rotation.x = -0.2;
      hairGroup.add(visor);
    }

    head.add(hairGroup);
    this.skeleton.hair = hairGroup;

    // ----------------------------------------------------
    // 4. 3D ATHLETIC BACKPACK WITH HARNESS STRAPS
    // ----------------------------------------------------
    const packGeo = new THREE.BoxGeometry(0.46, 0.52, 0.28);
    const packMat = new THREE.MeshStandardMaterial({ color: config.packHex, roughness: 0.65 });
    const backpack = new THREE.Mesh(packGeo, packMat);
    backpack.position.set(0, 0.02, -0.3);
    backpack.castShadow = true;
    torso.add(backpack);
    this.skeleton.backpack = backpack;

    // Outer Pocket
    const pocketGeo = new THREE.BoxGeometry(0.36, 0.3, 0.1);
    const pocketMat = new THREE.MeshStandardMaterial({ color: config.trimHex, roughness: 0.5 });
    const pocket = new THREE.Mesh(pocketGeo, pocketMat);
    pocket.position.set(0, -0.09, -0.16);
    backpack.add(pocket);

    // Double Shoulder Harness Straps
    [-0.19, 0.19].forEach(x => {
      const strapGeo = new THREE.BoxGeometry(0.07, 0.54, 0.04);
      const strapMat = new THREE.MeshStandardMaterial({ color: config.packHex });
      const strap = new THREE.Mesh(strapGeo, strapMat);
      strap.position.set(x, 0.05, 0.15);
      backpack.add(strap);
    });

    // ----------------------------------------------------
    // 5. ATHLETIC ARMS, FOREARMS & HANDS
    // ----------------------------------------------------
    const armMat = new THREE.MeshStandardMaterial({ color: config.jerseyHex, roughness: 0.5 });
    const sleeveGeo = new THREE.CylinderGeometry(0.12, 0.11, 0.38, 14);
    const armGeo = new THREE.CylinderGeometry(0.085, 0.075, 0.36, 14);
    const handGeo = new THREE.SphereGeometry(0.075, 12, 12);

    // Left Arm Hierarchy
    const leftUpperArm = new THREE.Group();
    leftUpperArm.position.set(-0.38, 0.29, 0);
    torso.add(leftUpperArm);
    this.skeleton.leftUpperArm = leftUpperArm;

    const lSleeve = new THREE.Mesh(sleeveGeo, armMat);
    lSleeve.position.y = -0.17;
    leftUpperArm.add(lSleeve);

    const leftForeArm = new THREE.Group();
    leftForeArm.position.y = -0.35;
    leftUpperArm.add(leftForeArm);
    this.skeleton.leftForeArm = leftForeArm;

    const lSkinArm = new THREE.Mesh(armGeo, skinMat);
    lSkinArm.position.y = -0.18;
    leftForeArm.add(lSkinArm);

    const lHand = new THREE.Mesh(handGeo, skinMat);
    lHand.position.y = -0.37;
    leftForeArm.add(lHand);

    // Right Arm Hierarchy
    const rightUpperArm = new THREE.Group();
    rightUpperArm.position.set(0.38, 0.29, 0);
    torso.add(rightUpperArm);
    this.skeleton.rightUpperArm = rightUpperArm;

    const rSleeve = new THREE.Mesh(sleeveGeo, armMat);
    rSleeve.position.y = -0.17;
    rightUpperArm.add(rSleeve);

    const rightForeArm = new THREE.Group();
    rightForeArm.position.y = -0.35;
    rightUpperArm.add(rightForeArm);
    this.skeleton.rightForeArm = rightForeArm;

    const rSkinArm = new THREE.Mesh(armGeo, skinMat);
    rSkinArm.position.y = -0.18;
    rightForeArm.add(rSkinArm);

    const rHand = new THREE.Mesh(handGeo, skinMat);
    rHand.position.y = -0.37;
    rightForeArm.add(rHand);

    // ----------------------------------------------------
    // 6. SHORTS, LEGS & 3D SPORTS SNEAKERS
    // ----------------------------------------------------
    const shortsGeo = new THREE.CylinderGeometry(0.29, 0.32, 0.36, 16);
    const shortsMat = new THREE.MeshStandardMaterial({ color: config.shortsHex, roughness: 0.5 });
    const shorts = new THREE.Mesh(shortsGeo, shortsMat);
    shorts.position.set(0, -0.48, 0);
    torso.add(shorts);

    const thighGeo = new THREE.CylinderGeometry(0.11, 0.095, 0.4, 14);
    const calfGeo = new THREE.CylinderGeometry(0.09, 0.08, 0.4, 14);
    const sockGeo = new THREE.CylinderGeometry(0.085, 0.085, 0.15, 12);
    const sockMat = new THREE.MeshStandardMaterial({ color: 0xffffff, roughness: 0.3 });
    const shoeMat = new THREE.MeshStandardMaterial({ color: config.shoesHex, roughness: 0.3, metalness: 0.2 });

    const createSneaker = () => {
      const sGroup = new THREE.Group();

      const soleGeo = new THREE.BoxGeometry(0.2, 0.1, 0.42);
      const soleMat = new THREE.MeshStandardMaterial({ color: 0x111111 });
      const sole = new THREE.Mesh(soleGeo, soleMat);
      sole.position.set(0, 0.05, 0.04);
      sGroup.add(sole);

      const upperGeo = new THREE.BoxGeometry(0.19, 0.16, 0.36);
      const upper = new THREE.Mesh(upperGeo, shoeMat);
      upper.position.set(0, 0.14, 0.04);
      sGroup.add(upper);

      const toeCapGeo = new THREE.SphereGeometry(0.1, 12, 12);
      toeCapGeo.scale(0.9, 0.7, 1.1);
      const toeCap = new THREE.Mesh(toeCapGeo, shoeMat);
      toeCap.position.set(0, 0.12, 0.18);
      sGroup.add(toeCap);

      return sGroup;
    };

    // Left Leg Hierarchy
    const leftThigh = new THREE.Group();
    leftThigh.position.set(-0.2, -0.48, 0);
    torso.add(leftThigh);
    this.skeleton.leftThigh = leftThigh;

    const lThighMesh = new THREE.Mesh(thighGeo, skinMat);
    lThighMesh.position.y = -0.2;
    leftThigh.add(lThighMesh);

    const leftCalf = new THREE.Group();
    leftCalf.position.y = -0.4;
    leftThigh.add(leftCalf);
    this.skeleton.leftCalf = leftCalf;

    const lCalfMesh = new THREE.Mesh(calfGeo, skinMat);
    lCalfMesh.position.y = -0.2;
    leftCalf.add(lCalfMesh);

    const lSock = new THREE.Mesh(sockGeo, sockMat);
    lSock.position.y = -0.32;
    leftCalf.add(lSock);

    const lShoe = createSneaker();
    lShoe.position.set(0, -0.42, 0);
    leftCalf.add(lShoe);
    this.skeleton.leftShoe = lShoe;

    // Right Leg Hierarchy
    const rightThigh = new THREE.Group();
    rightThigh.position.set(0.2, -0.48, 0);
    torso.add(rightThigh);
    this.skeleton.rightThigh = rightThigh;

    const rThighMesh = new THREE.Mesh(thighGeo, skinMat);
    rThighMesh.position.y = -0.2;
    rightThigh.add(rThighMesh);

    const rightCalf = new THREE.Group();
    rightCalf.position.y = -0.4;
    rightThigh.add(rightCalf);
    this.skeleton.rightCalf = rightCalf;

    const rCalfMesh = new THREE.Mesh(calfGeo, skinMat);
    rCalfMesh.position.y = -0.2;
    rightCalf.add(rCalfMesh);

    const rSock = new THREE.Mesh(sockGeo, sockMat);
    rSock.position.y = -0.32;
    rightCalf.add(rSock);

    const rShoe = createSneaker();
    rShoe.position.set(0, -0.42, 0);
    rightCalf.add(rShoe);
    this.skeleton.rightShoe = rShoe;

    // ----------------------------------------------------
    // 7. POWER-UP AURA GLOW RING
    // ----------------------------------------------------
    const auraGeo = new THREE.RingGeometry(0.65, 0.9, 24);
    auraGeo.rotateX(-Math.PI / 2);
    const auraMat = new THREE.MeshBasicMaterial({
      color: 0x00e5ff,
      side: THREE.DoubleSide,
      transparent: true,
      opacity: 0.0
    });
    const auraMesh = new THREE.Mesh(auraGeo, auraMat);
    auraMesh.position.y = 0.1;
    rootGroup.add(auraMesh);
    this.skeleton.auraGlow = auraMesh;

    this.characterGroup.add(rootGroup);
    this.skeleton.root = rootGroup;
  }

  moveLeft() {
    if (this.currentLane > 0) {
      this.currentLane--;
      this.targetX = (this.currentLane - 1) * 2.5;
    }
  }

  moveRight() {
    if (this.currentLane < 2) {
      this.currentLane++;
      this.targetX = (this.currentLane - 1) * 2.5;
    }
  }

  jump() {
    if (!this.isJumping) {
      this.isJumping = true;
      this.jumpVelocity = 14.0;
      if (this.isSliding) this.isSliding = false;
    }
  }

  slide() {
    if (!this.isSliding) {
      this.isSliding = true;
      this.slideTimer = 0.85; // Slide duration
      if (this.isJumping) {
        // Fast downward drop if sliding in mid-air
        this.jumpVelocity = -22;
      }
    }
  }

  triggerHitAnimation() {
    this.isHit = true;
    this.hitTimer = 0.5; // Recoil animation duration
  }

  update(delta, runSpeed, isPowerUpActive = false) {
    const animSpeed = isPowerUpActive ? runSpeed * 1.6 : runSpeed;
    this.animTime += delta * animSpeed * 1.4;

    // Smooth X lane transition
    this.currentX += (this.targetX - this.currentX) * 16 * delta;

    // Hit Timer
    if (this.isHit) {
      this.hitTimer -= delta;
      if (this.hitTimer <= 0) {
        this.isHit = false;
      }
    }

    // Jump Physics Engine
    if (this.isJumping) {
      this.jumpY += this.jumpVelocity * delta;
      this.jumpVelocity += this.gravity * delta;
      if (this.jumpY <= 0) {
        this.jumpY = 0;
        this.isJumping = false;
      }
    }

    // Slide Timer
    if (this.isSliding) {
      this.slideTimer -= delta;
      if (this.slideTimer <= 0) {
        this.isSliding = false;
      }
    }

    // Position Character Group
    this.characterGroup.position.x = this.currentX;
    this.characterGroup.position.y = this.jumpY;

    // Update Ground Drop Shadow
    if (this.shadowMesh) {
      this.shadowMesh.position.x = this.currentX;
      const shadowScale = Math.max(0.4, 1.0 - (this.jumpY / 4.0) * 0.5);
      this.shadowMesh.scale.set(shadowScale, shadowScale, shadowScale);
      this.shadowMesh.material.opacity = Math.max(0.15, 0.7 - (this.jumpY / 4.0) * 0.5);
    }

    // ----------------------------------------------------
    // ADVANCED PROCEDURAL SKELETAL ANIMATION STATE MACHINE
    // ----------------------------------------------------
    const skel = this.skeleton;
    if (skel.torso) {
      const laneDiff = this.targetX - this.currentX;

      // Smooth banking roll & yaw during lane changes
      this.characterGroup.rotation.z = -laneDiff * 0.16;
      this.characterGroup.rotation.y = laneDiff * 0.1;

      if (this.isHit) {
        // --- RECOIL / HIT POSE ---
        skel.torso.position.y = 1.0;
        skel.torso.rotation.x = -0.55;
        if (skel.head) skel.head.rotation.x = 0.4;

        if (skel.leftUpperArm) {
          skel.leftUpperArm.rotation.x = -1.8;
          skel.leftUpperArm.rotation.z = -0.8;
        }
        if (skel.rightUpperArm) {
          skel.rightUpperArm.rotation.x = -1.8;
          skel.rightUpperArm.rotation.z = 0.8;
        }
        if (skel.leftThigh) skel.leftThigh.rotation.x = -0.6;
        if (skel.rightThigh) skel.rightThigh.rotation.x = 0.6;
      } else if (this.isSliding) {
        // --- SLIDE POSE ---
        skel.torso.position.y = 0.65;
        skel.torso.rotation.x = 0.95;

        if (skel.head) skel.head.rotation.x = -0.6;

        // Legs extend forward
        if (skel.leftThigh) skel.leftThigh.rotation.x = -1.3;
        if (skel.rightThigh) skel.rightThigh.rotation.x = -1.3;
        if (skel.leftCalf) skel.leftCalf.rotation.x = 0.2;
        if (skel.rightCalf) skel.rightCalf.rotation.x = 0.2;

        // Arms reach back for stability
        if (skel.leftUpperArm) skel.leftUpperArm.rotation.x = 1.1;
        if (skel.rightUpperArm) skel.rightUpperArm.rotation.x = 1.1;

      } else if (this.isJumping) {
        // --- JUMP / AIRBORNE POSE ---
        skel.torso.position.y = 1.18;
        skel.torso.rotation.x = -0.15;

        if (skel.head) skel.head.rotation.x = 0.1;

        // Arms lift up overhead
        if (skel.leftUpperArm) {
          skel.leftUpperArm.rotation.x = -2.2;
          skel.leftUpperArm.rotation.z = -0.3;
        }
        if (skel.rightUpperArm) {
          skel.rightUpperArm.rotation.x = -2.2;
          skel.rightUpperArm.rotation.z = 0.3;
        }

        // Legs tuck back slightly at knees
        if (skel.leftThigh) skel.leftThigh.rotation.x = 0.45;
        if (skel.rightThigh) skel.rightThigh.rotation.x = 0.2;
        if (skel.leftCalf) skel.leftCalf.rotation.x = -0.8;
        if (skel.rightCalf) skel.rightCalf.rotation.x = -0.5;

      } else {
        // --- PROFESSIONAL RUNNING CYCLE ---
        // Torso forward athletic tilt & natural hip bounce
        skel.torso.rotation.x = 0.16;
        const hipBounce = Math.abs(Math.sin(this.animTime * 2)) * 0.08;
        skel.torso.position.y = 1.18 + hipBounce;

        // Shoulder twist counter-rotation
        skel.torso.rotation.y = Math.sin(this.animTime) * 0.12;

        if (skel.head) {
          skel.head.rotation.x = -0.08;
          skel.head.rotation.y = -skel.torso.rotation.y * 0.5; // Head stays focused forward
        }

        // Legs gait swing
        const legSwing = Math.sin(this.animTime) * 0.85;
        if (skel.leftThigh) skel.leftThigh.rotation.x = legSwing;
        if (skel.rightThigh) skel.rightThigh.rotation.x = -legSwing;

        // Knee bend flex during backward leg swing
        if (skel.leftCalf) skel.leftCalf.rotation.x = legSwing < 0 ? -legSwing * 1.3 : -0.1;
        if (skel.rightCalf) skel.rightCalf.rotation.x = -legSwing < 0 ? legSwing * 1.3 : -0.1;

        // Opposite arm swing with forearm elbow flex
        if (skel.leftUpperArm) {
          skel.leftUpperArm.rotation.x = -legSwing * 0.9;
          skel.leftUpperArm.rotation.z = -0.1;
        }
        if (skel.rightUpperArm) {
          skel.rightUpperArm.rotation.x = legSwing * 0.9;
          skel.rightUpperArm.rotation.z = 0.1;
        }

        if (skel.leftForeArm) skel.leftForeArm.rotation.x = -0.4 - Math.sin(this.animTime) * 0.2;
        if (skel.rightForeArm) skel.rightForeArm.rotation.x = -0.4 + Math.sin(this.animTime) * 0.2;

        // Backpack subtle lag movement
        if (skel.backpack) {
          skel.backpack.rotation.x = Math.sin(this.animTime * 2) * 0.06;
        }
      }
    }

    // Power-Up Aura Ring Glow Animation
    if (skel.auraGlow) {
      if (isPowerUpActive) {
        skel.auraGlow.material.opacity = 0.7 + Math.sin(this.animTime * 6) * 0.3;
        skel.auraGlow.rotation.z += 3.0 * delta;
      } else {
        skel.auraGlow.material.opacity = 0;
      }
    }
  }

  getBounds() {
    const yHeight = this.isSliding ? 0.75 : 1.75;
    const yMin = this.jumpY;
    const yMax = this.jumpY + yHeight;

    return {
      minX: this.currentX - 0.45,
      maxX: this.currentX + 0.45,
      minY: yMin,
      maxY: yMax,
      z: 0 // Runner is positioned at Z = 0
    };
  }
}

