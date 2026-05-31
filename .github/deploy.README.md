# Deploy desde GitHub Actions al Home Server

Resumen
- CI (build + tests) en pushes a `main`. Si los tests pasan, rsync → servidor y deploy atómico (releases + symlink `current`).
- No usa registry; usa rsync + docker compose.
- Conexión mediante **Tailscale OAuth** (autenticación por identidad, sin claves SSH, sin exponer puertos al mundo).

---

## Secrets requeridos (repo → Settings → Secrets)

### Conexión
- `SSH_HOST`: **IP asignada en Tailscale** (`100.x.x.x`). No uses la IP local (`192.168.x.x`).
- `SSH_USER`: usuario remoto (recomendado: `deploy`).
- `DEPLOY_PATH`: directorio base en el servidor (ej: `/home/deploy/apps/gymtracker`).

### Tailscale OAuth (obligatorio)
- `TAILSCALE_CLIENT_ID`: OAuth client ID de Tailscale.
- `TAILSCALE_SECRET`: OAuth client secret de Tailscale.

### Opcional
- `SSH_PORT`: puerto ssh (default 22).
- `HEALTHCHECK_URL`: URL a comprobar tras el deploy (ej: http://localhost:5000/health)
- `KEEP_RELEASES`: cuántos releases conservar (default 5)

---

## Preguntas frecuentes

### ¿Qué pongo en SSH_HOST?

**Usa la IP de Tailscale de tu servidor**, no la IP local.

Para obtenerla, desde tu servidor ejecuta:
```bash
tailscale ip -4
```

Verás algo como `100.x.x.x`. Esa es la IP que debes poner en el secret `SSH_HOST`.

### ¿Por qué no la IP local?

Porque el runner de GitHub Actions está en internet, no en tu red local. La conexión se hace a través de la red mesh de Tailscale, que asigna una IP única en el rango `100.x.x.x` a cada nodo.

---

## Configuración de Tailscale OAuth

### 1. Crear OAuth App en Tailscale

1. Ve a https://login.tailscale.com/admin/settings/oauth
2. Crea un nuevo **OAuth client** con:
   - **Name**: `github-actions-deploy`
   - **Scopes**: 
     - `devices:write` (para crear sesiones de dispositivo)
     - `tags:read` (opcional, para verificar tags)
3. Copia el **Client ID** y **Secret**

### 2. Configurar Tags en Tailscale

Los tags permiten controlar qué dispositivos pueden acceder a qué recursos.

1. Ve a https://login.tailscale.com/admin/acls
2. Añade el tag `tag:ci` a tu OAuth client:
   - Edita el OAuth client que creaste
   - Selecciona el tag `tag:ci`

### 3. Configurar ACLs

```json
{
  "tagOwners": {
    "tag:ci": ["autogroup:admin"],
    "tag:server": ["autogroup:admin"]
  },
  "acls": [
    {
      "action": "accept",
      "src": ["tag:ci"],
      "dst": ["tag:server:22"]
    }
  ],
  "ssh": [
    {
      "action": "accept",
      "src": ["tag:ci"],
      "dst": ["tag:server"],
      "users": ["deploy"]
    }
  ]
}
```

### 4. Asegurar que tu servidor tiene el tag `tag:server`

En tu servidor:
```bash
sudo tailscale up --advertise-tags=tag:server
```

---

## Preparación del servidor

1. Crear usuario deploy (opcional pero recomendado):
   ```bash
   sudo useradd -m -s /bin/bash deploy
   sudo usermod -aG docker deploy
   ```

2. Crear carpetas y permisos:
   ```bash
   sudo mkdir -p /home/deploy/apps/gymtracker/releases
   sudo chown -R deploy:deploy /home/deploy/apps/gymtracker
   ```

3. Habilitar Tailscale SSH:
   ```bash
   sudo tailscale up --ssh --advertise-tags=tag:server
   ```

4. Obtener Tailscale IP del servidor:
   ```bash
   tailscale ip -4
   ```

---

## Cómo funciona el deploy

1. El runner de GitHub Actions se conecta a Tailscale usando OAuth (sin auth key manual).
2. El runner SSH al servidor usando la IP de Tailscale (`100.x.x.x`).
3. Se crea un directorio de release y se rsynccea el código.
4. Se actualiza el symlink `current` y se levantan los contenedores.
5. Si algo falla, rollback automático al release anterior.
6. Healthcheck opcional para verificar que la app funciona.

---

## Rollback manual

Conéctate al servidor (directamente o por Tailscale):
```bash
cd /home/deploy/apps/gymtracker/releases
ls -1t
ln -sfn releases/<previous> ../current
cd ../current
docker compose down
docker compose up -d
```

---

## Buenas prácticas
- Usa **Tailscale OAuth** en vez de auth keys para CI/CD.
- Usuario limitado (`deploy`) en el servidor.
- Configura ACLs restrictivos con tags.
- No exponer .git ni .github.
- Mantén backups y una forma de rollback.