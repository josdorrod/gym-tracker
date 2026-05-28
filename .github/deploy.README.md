# Deploy desde GitHub Actions al Home Server

Resumen
- CI (build + tests) en pushes a `main`. Si los tests pasan, rsync → servidor y deploy atómico (releases + symlink `current`).
- No usa registry; usa rsync + docker compose.

Secrets requeridos (repo → Settings → Secrets):
- SSH_PRIVATE_KEY : clave privada (ed25519/rsa) para deploy (sin passphrase).
- SSH_HOST : IP o dominio del servidor.
- SSH_USER : usuario remoto (recomendado: deploy).
- SSH_PORT : puerto ssh (default 22).
- DEPLOY_PATH : directorio base en el servidor (ej: /home/deploy/apps/gymtracker).
Opcional:
- HEALTHCHECK_URL : URL a comprobar tras el deploy (ej: http://localhost:5000/health)
- KEEP_RELEASES : cuántos releases conservar (default 5)

Preparación del servidor (resumen)
1. Crear usuario deploy (opcional pero recomendado):
   sudo useradd -m -s /bin/bash deploy
   sudo usermod -aG docker deploy

2. Crear carpetas y permisos:
   sudo mkdir -p /home/deploy/apps/gymtracker/releases
   sudo chown -R deploy:deploy /home/deploy/apps/gymtracker

3. Generar clave local y copiar public key al servidor:
   ssh-keygen -t ed25519 -f ./deploy_key -C "deploy@gymtracker"
   cat deploy_key.pub | ssh deploy@YOUR_HOST 'mkdir -p ~/.ssh && cat >> ~/.ssh/authorized_keys && chmod 700 ~/.ssh && chmod 600 ~/.ssh/authorized_keys'

4. Probar docker compose:
   ssh -i deploy_key -p 22 deploy@YOUR_HOST 'docker compose version'

Cómo funciona el deploy
- GitHub crea releases/<timestamp>-<sha> y rsyncea el repo allí.
- En el servidor el symlink `current` se actualiza a la nueva release.
- Se ejecuta `docker compose down` + `docker compose up -d --build`.
- Opcional healthcheck: si falla, workflow marca error y no completa (rollback manual posible).

Rollback manual
- Conéctate al servidor:
  cd /home/deploy/apps/gymtracker/releases
  ls -1t
  ln -sfn releases/<previous> ../current
  cd ../current
  docker compose down
  docker compose up -d

Buenas prácticas
- Clave dedicada, usuario limitado.
- No exponer .git ni .github.
- Mantén backups y una forma de rollback.
- Considera usar GHCR/registry más adelante para builds más rápidos.
