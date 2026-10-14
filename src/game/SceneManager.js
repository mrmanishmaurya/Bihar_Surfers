// SceneManager.js - Three.js WebGL Scene, Golden Hour Lighting, Camera Controller & Particle Engine
import * as THREE from 'three';

export class ParticleEffect {
  constructor(scene) {
    this.scene = scene;
    this.particles = [];

    // Shared geometry and materials for maximum performance
    this.sparkGeo = new THREE.SphereGeometry(0.08, 6, 6);
    this.goldMat = new THREE.MeshBasicMaterial({ color: 0xffd700 });
    this.dustMat = new THREE.MeshBasicMaterial({ color: 0xcccccc, transparent: true, opacity: 0.6 });
    this.auraMat = new THREE.MeshBasicMaterial({ color: 0x00e5ff, transparent: true, opacity: 0.8 });
  }

  spawnCoinSparks(x, y, z) {
    for (let i = 0; i < 12; i++) {
      const mesh = new THREE.Mesh(this.sparkGeo, this.goldMat);
      mesh.position.set(x, y, z);
      this.scene.add(mesh);

      const vx = (Math.random() - 0.5) * 6.0;
      const vy = Math.random() * 5.0 + 2.0;
      const vz = (Math.random() - 0.5) * 6.0;

      this.particles.push({
        mesh,
        vx,
        vy,
        vz,
        life: 0.45,
        maxLife: 0.45,
        gravity: -12
      });
    }
  }

  spawnDustPuff(x, y, z) {
    for (let i = 0; i < 8; i++) {
      const scale = 0.15 + Math.random() * 0.15;
      const mesh = new THREE.Mesh(this.sparkGeo, this.dustMat.clone());
      mesh.scale.set(scale, scale, scale);
      mesh.position.set(x + (Math.random() - 0.5) * 0.6, y, z + (Math.random() - 0.5) * 0.6);
      this.scene.add(mesh);

      this.particles.push({
        mesh,
        vx: (Math.random() - 0.5) * 2.0,
        vy: Math.random() * 1.5 + 0.5,
        vz: (Math.random() - 0.5) * 2.0,
        life: 0.35,
        maxLife: 0.35,
        gravity: -2
      });
    }
  }

  update(delta) {
    for (let i = this.particles.length - 1; i >= 0; i--) {
      const p = this.particles[i];
      p.life -= delta;

      if (p.life <= 0) {
        this.scene.remove(p.mesh);
        p.mesh.geometry.dispose();
        if (p.mesh.material.map) p.mesh.material.map.dispose();
        this.particles.splice(i, 1);
      } else {
        p.vy += p.gravity * delta;
        p.mesh.position.x += p.vx * delta;
        p.mesh.position.y += p.vy * delta;
        p.mesh.position.z += p.vz * delta;

        const ratio = p.life / p.maxLife;
        if (p.mesh.material.transparent) {
          p.mesh.material.opacity = ratio * 0.7;
        }
        p.mesh.scale.multiplyScalar(0.95);
      }
    }
  }
}

export class SceneManager {
  constructor(container) {
    this.container = container;

    // 1. Three.js Scene
    this.scene = new THREE.Scene();
    this.scene.background = new THREE.Color(0xff9e3d); // Golden-hour sunset sky
    this.scene.fog = new THREE.FogExp2(0xff9900, 0.012); // Atmospheric distance fog

    // Particle System
    this.particleEffect = new ParticleEffect(this.scene);

    // 2. Camera Setup (Third Person Chase Camera)
    const aspect = window.innerWidth / window.innerHeight;
    this.baseFov = 60;
    this.camera = new THREE.PerspectiveCamera(this.baseFov, aspect, 0.1, 220);
    this.camera.position.set(0, 3.8, -5.8);
    this.camera.lookAt(0, 1.8, 10);

    // Camera shake offset
    this.shakeAmount = 0;

    // 3. Renderer Setup
    this.renderer = new THREE.WebGLRenderer({ antialias: true, alpha: false, powerPreference: 'high-performance' });
    this.renderer.setSize(window.innerWidth, window.innerHeight);
    this.renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));
    this.renderer.shadowMap.enabled = true;
    this.renderer.shadowMap.type = THREE.PCFSoftShadowMap;
    this.renderer.toneMapping = THREE.ACESFilmicToneMapping;
    this.renderer.toneMappingExposure = 1.15;

    this.container.appendChild(this.renderer.domElement);

    // 4. Lighting Setup
    this.setupLighting();

    window.addEventListener('resize', () => this.onWindowResize());
  }

  setupLighting() {
    const ambientLight = new THREE.AmbientLight(0xffebd2, 0.85);
    this.scene.add(ambientLight);

    const hemiLight = new THREE.HemisphereLight(0xffb74d, 0x3e2723, 0.6);
    this.scene.add(hemiLight);

    this.dirLight = new THREE.DirectionalLight(0xffaa44, 1.6);
    this.dirLight.position.set(25, 40, -20);
    this.dirLight.castShadow = true;

    this.dirLight.shadow.mapSize.width = 1024;
    this.dirLight.shadow.mapSize.height = 1024;
    this.dirLight.shadow.camera.near = 0.5;
    this.dirLight.shadow.camera.far = 120;
    this.dirLight.shadow.camera.left = -18;
    this.dirLight.shadow.camera.right = 18;
    this.dirLight.shadow.camera.top = 30;
    this.dirLight.shadow.camera.bottom = -12;

    this.scene.add(this.dirLight);
  }

  addCameraShake(intensity = 0.45) {
    this.shakeAmount = intensity;
  }

  updateCamera(targetX, targetY, speedMultiplier = 1.0, delta = 0.016) {
    // Dynamic Speed FOV
    const targetFov = this.baseFov + (speedMultiplier - 1.0) * 18;
    this.camera.fov += (targetFov - this.camera.fov) * 0.1;
    this.camera.updateProjectionMatrix();

    // Smooth camera chase position
    const desiredX = targetX * 0.45;
    const desiredY = 3.8 + targetY * 0.45;

    let shakeX = 0;
    let shakeY = 0;
    if (this.shakeAmount > 0.01) {
      shakeX = (Math.random() - 0.5) * this.shakeAmount;
      shakeY = (Math.random() - 0.5) * this.shakeAmount;
      this.shakeAmount *= 0.88; // Smooth decay
    }

    this.camera.position.x += (desiredX + shakeX - this.camera.position.x) * 0.12;
    this.camera.position.y += (desiredY + shakeY - this.camera.position.y) * 0.12;

    this.camera.lookAt(targetX * 0.3, 1.8 + targetY * 0.25, 12);

    // Update Particles
    this.particleEffect.update(delta);
  }

  render() {
    this.renderer.render(this.scene, this.camera);
  }

  onWindowResize() {
    const width = window.innerWidth;
    const height = window.innerHeight;
    this.camera.aspect = width / height;
    this.camera.updateProjectionMatrix();
    this.renderer.setSize(width, height);
  }
}

