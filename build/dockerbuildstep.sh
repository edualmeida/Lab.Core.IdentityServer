docker network create tulip-net || true
docker build -f Lab.Core.IdentityServer/Dockerfile -t identityserver-img .
docker rm -f core-identityserver
docker run --restart unless-stopped --name core-identityserver --net tulip-net -d -p 5001:8080 identityserver-img
